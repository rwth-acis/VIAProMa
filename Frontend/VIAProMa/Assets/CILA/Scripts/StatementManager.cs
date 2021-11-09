using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinCan;
using TinCan.LRSResponses;
using System;
using BarGraph.VittorCloud;
using i5.VIAProMa.UI;
using System.Linq;


[Serializable]
public class listData
{
    public string email;
    public List<double> scoreMin = new List<double>();
    public List<double> scoreMax = new List<double>();
    public List<double> scoreRaw = new List<double>();
    public List<double> scoreScaled = new List<double>();
    public DateTime storedDate;
}
public class StatementManager : MonoBehaviour
{
    public static StatementManager instance;
    public List<listData> listDatas = new List<listData>();
    public bool isStudent;
    bool StudentMode;
    bool MentorMode;
    public string nameStudent;

    private RemoteLRS lrs;
    StatementsResultLRSResponse lrsResponse;
    public string dateTimeString = "2021-10-01";

    public List<BarGraphDataSet> DataSet;
    public List<BarGraphDataSet> DataSet2;
    BarGraphGenerator barGraphGenerator;
    Vector3 defScale;
    public Vector3 scaleSizeMentor;
    public Vector3 scaleSizeStudent;
    public GameObject barChartPrefab;
    public GameObject scatterPrefab;

    // Use this for initialization
    private void Awake()
    {
        defScale = transform.localScale;
        instance = this;
    }

    private void OnEnable()
    {
        //nameStudent = "suchi.julidayani@rwth-aachen.de";

        lrs = new RemoteLRS(
           "https://lrs.tech4comp.dbis.rwth-aachen.de/data/xAPI",
        "79c09762728c78bab3309dd56899e0a5d67a4f76",
        "be4b2e4b958c2c5bd119fe1dd31dc466a85af8c4"
        );

        barGraphGenerator = GetComponent<BarGraphGenerator>();
        GetStatement();

        print(barGraphGenerator);

        // if the DataSet2 list is empty then return
        if(DataSet2.Count == 0)
        {
            Debug.LogError("Dataset2 is Empty!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetStatement();
        }
    }

    
}
