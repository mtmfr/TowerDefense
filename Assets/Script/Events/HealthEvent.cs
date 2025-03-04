using UnityEngine;
using System;

public static class HealthEvent
{
    public static event Action<int, int> OnDamageReceived;
    public static void InflictDamage(int damagedObjectInstanceId, int damageDealt)
    {
        OnDamageReceived?.Invoke(damagedObjectInstanceId, damageDealt);
    }

    public static event Action<int, int> OnHealthHealed;
    public static void Heal(int healedObjectInstanceId, int heathRegained)
    {
        OnHealthHealed?.Invoke(healedObjectInstanceId, heathRegained);
    }
}
