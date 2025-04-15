using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine.SocialPlatforms;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 250;
    private int _currentHealth;
    public TMP_Text healthText;

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
            HandleDeath();
        }
        
        UpdateHealthClientRpc(_currentHealth);
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    private void HandleDeath()
    {
        Debug.Log("Player is dead");
    }
}
