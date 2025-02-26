using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int health;

    private void OnEnable()
    {
        health = maxHealth;

        HealthEvent.OnDamageReceived += TakeDamage;

        GameManager.OnGameStateChange += SetHealthUI;
    }

    private void OnDisable()
    {
        HealthEvent.OnDamageReceived -= TakeDamage;

        GameManager.OnGameStateChange -= SetHealthUI;
    }

    private void SetHealthUI(GameState gameState)
    {
        if (gameState != GameState.Game)
            return;

        GameUIEvent.SetHealth(maxHealth);
    }

    private void TakeDamage(int id, int damageTaken)
    {
        if (id != gameObject.GetInstanceID())
            return;

        if (health - damageTaken > 0)
            health -= damageTaken;
        else
            Death();

        GameUIEvent.UpdateHealth(health);
    }

    private void Death()
    {
        GameManager.UpdateGameState(GameState.GameOver);
    }
}
