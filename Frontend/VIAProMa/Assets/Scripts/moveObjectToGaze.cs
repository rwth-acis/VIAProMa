using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class moveObjectToGaze : MonoBehaviour
{
    private Vector3 up = new Vector3(0f,0.1f,-0.1f);
    private Vector3 far = new Vector3(0f,-10f,0f);
    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        gameObject.transform.position = far;
        mat.color = Color.black;
    }

    void Update()
    {
        //Debug.Log("Current GazePointer : " + giveGaze());
        /*print("Current InputSystemProfile: " + MixedRealityToolkit.InputSystem.InputSystemProfile);
        Debug.Log("Current IsInputEnabled : " + MixedRealityToolkit.InputSystem.IsInputEnabled);
        Debug.Log("Current GazeCursor : " + MixedRealityToolkit.InputSystem.GazeProvider.GazeCursor.IsVisible);
        Debug.Log("Current GazePointer : " + MixedRealityToolkit.InputSystem.GazeProvider.GazePointer);*/
        //Debug.Log("Detected Controllers : " + MixedRealityToolkit.InputSystem.DetectedControllers);
        //Debug.Log("Enabled ? : " + MixedRealityToolkit.InputSystem.GazeProvider.Enabled);
        //Debug.Log("Pointer ? : " + MixedRealityToolkit.InputSystem);
        //Debug.Log("Direction : " + giveGaze().GazeDirection);
        //print("Current Target : " + giveGaze().GazeTarget);
        if (giveGaze().GazeTarget)
        {
            Vector3 currentHitPosition = giveGaze().HitPosition;
            gameObject.transform.position = currentHitPosition + up;
            Vector3 angle = new Vector3(gameObject.transform.eulerAngles.x, giveGaze().GazeDirection.x * 90, gameObject.transform.eulerAngles.z);
            gameObject.transform.eulerAngles = angle;
        } else {gameObject.transform.position = far;}
    }

    public IMixedRealityGazeProvider giveGaze()
    {
        IMixedRealityGazeProvider currentGaze = MixedRealityToolkit.InputSystem.GazeProvider;
        return currentGaze;
        
    } 
}