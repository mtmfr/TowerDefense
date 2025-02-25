using UnityEngine;

public class ESM_AttackingState : ESM_EnemyBaseState
{
    private int attackPower;
    private float attackSpeed;

    public ESM_AttackingState(Enemy enemy, int attackPower, float attackSpeed)
    {
        this.enemy = enemy;
        this.attackPower = attackPower;
        this.attackSpeed = attackSpeed;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        Debug.Log("bouh");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }
}
