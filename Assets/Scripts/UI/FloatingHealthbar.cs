using UnityEngine;
using Unity.Netcode;

public class FloatingHealthbar : NetworkBehaviour
{
    
    private Camera camera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            GetComponentInChildren<FloatingHealthbar>().enabled = false;
            return;
        }
        
        camera = GetComponentInChildren<Camera>();
    }
    
    void Update()
    {
        Vector3 directionToCamera = camera.transform.position - transform.position;
        directionToCamera.y = 0;
        
        transform.rotation = Quaternion.LookRotation(directionToCamera);
    }
}
