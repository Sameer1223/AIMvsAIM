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
    }

    private void StartGame()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
        
        UpdateScoreClientRpc(playerOneScore, playerTwoScore);
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

        foreach (var player in FindObjectsOfType<NetworkedPlayerController>())
        {
            player.TeleportServerRpc(SpawnManager.Instance.GetNewSpawnPosition());
        }
    }
    
    private void EndGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int p1Score, int p2Score)
    {
        p1Text.text = p1Score.ToString();
        p2Text.text = p2Score.ToString();
    }
}
