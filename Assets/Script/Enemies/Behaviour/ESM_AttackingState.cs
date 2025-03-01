using UnityEngine;

public class ESM_AttackingState : ESM_EnemyBaseState
{
    private int attackPower;
    private float timeBetweenAttack;
    private GameObject attackTarget;

    private float attackTimer = 0;

    private string animName;
    public ESM_AttackingState(Enemy enemy, int attackPower, float timeBetweenAttack, string animName, GameObject attackTarget = null)
    {
        this.enemy = enemy;
        this.attackPower = attackPower;
        this.timeBetweenAttack = timeBetweenAttack;
        this.animName = animName;
        this.attackTarget = attackTarget;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
    }

    public override void OnUpdate()
    {
        base.OnFixedUpdate();
        Attack();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    public override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);

        if (!collision.gameObject.TryGetComponent<Tile>(out _))
            enemy.ChangeState(new ESM_MovingState(enemy, enemy.movementSpeed, GridManager.instance.path, enemy.walkAnimName));
    }

    private void Attack()
    {
        if (attackTimer >= timeBetweenAttack)
        {
            enemy.animator.Play(animName);
            HealthEvent.InflictDamage(attackTarget.GetInstanceID(), attackPower);
            attackTimer = 0;
        }
        else attackTimer += Time.deltaTime;
    }
}
