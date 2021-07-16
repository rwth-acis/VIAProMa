using System;
using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.ResourceManagagement;
using UnityEngine;
using i5.VIAProMa.Visualizations.BuildingProgressBar;
using i5.VIAProMa.Visualizations.ProgressBars;
using i5.VIAProMa.Visualizations.CommitStatistics;
using i5.VIAProMa.Visualizations.KanbanBoard;
using i5.VIAProMa.Visualizations.Competence;
using i5.VIAProMa.Visualizations.Minimap;


public class MiniObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject minCorner;
    [SerializeField] private GameObject maxCorner;
    [SerializeField] private GameObject scaleIndicatorObject;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float yOffset;

    // List of objects already tracked
    private HashSet<GameObject> trackedObjects;

    // List of mini objects already spawned
    //private List<GameObject> miniObjects;
    private Dictionary<GameObject, GameObject> miniObjDict;

    // Mini objects have their own prefab, which is spawned to represent the max object on the minimap
    [SerializeField] private GameObject miniObjectParent;
    private float currentScale;
    private Vector3 globalCenter;

    [Header("Mini Object Prefabs")] [SerializeField]
    private GameObject miniDefaultObject;

    [SerializeField] private GameObject miniBuilding;
    [SerializeField] private GameObject miniCommitStats;
    [SerializeField] private GameObject miniCompetence;
    [SerializeField] private GameObject miniKanban;
    [SerializeField] private GameObject miniProgressBar;
    [SerializeField] private GameObject miniMinimap;


    public void Awake()
    {
        // newly spawned game objects will be automatically added to the list
        ResourceManager.Instance.RegisterGameObjectSpawnedCallback(AddTrackedObject);
        trackedObjects = new HashSet<GameObject>();
        //miniObjects = new List<GameObject>();
        miniObjDict = new Dictionary<GameObject, GameObject>();
        currentScale = 0.5f;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLocalTransform();
        //scaleIndicatorObject.transform.localScale = (new Vector3(1, 1, 1)) * 0.5f;

        foreach (var g in miniObjDict)
        {
            var maxi = g.Key;
            var mini = g.Value;

            if (maxi is null)
            {
                miniObjDict.Remove(maxi);
                continue;
            }

            mini.transform.localPosition = TranslateIntoLocalCoordinates(maxi.transform.position);
            mini.transform.localScale = maxi.transform.lossyScale * currentScale;
            mini.transform.localRotation = maxi.transform.rotation;
        }

        //int i = 0;
        //foreach (GameObject g in trackedObjects)
        //{
        //    miniObjects[i].transform.localPosition = TranslateIntoLocalCoordinates(g.transform.position);
        //    miniObjects[i].transform.localScale = g.transform.lossyScale * currentScale;
        //    miniObjects[i].transform.localRotation = g.transform.rotation;
        //    i++;
        //}
    }


    //Calculates the GlobalCenter as the middle of the Axis Aligned Bounding Cuboid(AABC) of the tracked objects, and the current scale As the ratio between the local scale length of the cube spaned by the reference corners and the largest dimension of the AABC. Then saves these values in the according variables. Called Every Frame
    void CalculateLocalTransform()
    {
        float localXLength = maxCorner.transform.localPosition.x - minCorner.transform.localPosition.x;
        float localYLength = maxCorner.transform.localPosition.y - minCorner.transform.localPosition.y;
        float localZLength = maxCorner.transform.localPosition.z - minCorner.transform.localPosition.z;
        //print("X:" + localXLength);
        //print("Y:" + localYLength);
        //print("Z:" + localZLength);
        float globalMaxX = float.MinValue;
        float globalMinX = float.MaxValue;
        float globalMaxY = float.MinValue;
        float globalMinY = float.MaxValue;
        float globalMaxZ = float.MinValue;
        float globalMinZ = float.MaxValue;
        if (trackedObjects.Count > 0)
        {
            foreach (GameObject g in trackedObjects)
            {
                if (g.transform.position.x > globalMaxX)
                {
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
        }

        globalCenter = new Vector3((globalMaxX + globalMinX) / 2, (globalMaxY + globalMinY) / 2,
            (globalMaxZ + globalMinZ) / 2);
        float largestDimension = Mathf.Max(globalMaxX - globalMinX, globalMaxZ - globalMinZ);
        float xDimension = globalMaxX - globalMinX;
        float yDimension = globalMaxY - globalMinY;
        float zDimension = globalMaxZ - globalMinZ;
        if (largestDimension == 0)
        {
            currentScale = maxScale;
        }
        else if (Mathf.Max((localXLength / 2 / xDimension), (localZLength / 2 / zDimension)) < minScale)
        {
            currentScale = minScale;
        }
        else if (Mathf.Max((localXLength / 2 / xDimension), (localZLength / 2 / zDimension)) > maxScale)
        {
            currentScale = maxScale;
        }
        else
        {
            currentScale = Mathf.Max((localXLength / 2 / xDimension), (localZLength / 2 / zDimension));
        }
    }

    //Gets the global position of a tracked object and returns the  corresponding local position of the corresponding mini-object in the coordinate system of the minimap
    private Vector3 TranslateIntoLocalCoordinates(Vector3 globalPos)
    {
        Vector3 localPos = globalPos - globalCenter;
        localPos = localPos * currentScale;
        localPos.y += yOffset;
        return localPos;
    }

    public void AddTrackedObject(GameObject objectToTrack)
    {
        if (trackedObjects.Contains(objectToTrack))
        {
            return;
        }

        // Add newly spawned (big) object to list
        trackedObjects.Add(objectToTrack);
        // Create new mini object for the minimap
        //int miniObjectTypeIndex = GetMiniObjectTypeIndex(objectToTrack);
        GameObject newMiniobject = InstantiateMiniObject(objectToTrack.GetType());
        newMiniobject.transform.parent = miniObjectParent.transform;
        //miniObjects.Add(newMiniobject);

        miniObjDict.Add(objectToTrack, newMiniobject);
    }

    // TODO: Use this instead once Unity supports C# 8!
    //private GameObject InstantiateMiniObject<T>(T miniObjType) => miniObjType switch
    //{
    //    CommitStatisticsVisualizer => Instantiate(miniCommitStats),
    //    CompetenceDisplay => Instantiate(miniCompetence),
    //    KanbanBoardColumn => Instantiate(miniKanban),
    //    Minimap => Instantiate(miniMinimap),
    //    ProgressBar => Instantiate(miniProgressBar),
    //    _ => Instantiate(miniDefaultObject)
    //};

    // Helper function to instantiate a new mini objetc of the correct type
    private GameObject InstantiateMiniObject<T>(T miniObject)
    {
        if (miniObject is CommitStatisticsVisualizer)
        {
            return Instantiate(miniCommitStats);
        }
        if (miniObject is CompetenceDisplay)
        {
            return Instantiate(miniCompetence);
        }
        if (miniObject is KanbanBoardColumn)
        {
            return Instantiate(miniKanban);
        }
        if (miniObject is Minimap)
        {
            return Instantiate(miniMinimap);
        }
        if (miniObject is ProgressBar)
        {
            return Instantiate(miniProgressBar);
        }

        return Instantiate(miniDefaultObject);
    }

    //public int GetMiniObjectTypeIndex(GameObject objectToTrack)
    //{
    //    if (!(objectToTrack.GetComponents(typeof(ProgressBar)).Length == 0))
    //    {
    //        return 1;
    //    }

    //    return 0;
    //}
}