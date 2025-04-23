using UnityEngine;
using Unity.Netcode;

public class FloatingHealthbar : MonoBehaviour
{
    private Camera localPlayerCamera;

    void Start()
    {
        var networkObject = GetComponentInParent<NetworkObject>();
        if (networkObject != null && networkObject.IsOwner)
        {
            enabled = false;
            return;
        }
        StartCoroutine(FindCameraWhenReady());
    }

    private System.Collections.IEnumerator FindCameraWhenReady()
    {
        while (localPlayerCamera == null)
        {
            var playerCamera = Camera.main;
            if (playerCamera != null)
            {
                localPlayerCamera = playerCamera;
                break;
            }
            yield return null; // wait a frame
        }
    }

    void LateUpdate()
    {
        if (localPlayerCamera == null) return;
        
        transform.LookAt(transform.position + localPlayerCamera.transform.rotation * Vector3.forward,
            localPlayerCamera.transform.rotation * Vector3.up);
    }
}