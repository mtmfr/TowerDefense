using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy", menuName = "Scriptable Objects/SO_Enemy")]
public class SO_Enemy : ScriptableObject
{
    [field: SerializeField] public int health { get; protected set; }
    [field: SerializeField] public int speed { get; protected set; }
    [field: SerializeField] public int attack { get; protected set; }
    [field: SerializeField] public int goldDropped { get; protected set; }

    [Tooltip("Time (in s) between 2 attack")]
    [field: SerializeField] public float attackDelay { get; protected set; }
    [Tooltip("How much credit the wave use to have this enemy")]
    [field: SerializeField] public int waveCost { get; protected set; }

    [field: SerializeField] public AnimationClip walkAnim { get; private set; }
    [field: SerializeField] public AnimationClip attackAnim { get; private set; }
}
