using Unity.Netcode;
using UnityEngine;

public class FirstPersonView : NetworkBehaviour
{
    
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float minPitch = -90f;
    public float maxPitch = 90f;
    
    private float pitch = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) cameraTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
