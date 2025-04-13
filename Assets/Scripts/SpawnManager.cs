using Unity.Netcode;
using UnityEngine;
public class SpawnManager : MonoBehaviour
{
    private static SpawnManager Instance { get; set; }
    public GameObject playerPrefab;
    
    public Vector2 xBounds = new(-15f, 15f);
    public Vector2 zBounds = new(-15f, 15f);
    public float yValue = 0f;
    
    private void Awake() {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
    
    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(xBounds.x, xBounds.y),
                yValue,
                Random.Range(zBounds.x, zBounds.y)
            );

            GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }
    
}
