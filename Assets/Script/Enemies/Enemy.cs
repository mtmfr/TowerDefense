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
        delayBetweenAttack = enemySo.attackSpeed;

        currentState = new ESM_MovingState(this, movementSpeed, GridManager.instance.path);
        currentState.OnEnterState();

        HealthEvent.OnDamageReceive += ReceiveDamage;
    }

    private void OnDisable()
    {
        currentState = null;
        HealthEvent.OnDamageReceive -= ReceiveDamage;
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(collision);
    }

    public void ChangeState(ESM_EnemyBaseState nextState)
    {
        currentState.OnExitState();
        currentState = nextState;
        currentState.OnEnterState();
    }

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
}
