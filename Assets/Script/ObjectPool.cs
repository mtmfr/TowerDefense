using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool
{
    private static List<MonoBehaviour> inactiveObjectValues = new();
    private static List<MonoBehaviour> activeObjectValues = new();

    private static Dictionary<Type, List<MonoBehaviour>> inactiveObjects = new();
    private static Dictionary<Type, List<MonoBehaviour>> activeObjects = new();

    public static T GetObject<T>(T toActivate, Vector3 position, Quaternion rotation) where T : MonoBehaviour
    {
        Type toActivateType = toActivate.GetType();

        if (!inactiveObjects.ContainsKey(toActivateType))
        {
            toActivate = GameObject.Instantiate(toActivate, position, rotation);

            activeObjectValues.Add(toActivate);

            if (!activeObjects.ContainsKey(toActivateType))
                activeObjects.Add(toActivateType, activeObjectValues);

            return toActivate;
        }

        toActivate = GetObjectToActivate(toActivate);

        toActivate.transform.SetPositionAndRotation(position, rotation);

        AddToActiveObjectValue(toActivate);

        if (!activeObjects.ContainsKey(toActivateType))
            activeObjects.Add(toActivateType, activeObjectValues);

        return toActivate;
    }

    private static T GetObjectToActivate<T>(T objectToActivate) where T : MonoBehaviour
    {
        T toActivate = null;
        Type type = objectToActivate.GetType();

        foreach (MonoBehaviour inactive in inactiveObjectValues)
        {
            if (inactive.GetType() == type)
            {
                toActivate = (T)inactive;
                break;
            }
        }

        if (toActivate != null)
        {
            
            toActivate.gameObject.SetActive(true);
            return toActivate;
        }

        toActivate = GameObject.Instantiate(objectToActivate);

        return toActivate;
    }

    public static void SetObjectInactive<T>(T toDeactivate) where T : MonoBehaviour
    {
        Type toDeactivateType = toDeactivate.GetType();

        if (!activeObjects.ContainsKey(toDeactivateType))
            return;

        toDeactivate.gameObject.SetActive(false);

        AddToInactiveObjectValue(toDeactivate);

        if (!inactiveObjects.ContainsKey(toDeactivateType))
            inactiveObjects.Add(toDeactivateType, inactiveObjectValues);
    }

    #region Control active & inactive values
    private static void AddToInactiveObjectValue(MonoBehaviour deactivated)
    {
        inactiveObjectValues.Add(deactivated);
        activeObjectValues.Remove(deactivated);
    }

    private static void AddToActiveObjectValue(MonoBehaviour activated)
    {
        inactiveObjectValues.Remove(activated);
        activeObjectValues.Add(activated);
    }
    #endregion
}
