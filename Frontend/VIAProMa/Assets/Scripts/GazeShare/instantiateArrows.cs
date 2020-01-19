using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;
using TMPro;

public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{
    protected Vector3 targetPosition;
    protected Vector3 up = new Vector3(0f, 0.025f, 0f);
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Quaternion rot = Quaternion.Euler(0, 0, -90);
    protected bool isUsingVive;
    protected int clickableObjectCount = 0;
    [HideInInspector] public bool sharing;
    [HideInInspector] public bool sharingGlobal;
    protected TextMeshPro shareGazeText;

    public void Start()
    {
        sharing = true;
        sharingGlobal = true;
        setTextOfShareLabel();
        setTextOfGlobalGazingLabel();
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
        }
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            moveMyArrow();
            setColorOfArrow();
        }
        else
        {
            moveOtherArrows();
            setColorOfArrow();
        }
    }

    protected void moveMyArrow()
    {
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && getIsUsingVive() == false && sharing == true && sharingGlobal == true)
        {
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            transform.rotation = rot;
            GetComponentInChildren<TextMeshPro>().text = "Hololens";
        }
        else if (getIsUsingVive() == true && sharing == true && sharingGlobal == true)
        {
            transform.rotation = rot;
            transform.position = getHitPositionOfPointedObjectFinal() + up;
            GetComponentInChildren<TextMeshPro>().text = "HTC Vive";
        }
        else
        {
            transform.rotation = rot;
            transform.position = far;
        }
    }

    protected void moveOtherArrows()
    {
        if(sharingGlobal == true)
        {
            transform.position = targetPosition;
            transform.rotation = rot;
            foreach (GameObject arrow in getAllGameObjectsArrow())
            {
                if (photonView.OwnerActorNr == arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr)
                {
                    setArrowTextLabel(getIsUsingVive());
                }
            }
        }
        else
        {
            transform.position = far;
        }

    }

    protected void setArrowTextLabel(bool viveTrue)
    {
        if(viveTrue == false)
        {
            GetComponentInChildren<TextMeshPro>().text = "Hololens";
        } else { GetComponentInChildren<TextMeshPro>().text = "HTV Vive"; }
    }

    protected bool getIsUsingVive()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceType == InputSourceType.Hand)
            {
                isUsingVive = true;
            }
        }
        return isUsingVive;
    }

    protected Color getColorForUser(int userID)
    {
        switch (userID)
        {
            case 1:
                return Color.red;
                break;

            case 2:
                return Color.green;
                break;

            case 3:
                return Color.black;
                break;

            default: return Color.white;
        }
    }

    protected void setColorOfArrow()
    {
        GetComponent<Renderer>().material.color = getColorForUser(photonView.OwnerActorNr);
    }

    protected GameObject[] getAllGameObjectsWithArrowScriptTesting()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("showArrow");
        return arrayAll;
    }

    protected GameObject getShareGazeLabelObject()
    {
        GameObject shareLabelObject = GameObject.Find("ShareGazeLabel");
        return shareLabelObject;
    }

    protected GameObject getGlobalGazingLabelObject()
    {
        GameObject globalGazinglabelObject = GameObject.Find("GlobalGazingLabel");
        return globalGazinglabelObject;
    }

    protected Vector3 getHitPositionOfPointedObjectFinal()
    {
        Vector3 hitPositionResult = far;
        foreach (GameObject controller in getAllGameObjectsWithArrowScriptTesting())
        {
            if (controller.GetComponent<ArrowControllerHandler>().pointerHitPosition != far)
            {
                hitPositionResult = controller.GetComponent<ArrowControllerHandler>().pointerHitPosition;
            }
        }
        return hitPositionResult;
    }

    public void setTextOfShareLabel()
    {
        if(sharing == true)
        {
            getShareGazeLabelObject().GetComponent<TextMeshPro>().text = "Unshare my Gaze";
        }
        else
        {
            getShareGazeLabelObject().GetComponent<TextMeshPro>().text = "Share my Gaze";
        }
    }

    public void setTextOfGlobalGazingLabel()
    {
        if (sharingGlobal == true)
        {
            getGlobalGazingLabelObject().GetComponent<TextMeshPro>().text = "Disable Gazing";
        }
        else
        {
            getGlobalGazingLabelObject().GetComponent<TextMeshPro>().text = "Enable Gazing";
        }
    }

    protected GameObject[] getAllGameObjectsArrow()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("arrow");
        return arrayAll;
    }
}