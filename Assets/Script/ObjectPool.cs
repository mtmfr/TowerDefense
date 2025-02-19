using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPool
{
    /// <summary>
    /// Check if there are any inactive gameObject with the same type as the one in the argument
    /// </summary>
    /// <typeparam name="T">an object that inherit from the MonoBehaviour class</typeparam>
    /// <param name="objectToCheck">the objcet to get the type from</param>
    /// <returns>true if there are any inactive gameObject of the type</returns>
    public static bool IsAnyObjectInactive<T>(T objectToCheck) where T : MonoBehaviour
    {
        // Get the type of the object to check in advance to avoid calling GetType() on each iteration
        Type targetType = objectToCheck.GetType();

        // Retrieve all objects of type T (including inactive objects)
        List<T> Tobjects = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(T =>
        {
            //check the type of the current object
            bool hasRightType = T.GetType() == targetType;

            //check if the current check object is inactive
            bool isActive = T.gameObject.activeInHierarchy;

            return hasRightType && !isActive;
        }).ToList();

        return Tobjects.Count != 0;
    }
    public static bool IsAnyObjectInactive<T>(Type type) where T : MonoBehaviour
    {
        // Get the type of the object to check in advance to avoid calling GetType() on each iteration
        Type targetType = type;

        // Retrieve all objects of type T (including inactive objects)
        List<T> Tobjects = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(T =>
        {
            //check the type of the current object
            bool hasRightType = T.GetType() == targetType;

            //check if the current check object is inactive
            bool isActive = T.gameObject.activeInHierarchy;

            return hasRightType && !isActive;
        }).ToList();

        return Tobjects.Count != 0;
    }

    public static T GetInactiveObject<T>(T objectToGet) where T : MonoBehaviour
    {
        // Retrieve all objects of type T, including inactive ones
        List<T> values = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(element =>
        {
            bool type = element.GetType() == objectToGet.GetType();

            return type;
        }).ToList();

        // Find the first inactive object (inactive objects won't be active in the hierarchy)
        T inactiveObject = values.FirstOrDefault(chose => !chose.gameObject.activeInHierarchy);

        // Return the first inactive object found, or null if none found
        return inactiveObject;
    }

    public static List<T> GetActiveObjects<T>() where T : MonoBehaviour
    {
        List<T> values = GameObject.FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();

        return values;
    }
}
