using Unity.Netcode;
using UnityEngine;

public class FirstPersonView : NetworkBehaviour
{
    
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float minPitch = -90f;
    public float maxPitch = 90f;
    public const float valorantSensMultiplier = 3.18f;
    
    private float pitch = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Camera.main.fieldOfView = PlayerPrefs.GetFloat("FOV", 103f);
        
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f) * valorantSensMultiplier;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) cameraTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
