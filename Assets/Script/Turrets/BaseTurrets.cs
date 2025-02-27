using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseTurrets : MonoBehaviour
{
    [SerializeField] protected SO_Turrets turretsStats;
    [SerializeField] protected GameObject turretBarrel;
    protected SphereCollider col;

    protected int attackPower;
    protected float attackRange;
    protected float timeBetweenAttack;
    protected float attackTimer = 0;

    private List<Enemy> enemiesInRange = new();

    private void Start()
    {
        attackPower = turretsStats.attack;
        attackRange = turretsStats.range;
        timeBetweenAttack = turretsStats.attackCooldown;

        col = GetComponent<SphereCollider>();
        col.radius = attackRange;
    }

    private void Update()
    {
        Attack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
            enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (enemiesInRange.Contains(enemy))
                enemiesInRange.Remove(enemy);
        }

        if (enemiesInRange.Count == 0)
            attackTimer = timeBetweenAttack;
    }

    protected abstract void Attack();

    protected Enemy GetClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
            return null;

        Enemy closestEnemy = enemiesInRange.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();

        return closestEnemy;
    }
}