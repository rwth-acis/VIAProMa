using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using System.Diagnostics;
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
        //These arrays need to be allocated persitent, because allocating and disposing them each frame needs way too long.
        int curveCountEstimate = 10;
        jobData = new SimpleCurveGenerationJob();
        jobData.InitialiseArrays(curveCountEstimate);
        jobData.boxes = new NativeArray<BoundingBoxes>(curveCountEstimate, Allocator.Persistent);
        jobData.start = new NativeArray<Vector3>(curveCountEstimate, Allocator.Persistent);
        jobData.goal = new NativeArray<Vector3>(curveCountEstimate, Allocator.Persistent);
        

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

                //Setup the job
                for (int i = 0; i < count; i++)
                {
                    int curveIndex = boxList[i].curveIndex;
                    jobData.start[i] = curves[curveIndex].start.transform.position;
                    jobData.goal[i] = curves[curveIndex].goal.transform.position;
                    //The normal copy function is not possible, because boxes and boxList may have diffent lenghtes
                    jobData.boxes[i] = boxList[i];
                }


              
                JobHandle handel = jobData.Schedule(count, 1);
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
                //startArray.Dispose();
                //goalArray.Dispose();
                //boxes.Dispose();
                //jobData.DisposeArrays();

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

    public static async void test(List<ConnectionCurve> curves)
    {
        Stopwatch watch;
        while (curves.Count == 0)
        {
            await Task.Yield();
        }
        ConnectionCurve curve = curves[0];
        while (true)
        {
            watch = Stopwatch.StartNew();
            Vector3[] path = SimpleCurveGerneration.StartGeneration(curve.start.transform.position, curve.goal.transform.position, SimpleCurveGerneration.CalculateBoundingBoxes(curve.start, curve.goal), 60);
            watch.Stop();
            UnityEngine.Debug.Log("Time: " + watch.Elapsed.TotalMilliseconds);
            await Task.Yield();
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
