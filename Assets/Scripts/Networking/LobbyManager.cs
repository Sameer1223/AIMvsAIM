using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Networking;
using NUnit.Framework;
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
    public TMP_Dropdown jumpForceDropdown;
    public Toggle reloadToggle;
    public TMP_Text playerCountText;
    public TMP_Text readyCountText;
    public TMP_Text lobbyCodeText;
    public Button readyButton;
    public Button startGameButton;
    public Button backButton;

    [Header("Settings")] 
    public LobbySettingsSO lobbySettings;

    private NetworkVariable<int> playerCount = new(0);
    private NetworkVariable<int> readyCount = new(0);
    private Dictionary<ulong, bool> readyStatus = new();
    
    private NetworkVariable<int> selectedSize = new();
    private NetworkVariable<int> selectedSpeed = new();
    private NetworkVariable<int> selectedWeapon = new();
    private NetworkVariable<int> selectedJumpForce = new();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public override void OnNetworkSpawn()
    {
        SetLobbyCodeText(RelayManager.lobbyCode);
        
        sizeDropdown.SetValueWithoutNotify(selectedSize.Value);
        speedDropdown.SetValueWithoutNotify(selectedSpeed.Value);
        weaponDropdown.SetValueWithoutNotify(selectedWeapon.Value);
        jumpForceDropdown.SetValueWithoutNotify(selectedJumpForce.Value);
        
        selectedSize.OnValueChanged += (_, newVal) => sizeDropdown.SetValueWithoutNotify(newVal);
        selectedSpeed.OnValueChanged += (_, newVal) => speedDropdown.SetValueWithoutNotify(newVal);
        selectedWeapon.OnValueChanged += (_, newVal) => weaponDropdown.SetValueWithoutNotify(newVal);
        selectedJumpForce.OnValueChanged += (_, newVal) => jumpForceDropdown.SetValueWithoutNotify(newVal);
        
        playerCount.OnValueChanged += (_, _) => UpdateCountText();
        readyCount.OnValueChanged += (_, _) => UpdateCountText();

        if (IsHost)
        {
            sizeDropdown.onValueChanged.AddListener(index => selectedSize.Value = index);
            speedDropdown.onValueChanged.AddListener(index => selectedSpeed.Value = index);
            jumpForceDropdown.onValueChanged.AddListener(index => selectedJumpForce.Value = index);
            weaponDropdown.onValueChanged.AddListener(index =>
            {
                selectedWeapon.Value = index;
                reloadToggle.interactable = selectedWeapon.Value != 1;
            });
            
            playerCount.Value++;
            readyStatus[NetworkManager.Singleton.LocalClientId] = false;
        }
        else
        {
            startGameButton.interactable = false;
            sizeDropdown.interactable = false;
            speedDropdown.interactable = false;
            weaponDropdown.interactable = false;
            jumpForceDropdown.interactable = false;
            reloadToggle.interactable = false;
            
            IncrementPlayerCountServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
        readyButton.onClick.AddListener(OnReadyClicked);
        startGameButton.onClick.AddListener(OnStartGameClickedClientRpc);
        backButton.onClick.AddListener(OnBackClicked);
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        UpdateCountText();
    }
    
    private void OnDestroy()
    {
        if (IsSpawned)
        {
            playerCount.Value--;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            readyStatus.Remove(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnReadyClicked()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        
        if (!readyStatus.ContainsKey(clientId))
            readyStatus[clientId] = false;
        
        readyStatus[clientId] = !readyStatus[clientId];
        bool isReady = readyStatus[clientId];
        
        readyButton.GetComponentInChildren<TMP_Text>().text = isReady? "Unready" : "Ready";
        RequestReadyServerRpc(clientId, isReady);
    }

    private void OnBackClicked()
    {
        if (IsHost && NetworkManager.Singleton.ConnectedClientsList.Count > 1)
        {
            HandleServerDisconnectClientRpc();
        }
        else
        {
            ShutdownAndReturnToMenu();
        }
    }
    
    private void ShutdownAndReturnToMenu()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene("Main Menu");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestReadyServerRpc(ulong clientId, bool isReady)
    {
        readyStatus[clientId] = isReady;
        readyCount.Value = CountReadyPlayers();
        startGameButton.interactable = readyCount.Value == 2;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void IncrementPlayerCountServerRpc(ulong clientId)
    {
        playerCount.Value++;
        readyStatus[clientId] = false;
    }
    
    private void SetLobbyCodeText(string lobbyCode)
    {
        var code = lobbyCode ?? "------"; 
        lobbyCodeText.text = $"Lobby Code: {code}";
    }
    
    private void UpdateCountText()
    {
        playerCountText.text = $"Players: {playerCount.Value} / 2";
        readyCountText.text = $"Ready: {readyCount.Value} / {playerCount.Value}";
    }
    
    private int CountReadyPlayers()
    {
        return readyStatus.Count(status => status.Value);
    }

    [ClientRpc]
    private void OnStartGameClickedClientRpc()
    {
        if (readyCount.Value != playerCount.Value) return;
        
        lobbySettings.selectedSize = (LobbySettingsSO.Size) sizeDropdown.value;
        lobbySettings.selectedSpeed = (LobbySettingsSO.Speed) speedDropdown.value;
        lobbySettings.selectedWeapon = (LobbySettingsSO.AimWeapon) weaponDropdown.value;
        lobbySettings.selectedJumpForce = (LobbySettingsSO.JumpForce) jumpForceDropdown.value;
            
        NetworkManager.Singleton.SceneManager.LoadScene("Arena", LoadSceneMode.Single);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsHost)
        {
            HandleClientDisconnectServerRpc(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleClientDisconnectServerRpc(ulong clientId)
    {
        if (readyStatus.TryGetValue(clientId, out bool wasReady) && wasReady)
        {
            readyCount.Value = Mathf.Max(readyCount.Value - 1, 0);
        }

        readyStatus.Remove(clientId);
        playerCount.Value = Mathf.Max(playerCount.Value - 1, 0);
    }

    [ClientRpc]
    private void HandleServerDisconnectClientRpc()
    {
        ShutdownAndReturnToMenu();
    }
}
