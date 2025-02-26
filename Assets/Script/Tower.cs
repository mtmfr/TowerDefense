using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] int health;

    private void OnEnable()
    {
        HealthEvent.OnDamageReceived += TakeDamage;
    }

    private void OnDisable()
    {
        HealthEvent.OnDamageReceived -= TakeDamage;
    }

    private void TakeDamage(int id, int damageTaken)
    {
        if (id != gameObject.GetInstanceID())
            return;

        if (health - damageTaken > 0)
            health -= damageTaken;
        else
            Death();

        Debug.Log("aieuouh");
    }

    private void Death()
    {
        GameManager.UpdateGameState(GameState.GameOver);
    }
}
