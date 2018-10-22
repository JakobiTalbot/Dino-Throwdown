using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // static reference to the instance
    public static T Instance { get; protected set; }
    // gets whether an instance of this singleton exists
    public static bool InstanceExists
    {
        get { return Instance != null;  }
    }

    // Awake method to associate singleton with instance
    protected virtual void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = (T)this;
        }
    }

    // OnDestroy method to clear singleton association
    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}