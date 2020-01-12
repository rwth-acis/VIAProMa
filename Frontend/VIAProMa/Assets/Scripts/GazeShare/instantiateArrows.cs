using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;


public class InstantiateArrows : MonoBehaviourPun, IPunObservable, IMixedRealityPointerHandler
{
    
    protected Vector3 targetPosition;
    protected Material targetMaterial;
    protected Vector3 up = new Vector3(0f, 0.1f, 0f);
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    public float lerpSpeed = 1f;
    protected Quaternion rot = Quaternion.Euler(0, 0, -90);
    //protected bool isSharing = true;
    //protected bool targetIsSharing = true;
    //Material mat;

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if we are writing to the stream => we are the local player and want to transmit our position and rotations
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(GetComponent<Renderer>().material);
        }
        else // we are reading the network stream, i.e. we receive a remote player position and rotation
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            targetMaterial = (Material)stream.ReceiveNext();
        }
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            moveMyArrow();
            foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
            {
                Debug.Log("Controller : " + controller.InputSource.SourceType);
                if(controller.InputSource.SourceType == InputSourceType.Hand)
                {
                    Debug.Log("YES");
                }
            }
            } else
        {
            moveOtherArrows();
        }
    }

    /*public void toggleSharing()
    {
        isSharing = !isSharing;
        Debug.Log(isSharing);
    }*/

    protected void moveMyArrow()
    {
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget /*&& isSharing == true*/)
        {
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            transform.rotation = rot;
            GetComponent<Renderer>().material.color = getColorForUser(photonView.OwnerActorNr);
        }
        else { transform.position = far; }
    }

    protected void moveOtherArrows()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed * Time.deltaTime);
        GetComponent<Renderer>().material.color = targetMaterial.color;
    }

    protected Color getColorForUser(int userID)
    {
        switch (userID)
        {
            case 1: return Color.red;
                break;

            case 2: return Color.green;
                break;

           case 3: return Color.black;
                break;

            default: return Color.white;
        }
    }

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
