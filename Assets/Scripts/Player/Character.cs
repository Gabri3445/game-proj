using Input;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterInput _inputActions;
    public float speed;
    private bool _canCharacterMove = true;
    private bool _isSideMovementAllowed = true;

    private void Awake()
    {
        _inputActions = new CharacterInput();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Jump.performed += _ => CharacterJump();
    }

    private void CharacterJump()
    {
        
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
            if (value > 0 && transform.position.x >= 0)
            {
                transform.Translate(Vector3.right * -1);
            }
            else if (value < 0 && transform.position.x <= 0)
            {
                transform.Translate(Vector3.left * -1);
            }
        }
    }

    void CharacterForwardMovement(float value)
    {
        if (_canCharacterMove)
        {
            var movement = new Vector3(0, value * Time.deltaTime, 0);
            transform.Translate(movement * speed);
        }
    }
}
