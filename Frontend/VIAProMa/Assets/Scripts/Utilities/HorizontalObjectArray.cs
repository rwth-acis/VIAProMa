using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aligns objects stored in the collection array horizontally with regard to the axis-aligned bounding boxes of the individual items
/// </summary>
public class HorizontalObjectArray : MonoBehaviour
{
    [SerializeField] GameObject[] collection = new GameObject[0];

    public float gap;

    private BoxCollider[] colliders;
    
    public GameObject[] Collection
    {
        get { return collection; }
        set { collection = value; Initialize(); }
    }

    /// <summary>
    /// Initializes the component
    /// </summary>
    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the array by fetching the box colliders of the GameObjects in the collection
    /// </summary>
    private void Initialize()
    {
        colliders = new BoxCollider[collection.Length];
        for (int i = 0; i < collection.Length; i++)
        {
            collection[i].transform.parent = transform;
            colliders[i] = collection[i].GetComponent<BoxCollider>();
        }
    }

    /// <summary>
    /// Aligns the transforms of the collection array with respect to their axis aligned bounding boxes and the gap between them
    /// </summary>
    private void Update()
    {
        if (collection.Length == 0)
        {
            return;
        }

        // store the rotation and reset it to identity so that we do not need to worry about it
        // in the end we will set the rotation back to what it was
        Quaternion originalRot = transform.rotation;
        transform.rotation = Quaternion.identity;
        float x = 0;
        for (int i = 0; i < collection.Length; i++)
        {
            Vector3 offset = colliders[i].bounds.center - colliders[i].transform.position;
            x += colliders[i].bounds.size.x / 2f;
            collection[i].transform.localPosition = new Vector3(x - offset.x, 0, 0);
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
