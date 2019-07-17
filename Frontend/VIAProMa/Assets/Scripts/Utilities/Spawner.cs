using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;

    protected GameObject instance;

    public GameObject SpawnedInstance { get => instance; }

    protected virtual void Awake()
    {
        instance = Instantiate(prefab);
        Setup();
    }

    protected virtual void Setup()
    {
    }
}
