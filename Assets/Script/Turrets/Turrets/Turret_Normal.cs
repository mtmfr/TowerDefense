using UnityEngine;

public class Turret_Normal : BaseTurrets
{
    MuzzleFlash flash;
    protected override void Attack()
    {
        enemyToTarget = GetClosestEnemy();

        if (!CanAttackEnemy(enemyToTarget))
            return;

        LookAtEnemy(enemyToTarget);

        flash = ObjectPool.GetObject(shotEffect, muzzleEnd[0].position, muzzleEnd[0].rotation);
        flash.StartEffect();

        HealthEvent.InflictDamage(enemyToTarget.GetInstanceID(), attackPower);
        attackTimer = 0;
    }
}
