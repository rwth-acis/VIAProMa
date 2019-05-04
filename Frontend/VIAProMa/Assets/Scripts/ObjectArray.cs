using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArray : MonoBehaviour
{
    [SerializeField] GameObject[] collection;

    public float gap;

    private BoxCollider[] colliders;

    private void Awake()
    {
        colliders = new BoxCollider[collection.Length];
        for (int i=0;i<collection.Length;i++)
        {
            collection[i].transform.parent = transform;
            colliders[i] = collection[i].GetComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        // store the rotation and reset it to identity so that we do not need to worry about it
        // in the end we will set the rotation back to what it was
        Quaternion originalRot = transform.rotation;
        transform.rotation = Quaternion.identity;
        float x = 0;
        for (int i = 0; i < collection.Length; i++)
        {
            x += colliders[i].bounds.size.x / 2f;
            collection[i].transform.localPosition = new Vector3(x, 0, 0);
            x += colliders[i].bounds.size.x / 2f + gap;
        }
        float fullWidth = x - gap; // x has now accumulated the full width of the array + one gap
        // now move the objects back by half the full width so that they are centered on the parent
        for (int i=0;i<collection.Length;i++)
        {
            collection[i].transform.localPosition -= new Vector3(fullWidth / 2f, 0, 0);
        }
        transform.rotation = originalRot;
    }
}
