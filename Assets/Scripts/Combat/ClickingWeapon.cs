using UnityEngine;

public class ClickingWeapon : Weapon
{
    public LayerMask enemyPlayerLayer;
    private Transform _cameraTransform;
    private int multiplier = 1;
    
    private void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }
    
    public override void Fire()
    {
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, Mathf.Infinity,
                enemyPlayerLayer))
            return;

        multiplier = hit.collider.gameObject.CompareTag("Head") ? 3 : 1;

        int calculatedDamage = damage * multiplier;
        if (hit.transform.parent.TryGetComponent(out Health health))
        {
            health.TakeDamageServerRpc(calculatedDamage);
        }
    }
}
