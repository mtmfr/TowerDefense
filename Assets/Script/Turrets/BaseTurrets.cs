using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseTurrets : MonoBehaviour
{
    [field: SerializeField] public List<SO_Turrets> turretsStats { get; protected set; }
    private int level = 1;
    private int maxLevel;

    [SerializeField] protected GameObject turretBarrel;
    protected SphereCollider col;

    [SerializeField] protected ParticleSystem shotExplosion;

    #region  turrets stats
    protected int attackPower;
    protected float attackRange;

    protected float timeBetweenAttack;
    protected float attackTimer = 0;

    protected float timeBetweenBullets;
    protected float bulletTimer = 0;

    protected int numberOfBullet;
    protected int bulletShot = 0;
    #endregion

    private List<Enemy> enemiesInRange = new();
    protected Enemy enemyToTarget;

    private void Start()
    {
        maxLevel = turretsStats.Count;
        int id = level - 1;
        attackPower = turretsStats[id].attack;
        attackRange = turretsStats[id].range;
        timeBetweenAttack = turretsStats[id].attackCooldown;
        timeBetweenBullets = turretsStats[id].bulletsCooldown;
        numberOfBullet = turretsStats[id].bulletsToFire;

        col = GetComponent<SphereCollider>();
        col.radius = attackRange;
    }

    private void OnEnable()
    {
        SelectorEvent.OnLevelUp += LevelUp;
    }

    private void OnDisable()
    {
        SelectorEvent.OnLevelUp -= LevelUp;
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
            enemiesInRange.Remove(enemy);
            if (enemyToTarget == enemy)
                enemyToTarget = null;
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

    private void LevelUp(BaseTurrets upgradedTurret)
    {
        //Check if the turret should gain one level
        if (upgradedTurret != this)
            return;

        if (level >= maxLevel)
            return;

        if (maxLevel + 1 > maxLevel)
            return;

        level++;

        //Check for gold
        if (GameManager.gold - turretsStats[level].cost < 0)
            return;

        Debug.Log("level up");
        GameManager.UseGold(turretsStats[level].cost);

        int id = level - 1;

        attackPower = turretsStats[id].attack;
        attackRange = turretsStats[id].range;
        timeBetweenAttack = turretsStats[id].attackCooldown;
        timeBetweenBullets = turretsStats[id].bulletsCooldown;
        numberOfBullet = turretsStats[id].bulletsToFire;
    }
}