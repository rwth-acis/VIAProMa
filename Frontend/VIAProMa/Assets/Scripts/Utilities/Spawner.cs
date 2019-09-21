using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] private bool destroyWithSpawner = true;

    protected GameObject instance;

    public GameObject SpawnedInstance { get => instance; }

    public bool DestoryWithSpawner { get => destroyWithSpawner; set => destroyWithSpawner = value; }

    protected virtual void Awake()
    {
        instance = Instantiate(prefab);
        Setup();
    }

    protected virtual void Setup()
    {
    }

    protected virtual void OnDestroy()
    {
        if (destroyWithSpawner && SpawnedInstance != null)
        {
            Destroy(SpawnedInstance);
        }
    }
}
