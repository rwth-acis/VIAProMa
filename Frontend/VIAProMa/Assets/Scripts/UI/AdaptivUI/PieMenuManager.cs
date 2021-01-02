using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;



public class PieMenuManager : Singleton<PieMenuManager>
{
    [SerializeField]
    GameObject pieMenuPrefab;
    [SerializeField]
    GameObject mainCamera;

    GameObject instantiatedPieMenu;

    //Variables for the projection process
    Vector3 transformedPieMenuPosition;
    Matrix4x4 planeToStandart;
    Matrix4x4 standartToPlane;

    IMixedRealityPointer pointer;
    IMixedRealityInputSource invokingSource;

    [SerializeField]
    public List<MenuEntry> menuEntries;

    int currentlyHighlighted;

    public GameObject menuCursor;
    GameObject instiatedMenuCursor;

    //Dictionary<IMixedRealityInputSource, VirtualTool> virtualTools;
    VirtualTool virtualTool;
    [SerializeField]
    GameObject virtualToolPrefab;

    void Start()
    {

    }


    void Update()
    {
        if (instantiatedPieMenu != null)
        {
            //Project the pointer on the plane formed by the pie menu
            Vector3 projectedPoint = pointer.Position;
            projectedPoint = standartToPlane * new Vector4(projectedPoint.x, projectedPoint.y, projectedPoint.z, 1);
            projectedPoint.z = 0;

            instiatedMenuCursor.transform.position = planeToStandart * new Vector4(projectedPoint.x, projectedPoint.y, projectedPoint.z, 1);
            currentlyHighlighted = CalculatePieceID(projectedPoint);
            instantiatedPieMenu.SendMessage("highlightPiece", currentlyHighlighted);
        }
    }

    int CalculatePieceID(Vector2 projectedPointer)
    {
        float angle = Vector2.SignedAngle(Vector2.down, projectedPointer);
        if (angle < 0)
        {
            angle = 360 + angle;
        }
        int i = (int)(angle / 360 * menuEntries.Count);
        return i;
    }

    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already open by another controller
        if (instantiatedPieMenu == null)
        {
            pointer = eventData.InputSource.Pointers[0];
            invokingSource = eventData.InputSource;
            instantiatedPieMenu = Instantiate(pieMenuPrefab, pointer.Position, Quaternion.identity);
            instantiatedPieMenu.GetComponent<PieMenuRenderer>().constructor();
            instantiatedPieMenu.transform.LookAt(mainCamera.transform);

            Vector3 pieMenuPosition = instantiatedPieMenu.transform.position;
            Vector3 direction = instantiatedPieMenu.transform.rotation * Vector3.right;
            Vector3 up = instantiatedPieMenu.transform.rotation * Vector3.up;
            Vector3 normal = instantiatedPieMenu.transform.rotation * Vector3.forward;

            planeToStandart = new Matrix4x4(direction, up, normal, new Vector4(pieMenuPosition.x, pieMenuPosition.y, pieMenuPosition.z, 1));
            standartToPlane = planeToStandart.inverse;
            transformedPieMenuPosition = standartToPlane * new Vector4(pieMenuPosition.x, pieMenuPosition.y, pieMenuPosition.z);

            instiatedMenuCursor = Instantiate(menuCursor);
        }
    }

    public void MenuClose(BaseInputEventData eventData)
    {
        //Only the input source that opend the menu can close it again
        if (eventData.InputSource == invokingSource && instantiatedPieMenu != null)
        {
            if (virtualTool != null)
            {
                Destroy(virtualTool);
            }
            virtualTool = Instantiate(virtualToolPrefab).GetComponent<VirtualTool>();

            virtualTool.inputSource = invokingSource;
            MenuEntry currentEntry = menuEntries[currentlyHighlighted];
            virtualTool.InputAction = currentEntry.InputAction;
            virtualTool.OnInputActionStarted = currentEntry.toolActionOnSelectStart;
            virtualTool.OnInputActionEnded = currentEntry.toolActionOnSelectEnd;

            Destroy(instantiatedPieMenu);
            invokingSource = null;

            Destroy(instiatedMenuCursor);
        }
    }
}
