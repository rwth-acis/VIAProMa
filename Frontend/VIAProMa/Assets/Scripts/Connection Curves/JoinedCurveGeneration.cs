using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using HoloToolkit.Unity;

public class JoinedCurveGeneration  : Singleton<JoinedCurveGeneration>
{
    public float standartHeight = 0.2f;
    SimpleCurveGenerationJob jobData;


    /// <summary>
    /// Updates the curves that are provided through the curves list. Can't be a normal update fuction, because it can yield when calculation take too long. Is invoked by the ConnectionCurveManager.
    /// </summary>
    /// <param name="curves"></param>
    /// <param name="stepSize"></param>
    public async void UpdateAsyc(List<ConnectionCurve> curves, float stepSize)
    {
        //Specifies, for how many curves memory is alllocated (memory of nativ arrrays has to be manged by hand in UnityJobs). 
        //If more curves are used than memory is allocated for, the number of allocated curves get increased by allocationSteps.
        //If there are less curves than curveCountEstimate-1.5*allocationSteps, memory for allocationSteps curves gets freed again.
        int allocationSteps = 5;
        int curveCountEstimate = allocationSteps;
        jobData = new SimpleCurveGenerationJob();
        jobData.InitialiseArrays(curveCountEstimate);
        
        try
        {
            while (true)
            {
                //Check the standart curve and calculate the boundingboxes on the main thread
                List<BoundingBoxes> boxList = new List<BoundingBoxes>();

                for (int i = 0; i < curves.Count; i++)
                {
                    if (curves[i] != null)
                    {
                        //Try to use the standart curve
                        Vector3[] standartCurve = SimpleCurveGerneration.TryToUseStandartCurve(curves[i].start, curves[i].goal, 60);
                        if (standartCurve != null)
                        {
                            curves[i].lineRenderer.positionCount = standartCurve.Length;
                            curves[i].lineRenderer.SetPositions(standartCurve);
                        }
                        else
                        {
                            BoundingBoxes box = SimpleCurveGerneration.CalculateBoundingBoxes(curves[i].start, curves[i].goal);
                            box.curveIndex = i;
                            boxList.Add(box);
                        }
                    }
                }

                int count = boxList.Count;

                //Are there curently more curves than memory was allocated for?
                if (count > curveCountEstimate)
                {
                    jobData.DisposeArrays();
                    curveCountEstimate += allocationSteps;
                    jobData.InitialiseArrays(curveCountEstimate);
                    Debug.Log("Increas array size");
                }
                //Are there much less curves than memory was allocated for?
                else if (count < curveCountEstimate - 1.5 * allocationSteps)
                {
                    jobData.DisposeArrays();
                    curveCountEstimate -= allocationSteps;
                    jobData.InitialiseArrays(curveCountEstimate);
                    Debug.Log("Decreas array size");
                }

                //Setup the job
                for (int i = 0; i < count; i++)
                {
                    int curveIndex = boxList[i].curveIndex;
                    jobData.start[i] = curves[curveIndex].start.transform.position;
                    jobData.goal[i] = curves[curveIndex].goal.transform.position;
                    //The normal copy function is not possible, because boxes and boxList may have diffent lenghtes
                    jobData.boxes[i] = boxList[i];
                }


              
                JobHandle handel = jobData.Schedule(count, 32);
                handel.Complete();


                var tasks = new Dictionary<Task<Vector3[]>, ConnectionCurve>();
                //Fetch the results
                for (int i = 0; i < count; i++)
                {
                    if (curves[i] != null)
                    {
                        Vector3[] simpleCurve = jobData.ReadResult(i);
                        ConnectionCurve curve = curves[jobData.boxes[i].curveIndex];
                        tasks.Add(JoinedCurve(curve,simpleCurve,stepSize), curve);
                    }
                }

                //The following actions can't be done in a Unity job, becuase they are not thread safe.
                while (tasks.Count > 0)
                {
                    Task<Vector3[]> finishedTask = await Task.WhenAny(tasks.Keys);
                    ConnectionCurve connectionCurve = tasks[finishedTask];
                    //connectionCurve can somehow be null here
                    if (connectionCurve != null)
                    {
                        connectionCurve.lineRenderer.positionCount = finishedTask.Result.Length;
                        connectionCurve.lineRenderer.SetPositions(finishedTask.Result);
                    }
                    tasks.Remove(finishedTask);
                }
                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.InnerException);
        }
    }


    static async Task<Vector3[]> JoinedCurve(ConnectionCurve connectionCurve, Vector3[] simpleCurve , float stepSize, int segmentCount = 60)
    {
        try
        {
            //Check, if the simple curve is collision free
            if (simpleCurve != null && !CurveGenerator.CurveCollsionCheck(simpleCurve, connectionCurve.start, connectionCurve.goal))
            {
                return simpleCurve;
            }
            //If not, use A* or Greedy
            Vector3[] curve;
            IntTriple startCell = IntTriple.VectorToCell(connectionCurve.start.transform.position, stepSize);
            IntTriple goalCell = IntTriple.VectorToCell(connectionCurve.goal.transform.position, stepSize);
            Task<GridSearch.SearchResult<IntTriple>> astarTask = AStar.AStarGridSearchAsync(startCell, goalCell, stepSize, connectionCurve.start, connectionCurve.goal);
            await astarTask;
            if (astarTask.Result.path == null)
            {
                Task<GridSearch.SearchResult<IntTriple>> greedyTask = Greedy.GreedyGridSearchAsync(startCell, goalCell, stepSize, connectionCurve.start, connectionCurve.goal);
                await greedyTask;
                if (greedyTask.Result.path == null)
                {
                    curve = SimpleCurveGerneration.StandartCurve(connectionCurve.start.transform.position, connectionCurve.goal.transform.position, segmentCount, 0.5f);
                }
                else
                {
                    curve = CurveGenerator.IntTripleArrayToCurve(greedyTask.Result.path, connectionCurve.start.transform.position, connectionCurve.goal.transform.position, stepSize);
                }
            }
            else
            {
                curve = CurveGenerator.IntTripleArrayToCurve(astarTask.Result.path, connectionCurve.start.transform.position, connectionCurve.goal.transform.position, stepSize);
            }
            
            return curve;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.InnerException);
            return null;
        }
    }

    private void OnApplicationQuit()
    {
        //Necessary to prevent memory leaks. Native arrays are not cleaned up by the garbage collector.
        jobData.DisposeArrays();
    }

}
