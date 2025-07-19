using System;
using System.Collections;
using Input;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float sprintSpeed;
    public float sidewaysMovementDuration;
    private bool _canCharacterMove = true;
    private float _currentSpeed;
    private CharacterPosition _characterPosition;
    private CharacterInput _inputActions;
    private bool _isGrounded = true;
    private bool _isSideMovementAllowed = true;
    private Rigidbody _rigidBody;
    private Vector3 _originalPosition;

    private void Awake()
    {
        _characterPosition = CharacterPosition.Center;
        _inputActions = new CharacterInput();
        _rigidBody = GetComponent<Rigidbody>();
        _originalPosition = _rigidBody.position;
    }

    private void FixedUpdate()
    {
        CharacterMovement(_inputActions.Player.Move.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Jump.performed += _ => CharacterJump();
        _inputActions.Player.Sprint.performed += _ => _currentSpeed = sprintSpeed;
        _inputActions.Player.Sprint.canceled += _ => _currentSpeed = speed;
        _currentSpeed = speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGrounded = true;
        else if (other.gameObject.CompareTag("Enemy")) Debug.Log("Hit enemy");
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void CharacterJump()
    {
        if (!_isGrounded || !_canCharacterMove) return;
        _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
}