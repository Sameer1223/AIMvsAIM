using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;
    private CharacterController controller;

    [SerializeField] private int playerLayer = 3;
    private Weapon currentWeapon;
    public string selectedWeapon = "Clicking";
    //public ClickingWeapon currentWeapon;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        currentWeapon = selectedWeapon switch
        {
            "Clicking" => GetComponent<ClickingWeapon>(),
            "Tracking" => GetComponent<TrackingWeapon>(),
            _ => currentWeapon
        };
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        gameObject.layer = playerLayer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = playerLayer;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        HandleMovement();
        HandleJump();
        HandleFiring();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // Send position update to the network
        UpdatePositionServerRpc(transform.position);
    }

    private void HandleMovement()
    {
        // Ground detection
        isGrounded = controller.isGrounded;

        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to the player's rotation
        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep player grounded
        }

        // Apply movement
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move the player using the CharacterController
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleFiring()
    {
        switch (selectedWeapon)
        {
            case "Clicking" when Input.GetButtonDown("Fire1"):
            case "Tracking" when Input.GetButton("Fire1"):
                currentWeapon.Fire();
                break;
        }
    }

    private void HandleJump()
    {
        if (!isGrounded || !Input.GetButtonDown("Jump")) return;
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);  // Calculate jump force

        // Send jump action to the network
        JumpClientRpc();
    }

    [ClientRpc]
    private void JumpClientRpc()
    {
        if (!IsOwner)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);  // Simulate jump on remote clients
        }
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 newPosition)
    {
        // Update position for all clients
        UpdatePositionClientRpc(newPosition);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            // Lerp to smooth the transition of remote player position
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothTime);
        }
    }
}
