using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; set; }
    public GameObject playerPrefab;
    
    public Vector2 xBounds = new(-3f, 3f);
    public Vector2 zBounds = new(11f, 16f);
    public float yValue = 0f;
    
    private void Awake() {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        
        if (NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (!NetworkManager.Singleton.IsHost || sceneName != "Arena") return;
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        Vector3 spawnPos = GetNewSpawnPosition(clientId);

        GameObject player = Instantiate(playerPrefab, spawnPos, clientId == 0 ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        player.GetComponent<NetworkedPlayerController>().TeleportServerRpc(spawnPos);
    }
    
    public Vector3 GetNewSpawnPosition(ulong clientId)
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(xBounds.x, xBounds.y),
            yValue,
            clientId == 0 ? Random.Range(zBounds.x, zBounds.y) : Random.Range(-zBounds.y, -zBounds.x)
        );
        
        return spawnPosition;
    }
    
}
