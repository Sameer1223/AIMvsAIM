using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : NetworkBehaviour
{
    public static GameLoop Instance { get; set; }
    
    private int playerOneScore;
    private int playerTwoScore;
    [SerializeField] private TMP_Text p1Text;
    [SerializeField] private TMP_Text p2Text;
    [SerializeField] private TMP_Text gameEndText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartGame();
        HighlightLocalPlayerScore();
    }

    private void StartGame()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
        
        UpdateScoreClientRpc(playerOneScore, playerTwoScore);
    }
    
    private void HighlightLocalPlayerScore()
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;

        if (localId == 0)
        {
            p1Text.color= new Color32(202, 163, 104, 255);
        }
        else if (localId == 1)
        {
            p2Text.color= new Color32(202, 163, 104, 255);
        }
    }

    public void EndRound(ulong winnerClientId)
    {
        if (winnerClientId == 0)
        {
            playerOneScore++;
        }
        else
        {
            playerTwoScore++;
        }
        
        UpdateScoreClientRpc(playerOneScore, playerTwoScore);

        if (Mathf.Max(playerOneScore, playerTwoScore) == 7)
        {
            EndGame();
        }
        
        ResetAllPlayersHealth();
        
        foreach (var player in FindObjectsOfType<NetworkedPlayerController>())
        {
            player.TeleportServerRpc(SpawnManager.Instance.GetNewSpawnPosition(player.OwnerClientId));
        }
    }
    
    private void EndGame()
    {
        ulong winnerClientId = playerOneScore > playerTwoScore ? 0ul : 1ul;
        ShowEndGameMessageClientRpc(winnerClientId);
    }
    
    [ClientRpc]
    private void ShowEndGameMessageClientRpc(ulong winnerClientId)
    {
        bool isVictory = NetworkManager.Singleton.LocalClientId == winnerClientId;
        
        gameEndText.gameObject.SetActive(true);
        gameEndText.text = isVictory ? "Victory!" : "Defeat!";
        gameEndText.color = isVictory ? new Color32(150, 255, 196, 255) : new Color32(246, 98, 112, 255);
        
        StartCoroutine(DelayedSceneLoad());
    }

    public IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(4f);
        gameEndText.gameObject.SetActive(false);
        if (IsServer)
        {
            foreach (var player in FindObjectsOfType<NetworkedPlayerController>())
            {
                if (player.NetworkObject.IsSpawned)
                {
                    player.NetworkObject.Despawn(true);
                }
            }
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
        }
    }
    
    public static void ResetAllPlayersHealth()
    {
        foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
        {
            var clientObject = clientPair.Value.PlayerObject;
            if (clientObject == null) continue;

            var health = clientObject.GetComponent<Health>();
            if (health != null)
            {
                health.ResetHealthServerRpc();
            }
        }
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int p1Score, int p2Score)
    {
        p1Text.text = p1Score.ToString();
        p2Text.text = p2Score.ToString();
    }
}
