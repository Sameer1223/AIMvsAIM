using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 250;
    private int _currentHealth;
    public TMP_Text healthText;
    public Image healthBarFill;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            healthText.enabled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        _currentHealth -= damage;
        
        Debug.Log($"New player health: {_currentHealth}");
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            HandleDeathServerRpc();
        }
        
        UpdateHealthClientRpc(_currentHealth);
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        healthText.text = newHealth.ToString();
        healthBarFill.fillAmount = (float) newHealth / maxHealth;
    }
    
    [ClientRpc]
    private void ResetHealthClientRpc()
    {
        _currentHealth = maxHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleDeathServerRpc()
    {
        ulong deadClientId = OwnerClientId;
        ulong winnerClientId = GetOpponentClientId(deadClientId);

        GameLoop.Instance.EndRound(winnerClientId);
        
        //gameObject.SetActive(false);
        ResetHealthClientRpc();
    }
    
    private ulong GetOpponentClientId(ulong deadClientId)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key != deadClientId)
                return client.Key;
        }
        return 0;
    }
}
