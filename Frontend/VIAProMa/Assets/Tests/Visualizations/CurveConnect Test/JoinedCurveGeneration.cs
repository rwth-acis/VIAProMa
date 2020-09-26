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
    public static async void UpdateAsyc(List<ConnectionCurve> curves, float stepSize)
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
                jobData.InitialiseArrays(count);
              
                JobHandle handel = jobData.Schedule(boxes.Length, 1);
                handel.Complete();

                //Fetch the results
                for (int i = 0; i < count; i++)
                {
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
                jobData.DisposeArrays();

                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.InnerException);
            //Try to recover
            UpdateAsyc(curves, stepSize);
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
            return null;
        }
    }
}
