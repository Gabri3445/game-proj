using Input;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterInput _inputActions;
    public float speed;
    public float jumpForce;
    private bool _canCharacterMove = true;
    private bool _isSideMovementAllowed = true;
    private bool _isGrounded = true;
    private Rigidbody _rigidBody;
    private CharacterPosition _characterPosition;

    private void Awake()
    {
        _characterPosition = CharacterPosition.Center;
        _inputActions = new CharacterInput();
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Jump.performed += _ => CharacterJump();
    }

    private void CharacterJump()
    {
        if (_isGrounded && _canCharacterMove)
        {
            _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        CharacterMovement(_inputActions.Player.Move.ReadValue<Vector2>());
    }

    private void CharacterMovement(Vector2 movement)
    {
        if (movement.x != 0)
        {
            CharacterLateralMovement(movement.x);
        }
        else
        {
            _isSideMovementAllowed = true;
        }

        if (movement.y != 0)
        {
            CharacterForwardMovement(movement.y);
        }
        
    }

    void CharacterLateralMovement(float value)
    {
        if (_canCharacterMove && _isSideMovementAllowed)
        {
            _isSideMovementAllowed = false;
            if (value > 0)
            {
                if (_characterPosition is CharacterPosition.Left or CharacterPosition.Center)
                {
                    transform.Translate(Vector3.right);
                    _characterPosition++;
                }
            }
            else if (value < 0)
            {
                if (_characterPosition is CharacterPosition.Right or CharacterPosition.Center)
                {
                    transform.Translate(Vector3.left);
                    _characterPosition--;
                }
            }
        }
    }

    void CharacterForwardMovement(float value)
    {
        if (_canCharacterMove)
        {
            var movement = new Vector3(0, 0, value * Time.deltaTime);
            transform.Translate(movement * speed);
        }
    }
}
