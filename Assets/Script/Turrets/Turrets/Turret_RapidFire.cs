using Unity.VisualScripting;
using UnityEngine;

public class Turret_RapidFire : BaseTurrets
{
    bool canAttack = false;
    protected override void Attack()
    {
        if (canAttack == true)
        {
            if (bulletShot < numberOfBullet)
            {

                if (bulletTimer >= timeBetweenBullets)
                {
                    Vector3 enemyPosition = enemyToTarget.transform.position;
                    enemyPosition.y = turretBarrel.transform.position.y;
                    turretBarrel.transform.LookAt(enemyPosition);

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
                    return;

                bulletShot = 0;
                canAttack = true;
                attackTimer = 0;
            }
            else attackTimer += Time.deltaTime;
        }
    }
}