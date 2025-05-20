using System.Collections;
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
    private Coroutine _flashRoutine;
    private readonly Color32 _flashColor = new Color32(255, 70, 84, 255);
    private readonly Color32 _healthColor = new Color32(104, 238, 125, 255);
    public DamageFlash df;
    private bool isInvulnerable = false;


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
        if (isInvulnerable || currentHealth <= 0) return;
        currentHealth -= damage;
        
        Debug.Log($"New player health: {currentHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleDeathServerRpc();
        }
        
        UpdateHealthClientRpc(currentHealth, true);
    }

    private IEnumerator FlashHealthText()
    {
        healthText.color = _flashColor;
        yield return new WaitForSeconds(0.2f);
        healthText.color = _healthColor;
        
        df.Flash();
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth, bool shouldFlash)
    {
        healthText.text = newHealth.ToString();
        healthBarFill.fillAmount = (float)newHealth / maxHealth;

        // Only flash if it's local player and shouldFlash is true
        if (!IsOwner || !shouldFlash || newHealth <= 0)
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
                _flashRoutine = null;
            }
            return;
        }

        if (_flashRoutine != null)
        {
            //StopCoroutine(_flashRoutine);
        }
        _flashRoutine = StartCoroutine(FlashHealthText());
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ResetHealthServerRpc()
    {
        currentHealth = maxHealth;
        isInvulnerable = true;
        
        UpdateHealthClientRpc(currentHealth, false);
        StartCoroutine(RemoveInvulnerabilityAfterDelay(0.5f)); // half-second protection
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
            _flashRoutine = null;
            df.Reset();
        }
    }

    private IEnumerator RemoveInvulnerabilityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isInvulnerable = false;
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
