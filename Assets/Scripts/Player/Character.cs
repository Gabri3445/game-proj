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
    private int _checkpointNumber;
    private float _currentSpeed;
    private GameInstance _gameInstance;
    private GameUIManager _gameUIManager;
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
        _gameInstance = GameObject.Find("GameInstanceObject").GetComponent<GameInstance>();
        _gameInstance.checkpoint = _originalPosition;
        _gameUIManager = _gameInstance.gameUIManager;
    }

    private void Start()
    {
        _gameUIManager = _gameInstance.gameUIManager;
        Debug.Log(_gameUIManager);
    }

    private void Update()
    {
        _movementInput = InputActions.Player.Move.ReadValue<Vector2>();
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
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            GroundAnim();
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            var checkpointId = other.GetComponent<Checkpoint>().checkpointNumber;
            Debug.Log($"Checkpoint hit: {checkpointId}");

            if (_checkpointNumber > checkpointId) return;

            _checkpointNumber++;
            _gameUIManager.OnCheckPointChange(_checkpointNumber);
            _gameInstance.checkpoint = other.transform.position;

            Debug.Log("Hit checkpoint at coords " + transform.position);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
            if (_gameInstance.livesRemaining != 0) _gameInstance.livesRemaining--;
            _gameUIManager.OnGameOver();
        }
        else if (other.gameObject.CompareTag("LevelEnd"))
        {
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
        //if (Time.frameCount < 60) return; //TODO: find a better way
        _animator.Play("Grounded", 0, 0f); // Andrea dice no porco dio
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
        transform.position = _gameInstance.checkpoint;
    }
}