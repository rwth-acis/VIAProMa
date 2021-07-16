using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.ResourceManagagement;
using UnityEngine;
using i5.VIAProMa.Visualizations.ProgressBars;

public class MiniObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject minCorner;
    [SerializeField] private GameObject maxCorner;
    [SerializeField] private GameObject scaleIndicatorObject;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float yOffset;
    // List of objects already tracked
    [SerializeField] private HashSet<GameObject> trackedObjects;

    [SerializeField] private List<GameObject> miniObjects;
    // Mini objects have their own prefab, which is spawned to represent the max object on the minimap
    [SerializeField] private List<GameObject> miniObjectPrefabs;
    [SerializeField] private GameObject miniObjectParent;
    private float currentScale;
    private Vector3 globalCenter;

    public void Awake()
    {
        // newly spawned game objects will be automatically added to the list
        ResourceManager.Instance.RegisterGameObjectSpawnedCallback(AddTrackedObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] objs = FindObjectsOfType<GameObject>();

        CalculateLocalTransform();
        scaleIndicatorObject.transform.localScale = (new Vector3(1,1,1)) *currentScale;
        int i = 0;
        foreach (GameObject g in trackedObjects) {
            miniObjects[i].transform.localPosition = TranslateIntoLocalCoordinates(g.transform.position);
            miniObjects[i].transform.localScale = g.transform.lossyScale * currentScale;
            miniObjects[i].transform.localRotation = g.transform.rotation;
            i++;
        }
    }


    //Calculates the GlobalCenter as the middle of the Axis Aligned Bounding Cuboid(AABC) of the tracked objects, and the current scale As the ratio between the local scale length of the cube spaned by the reference corners and the largest dimension of the AABC. Then saves these values in the according variables. Called Every Frame
    void CalculateLocalTransform() {
        float localXLength = maxCorner.transform.localPosition.x - minCorner.transform.localPosition.x;
        float localYLength = maxCorner.transform.localPosition.y - minCorner.transform.localPosition.y;
        float localZLength = maxCorner.transform.localPosition.z - minCorner.transform.localPosition.z;
        print("X:" + localXLength);
        print("Y:" + localYLength);
        print("Z:" + localZLength);
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
        float xDimension = globalMaxX - globalMinX;
        float yDimension = globalMaxY - globalMinY;
        float zDimension = globalMaxZ - globalMinZ;
        if (largestDimension == 0)
        {
            currentScale = maxScale;
        }
        else if (Mathf.Max((localXLength/2 / xDimension),(localZLength / 2 / zDimension))<minScale)
        {
            currentScale = minScale;
        }
        else if (Mathf.Max((localXLength / 2 / xDimension), (localZLength / 2 / zDimension)) > maxScale)
        {
            currentScale = maxScale;
        }
        else {
            currentScale = Mathf.Max((localXLength / 2 / xDimension), (localZLength / 2 / zDimension));
        }
    }
    
    //Gets the global position of a tracked object and returns the  corresponding local position of the corresponding mini-object in the coordinate system of the minimap
    private Vector3 TranslateIntoLocalCoordinates(Vector3 globalPos) {
        Vector3 localPos = globalPos - globalCenter;
        localPos = localPos * currentScale;
        localPos.y += yOffset;
        return localPos;
    }

    public void AddTrackedObject(GameObject objectToTrack) {
        if (trackedObjects.Contains(objectToTrack))
        {
            return;
        }

        trackedObjects.Add(objectToTrack);
        int miniObjectTypeIndex = GetMiniObjectTypeIndex(objectToTrack);
        GameObject newMiniobject = Instantiate(miniObjectPrefabs[miniObjectTypeIndex]);
        newMiniobject.transform.parent = miniObjectParent.transform;
        miniObjects.Add(newMiniobject);
    }

    public int GetMiniObjectTypeIndex(GameObject objectToTrack) {
        if (!(objectToTrack.GetComponents(typeof(ProgressBar)).Length == 0)) {
            return 1;
        }
        return 0;
    }
}
