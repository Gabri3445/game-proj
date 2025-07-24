using System.Collections;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float sprintSpeed;
    public float sidewaysMovementDuration;
    public bool isGrounded;
    private readonly bool _canCharacterMove = true;
    private Animator _animator;
    private CharacterPosition _characterPosition;
    private Checkpoint _checkpoint;
    private int _checkpointNumber;
    private float _currentSpeed;
    private GameInstance _gameInstance;
    private GameUIManager _gameUIManager;
    private int _isColliding;
    private bool _isSideMovementAllowed = true;
    private Vector2 _movementInput;
    private Vector3 _originalPosition;
    private Rigidbody _rigidBody;
    public CharacterInput InputActions { get; private set; }

    private void Awake()
    {
        _characterPosition = CharacterPosition.Center;
        InputActions = new CharacterInput();
        _rigidBody = GetComponent<Rigidbody>();
        _originalPosition = _rigidBody.position;
        _animator = GetComponent<Animator>();
        _animator.enabled = true;
    }

    private void Start()
    {
        _gameInstance = GameInstance.Instance;
        _gameInstance.checkpoint = _originalPosition;
        _gameUIManager = _gameInstance.gameUIManager;
        Debug.Log(_gameUIManager);
    }

    private void Update()
    {
        _movementInput = InputActions.Player.Move.ReadValue<Vector2>();
#if UNITY_EDITOR
        var left = transform.TransformDirection(Vector3.left) * 1;
        var right = transform.TransformDirection(Vector3.right) * 1;
        Debug.DrawRay(
            new Vector3(transform.position.x - 0.45f, transform.position.y + 0.4f, transform.position.z + 0.4f), left,
            Color.green);
        Debug.DrawRay(
            new Vector3(transform.position.x - 0.45f, transform.position.y + 0.4f, transform.position.z - 0.4f), left,
            Color.green);
        Debug.DrawRay(
            new Vector3(transform.position.x + 0.45f, transform.position.y + 0.4f, transform.position.z + 0.4f), right,
            Color.green);
        Debug.DrawRay(
            new Vector3(transform.position.x + 0.45f, transform.position.y + 0.4f, transform.position.z - 0.4f), right,
            Color.green);
#endif
        //Vector3 right = transform.TransformDirection(Vector3.right) * 10;
        //Debug.DrawRay(new Vector3(transform.position.x - 0.4f, transform.position.y + 0.4f, transform.position.z), right, Color.green);
    }

    private void FixedUpdate()
    {
        CharacterMovement(_movementInput);
    }


    private void OnEnable()
    {
        InputActions.Player.Enable();
        InputActions.Player.Jump.performed += OnJump;
        InputActions.Player.Sprint.performed += OnSprintStart;
        InputActions.Player.Sprint.canceled += OnSprintEnd;
        _currentSpeed = speed;
    }

    private void OnDisable()
    {
        InputActions.Player.Jump.performed -= OnJump;
        InputActions.Player.Sprint.performed -= OnSprintStart;
        InputActions.Player.Sprint.canceled -= OnSprintEnd;
        InputActions.Player.Disable();
    }


    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(_isColliding);
        if (other.gameObject.CompareTag("Ground"))
        {
            _isColliding++;
            if (_isColliding == 1) GroundAnim();

            isGrounded = true;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        _isColliding--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            var checkpoint = other.gameObject.GetComponent<Checkpoint>();
            var checkpointId = checkpoint.checkpointNumber;
            Debug.Log($"Checkpoint hit: {checkpointId}");

            if (_checkpointNumber > checkpointId) return;

            _checkpoint = checkpoint;
            StartCoroutine(_checkpoint.ShowCheckPointText());

            _checkpointNumber++;
            _gameUIManager.OnCheckPointChange(_checkpointNumber);
            _gameInstance.checkpoint = other.transform.position;

            Debug.Log("Hit checkpoint at coords " + transform.position);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
            if (_gameInstance.livesRemaining != 0) _gameInstance.livesRemaining--;
            MusicManager.Instance.PlayExplosion();
            _gameUIManager.OnGameOver();
        }
        else if (other.gameObject.CompareTag("LevelEnd"))
        {
            _gameUIManager.OnLevelEnd();
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        CharacterJump();
    }

    private void OnSprintStart(InputAction.CallbackContext context)
    {
        _currentSpeed = sprintSpeed;
    }

    private void OnSprintEnd(InputAction.CallbackContext context)
    {
        _currentSpeed = speed;
    }

    private void GroundAnim()
    {
        //if (Time.frameCount < 60) return; // Andrea dice no porco dio
        _animator.Play("Grounded", 0, 0f);
    }


    private void CharacterJump()
    {
        if (!isGrounded || !_canCharacterMove) return;
        _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _animator.Play("Jump", 0, 0f); //TODO: somehow figure out the top of the jump and animate it that way?
        isGrounded = false;
    }


    private void CharacterMovement(Vector2 movement)
    {
        if (movement.x != 0) CharacterLateralMovement(movement.x);
        else
            _isSideMovementAllowed = true;
        //var movementSpeed = _inputActions.Player.Sprint.IsPressed() ? sprintSpeed : 1;
        /*var movementSpeed = speed;
        if (_isSprinting) movementSpeed = sprintSpeed;*/
        CharacterForwardMovement(movement.y * _currentSpeed);
    }

    private void CharacterLateralMovement(float value)
    {
        if (Physics.Raycast(
                new Vector3(transform.position.x - 0.45f, transform.position.y + 0.4f, transform.position.z + 0.4f),
                Vector3.left, out var hitLeftFront, 1))
            if (hitLeftFront.transform.CompareTag("Ground"))
                _isSideMovementAllowed = false;

        if (Physics.Raycast(
                new Vector3(transform.position.x - 0.45f, transform.position.y + 0.4f, transform.position.z - 0.4f),
                Vector3.left, out var hitLeftBack, 1))
            if (hitLeftBack.transform.CompareTag("Ground"))
                _isSideMovementAllowed = false;

        if (Physics.Raycast(
                new Vector3(transform.position.x + 0.45f, transform.position.y + 0.4f, transform.position.z + 0.4f),
                Vector3.right, out var hitRightFront, 1))
            if (hitRightFront.transform.CompareTag("Ground"))
                _isSideMovementAllowed = false;

        if (Physics.Raycast(
                new Vector3(transform.position.x + 0.45f, transform.position.y + 0.4f, transform.position.z - 0.4f),
                Vector3.right, out var hitRightBack, 1))
            if (hitRightBack.transform.CompareTag("Ground"))
                _isSideMovementAllowed = false;

        if (!_canCharacterMove || !_isSideMovementAllowed) return;
        _isSideMovementAllowed = false;

        switch (value)
        {
            case > 0:
            {
                if (_characterPosition is CharacterPosition.Left or CharacterPosition.Center)
                {
                    //transform.Translate(Vector3.right);
                    StartCoroutine(MoveLaterallySmooth(transform.position.x, transform.position.x + Vector3.right.x,
                        sidewaysMovementDuration));
                    _characterPosition++;
                }

                break;
            }
            case < 0:
            {
                if (_characterPosition is CharacterPosition.Right or CharacterPosition.Center)
                {
                    //transform.Translate(Vector3.left);
                    StartCoroutine(MoveLaterallySmooth(transform.position.x, transform.position.x + Vector3.left.x,
                        sidewaysMovementDuration));
                    _characterPosition--;
                }

                break;
            }
        }
    }

    private IEnumerator MoveLaterallySmooth(float orig, float movement, float duration)
    {
        _isSideMovementAllowed = false;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            var vector3 = transform.position;
            vector3.x = Mathf.Lerp(orig, movement, elapsed / duration);
            transform.position = vector3;
            yield return null;
        }

        var position = transform.position;
        position.x = movement;
        transform.position = position;
    }

    private void CharacterForwardMovement(float value)
    {
        if (!_canCharacterMove) return;
        if (value < 0 && _rigidBody.position.z < _originalPosition.z) return;
        var movement = new Vector3(0, 0, value * Time.fixedDeltaTime);
        /*transform.Translate(movement * speed);*/
        _rigidBody.MovePosition(_rigidBody.position + movement);
    }

    public void ReturnToCheckpoint()
    {
        _isColliding = 0;
        _characterPosition = CharacterPosition.Center;
        transform.position = _gameInstance.checkpoint;
    }
}