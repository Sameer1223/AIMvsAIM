using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 250;
    [FormerlySerializedAs("_currentHealth")] public int currentHealth;
    public TMP_Text healthText;
    public Image healthBarFill;

    private void Awake()
    {
        currentHealth = maxHealth;
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
        currentHealth -= damage;
        
        Debug.Log($"New player health: {currentHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleDeathServerRpc();
        }
        
        UpdateHealthClientRpc(currentHealth);
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        healthText.text = newHealth.ToString();
        healthBarFill.fillAmount = (float) newHealth / maxHealth;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ResetHealthServerRpc()
    {
        currentHealth = maxHealth;
        UpdateHealthClientRpc(currentHealth);
    }


    [ServerRpc(RequireOwnership = false)]
    private void HandleDeathServerRpc()
    {
        ulong deadClientId = OwnerClientId;
        ulong winnerClientId = GetOpponentClientId(deadClientId);

        GameLoop.Instance.EndRound(winnerClientId);
        
        //gameObject.SetActive(false);
        //ResetHealthClientRpc();
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
