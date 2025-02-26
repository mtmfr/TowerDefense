using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ESM_MovingState : ESM_EnemyBaseState
{
    private float speed;
    private List<Tile> path;
    private List<Tile> parcouredTiles = new();

    private Tile currentTile;
    private Tile nextTile;

    private Tile destination;

    public ESM_MovingState(Enemy enemy, float speed, List<Tile> path)
    {
        this.speed = speed;
        this.path = path;
        this.enemy = enemy;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        SetTiles();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (currentTile != destination)
            MoveToNextTile();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (!collision.gameObject.TryGetComponent<Tile>(out _))
            enemy.ChangeState(new ESM_AttackingState(enemy, enemy.attackPower, enemy.delayBetweenAttack, collision.gameObject));
    }

    private void SetTiles()
    {
        List<Tile> pathCopy = new(path);
        currentTile = pathCopy.OrderBy(tile => Vector3.Distance(tile.transform.position, enemy.transform.position)).FirstOrDefault();
        destination = path[path.Count - 1];

        int nextTileId = path.IndexOf(currentTile) + 1;

        nextTile = path[nextTileId];
    }

    private void MoveToNextTile()
    {
        Vector3 dir = nextTile.transform.position - currentTile.transform.position;

        enemy.rb.linearVelocity = dir.normalized * speed;

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
