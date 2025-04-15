using System;
using UnityEngine;
using Unity.Netcode;

public class TrackingWeapon : MonoBehaviour
{
    public LayerMask layerMask;
    
    [Header("Weapon Variables")] 
    public float damagePerSecond = 15f;
    public float range = 100f;

    private bool isFiring;
    private Camera playerCamera;
    private float damageTickTime = 0.0625f;
    private float lastTickTime = 0f;

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }
    //
    // private void Update()
    // {
    //     if (!IsOwner) return;
    //     
    //     if (isFiring && Time.time - lastTickTime >= damageTickTime)
    //     {
    //         lastTickTime = Time.time;
    //         Vector3 origin = playerCamera.transform.position + playerCamera.transform.forward * 0.5f;
    //         Vector3 direction = playerCamera.transform.forward;
    //         FireRayServerRpc(origin, direction);
    //     }
    // }
    //
    // public override void HandleInput()
    // {
    //     isFiring = true;
    // }
    //
    // public override void StopFiring()
    // {
    //     isFiring = false;
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // private void FireRayServerRpc(Vector3 origin, Vector3 direction)
    // {
    //     if (Physics.Raycast(origin, direction, out RaycastHit hit, range, layerMask))
    //     {
    //         Debug.DrawRay(origin, direction * range, Color.red, 0.1f);
    //         var targetNetworkObject = hit.collider.gameObject.GetComponentInParent<NetworkObject>();
    //         if (targetNetworkObject != null && targetNetworkObject.OwnerClientId != OwnerClientId)
    //         {
    //             var health = hit.collider.GetComponentInParent<Health>();
    //             Debug.Log(health + "hit");
    //              if (health != null) health.TakeDamageServerRpc(damagePerSecond * damageTickTime);
    //         }
    //     }
    // }
}
