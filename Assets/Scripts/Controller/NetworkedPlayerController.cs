using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayerController : NetworkBehaviour
{
    [Header("Movement Variables")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float smoothTime = 0.1f;
    
    [Header("Movement Checks")]
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;
    
    [Header("Game Object Components")]
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;
    private CharacterController controller;

    [Header("Weapon Variables")]
    [SerializeField] private int playerLayer = 3;
    private Weapon currentWeapon;
    private string selectedWeapon;
    
    [Header("Game Settings")]
    public LobbySettingsSO lobbySettingsSO;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        SetGameSettings();
    }

    private void SetGameSettings()
    {
        Vector3 bodySize;
        Vector3 headSize;
        
        switch (lobbySettingsSO.selectedSize)
        {
            case LobbySettingsSO.Size.Large:
                bodySize = new Vector3(0.7f, 0.7f, 0.7f);
                headSize = new Vector3(0.3f, 0.3f, 0.3f);
                break;
            case LobbySettingsSO.Size.Medium:
                bodySize = new Vector3(0.5f, 0.7f, 0.5f);
                headSize = new Vector3(0.25f, 0.25f, 0.25f);
                break;
            case LobbySettingsSO.Size.Small:
                bodySize = new Vector3(0.3f, 0.7f, 0.3f);
                headSize = new Vector3(0.15f, 0.15f, 0.15f);
                
                var pos = head.transform.localPosition;
                pos.y = 1.43f;
                head.transform.localPosition = pos;
                break;
            default:
                bodySize = new Vector3(0.7f, 0.7f, 0.7f);
                headSize = new Vector3(0.3f, 0.3f, 0.3f);
                break;
        }
        
        body.transform.localScale = bodySize;
        head.transform.localScale = headSize;

        moveSpeed = lobbySettingsSO.selectedSpeed switch
        {
            LobbySettingsSO.Speed.Slow => 4f,
            LobbySettingsSO.Speed.Fast => 8f,
            _ => 6f
        };
        
        jumpForce = lobbySettingsSO.selectedJumpForce switch
        {
            LobbySettingsSO.JumpForce.Low => 0.5f,
            LobbySettingsSO.JumpForce.High => 2.5f,
            _ => 1.5f
        };
        
        currentWeapon = lobbySettingsSO.selectedWeapon switch
        {
            LobbySettingsSO.AimWeapon.Clicking => GetComponent<ClickingWeapon>(),
            LobbySettingsSO.AimWeapon.Tracking => GetComponent<TrackingWeapon>(),
            _ => GetComponent<ClickingWeapon>()
        };
        
        selectedWeapon = lobbySettingsSO.selectedWeapon.ToString();
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

    // private void FixedUpdate()
    // {
    //     if (!IsOwner) return;
    //
    //     // Send position update to the network
    //     UpdatePositionServerRpc(transform.position);
    // }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        Vector3 move = moveDirection * moveSpeed;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);
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
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        
        JumpClientRpc();
    }

    [ClientRpc]
    private void JumpClientRpc()
    {
        if (!IsOwner)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
    
    // [ServerRpc(RequireOwnership = false)]
    // private void UpdatePositionServerRpc(Vector3 newPosition)
    // {
    //     UpdatePositionClientRpc(newPosition);
    // }
    //
    // [ClientRpc]
    // private void UpdatePositionClientRpc(Vector3 newPosition)
    // {
    //     if (!IsOwner)
    //     {
    //         transform.position = Vector3.Lerp(transform.position, newPosition, smoothTime);
    //     }
    // }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportServerRpc(Vector3 newPosition)
    {
        TeleportClientRpc(newPosition);
    }
    
    [ClientRpc]
    public void TeleportClientRpc(Vector3 newPosition)
    {
        if (!IsOwner) return;
        
        controller.enabled = false;
        transform.position = newPosition;
        controller.enabled = true;
        
        velocity = Vector3.zero;
    }
}
