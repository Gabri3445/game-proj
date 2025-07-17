using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 0.2f;
    public float sprintSpeed = 0.3f;
    public float mouseSensitivity = 150f; //You can change the number any numbers you want, but always put f after.
    public new Transform transform;
    private CharacterController _controller;
    private InputAction _look;
    private InputAction _move;
    private InputAction _sprint;

    private float _xRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _move = InputSystem.actions.FindAction("Move");
        _sprint = InputSystem.actions.FindAction("Sprint");
        _look = InputSystem.actions.FindAction("Look");
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        var input = _move.ReadValue<Vector2>();
        var sprint = _sprint.IsPressed();
        if (!sprint)
        {
            _controller.Move(new Vector3(input.x, 0, input.y) * speed);
            return;
        }

        _controller.Move(new Vector3(input.x, 0, input.y) * sprintSpeed);
        var mouse = _look.ReadValue<Vector2>();
        mouse.y *= mouseSensitivity;
        mouse.x *= mouseSensitivity;
        _xRotation -= mouse.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouse.x);
    }
}