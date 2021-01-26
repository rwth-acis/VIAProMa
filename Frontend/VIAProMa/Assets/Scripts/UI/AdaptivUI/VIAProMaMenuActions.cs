using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
public class VIAProMaMenuActions : MonoBehaviour
{
    public void DoubleSize(BaseInputEventData eventData)
    {
        GameObject target = eventData.InputSource.Pointers[0]?.Result?.CurrentPointerTarget;
        if (target != null)
        {
            target.transform.root.localScale *= 2;
        }
    }

    public void HalveeSize(BaseInputEventData eventData)
    {
        GameObject target = eventData.InputSource.Pointers[0]?.Result?.CurrentPointerTarget;
        if (target != null)
        {
            target.transform.root.localScale /= 2;
        }
    }


    /// <summary>
    /// Main function for tool to remove visualisations.
    /// </summary>
    /// <param name="eventData"></param>
    public void Remove(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromGameObject(eventData.InputSource.Pointers[0].Result.CurrentPointerTarget);
        if (target == null)
        {
            return;
        }
        if (target.GetComponentInChildren<PhotonView>() != null)
        {
            PhotonNetwork.Destroy(target);
        }
        else
        {
            Destroy(target);
        }
    }

    //Adjust Tool

    BoundingBoxStateController[] boundingBoxStateControllers;

    public void StartAdjusting()
    {
        boundingBoxStateControllers = FindObjectsOfType<BoundingBoxStateController>();
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            boundingbox.BoundingBoxActive = true;
        }
    }

    public void StopAdjusting()
    {
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (boundingbox != null)
            {
                boundingbox.BoundingBoxActive = false;
            }
        }
    }

    public void OpenConfigurationWindow(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromGameObject(eventData.InputSource.Pointers[0].Result.CurrentPointerTarget, new Type[] {typeof(ConfigurationWindow)}, true, false);
        if (target != null)
        {
            ConfigurationWindow configurationWindow = target.transform.GetComponentInChildren<ConfigurationWindow>(true);
            if (configurationWindow != null)
            {
                configurationWindow.Open();
                RotateToCameraOnXZPlane(configurationWindow.gameObject, eventData.InputSource.Pointers[0].Position);
            }
        }
    }

    private void RotateToCameraOnXZPlane(GameObject objectToRotate, Vector3 newPosition)
    {
        objectToRotate.transform.LookAt(Camera.main.transform);
        //Set the x and y rotation to 0 and flip it around because with LookAt, the prefab faces away from the camera and is tilted strangly
        Vector3 rotation = objectToRotate.transform.eulerAngles;
        objectToRotate.transform.SetPositionAndRotation(newPosition, Quaternion.Euler(0, 180 + rotation.y, 0));
    }

    public void UndoToolAction()
    {
        Debug.Log("Undo");
    }

    public void RedoToolAction()
    {
        Debug.Log("Redo");
    }

    public void OpenBoudningBoxWithoutManipulation()
    {
        boundingBoxStateControllers = FindObjectsOfType<BoundingBoxStateController>();
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (GetVisualisationFromGameObject(boundingbox.gameObject) != null)
            {
                boundingbox.BoundingBoxActive = true;
                boundingbox.manipulationHandler.enabled = false;
            }
        }
    }

    public void CloseBoudningBoxWithoutManipulation()
    {
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (boundingbox != null)
            {
                boundingbox.BoundingBoxActive = false;
                boundingbox.manipulationHandler.enabled = true;
            }
        }
    }

    public Material highlightMaterial;
    public Material wireframeMaterial;
    Material previousMaterial;
    public void HighlightBoudningBox(FocusEventData data)
    {
        BoundingBox box = GetVisualisationFromGameObject(data.NewFocusedObject)?.GetComponentInChildren<BoundingBox>();
        if (box != null && GetVisualisationFromGameObject(data.NewFocusedObject) != null)
        {
            previousMaterial = box.BoxMaterial;
            box.BoxMaterial = highlightMaterial;
            box.ShowWireFrame = true;
            box.WireframeMaterial = wireframeMaterial;
        }
    }

    public void DeHighlightBoundingBox(FocusEventData data)
    {
        BoundingBox box = GetVisualisationFromGameObject(data.OldFocusedObject)?.GetComponentInChildren<BoundingBox>();
        if (box != null)
        {
            box.BoxMaterial = previousMaterial;
            box.ShowWireFrame = false;
        }
    }

    public void SpawnCurrentIconOverVisualisation(FocusEventData data)
    {
        if (GetVisualisationFromGameObject(data.NewFocusedObject) != null)
        {
            if (instantiatedIcon != null)
            {
                //This shouldn't happen, but just to be sure
                Destroy(instantiatedIcon);
            }
            instantiatedIcon = Instantiate(highlightSprite);
            instantiatedIcon.GetComponentInChildren<Image>().sprite = data.Pointer.Controller.Visualizer.GameObjectProxy.GetComponentInChildren<ViveWandVirtualTool>().currentEntry.iconTool;
        }
    }

    public void DestroyCurrentIconOverVisualisation()
    {
        Destroy(instantiatedIcon);
    }

    public GameObject highlightSprite;
    GameObject instantiatedIcon;
    public void UpdateCurrentIconOverVisualisation(FocusEventData data)
    {
        if (instantiatedIcon != null)
        {
            GameObject target = GetVisualisationFromGameObject(data.NewFocusedObject);
            Collider collider = target.GetComponentInChildren<BoundingBox>()?.GetComponent<Collider>();
            if (collider != null)
            {
                Vector3 maxpoint = collider.bounds.max;
                Vector3 minpoint = collider.bounds.min;
                minpoint.y = maxpoint.y;
                Vector3 topMiddle = minpoint + 0.5f * (maxpoint - minpoint);
                topMiddle.y += 0.3f;
                RotateToCameraOnXZPlane(instantiatedIcon, topMiddle);
            }
        }
    }

    private GameObject GetVisualisationFromGameObject(GameObject gameObject, Type[] typesToExclude = null, bool checkAbove = false, bool checkBelow = false)
    {
            //If wished, check if any of the children of the target is of a type that should be excluded
            if (typesToExclude != null && checkBelow)
            {
                foreach (Type type in typesToExclude)
                {
                    if (gameObject.GetComponentInChildren(type,true) != null)
                    {
                        return null;
                    }
                }
            }

            if (gameObject != null)
            {
                while (gameObject != null && !IsVisualisation(gameObject))
                {
                    //If wished, check if the current object (i.e. a object above in the hirachy of the original target) is of a type that should be excluded
                    if (typesToExclude != null && checkAbove)
                    {
                        foreach (Type type in typesToExclude)
                        {
                            if (gameObject.GetComponent(type) != null)
                            {
                                return null;
                            } 
                        }
                    }

                    gameObject = gameObject.transform.parent?.gameObject;
                }
                if (gameObject != null && IsVisualisation(gameObject))
                {
                    return gameObject;
                }
            }
        return null;
    }

    bool IsVisualisation(GameObject gameObject)
    {
        return gameObject.GetComponent<Visualization>() != null;
    }
}
