using System.Collections;
using UnityEngine;

public class ClickingWeapon : Weapon
{
    public LayerMask enemyPlayerLayer;
    public LayerMask hittableLayer;
    private Transform _cameraTransform;
    private int multiplier = 1;
    
    // public int magazineSize = 4;
    // public float reloadTime = 2f;
    //
    // private int currentAmmo;
    // private bool isReloading = false;

    private void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    // private void Start()
    // {
    //     currentAmmo = magazineSize;
    // }

    public override void Fire()
    {
        // if (isReloading || currentAmmo <= 0) return;

        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, Mathf.Infinity,
                hittableLayer))
        {
            //ConsumeAmmo();
            return;
        }

        if (((1 << hit.collider.gameObject.layer) & enemyPlayerLayer) == 0)
        {
            //ConsumeAmmo();
            return;
        }
        multiplier = hit.collider.gameObject.CompareTag("Head") ? 3 : 1;

        int calculatedDamage = damage * multiplier;
        if (hit.transform.parent.TryGetComponent(out Health health))
        {
            health.TakeDamageServerRpc(calculatedDamage);
        }
    }
    
    // private IEnumerator Reload()
    // {
    //     isReloading = true;
    //     Debug.Log("Reloading...");
    //
    //     yield return new WaitForSeconds(reloadTime);
    //
    //     currentAmmo = magazineSize;
    //     isReloading = false;
    //     Debug.Log("Reload complete.");
    // }
    
    // private void ConsumeAmmo()
    // {
    //     currentAmmo--;
    //     if (currentAmmo <= 0)
    //     {
    //         StartCoroutine(Reload());
    //     }
    // }
}
