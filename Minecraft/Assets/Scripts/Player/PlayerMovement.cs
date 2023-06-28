using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;

    public float jumpHeight = 3f;

    Vector3 velocity;
    public float gravity = -9.81f;

    float x;
    float z;

    void Update()
    {
        GetAxis();
        Move();
        Jump();
        GravityHandler();
    }
    void GetAxis()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
    }

    void Move()
    {
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    void GravityHandler()
    {
        velocity.y -= gravity * Time.deltaTime;

        controller.Move(velocity * gravity * Time.deltaTime);

        if (controller.isGrounded)
        {
            //velocity.y = Mathf.Clamp(velocity.y = -gravity * Time.deltaTime, 1f, 5f);
            velocity.y = 2f;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y -= Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }
    }
}
