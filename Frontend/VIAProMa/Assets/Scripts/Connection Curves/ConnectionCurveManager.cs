using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Threading.Tasks;
using HoloToolkit.Unity;

/// <summary>
/// Manages the connect and disconnect process of Connection Curves and provides functions to manage curves.
/// </summary>
public class ConnectionCurveManager : Singleton<ConnectionCurveManager>
{
    public bool onlineTestMode;
    static float stepSize = 0.5f;
    public static int curveSegmentCount = 60;
    public GameObject DeleteCube;
    GameObject instantiatedDeletCube;
    public List<ConnectionCurve> curves;
    IMixedRealityPointer mainPointer = null;
    JoinedCurveGeneration curveGenerator;
    public enum State
    {
        defaultMode,
        connecting,
        disconnecting
    }
    
    public State currState = State.defaultMode;
    public GameObject curveConnectPrefab;
    public GameObject tempGoalPrefab;
    AppBarStateController caller;
    bool lastFrameClicked = false;
    bool deletedSomething = false;
    bool startedDeletion = false;

    //Temp Curve
    ConnectionCurve tempCurve;
    DateTime clickTimeStamp;

    List<ConnectionCurve> test;


    GameObject tempGoal = null;


    void Start()
    {
        clickTimeStamp = DateTime.Now;
        JoinedCurveGeneration.UpdateAsyc(curves,stepSize);
    }

    void Update()
    {
        bool thisFrameClicked = false;
        bool pointerValid = false;
        //Check, if there is a valid pointer (the dot at the end of the ray from thte input source) in the scene.
        //For some ungodly reasons objects from the mrtk behave strange when they should be null. They can then still be dereferenced and != null sometimes still yields true, but there content is useless.
        //But ToString then returns null.
        if (mainPointer != null)
        {
            pointerValid = mainPointer.ToString() != "null";
        }
        if (pointerValid)
        {
            thisFrameClicked = ((AnimatedCursor)mainPointer.BaseCursor).CursorState == CursorStateEnum.Select && (DateTime.Now - clickTimeStamp).TotalMilliseconds > 30;
        }
        switch (currState)
        {
            case State.connecting:
                GameObject target;
                if (pointerValid)
                {
                    target = GetParentWithPhotonView(mainPointer.Result?.CurrentPointerTarget);
                    var view = tempCurve.GetComponent<PhotonView>();
                    var goalView = target?.GetComponent<PhotonView>();
                    //Is the coursor pointing on a valid target?
                    if (target != null && target != tempCurve.start)
                    {
                        //Set the goal of the temp curve to the temp goal
                        if (tempCurve.goal != target)
                        {
                            view.RPC("SetGoal", RpcTarget.All, goalView.ViewID);
                        }
                    }
                    else
                    {
                        //Restore the normal temp curve
                        if (tempCurve.goal != tempCurve)
                        {
                            view.RPC("SetGoal", RpcTarget.All, tempGoal.GetComponent<PhotonView>().ViewID);
                        }
                        tempGoal.transform.position = mainPointer.Position;
                    }
                    var cursor = (AnimatedCursor)mainPointer.BaseCursor;
                    if (thisFrameClicked)
                    {
                        if (target != null && target != tempCurve.start)
                            CreateConnectionCurveScene(tempCurve.start, target);
                        ChangeState(State.defaultMode);
                    }
                }
                else
                {
                    ChangeState(State.defaultMode);
                }
                break;

            case State.disconnecting:
                if (pointerValid)
                {
                    //Update Delete Cube transform
                    var ray = mainPointer.Rays[0];
                    instantiatedDeletCube.transform.position = ray.Origin + ray.Direction.normalized * 0.5f;
                    instantiatedDeletCube.transform.rotation = mainPointer.Rotation;

                    //Necassrary because you shouldn't delete an object from a list, while iterating over it
                    List<ConnectionCurve> curvesToDelete = new List<ConnectionCurve>();
                    //Check which curves colide with the delete cube
                    foreach (ConnectionCurve connectionCurve in curves)
                    {
                        Vector3[] curve = new Vector3[connectionCurve.lineRenderer.positionCount];
                        connectionCurve.lineRenderer.GetPositions(curve);
                        var view = connectionCurve.GetComponent<PhotonView>();
                        if (CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal, 0b100000000, false, 0.05f))
                        {
                            connectionCurve.isMarked = true;
                            //TODO is marked is not used properly
                            if (PhotonNetwork.InRoom)
                            {
                                view.RPC("SetColor", RpcTarget.All, ColorPhoton(Color.red), ColorPhoton(Color.yellow));
                            }
                            else
                            {
                                connectionCurve.SetColor(ColorPhoton(Color.red), ColorPhoton(Color.yellow));
                            }

                            if (thisFrameClicked)
                            {
                                curvesToDelete.Add(connectionCurve);
                            }
                        }
                        else if(connectionCurve.isMarked)
                        {
                            connectionCurve.isMarked = false;
                            if (PhotonNetwork.InRoom)
                            {
                                view.RPC("ResetColor", RpcTarget.All);
                            }
                            else
                            {
                                connectionCurve.ResetColor();
                            }
                        }
                    }

                    foreach (ConnectionCurve connectionCurve in curvesToDelete)
                    {
                        DeleteConnectionCurve(connectionCurve);
                        deletedSomething = true;
                    }

                    //When the user clicked and enough time passed since starting the delete mode, then stopped clicking and if that wasn't the click that started the disconnect mode and it 
                    //was nothing disconnected while in disconnect mode, go back to default mode
                    if (lastFrameClicked && !thisFrameClicked && curvesToDelete.Count == 0 && !deletedSomething && !startedDeletion)
                    {
                        ChangeState(State.defaultMode);
                    }

                    if (lastFrameClicked && !thisFrameClicked)
                    {
                        deletedSomething = false;
                    }
                    startedDeletion = false;
                }
                else
                {
                    ChangeState(State.defaultMode);
                }
                break;
        }
        lastFrameClicked = thisFrameClicked;
    }

    /// <summary>
    /// Change the state of the Connection Curve Manager. The function automaticly ends all effects from the old state.
    /// </summary>
    public void ChangeState(State state , GameObject start = null, AppBarStateController caller = null)
    {
        
        if (state == currState)
            return;

        //End the effects from the old state
        switch (currState)
        {
            case State.disconnecting:
                StopDisconnect();
                break;
            case State.connecting:
                StopConnect();
                break;
        }

        //Initalise the new state
        switch (state)
        {
            case State.defaultMode:
                currState = State.defaultMode;
                break;
            case State.disconnecting:
                StartDisconnect();
                currState = State.disconnecting;
                break;
            case State.connecting:
                StartConnecting(start, caller);
                currState = State.connecting;
                break;
        }
        clickTimeStamp = DateTime.Now;
    }

    /// <summary>
    /// Destroys a Connection Curve while resepecting that it can be networked and the the requestimg client may not have the ownership at the moment.
    /// </summary>
    async void DeleteConnectionCurve(ConnectionCurve connectionCurve)
    {
        if (PhotonNetwork.InRoom)
        {
            var view = connectionCurve.GetComponent<PhotonView>();
            view.RequestOwnership();
            //Curves remove them selfe from the list. This is done here again, so a client doesn't queu a curve multiple times for deletion
            curves.Remove(connectionCurve);
            while (view.Owner != PhotonNetwork.LocalPlayer)
            {
                await Task.Yield();
            }
            PhotonNetwork.Destroy(view);
        }
        else
        {
            Destroy(connectionCurve.gameObject);
        }
    }

    /// <summary>
    /// Creates a Connection Curve from start to goal. When the user is in a room, the curve is instantiated as photon scene object. Otherwise it is instantiated normally.
    /// </summary>
    public void CreateConnectionCurveScene(GameObject start, GameObject goal)
    {
        if (PhotonNetwork.InRoom)
        {
            object[] data = new object[2];
            data[0] = start.GetComponent<PhotonView>().ViewID;
            data[1] = goal.GetComponent<PhotonView>().ViewID;
            ResourceManager.Instance.SceneNetworkInstantiate(curveConnectPrefab, Vector3.zero, Quaternion.identity, (x) => {}, data);
        }
        else
        {
            ConnectionCurve curve = Instantiate(curveConnectPrefab).GetComponent<ConnectionCurve>();
            curve.start = start;
            curve.goal = goal;
        }
    }

    /// <summary>
    /// Creates a Connection Curve from start to goal. When the user is in a room, the curve is instantiated as photon object, with the creator as owner and creator. Otherwise it is instantiated normally.
    /// </summary>
    public ConnectionCurve CreateConnectionCurveOwn(GameObject start, GameObject goal)
    {
        ConnectionCurve curve;
        if (PhotonNetwork.InRoom)
        {
            object[] data = new object[2];
            data[0] = start.GetComponent<PhotonView>().ViewID;
            data[1] = goal.GetComponent<PhotonView>().ViewID;
            curve = PhotonNetwork.Instantiate("Connection Curve", Vector3.zero, Quaternion.identity, 0, data).GetComponent<ConnectionCurve>();
        }
        else
        {
            curve = Instantiate(curveConnectPrefab).GetComponent<ConnectionCurve>();
            curve.start = start;
            curve.goal = goal;
        }
        return curve;
    }

    /// <summary>
    /// Start the connect process.
    /// </summary>
    void StartConnecting(GameObject start, AppBarStateController caller)
    {
        RefreshPointer();
        this.caller = caller;
        if (PhotonNetwork.InRoom)
        {
            tempGoal = PhotonNetwork.Instantiate("Temp Goal", mainPointer.Position, Quaternion.identity);
            tempCurve = CreateConnectionCurveOwn(start, tempGoal);
            tempCurve.GetComponent<PhotonView>().RPC("SetColor", RpcTarget.All, ColorPhoton(Color.yellow), ColorPhoton(Color.yellow));
        }
        else
        {
            tempGoal = Instantiate(tempGoalPrefab);
            tempCurve = CreateConnectionCurveOwn(start, tempGoal);
            tempCurve.SetColor(ColorPhoton(Color.yellow), ColorPhoton(Color.yellow));
        }
    }

    /// <summary>
    /// Stop the connect process.
    /// </summary>
    void StopConnect()
    {
        caller.Collapse();
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Destroy(tempGoal);
        }
        else
        {
            Destroy(tempGoal);
        }
        DeleteConnectionCurve(tempCurve);
        startedDeletion = false;
    }

    /// <summary>
    /// Start the disconnect process.
    /// </summary>
    void StartDisconnect()
    {
        if (currState != State.disconnecting)
        {
            currState = State.disconnecting;
            RefreshPointer();
            instantiatedDeletCube = Instantiate(DeleteCube); 
        }
        startedDeletion = true;
    }

    /// <summary>
    /// Stop the disconnect process.
    /// </summary>
    void StopDisconnect()
    {
        Destroy(instantiatedDeletCube);
        foreach (ConnectionCurve curve in curves)
        {
            if (curve.isMarked)
            {
                if (PhotonNetwork.InRoom)
                {
                    curve.GetComponent<PhotonView>().RPC("ResetColor", RpcTarget.All);
                }
                else
                {
                    curve.ResetColor();
                }
            }
        }
        deletedSomething = false;
    }

    /// <summary>
    /// Delet all Connection Curves connected to a GameObject
    /// </summary>
    public void DeleteAllCurvesFromObject(GameObject startOrEndPoint)
    {
        List<ConnectionCurve> curvesToDelete = new List<ConnectionCurve>();
        foreach (ConnectionCurve connectionCurve in curves)
        {
            if (connectionCurve.start == startOrEndPoint || connectionCurve.goal == startOrEndPoint)
            {
                curvesToDelete.Add(connectionCurve);
            }
        }
        foreach (ConnectionCurve connectionCurve in curvesToDelete)
        {
            DeleteConnectionCurve(connectionCurve);
        }
        
    }

    /// <summary>
    /// Delet all Connection Curves managed by the ConnectionCurveManager
    /// </summary>
    public void DeleteAllCurves()
    {
        var curvesToDelete = new List<ConnectionCurve>(curves);
        foreach (ConnectionCurve connectionCurve in curvesToDelete)
        {
            DeleteConnectionCurve(connectionCurve);
        }
    }

    /// <summary>
    /// Recalculate the MRTK pointer. This is necassary in regular intervalls, because user can connect or disconnect input sources at any time.
    /// </summary>
    public void RefreshPointer()
    {
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == InputSourceType.Controller || source.SourceType == InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is IMixedRealityNearPointer)
                    {
                        // Ignore near pointers, we only want the rays
                        continue;
                    }
                    if (p.Result != null)
                    {
                        mainPointer = p;
                        return;
                    }

                }
            }
        }
    }

    public float[] ColorPhoton(Color color)
    {
        float[] arr = new float[4];
        arr[0] = color.r;
        arr[1] = color.g;
        arr[2] = color.b;
        arr[3] = color.a;
        return arr;
    }

    /// <summary>
    /// Return the closest object in the hirachy above the provided gameObject that has a PhotonView. If such an object doesn't exist, null is returned.
    /// </summary>
    public static GameObject GetParentWithPhotonView(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return null;
        }
        var view = gameObject.GetComponent<PhotonView>();
        var parent = gameObject.transform.parent?.gameObject;
        if (view != null)
        {
            return gameObject;
        }
        else if (parent != null)
        {
            return GetParentWithPhotonView(parent);
        }
        else
        {
            return null;
        }
    }
}