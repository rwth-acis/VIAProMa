using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // an OnDestroy() callback would be better but we can't change all Visualizations
        var badKeys = miniObjDict.Where(pair => pair.Value == null)
            .Select(pair => pair.Key)
            .ToList();
        foreach (var badKey in badKeys)
        {
            miniObjDict.Remove(badKey);
            Destroy(badKey);
        }

        CalculateLocalTransform();
        scaleIndicatorObject.transform.localScale = (new Vector3(1, 1, 1)) * currentScale;

        foreach (var g in miniObjDict)
        {
            var maxi = g.Value;
            var mini = g.Key;

            mini.transform.localPosition = TranslateIntoLocalCoordinates(maxi.transform.position);
            mini.transform.localScale = maxi.transform.lossyScale * currentScale;
            mini.transform.localRotation = maxi.transform.rotation;
        }

    }


    //Calculates the GlobalCenter as the middle of the Axis Aligned Bounding Cuboid(AABC) of the tracked objects, and the current scale As the ratio between the local scale length of the cube spaned by the reference corners and the largest dimension of the AABC. Then saves these values in the according variables. Called Every Frame
    void CalculateLocalTransform()
    {
        float localXLength = maxCorner.transform.localPosition.x - minCorner.transform.localPosition.x;
        float localYLength = maxCorner.transform.localPosition.y - minCorner.transform.localPosition.y;
        float localZLength = maxCorner.transform.localPosition.z - minCorner.transform.localPosition.z;

        float globalMaxX = float.MinValue;
        float globalMinX = float.MaxValue;
        float globalMaxY = float.MinValue;
        float globalMinY = float.MaxValue;
        float globalMaxZ = float.MinValue;
        float globalMinZ = float.MaxValue;

        if (miniObjDict.Count > 0)
        {
            foreach (GameObject g in miniObjDict.Values)
            {
                // possible to get a null game object if we haven't done a fresh Update()
                if (g == null)
                {
                    continue;
                }

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
        if (miniObjDict.ContainsValue(objectToTrack))
        {
            return;
        }

        // Add newly spawned (big) object to list
        //trackedObjects.Add(objectToTrack);
        // Create new mini object for the minimap
        //int miniObjectTypeIndex = GetMiniObjectTypeIndex(objectToTrack);
        GameObject newMiniobject = InstantiateMiniObject(objectToTrack);
        newMiniobject.transform.parent = miniObjectParent.transform;
        //miniObjects.Add(newMiniobject);

        miniObjDict.Add(newMiniobject, objectToTrack);
    }

    // Helper function to instantiate a new mini objetc of the correct type
    private GameObject InstantiateMiniObject(GameObject miniObject)
    {
        if (miniObject.GetComponent<BuildingProgressBarVisuals>())
        {
            return InstantiateMiniObject(miniBuilding);
        }

        if (miniObject.GetComponent<CommitStatisticsVisualizer>())
        {
            return Instantiate(miniCommitStats);
        }

        if (miniObject.GetComponent<CompetenceDisplay>())
        {
            return Instantiate(miniCompetence);
        }

        if (miniObject.GetComponent<KanbanBoardColumn>())
        {
            return Instantiate(miniKanban);
        }

        if (miniObject.GetComponent<Minimap>())
        {
            return Instantiate(miniMinimap);
        }

        if (miniObject.GetComponent<ProgressBar>())
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