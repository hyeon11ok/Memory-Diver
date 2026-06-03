using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Movement3D_Physics movement;
    private Vector3 inputDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<Movement3D_Physics>();
    }

    void FixedUpdate()
    {
        movement.Move_Velocity(inputDir, 5f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed || context.canceled)
        {
            Vector2 input = context.ReadValue<Vector2>();
            inputDir = new Vector3(input.x, 0, input.y);
        }
    }
}
