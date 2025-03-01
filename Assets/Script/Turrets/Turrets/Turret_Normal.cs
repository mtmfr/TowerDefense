using UnityEngine;

public class Turret_Normal : BaseTurrets
{
    protected override void Attack()
    {
        enemyToTarget = GetClosestEnemy();

        if (enemyToTarget == null)
            return;

        Vector3 enemyPosition = enemyToTarget.transform.position;
        enemyPosition.y = turretBarrel.transform.position.y;
        turretBarrel.transform.LookAt(enemyPosition);

        if (attackTimer > timeBetweenAttack)
        {
            HealthEvent.InflictDamage(enemyToTarget.gameObject.GetInstanceID(), attackPower);
            attackTimer = 0;
        }            
        else
            attackTimer += Time.deltaTime;
    }
}
