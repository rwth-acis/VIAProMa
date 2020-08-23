using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Threading.Tasks;


public class LineController : MonoBehaviour
{
    public static float stepSize = 1;
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

    //Temp Curve
    ConnectionCurve tempCurve;
    DateTime clickTimeStamp;



    //Test
    public GameObject startTest;
    public GameObject goalTest;
    GameObject tempGoal = null;


    // Start is called before the first frame update
    void Start()
    {
        curves = new List<ConnectionCurve>();
        curveGenerator = GetComponent<JoinedCurveGeneration>();

        //Test
        if (startTest != null && goalTest != null)
        {
            CreateConnectionCurveScene(startTest, goalTest);
        }

        Task test = JoinedCurveGeneration.UpdateAsync(curves,stepSize);
    }

    // Update is called once per frame
    void Update()
    {     
        switch (currState)
        {
            case State.connecting:
                GameObject target = null;
                //For some ungodly reasons objects from the mrtk behave strange when they should be null. They can then still be dereferenced and != null still yields true, but there content is useless.
                //But ToString then returns null.
                if (mainPointer.ToString() != "null")
                {
                    if (mainPointer.Result != null && mainPointer.Result.CurrentPointerTarget != null)
                    {
                        tempCurve.goal = mainPointer.Result.CurrentPointerTarget.transform.root.gameObject;
                    }
                    else
                    {
                        tempCurve.goal = tempGoal;
                        tempCurve.goal.transform.position = mainPointer.Position;
                    }
                    var cursor = (AnimatedCursor)mainPointer.BaseCursor;
                    if (cursor.CursorState == CursorStateEnum.Select && (DateTime.Now - clickTimeStamp).TotalMilliseconds > 30)
                    {
                        if (mainPointer.Result.CurrentPointerTarget != null)
                            target = mainPointer.Result.CurrentPointerTarget.transform.root.gameObject;
                        if (target != null)
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
                if (mainPointer.ToString() != "null")
                {
                    //Update Delete Cube transform
                    var ray = mainPointer.Rays[0];
                    instantiatedDeletCube.transform.position = ray.Origin + ray.Direction.normalized * 0.5f;
                    instantiatedDeletCube.transform.rotation = Quaternion.LookRotation(ray.Direction.normalized, Vector3.up);

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
                            view.RPC("SetColor",RpcTarget.All, ColorPhoton(Color.red), ColorPhoton(Color.yellow));
                            if (((AnimatedCursor)mainPointer.BaseCursor).CursorState == CursorStateEnum.Select)
                            {
                                curvesToDelete.Add(connectionCurve);
                            }
                        }
                        else if(connectionCurve.isMarked)
                        {
                            connectionCurve.isMarked = false;
                            view.RPC("ResetColor", RpcTarget.All);
                        }
                    }

                    foreach (ConnectionCurve connectionCurve in curvesToDelete)
                    {
                        DeleteConnectionCurve(connectionCurve);
                    }
                }
                else
                {
                    ChangeState(State.defaultMode);
                }
                break;
        }
    }

    public void ChangeState(State state , GameObject start = null)
    {
        
        if (state == currState)
            return;

        //End the effects from the old state
        switch (currState)
        {
            case State.disconnecting:
                StopDisconnecting();
                break;
            case State.connecting:
                StopConnecting();
                break;
        }

        //Initalise the new state
        switch (state)
        {
            case State.defaultMode:
                currState = State.defaultMode;
                break;
            case State.disconnecting:
                StartDisconnecting();
                currState = State.disconnecting;
                break;
            case State.connecting:
                StartConnecting(start);
                currState = State.connecting;
                break;
        }
    }

    async Task DeleteConnectionCurve(ConnectionCurve connectionCurve)
    {
        var view = connectionCurve.GetComponent<PhotonView>();
        view.RequestOwnership();
        //Curves remove themselfe from the list. This is done here again, so a client doesn't queu a curve multiple times for deletion
        curves.Remove(connectionCurve);
        //If you are not the master client, wait for the ownership
        while (view.Owner != PhotonNetwork.LocalPlayer)
        {
            await Task.Yield();
        }
        PhotonNetwork.Destroy(view);
    }

    void CreateConnectionCurveScene(GameObject start, GameObject goal)
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
            Instantiate(curveConnectPrefab).GetComponent<ConnectionCurve>();
        }
    }
    ConnectionCurve CreateConnectionCurveOwn(GameObject start, GameObject goal)
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
        }
        return curve;
    }

    void StartConnecting(GameObject start)
    {
        RefreshPointer();
        tempGoal = PhotonNetwork.Instantiate("Temp Goal", mainPointer.Position, Quaternion.identity);
        tempCurve = CreateConnectionCurveOwn(start,tempGoal);
        tempCurve.GetComponent<PhotonView>().RPC("SetColor", RpcTarget.All, ColorPhoton(Color.yellow), ColorPhoton(Color.yellow));
        clickTimeStamp = DateTime.Now;
    }


    void StopConnecting()
    {
        PhotonNetwork.Destroy(tempGoal);
        DeleteConnectionCurve(tempCurve);
    }

    void StartDisconnecting()
    {
        if (currState != State.disconnecting)
        {
            currState = State.disconnecting;
            RefreshPointer();
            instantiatedDeletCube = Instantiate(DeleteCube); 
        }
    }

    void StopDisconnecting()
    {
        Destroy(instantiatedDeletCube);
        foreach (ConnectionCurve curve in curves)
        {
            if (curve.isMarked)
            {
                curve.GetComponent<PhotonView>().RPC("ResetColor", RpcTarget.All);
            }
        }
    }
    public void DeleteCurves(GameObject startOrEndPoint)
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

    public void RefreshPointer()
    {
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == InputSourceType.Hand)
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
}