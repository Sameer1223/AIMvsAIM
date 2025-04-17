using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown sizeDropdown;
    public TMP_Dropdown speedDropdown;
    public TMP_Dropdown weaponDropdown;
    public TMP_Text playerCountText;
    public TMP_Text readyCountText;
    public Button readyButton;
    public Button startGameButton;

    [Header("Settings")] 
    public LobbySettingsSO lobbySettings;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0);
    private NetworkVariable<int> readyCount = new NetworkVariable<int>(0);
    private Dictionary<ulong, bool> readyStatus = new Dictionary<ulong, bool>();

    private void Start()
    {
        readyButton.onClick.AddListener(OnReadyClicked);
        startGameButton.onClick.AddListener(OnStartGameClicked);

        if (!IsHost)
        {
            startGameButton.interactable = false;
            sizeDropdown.interactable = false;
            speedDropdown.interactable = false;
            weaponDropdown.interactable = false;
        }

        //UpdateUI();
    }

    public override void OnNetworkSpawn()
    {
        playerCount.Value++;
        readyStatus[NetworkManager.Singleton.LocalClientId] = false;
        UpdateCountsClientRpc(playerCount.Value, readyCount.Value);
    }

    private void OnDestroy()
    {
        if (IsSpawned)
        {
            playerCount.Value--;
            readyStatus.Remove(NetworkManager.Singleton.LocalClientId);
            UpdateCountsClientRpc(playerCount.Value, readyCount.Value);
        }
    }

    private void OnReadyClicked()
    {
        readyStatus[NetworkManager.Singleton.LocalClientId] = true;
        RequestReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestReadyServerRpc(ulong clientId)
    {
        readyStatus[clientId] = true;
        readyCount.Value = CountReadyPlayers();
        UpdateCountsClientRpc(playerCount.Value, readyCount.Value);
    }

    [ClientRpc]
    private void UpdateCountsClientRpc(int players, int readyPlayers)
    {
        playerCountText.text = $"Players: {players} / 2";
        readyCountText.text = $"Ready: {readyPlayers} / {players}";
    }

    private int CountReadyPlayers()
    {
        return readyStatus.Count(status => status.Value);
    }

    private void OnStartGameClicked()
    {
        if (readyCount.Value == playerCount.Value)
        {
            lobbySettings.selectedSize = (LobbySettingsSO.Size) sizeDropdown.value;
            lobbySettings.selectedSpeed = (LobbySettingsSO.Speed) speedDropdown.value;
            lobbySettings.selectedWeapon = (LobbySettingsSO.AimWeapon) weaponDropdown.value;

            NetworkManager.Singleton.SceneManager.LoadScene("Arena", LoadSceneMode.Single);
        }
    }

    // private void UpdateUI()
    // {
    //     playerCountText.text = "Players Connected: " + playerCount.Value;
    //     readyCountText.text = "Players Ready: " + readyCount.Value;
    // }
}
