using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T Instance => _instance;

    public static bool IsInitialised => _instance != null;

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("[" + typeof(T).ToString() + " => Singleton]: Tried to load a second instance of singleton");
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
