using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy", menuName = "Scriptable Objects/SO_Enemy")]
public class SO_Enemy : ScriptableObject
{
    [field: SerializeField] public int health { get; protected set; }
    [field: SerializeField] public int speed { get; protected set; }
    [field: SerializeField] public int attack { get; protected set; }
    [field: SerializeField] public float attackSpeed { get; protected set; }
}
