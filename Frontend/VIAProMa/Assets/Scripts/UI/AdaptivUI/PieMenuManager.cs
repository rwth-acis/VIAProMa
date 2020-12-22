using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;



public class PieMenuManager : Singleton<PieMenuManager>
{
    [SerializeField]
    GameObject pieMenuPrefab;
    [SerializeField]
    GameObject mainCamera;

    GameObject instantiatedPieMenu;
    IMixedRealityPointer pointer;
    IMixedRealityInputSource invokingSource;

    [SerializeField]
    public List<MenuEntry> menuEntries;

    public GameObject test;

    void Start()
    {
        
    }


    void Update()
    {
        if (instantiatedPieMenu != null)
        {
            //instantiatedPieMenu.transform.LookAt(mainCamera.transform);
            //Project the pointer on the plane formed by the pie menu
            Vector3 pieMenuPosition = instantiatedPieMenu.transform.position;
            Vector3 direction = instantiatedPieMenu.transform.rotation * Vector3.right;
            Vector3 up = instantiatedPieMenu.transform.rotation * Vector3.up;
            Vector3 normal = instantiatedPieMenu.transform.rotation * Vector3.forward;

            Matrix4x4 planeToStandart = new Matrix4x4(direction, up, normal, new Vector4(0,0,0,1));
            Matrix4x4 standartToPlane = planeToStandart.inverse;

            Vector3 projectedPoint = pointer.Position;
            projectedPoint = standartToPlane * new Vector4(projectedPoint.x, projectedPoint.y, projectedPoint.z, 1);
            projectedPoint.z = (standartToPlane * new Vector4(pieMenuPosition.x, pieMenuPosition.y, pieMenuPosition.z)).z;
            projectedPoint = planeToStandart * new Vector4(projectedPoint.x, projectedPoint.y, projectedPoint.z, 1);

            test.transform.position = projectedPoint;
        }
    }

    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already open by another controller
        if (instantiatedPieMenu == null)
        {
            pointer = eventData.InputSource.Pointers[0];
            invokingSource = eventData.InputSource;
            instantiatedPieMenu = Instantiate(pieMenuPrefab, pointer.Position, Quaternion.identity);
            instantiatedPieMenu.transform.LookAt(mainCamera.transform);
            menuEntries[0].toolAction.Invoke(null);
        }
    }

    public void MenuClose(BaseInputEventData eventData)
    {
        //Only the input source that opend the menu can close it again
        if ((invokingSource == null || eventData.InputSource == invokingSource) && instantiatedPieMenu != null)
        {
            Destroy(instantiatedPieMenu);
            invokingSource = null;
        }
    }

    public void TestAct()
    { }
}
