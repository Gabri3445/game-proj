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
    private bool _canCharacterMove = true;
    private float _currentSpeed;
    private CharacterPosition _characterPosition;
    public CharacterInput InputActions { get; private set; }
    private bool _isGrounded = true;
    private bool _isSideMovementAllowed = true;
    private Rigidbody _rigidBody;
    private Vector3 _originalPosition;
    private Animator _animator;
    private GameInstance _gameInstance;
    private GameUIManager _gameUIManager;
    private int _checkpointNumber = 0;

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
        _gameUIManager = GameObject.Find("GameUIManagerObject").GetComponent<GameUIManager>();
    }
    
    private void FixedUpdate()
    {
        CharacterMovement(InputActions.Player.Move.ReadValue<Vector2>()); //TODO:read in update?
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

    private void OnJump(InputAction.CallbackContext context) => CharacterJump();
    private void OnSprintStart(InputAction.CallbackContext context) => _currentSpeed = sprintSpeed;
    private void OnSprintEnd(InputAction.CallbackContext context) => _currentSpeed = speed;


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            GroundAnim();
        }
        else if (other.gameObject.CompareTag("Enemy")) Debug.Log("Hit enemy");
    }

    private void GroundAnim()
    {
        if (Time.frameCount < 60) return; //TODO: find a better way
        _animator.Play("Grounded", 0, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            var otherName = other.name;
            
            if (otherName.StartsWith("Checkpoint") && int.TryParse(otherName["Checkpoint".Length..], out var checkpointId))
            {
                Debug.Log($"Checkpoint hit: {checkpointId}");

                if (_checkpointNumber > checkpointId) return;

                _checkpointNumber++;
                _gameUIManager.OnCheckPointChange(_checkpointNumber);
                _gameInstance.checkpoint = other.transform.position;

                Debug.Log("Hit checkpoint at coords " + transform.position);
            }
            else
            {
                Debug.LogWarning($"Invalid checkpoint name: {otherName}");
            }
        }
    }


    private void CharacterJump()
    {
        if (!_isGrounded || !_canCharacterMove) return;
        _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _animator.Play("Jump", 0, 0f); //TODO: somehow figure out the top of the jump and animate it that way?
        _isGrounded = false;
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