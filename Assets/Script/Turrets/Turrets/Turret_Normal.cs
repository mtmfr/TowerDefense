using UnityEngine;

public class Turret_Normal : BaseTurrets
{
    protected override void Attack()
    {
        enemyToTarget = GetClosestEnemy();

        if (enemyToTarget == null)
        {
            attackTimer += Time.deltaTime;
            return;
        }

        if (enemyToTarget.gameObject.activeInHierarchy == false)
        {
            enemyToTarget = GetClosestEnemy();
            attackTimer += Time.deltaTime;
            return;
        }

        Vector3 enemyPosition = enemyToTarget.transform.position;
        enemyPosition.y = turretBarrel.transform.position.y;
        turretBarrel.transform.LookAt(enemyPosition);

        if (attackTimer > timeBetweenAttack)
        {
            MuzzleFlash flash = ObjectPool.GetObject(shotEffect, muzzleEnd[0].position, muzzleEnd[0].rotation);
            flash.StartEffect();

            HealthEvent.InflictDamage(enemyToTarget.gameObject.GetInstanceID(), attackPower);
            attackTimer = 0;
        }            
        else
            attackTimer += Time.deltaTime;
    }
}
