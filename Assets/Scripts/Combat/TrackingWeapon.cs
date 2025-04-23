using System;
using UnityEngine;
using Unity.Netcode;

public class TrackingWeapon : Weapon
{
    public LayerMask enemyPlayerLayer;
    public LayerMask hittableLayer;
    private Transform _cameraTransform;
    public float fireRate = 0.2f;

    private float _lastFireTime;
    
    private void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }
    
    public override void Fire()
    {
        if (Time.time < _lastFireTime + fireRate) return;
        
        _lastFireTime = Time.time;
        
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, Mathf.Infinity,
                hittableLayer))
            return;
        
        if (((1 << hit.collider.gameObject.layer) & enemyPlayerLayer) == 0) return;
        if (hit.transform.parent.TryGetComponent(out Health health))
        {
            health.TakeDamageServerRpc(damage);
        }
    }
}
