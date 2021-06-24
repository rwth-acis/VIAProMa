using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject minCorner;
    [SerializeField] private GameObject maxCorner;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private List<GameObject> trackedObjects;
    [SerializeField] private List<GameObject> miniObjects;
    private float currentScale;
    private Vector3 globalCenter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLocalTransform();
        int i = 0;
        foreach (GameObject g in trackedObjects) {
            miniObjects[i].transform.localPosition = TranslateIntoLocalCoordinates(g.transform.position);
            miniObjects[i].transform.localScale = g.transform.lossyScale * currentScale;
            i++;
        }
    }


    //Calculates the GlobalCenter as the middle of the Axis Aligned Bounding Cuboid(AABC) of the tracked objects, and the current scale As the ratio between the local scale length of the cube spaned by the reference corners and the largest dimension of the AABC. Then saves these values in the according variables. Called Every Frame
    void CalculateLocalTransform() {
        float localXLength = maxCorner.transform.localPosition.x - minCorner.transform.localPosition.x;
        float globalMaxX = float.MinValue;
        float globalMinX = float.MaxValue;
        float globalMaxY = float.MinValue;
        float globalMinY = float.MaxValue;
        float globalMaxZ = float.MinValue;
        float globalMinZ = float.MaxValue;
        foreach (GameObject g in trackedObjects) {
            if (g.transform.position.x > globalMaxX) {
                globalMaxX = g.transform.position.x;
            }
            if (g.transform.position.x < globalMinX)
            {
                globalMinX = g.transform.position.x;
            }
            if (g.transform.position.y > globalMaxY)
            {
                globalMaxY = g.transform.position.y;
            }
            if (g.transform.position.y < globalMinY)
            {
                globalMinY = g.transform.position.y;
            }
            if (g.transform.position.z > globalMaxZ)
            {
                globalMaxZ = g.transform.position.z;
            }
            if (g.transform.position.z < globalMinZ)
            {
                globalMinZ = g.transform.position.z;
            }
        }

        globalCenter = new Vector3((globalMaxX+globalMinX)/2, (globalMaxY + globalMinY) / 2, (globalMaxZ + globalMinZ) / 2);
        float largestDimension = Mathf.Max(globalMaxX - globalMinX, globalMaxZ - globalMinZ);
        if (largestDimension == 0)
        {
            currentScale = maxScale;
        }
        else if (localXLength/2 / largestDimension < minScale)
        {
            currentScale = minScale;
        }
        else if (localXLength/2 / largestDimension > maxScale)
        {
            currentScale = maxScale;
        }
        else {
            currentScale = localXLength/2 / largestDimension;
        }
    }
    
    //Gets the global position of a tracked object and returns the  corresponding local position of the corresponding mini-object in the coordinate system of the minimap
    private Vector3 TranslateIntoLocalCoordinates(Vector3 globalPos) {
        Vector3 localPos = globalPos - globalCenter;
        localPos = localPos * currentScale;
        return localPos;
    }
}
