using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    public List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
    }
    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;

    }
    private void Start()
    {
        CreatePool();
    }


    public void CreatePool()
    {

        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
            e => { Destroy(e); }
            );

            poolEffectList.Add(newPool);

        }
    }

    private void OnParticleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        ObjectPool<GameObject> objPool;
        switch (effectType)
        {
            case ParticaleEffectType.LeavesFalling01:
                objPool = poolEffectList[0];
                break;
            case ParticaleEffectType.LeavesFalling02:
                objPool = poolEffectList[1];
                break;
            case ParticaleEffectType.Rock:
                objPool = poolEffectList[2];
                break;
            case ParticaleEffectType.ReapableScenery:
                objPool = poolEffectList[3];
                break;
            default:
                objPool = null;
                break;
        }
        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }


    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
        yield break;
    }
}
