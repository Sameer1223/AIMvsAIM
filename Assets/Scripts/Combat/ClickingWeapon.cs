using UnityEngine;

public class ClickingWeapon : Weapon
{
    public LayerMask enemyPlayerLayer;
    public LayerMask hittableLayer;
    private Transform _cameraTransform;
    private int multiplier = 1;

    private void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    public override void Fire()
    {
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, Mathf.Infinity,
                hittableLayer))
        {
            return;
        }

        if (((1 << hit.collider.gameObject.layer) & enemyPlayerLayer) == 0)
        {
            return;
        }

        multiplier = hit.collider.gameObject.CompareTag("Head") ? 3 : 1;
        int calculatedDamage = damage * multiplier;

        if (hit.transform.parent.TryGetComponent(out Health health))
        {
            if (health.currentHealth - calculatedDamage <= 0)
            {
                AudioManager.Instance.PlayKillSound();
            }
            else
            {
                AudioManager.Instance.PlayHitSound();
            }
            
            health.TakeDamageServerRpc(calculatedDamage);
        }
    }
}
