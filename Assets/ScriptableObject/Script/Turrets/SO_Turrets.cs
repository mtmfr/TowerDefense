using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Turrets", menuName = "Scriptable Objects/SO_Turrets")]
public class SO_Turrets : ScriptableObject
{
    [field: SerializeField] public int attack { get; private set; }
    [field: SerializeField] public float range { get; private set; }
    [field: SerializeField] public float attackCooldown { get; private set; }
}
