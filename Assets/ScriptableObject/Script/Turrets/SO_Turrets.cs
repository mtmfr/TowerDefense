using UnityEngine;

[CreateAssetMenu(fileName = "SO_NormalTurret", menuName = "Scriptable Objects/Turret")]
public class SO_Turrets : ScriptableObject
{
    [field: SerializeField] public int attack { get; protected set; }
    [field: SerializeField] public int cost { get; protected set; }
    [field: SerializeField] public float range { get; protected set; }
    [field: SerializeField] public float attackCooldown { get; protected set; }

    [Header("Only useful if the object shoot multiple bullets in one attack")]
    [field: SerializeField] public float bulletsCooldown { get; private set; }
    [field: SerializeField] public int bulletsToFire { get; private set; }
}
