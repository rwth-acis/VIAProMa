using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which spawn a number of 3D objects as 3D object particles
/// Do not use this in huge numbers or with complex objects
/// </summary>
public class Particles3D : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPrefabs;
    [SerializeField] private float gapBetweenSpawns = 10f;

    private GameObject[] objects;
    private IEnumerator spawningCoroutine;
    private bool spawning;

    public bool Spawning
    {
        get
        {
            return spawning;
        }
        set
        {
            spawning = value;
            if (spawning)
            {
                spawningCoroutine = Spawn();
                StartCoroutine(spawningCoroutine);
            }
            else
            {
                if (spawningCoroutine != null)
                {
                    StopCoroutine(spawningCoroutine);
                }
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i].SetActive(false);
                }
            }
        }
    }

    private void Awake()
    {
        objects = new GameObject[objectPrefabs.Length];
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            objects[i] = Instantiate(objectPrefabs[i]);
            objects[i].SetActive(false);
            objects[i].transform.position = transform.position;
        }
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, objects.Length - 1);
            objects[randomIndex].transform.position = transform.position;
            objects[randomIndex].SetActive(true);
            yield return new WaitForSeconds(gapBetweenSpawns);
        }
    }


}
