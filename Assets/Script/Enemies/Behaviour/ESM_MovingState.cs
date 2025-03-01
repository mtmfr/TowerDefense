using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ESM_MovingState : ESM_EnemyBaseState
{
    private float speed;
    private List<Tile> path;
    private string animName;

    private Vector3 movementDirection;

    private Tile currentTile;
    private Tile nextTile;

    private Tile destination;

    public ESM_MovingState(Enemy enemy, float speed, List<Tile> path, string animName)
    {
        this.speed = speed;
        this.path = path;
        this.enemy = enemy;
        this.animName = animName;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        SetTiles();
        enemy.animator.Play(animName);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (currentTile != destination)
            MoveToNextTile();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        currentTile = null;
        nextTile = null;
        path = null;

    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (!collision.gameObject.TryGetComponent<Tile>(out _))
            enemy.ChangeState(new ESM_AttackingState(enemy, enemy.attackPower, enemy.delayBetweenAttack, enemy.attackAnimName, collision.gameObject));
    }

    private void SetTiles()
    {
        List<Tile> pathCopy = new(path);
        currentTile = pathCopy.OrderBy(tile => Vector3.Distance(tile.transform.position, enemy.transform.position)).FirstOrDefault();
        destination = path[path.Count - 1];

        int nextTileId = path.IndexOf(currentTile) + 1;

        nextTile = path[nextTileId];
        movementDirection = nextTile.transform.position - currentTile.transform.position;
    }

    private void MoveToNextTile()
    {
        float distanceFromPos = Vector3.Distance(enemy.transform.position, nextTile.transform.position);
        float distanceBetweenTiles = Vector3.Distance(currentTile.transform.position, nextTile.transform.position);

        if (distanceFromPos > distanceBetweenTiles * 2)
            SetTiles();

        enemy.rb.linearVelocity = movementDirection.normalized * speed;

        Vector3 enemyPos = new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z);
        Vector3 nextTilePos = new Vector3(nextTile.transform.position.x, 0, nextTile.transform.position.z);

        if (IsAtTileCenter(enemyPos, nextTilePos))
            UpdateNextTile();
    }

    private void UpdateNextTile()
    {
        currentTile = nextTile;

        if (currentTile == destination)
            return;

        int nextTileId = path.IndexOf(currentTile) + 1;

        nextTile = path[nextTileId];

        movementDirection = nextTile.transform.position - currentTile.transform.position;
    }

    private bool IsAtTileCenter(Vector3 position, Vector3 nextTilePosition)
    {
        float deltaX = nextTilePosition.x - position.x;
        float deltaZ = nextTilePosition.z - position.z;

        if (Mathf.Abs(deltaX) < 0.1f && Mathf.Abs(deltaZ) < 0.1f)
            return true;

        return false;
    }
}
