using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class moveObjectToGaze : MonoBehaviour, IMixedRealityPointerHandler
{
    private Vector3 up = new Vector3(0f,0.1f,0f);
    private Vector3 far = new Vector3(0f,-10f,0f);
    Material mat;
    public Text txt;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        gameObject.transform.position = far;
        mat.color = Color.black;
    }

    void Update()
    {
        //Debug.Log("Current GameObjectReference : " + giveGaze().GameObjectReference.name);
        if (giveGaze().GazeTarget)
        {
            Vector3 currentHitPosition = giveGaze().HitPosition;
            gameObject.transform.position = currentHitPosition + up;
            Vector3 angle = new Vector3(gameObject.transform.eulerAngles.x, giveGaze().GazeDirection.x * 90, gameObject.transform.eulerAngles.z);
            gameObject.transform.eulerAngles = angle;
            txt.text = giveGaze().GazeTarget.name;
            
        } else {gameObject.transform.position = far;}
    }

    public IMixedRealityGazeProvider giveGaze()
    {
        IMixedRealityGazeProvider currentGaze = MixedRealityToolkit.InputSystem.GazeProvider;
        return currentGaze;
        
    }

    /*public IMixedRealityGazeProvider giveGGV()
    {
        IMixedRealityGazeProvider currentGaze = MixedRealityToolkit.InputSystem.GazeProvider;
        return currentGaze;

    }
    */

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}