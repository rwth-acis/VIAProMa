using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class PrecisionData
{
    public string result;
    public TimeSpan elapsedTime;
    public float distance;
    public Vector3 centerPosition;
    public Vector3 hitPosition;
}

public class PrecisionTest : MonoBehaviour, IMixedRealityPointerHandler
{
    //[SerializeField] private GameObject center;
    //private float spawnRadius = 10f;
    private Vector3 hitPosition;
    private Vector3 centerPosition;
    private string result;
    // Timeout
    private TimeSpan spawnTime;
    private TimeSpan currentTime;
    private TimeSpan timeout;

    private PrecisionData precisionData;
    private string jsonPrecisionData;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        spawnTime = currentTime;
        //DateTime time = eventData.EventTime;
        IMixedRealityInputSource source = eventData.InputSource;

        //centerPosition = center.transform.localPosition;
        centerPosition = transform.position;
        hitPosition = RaycastVive.pointerHitPosition.Equals(InstantiateArrows.far) ? MixedRealityToolkit.InputSystem.GazeProvider.HitPosition : RaycastVive.pointerHitPosition;
        Debug.Log("Center: " + centerPosition);
        Debug.Log("Pointer: " + hitPosition);
        //float distance = Vector3.Distance(centerPosition, RaycastVive.pointerHitPosition) * 100f;
        //float distance = Vector3.Distance(centerPosition, MixedRealityToolkit.InputSystem.GazeProvider.HitPosition) * 100f;
        float distance = Vector3.Distance(centerPosition, hitPosition) * 100f;

        if (distance < 10)
        {
            result = "Bullseye!";
        }
        else if (distance < 20)
        {
            result = "White inner.";
        }
        else if (distance < 30)
        {
            result = "Red inner.";
        }
        else if (distance < 40)
        {
            result = "White outer...";
        }
        else if (distance <= 50)
        {
            result = "Red outer...";
        }

        //Debug.Log("Time: " + time + "; distance: " + distance.ToString("f"));
        Debug.Log(result + " Timer: " + GazeShareTester_Evaluation.elapsedTime + "; distance: " + distance.ToString("f"));

        precisionData = new PrecisionData();
        precisionData.result = result;
        precisionData.elapsedTime = GazeShareTester_Evaluation.timer.Elapsed;
        precisionData.distance = distance;
        precisionData.centerPosition = centerPosition;
        precisionData.hitPosition = hitPosition;
        jsonPrecisionData = JsonUtility.ToJson(precisionData);

        MoveTarget(gameObject);

        //TimerWindow.timer.Stop();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    // Start is called before the first frame update
    void Start()
    {
        hitPosition = InstantiateArrows.far;
        centerPosition = transform.position;

        spawnTime = GazeShareTester_Evaluation.timer.Elapsed;
        timeout = TimeSpan.FromSeconds(5.0);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = GazeShareTester_Evaluation.timer.Elapsed;
        if (currentTime - spawnTime > timeout)
        {
            precisionData = new PrecisionData();

            spawnTime = currentTime;
            MoveTarget(gameObject); 
        }
    }

    // Moves the target to a new position in the user's field of view and sets its spawn time to current time
    void MoveTarget(GameObject obj)
    {
        //gameObject.SetActive(false);
        obj.gameObject.transform.position = new Vector3(UnityEngine.Random.Range(0.5f, 3f), UnityEngine.Random.Range(0.5f, 3f), 2.5f);
        spawnTime = currentTime;
        //gameObject.transform.position = UnityEngine.Random.onUnitSphere * spawnRadius;
        //gameObject.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, transform.rotation, 360);
        //gameObject.SetActive(true);
    }
}
