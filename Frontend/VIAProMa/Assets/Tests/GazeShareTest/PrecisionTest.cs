using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using System.IO;

// JSON serializable class
[Serializable] public class PrecisionData
{
    public string result;
    public string elapsedTime;
    public float distance;
    public Vector3 centerPosition;
    public Vector3 hitPosition;
}

public class PrecisionTest : MonoBehaviour, IMixedRealityPointerHandler
{
    // Precision test
    private Vector3 hitPosition;
    private Vector3 centerPosition;
    private string hitArea;
    private float distance;
    private PrecisionData precisionData;
    private string jsonPrecisionData;
    // Timer
    private TimeSpan spawnTime;
    private TimeSpan currentTime;
    private TimeSpan timeout;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        // Write down the time when the target was spawned
        spawnTime = currentTime;
        // Precision test
        centerPosition = transform.position;
        hitPosition = RaycastVive.pointerHitPosition.Equals(InstantiateArrows.far) ? MixedRealityToolkit.InputSystem.GazeProvider.HitPosition : RaycastVive.pointerHitPosition;
        distance = Vector3.Distance(centerPosition, hitPosition) * 100f;
        hitArea = DetermineHitArea();
        OutputPrecision(hitArea);
        // Spawn the next target
        MoveTarget(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        hitPosition = InstantiateArrows.far;
        centerPosition = transform.position;
        // Timer
        spawnTime = GazeShareTester_Evaluation.timer.Elapsed;
        timeout = TimeSpan.FromSeconds(5.0);
        // Precision data
        WriteString("PrecisionData", "NEW TEST " + DateTime.Now);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = GazeShareTester_Evaluation.timer.Elapsed;
        if (currentTime - spawnTime > timeout)
        {
            OutputPrecision("TIMEOUT");
            MoveTarget(gameObject); 
        }
    }

    // Moves the target to a new position in the user's field of view and sets its spawn time to current time
    private void MoveTarget(GameObject obj)
    {
        obj.gameObject.transform.position = new Vector3(UnityEngine.Random.Range(0.5f, 3f), UnityEngine.Random.Range(0.5f, 3f), 2.5f);
        spawnTime = currentTime;
    }

    // Determines which area of the shooting target was hit by the user
    private string DetermineHitArea()
    {
        if (distance < 10)
        {
            return "Bullseye!";
        }
        else if (distance < 20)
        {
            return "White inner";
        }
        else if (distance < 30)
        {
            return "Red inner";
        }
        else if (distance < 40)
        {
            return "White outer";
        }
        else if (distance <= 50)
        {
            return "Red outer";
        }
        else
        {
            return "TIMEOUT";
        }
    } 

    // Creates a precisionData object, fills it with precision information, converts it to a JSON string and saves it in a file 
    private void OutputPrecision(string result)
    {
        precisionData = new PrecisionData();
        precisionData.elapsedTime = GazeShareTester_Evaluation.elapsedTime;
        precisionData.centerPosition = centerPosition;
        if (hitArea == "TIMEOUT")
        {
            precisionData.result = "TIMEOUT";
            precisionData.distance = float.NaN;
            precisionData.hitPosition = Vector3.positiveInfinity;
        }
        else
        {
            precisionData.result = hitArea;
            precisionData.distance = distance;
            precisionData.hitPosition = hitPosition;
        }

        jsonPrecisionData = JsonUtility.ToJson(precisionData);
        Debug.Log(jsonPrecisionData);
        jsonPrecisionData = JsonUtility.ToJson(precisionData, true);
        WriteString("PrecisionData", jsonPrecisionData);
    }

    // Retrieves the file path for the provided file name
    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.streamingAssetsPath, fileName);
    }

    // Opens the provided file and appends the provided string to it
    static void WriteString(string fileName, string input)
    {
        string filePath = GetFilePath(fileName);
        StreamWriter writer = new StreamWriter(filePath, true);
        writer.WriteLine(input);
        writer.Close();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}
