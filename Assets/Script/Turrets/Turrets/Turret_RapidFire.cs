using UnityEngine;

public class Turret_RapidFire : BaseTurrets
{
    MuzzleFlash flash;

    protected override void Attack()
    {
        enemyToTarget = GetClosestEnemy();

        if (!CanAttackEnemy(enemyToTarget))
            return;

        LookAtEnemy(enemyToTarget);

        if (bulletShot >= numberOfBullet)
        {
            attackTimer = 0;
            bulletShot = 0;
            return;
        }

        if (bulletTimer < timeBetweenBullets)
            bulletTimer += Time.deltaTime;
        else
        {
            int muzzleEndId = bulletShot % 2;

            flash = ObjectPool.GetObject(shotEffect, muzzleEnd[muzzleEndId].position, muzzleEnd[muzzleEndId].rotation);
            flash.StartEffect();
            HealthEvent.InflictDamage(enemyToTarget.GetInstanceID(), attackPower);
            bulletTimer = 0;

            bulletShot++;
        }
    }
}