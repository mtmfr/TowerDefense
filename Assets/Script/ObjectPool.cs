using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPool
{
    private static List<MonoBehaviour> inactiveObjects = new();
    private static List<MonoBehaviour> activeObjects = new();

    public static T GetInactive<T>(T toActivate, Vector3 position, Quaternion rotation) where T : MonoBehaviour
    {
        if (!inactiveObjects.Contains(toActivate))
            toActivate = GameObject.Instantiate(toActivate, position, rotation);
        else
        {
            toActivate.transform.SetPositionAndRotation(position, rotation);
            toActivate.gameObject.SetActive(true);

            inactiveObjects.Remove(toActivate);
        }

        activeObjects.Add(toActivate);
        return toActivate;
    }

    public static void SetObjectInactive<T>(T toDeactivate) where T : MonoBehaviour
    {
        List<T> deactivatedObject = new();

        foreach(MonoBehaviour activeObject in activeObjects)
        {
            if (activeObject.GetType() != toDeactivate.GetType())
                continue;

            toDeactivate.gameObject.SetActive(false);
            deactivatedObject.Add(toDeactivate);
        }

        foreach(T deactivated in deactivatedObject)
        {
            activeObjects.Remove(deactivated);
            inactiveObjects.Add(deactivated);
        }
    }
}
