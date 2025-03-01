using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour
{
    private ESM_EnemyBaseState currentState;

    public Rigidbody rb { get; private set; }
    private List<MeshRenderer> renderers = new();

    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material damagedColor;

    [field: SerializeField] public SO_Enemy enemySo { get; private set; }
    public int health { get; private set; }
    public int movementSpeed { get; private set; }
    public int attackPower { get; private set; }
    public float delayBetweenAttack { get; private set; }

    public Tile currentTile;

    #region Unity function
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>().ToList();
    }

    private void OnEnable()
    {
        health = enemySo.health;
        movementSpeed = enemySo.speed;
        attackPower = enemySo.attack;
        delayBetweenAttack = enemySo.attackDelay;

        currentState = new ESM_MovingState(this, movementSpeed, GridManager.instance.path);
        currentState.OnEnterState();

        HealthEvent.OnDamageReceived += ReceiveDamage;
    }

    private void OnDisable()
    {
        currentState = null;
        HealthEvent.OnDamageReceived -= ReceiveDamage;
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        currentState.OnCollisionExit(collision);
    }
    #endregion

    /// <summary>
    /// update the state of the enemy state machine
    /// </summary>
    /// <param name="nextState">the next state of the state machine. (need to be created using new())</param>
    public void ChangeState(ESM_EnemyBaseState nextState)
    {
        currentState.OnExitState();
        currentState = nextState;
        currentState.OnEnterState();
    }

    #region Damage receive
    private void ReceiveDamage(int id, int damageReceived)
    {
        if (gameObject.GetInstanceID() != id)
            return;

        if (health - damageReceived > 0)
            health -= damageReceived;
        else
        {
            Death();
            return;
        }

        StartCoroutine(TakeDamageColorChange());
    }

    private void Death()
    {
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = baseMaterial;
        }

        currentState = null;
        GameManager.AddGold(enemySo.goldDropped);
        ObjectPool.SetObjectInactive(this);
        EnemyEvent.Died(this);
        gameObject.SetActive(false);
    }

    private IEnumerator TakeDamageColorChange()
    {
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = damagedColor;
        }
        yield return new WaitForSeconds(0.2f);

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = baseMaterial;
        }
    }
    #endregion
}

public static class EnemyEvent
{
    public static event Action<Enemy> OnDeath;

    public static void Died(Enemy killedEnemy) => OnDeath?.Invoke(killedEnemy);
}