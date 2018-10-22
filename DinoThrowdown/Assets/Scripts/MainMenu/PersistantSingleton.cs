using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton that persists across multiple scenes
public class PersistantSingleton<T> : Singleton<T> where T : Singleton<T>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}