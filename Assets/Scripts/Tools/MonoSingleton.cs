using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    instance = new GameObject("Singleton of" + typeof(T)).AddComponent<T>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

        }
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Init()
    {

    }
}
