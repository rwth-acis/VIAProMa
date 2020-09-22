using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

public class JoinedCurveGeneration : MonoBehaviour
{
    public float standartHeight = 0.2f;



    //public static async Task UpdateAsync(List<ConnectionCurve> curves, float stepSize)
    //{
    //    try
    //    {
    //        while (true)
    //        {
    //            //Check the standart curve and calculate the boundingboxes on the main thread
    //            BoundingBoxes[] boxes = new BoundingBoxes[curves.Count];
    //            for(int i = 0; i < curves.Count; i++)
    //            {
    //                if (curves[i] != null)
    //                {
    //                    //Try to use the standart curve
    //                    Vector3[] result = SimpleCurveGerneration.TryToUseStandartCurve(curves[i].start, curves[i].goal,60);
    //                    if (result != null)
    //                    {
    //                        curves[i].lineRenderer.positionCount = result.Length;
    //                        curves[i].lineRenderer.SetPositions(result);
    //                    }
    //                    else
    //                    {
    //                        boxes[i] = SimpleCurveGerneration.CalculateBoundingBoxes(curves[i].start, curves[i].goal);
    //                    }
    //                }
    //            }
    //            //Calculate the simple curve on multiple threads (they dont't need the UnityAPI)
    //            var multiThreadTasks = new Dictionary<Task<Vector3[]>, ConnectionCurve>();
    //            var mainThreadTasks = new Dictionary<Task<Vector3[]>, ConnectionCurve>();
    //            for (int i = 0; i < boxes.Length; i++)
    //            {
    //                if (boxes[i] != null && curves[i] != null)
    //                {
    //                    Vector3 start = curves[i].start.transform.position;
    //                    Vector3 goal = curves[i].goal.transform.position;
    //                    BoundingBoxes box = boxes[i];
    //                    ConnectionCurve curve = curves[i];
    //                    multiThreadTasks.Add(Task.Run<Vector3[]>(() => { return SimpleCurveGerneration.StartGeneration(start, goal, box, 60); }),curve);
    //                }
    //            }

    //            while (multiThreadTasks.Count > 0)
    //            {
    //                Task<Vector3[]> finishedTask = await Task.WhenAny(multiThreadTasks.Keys);
    //                ConnectionCurve connectionCurve = multiThreadTasks[finishedTask];
    //                Vector3[] curve = finishedTask.Result;
    //                //connectionCurve can somehow be null here
    //                if (connectionCurve != null && !CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal))
    //                {
    //                    connectionCurve.lineRenderer.positionCount = finishedTask.Result.Length;
    //                    connectionCurve?.lineRenderer.SetPositions(finishedTask.Result);
    //                }
    //                else
    //                {
    //                    mainThreadTasks.Add(JoinedCurve(connectionCurve, stepSize), connectionCurve);
    //                }
    //                multiThreadTasks.Remove(finishedTask);
    //            }

    //            //When necassary, calculate the pathfinding curves, but on the main thread (they need the UnityAPI, which is not thread safe)
    //            while (mainThreadTasks.Count > 0)
    //            {
    //                Task<Vector3[]> finishedTask = await Task.WhenAny(mainThreadTasks.Keys);
    //                ConnectionCurve connectionCurve = mainThreadTasks[finishedTask];
    //                //connectionCurve can somehow be null here
    //                if (connectionCurve != null)
    //                {
    //                    connectionCurve.lineRenderer.positionCount = finishedTask.Result.Length;
    //                    connectionCurve?.lineRenderer.SetPositions(finishedTask.Result);
    //                }
    //                mainThreadTasks.Remove(finishedTask);
    //            }
    //            await Task.Yield();
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError(e.InnerException);
    //        throw e;
    //    }
    //}

    public static async Task UpdateAsync(List<ConnectionCurve> curves, float stepSize)
    {
        int segmentCount = 60;
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
                        Vector3[] standartCurve = SimpleCurveGerneration.TryToUseStandartCurve(curves[i].start, curves[i].goal, segmentCount);
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
                NativeArray<BoundingBoxes> boxes = new NativeArray<BoundingBoxes>(boxList.Count,Allocator.TempJob);
                int count = boxes.Length;
                boxes.CopyFrom(boxList.ToArray());

                //Setup the job
                NativeArray<Vector3> startArray = new NativeArray<Vector3>(count, Allocator.TempJob);
                NativeArray<Vector3> goalArray = new NativeArray<Vector3>(count, Allocator.TempJob);
                for (int i = 0; i < count; i++)
                {
                    int curveIndex = boxes[i].curveIndex;
                    startArray[i] = curves[curveIndex].start.transform.position;
                    goalArray[i] = curves[curveIndex].goal.transform.position;
                }

                SimpleCurveGenerationJob jobData = new SimpleCurveGenerationJob();
                jobData.boxes = boxes;
                jobData.start = startArray;
                jobData.goal = goalArray;
                //var dim2Arr = new NativeArray<int>(1, Allocator.TempJob);
                //dim2Arr[0] = (segmentCount + 1);
                //jobData.dim2Arr = dim2Arr;
                //jobData.dim2 = (segmentCount + 1);
                //var result = new NativeArray<Vector3>(63 * boxes.Length, Allocator.TempJob);
                //jobData.result = result;
                //jobData.result = new TwoDNativeArray<Vector3>(count,63, Allocator.TempJob);

                //---
                {
                    jobData.curvePoint0 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint1 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint2 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint3 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint4 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint5 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint6 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint7 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint8 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint9 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint10 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint11 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint12 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint13 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint14 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint15 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint16 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint17 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint18 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint19 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint20 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint21 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint22 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint23 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint24 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint25 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint26 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint27 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint28 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint29 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint30 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint31 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint32 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint33 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint34 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint35 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint36 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint37 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint38 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint39 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint40 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint41 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint42 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint43 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint44 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint45 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint46 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint47 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint48 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint49 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint50 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint51 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint52 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint53 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint54 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint55 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint56 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint57 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint58 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint59 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint60 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint61 = new NativeArray<Vector3>(count, Allocator.TempJob);
                    jobData.curvePoint62 = new NativeArray<Vector3>(count, Allocator.TempJob);
                }
                //---


                JobHandle handel = jobData.Schedule(boxes.Length, 1);
                handel.Complete();

                //Fetch the results
                for (int i = 0; i < count; i++)
                {
      
                    //int offset = i * (segmentCount+1);
                    //Vector3[] curve = new Vector3[segmentCount+1];
                    //for (int j = 0; j < (segmentCount + 1); j++)
                    //{
                    //    //curve[i] = result[offset+i];
                    //    curve[i] = jobData.result.oneDArray[offset+i];
                    //}
                    //if (curves[i] != null)
                    //{
                    //    curves[i].lineRenderer.positionCount = curve.Length;
                    //    curves[i].lineRenderer.SetPositions(curve);
                    //}
                    if (curves[i] != null)
                    {
                        Vector3[] curve = new Vector3[63];
                        curve = jobData.ReadResult(i);
                        int curveIndex = jobData.boxes[i].curveIndex;
                        curves[curveIndex].lineRenderer.positionCount = curve.Length;
                        curves[curveIndex].lineRenderer.SetPositions(curve);
                    }
                }

                boxes.Dispose();
                startArray.Dispose();
                goalArray.Dispose();

                //---
                {
                    jobData.curvePoint0.Dispose();
                    jobData.curvePoint1.Dispose();
                    jobData.curvePoint2.Dispose();
                    jobData.curvePoint3.Dispose();
                    jobData.curvePoint4.Dispose();
                    jobData.curvePoint5.Dispose();
                    jobData.curvePoint6.Dispose();
                    jobData.curvePoint7.Dispose();
                    jobData.curvePoint8.Dispose();
                    jobData.curvePoint9.Dispose();
                    jobData.curvePoint10.Dispose();
                    jobData.curvePoint11.Dispose();
                    jobData.curvePoint12.Dispose();
                    jobData.curvePoint13.Dispose();
                    jobData.curvePoint14.Dispose();
                    jobData.curvePoint15.Dispose();
                    jobData.curvePoint16.Dispose();
                    jobData.curvePoint17.Dispose();
                    jobData.curvePoint18.Dispose();
                    jobData.curvePoint19.Dispose();
                    jobData.curvePoint20.Dispose();
                    jobData.curvePoint21.Dispose();
                    jobData.curvePoint22.Dispose();
                    jobData.curvePoint23.Dispose();
                    jobData.curvePoint24.Dispose();
                    jobData.curvePoint25.Dispose();
                    jobData.curvePoint26.Dispose();
                    jobData.curvePoint27.Dispose();
                    jobData.curvePoint28.Dispose();
                    jobData.curvePoint29.Dispose();
                    jobData.curvePoint30.Dispose();
                    jobData.curvePoint31.Dispose();
                    jobData.curvePoint32.Dispose();
                    jobData.curvePoint33.Dispose();
                    jobData.curvePoint34.Dispose();
                    jobData.curvePoint35.Dispose();
                    jobData.curvePoint36.Dispose();
                    jobData.curvePoint37.Dispose();
                    jobData.curvePoint38.Dispose();
                    jobData.curvePoint39.Dispose();
                    jobData.curvePoint40.Dispose();
                    jobData.curvePoint41.Dispose();
                    jobData.curvePoint42.Dispose();
                    jobData.curvePoint43.Dispose();
                    jobData.curvePoint44.Dispose();
                    jobData.curvePoint45.Dispose();
                    jobData.curvePoint46.Dispose();
                    jobData.curvePoint47.Dispose();
                    jobData.curvePoint48.Dispose();
                    jobData.curvePoint49.Dispose();
                    jobData.curvePoint50.Dispose();
                    jobData.curvePoint51.Dispose();
                    jobData.curvePoint52.Dispose();
                    jobData.curvePoint53.Dispose();
                    jobData.curvePoint54.Dispose();
                    jobData.curvePoint55.Dispose();
                    jobData.curvePoint56.Dispose();
                    jobData.curvePoint57.Dispose();
                    jobData.curvePoint58.Dispose();
                    jobData.curvePoint59.Dispose();
                    jobData.curvePoint60.Dispose();
                    jobData.curvePoint61.Dispose();
                    jobData.curvePoint62.Dispose();
                }
                //---
                //dim2Arr.Dispose();
                //jobData.result.oneDArray.Dispose();
                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.InnerException);
            throw e;
        }
    }

    static async Task<Vector3[]> JoinedCurve(ConnectionCurve connectionCurve, float stepSize, int segmentCount = 60)
    {
        try
        {
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
            Debug.LogError(e.InnerException);
            throw e;
        }
    }
}
