using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [HideInInspector] public Vector3 velocityDirection;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    private float _gravityForce = 9.8f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        MoveCharacter(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        RotateCharacter(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        GravityHandling();        
    }

    public void MoveCharacter(Vector3 moveDirection)
    {
        velocityDirection.x = moveDirection.x * _moveSpeed;
        velocityDirection.z = moveDirection.z * _moveSpeed;
        controller.Move(velocityDirection * Time.deltaTime);
    }

    public void RotateCharacter(Vector3 moveDirection)
    {
        if (Vector3.Angle(transform.forward, moveDirection) > 0)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, moveDirection, _rotateSpeed, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void GravityHandling()
    {
        if (!controller.isGrounded)
        {
            velocityDirection.y -= _gravityForce * Time.deltaTime;
        }
        else
        {
            velocityDirection.y = -0.5f;
        }
    }
}
