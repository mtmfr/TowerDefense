using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseTurrets : MonoBehaviour
{
    [field: SerializeField] public List<SO_Turrets> turretsStats { get; protected set; }
    private int level = 1;
    private int maxLevel;

    [SerializeField] protected GameObject turretBarrel;
    [SerializeField] protected List<Transform> muzzleEnd;
    protected SphereCollider col;

    protected MuzzleFlash shotEffect;

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

    protected List<Enemy> enemiesInRange = new();
    protected Enemy enemyToTarget;

    private void Start()
    {
        maxLevel = turretsStats.Count;

        SetStats(level - 1, true);

        col = GetComponent<SphereCollider>();
        col.radius = attackRange;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += ClearEnemiesInRange;
        SelectorEvent.OnLevelUp += LevelUp;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= ClearEnemiesInRange;
        SelectorEvent.OnLevelUp -= LevelUp;
    }

    private void Update()
    {
        Attack();
    }

    #region Trigger events

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
                enemyToTarget = GetClosestEnemy();
        }

        if (enemiesInRange.Count == 0)
            attackTimer = timeBetweenAttack;
    }
    #endregion

    #region stats setter 
    private void SetStats(int id, bool setShotEffect = false)
    {
        attackPower = turretsStats[id].attack;
        attackRange = turretsStats[id].range;

        timeBetweenAttack = turretsStats[id].attackCooldown;
        timeBetweenBullets = turretsStats[id].bulletsCooldown;

        numberOfBullet = turretsStats[id].bulletsToFire;

        if (setShotEffect == true)
            shotEffect = turretsStats[id].firedEffect;
    }

    private void LevelUp(BaseTurrets upgradedTurret)
    {
        //Check if the turret can gain one level
        if (upgradedTurret != this)
            return;

        if (level >= maxLevel || level + 1 > maxLevel)
            return;

        //Check for gold
        if (GameManager.gold - turretsStats[level].cost < 0)
            return;

        //Level up;
        level++;

        int id = level - 1;

        GameManager.UseGold(turretsStats[id].cost);

        SetStats(id);
    }
    #endregion

    #region enemies in range
    private void ClearEnemiesInRange(GameState gameState)
    {
        if (gameState != GameState.Shop)
            return;

        enemiesInRange.Clear();
    }

    protected Enemy GetClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
            return null;

        Enemy closestEnemy = enemiesInRange.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();

        return closestEnemy;
    }
    #endregion

    #region attack
    protected abstract void Attack();

    /// <summary>
    /// Check if the turrets can attack enemies
    /// </summary>
    /// <returns>true if the cooldown is finished and there is an enemy to attack</returns>
    protected bool CanAttackEnemy(Enemy target)
    {
        if (attackTimer < timeBetweenAttack)
        {
            attackTimer += Time.deltaTime;
            return false;
        }

        if (target == null || !target.isActiveAndEnabled)
        {
            enemiesInRange.Remove(target);
            return false;
        }

        return true;
    }
    #endregion

    protected void LookAtEnemy(Enemy enemyToLook)
    {
        if (!enemyToLook)
            return;

        Vector3 enemyPosition = enemyToLook.transform.position;
        enemyPosition.y = turretBarrel.transform.position.y;
        turretBarrel.transform.LookAt(enemyPosition);
    }

}