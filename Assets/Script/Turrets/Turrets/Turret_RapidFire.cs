using Unity.VisualScripting;
using UnityEngine;

public class Turret_RapidFire : BaseTurrets
{
    bool canAttack = false;
    int muzzleEndId;

    protected override void Attack()
    {
        if (canAttack == true)
        {
            Vector3 enemyPosition = enemyToTarget.transform.position;
            enemyPosition.y = turretBarrel.transform.position.y;
            turretBarrel.transform.LookAt(enemyPosition);

            if (bulletShot < numberOfBullet)
            {
                if (bulletTimer >= timeBetweenBullets)
                {
                    muzzleEndId = bulletShot % 2;

                    MuzzleFlash flash = ObjectPool.GetObject(shotEffect, muzzleEnd[muzzleEndId].position, muzzleEnd[muzzleEndId].rotation);
                    flash.StartEffect();
                    HealthEvent.InflictDamage(enemyToTarget.gameObject.GetInstanceID(), attackPower);
                    bulletTimer = 0;

                    bulletShot++;
                }
                else bulletTimer += Time.deltaTime;
            }
            else canAttack = false;
        }
        else
        {
            if (attackTimer >= timeBetweenAttack)
            {
                enemyToTarget = GetClosestEnemy();

                if (enemyToTarget == null)
                {
                    attackTimer += Time.deltaTime;
                    return;
                }                    

                if (enemyToTarget.gameObject.activeInHierarchy == false)
                {
                    enemyToTarget = null;
                    return;
                }

                bulletShot = 0;
                canAttack = true;
                attackTimer = 0;
            }
            else attackTimer += Time.deltaTime;
        }
    }
}