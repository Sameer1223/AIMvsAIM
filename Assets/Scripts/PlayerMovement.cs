using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float gravity = -9.81f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;
        
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool jumpInput = Input.GetButtonDown("Jump");
        
        MovePlayerServerRpc(movementInput, jumpInput);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector2 movementInput, bool jumpInput)
    {
        MovePlayer(movementInput, jumpInput);
    }

    private void MovePlayer(Vector2 movementInput, bool jumpInput)
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        
        Vector3 movement = (transform.right * movementInput.x + transform.forward * movementInput.y).normalized;
        controller.Move(movement * (moveSpeed * Time.deltaTime));
        
        if (jumpInput && isGrounded) velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
