using System.Collections;
using UnityEngine;

public class TestTurret : BaseTurrets
{
    protected override void Attack()
    {
        Enemy enemyToTarget = GetClosestEnemy();

        if (enemyToTarget == null)
            return;

        Vector3 enemyPosition = enemyToTarget.transform.position;
        enemyPosition.y = turretBarrel.transform.position.y;
        turretBarrel.transform.LookAt(enemyPosition);

        if (attackTimer > TimeBetweenAttack)
        {
            HealthEvent.InflictDamage(enemyToTarget.gameObject.GetInstanceID(), attackPower);
            attackTimer = 0;
        }            
        else
            attackTimer += Time.deltaTime;
    }
}
