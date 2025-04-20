using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; set; }
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
        Vector3 spawnPos = new Vector3(
            Random.Range(xBounds.x, xBounds.y),
            yValue,
            Random.Range(zBounds.x, zBounds.y)
        );
        
        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
    
    public Vector3 GetNewSpawnPosition()
    {
        return new Vector3(
            Random.Range(xBounds.x, xBounds.y),
            yValue,
            Random.Range(zBounds.x, zBounds.y)
        );
    }
    
}
