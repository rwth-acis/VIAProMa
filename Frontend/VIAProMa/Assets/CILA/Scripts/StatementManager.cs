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
    public List<listData> listData = new List<listData>();
    public List<float> score = new List<float>();
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
    public GameObject scatterPlotPrefab;

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

    public void CheckRole()
    {
        transform.localScale = defScale;
        bool isExist = false;
        //Build out full Statement details based on verb viewed to get the user role
        var query = new StatementsQuery();
        query.verbId = new Uri("http://id.tincanapi.com/verb/viewed");
        query.limit = 500;
        lrsResponse = lrs.QueryStatements(query);
        print(lrsResponse.success);
        if (lrsResponse.success) // Get success
        {
            print(lrsResponse.content.statements.Count);
            for (int i = 0; i < lrsResponse.content.statements.Count; i++)
            {
                if (lrsResponse.content.statements[i] != null)
                {
                    print(lrsResponse.content.statements[i].ToJSON());
                    if (lrsResponse.content.statements[i].actor.account != null && lrsResponse.content.statements[i].actor.account.name.Contains("@"))
                    {
                        if (lrsResponse.content.statements[i].actor.account.name == nameStudent)
                        {
                            if (lrsResponse.content.statements[i].context.extensions.ToJSON().Contains("student"))
                            {
                                print("student");
                                isStudent = true;
                                GetStatement();
                                return;
                            }
                            else
                            {
                                print("mentor");
                                isStudent = false;
                                GetStatement();
                                return;
                            }
                        }
                    }
                }
            }
        }
        else //Get Statement fails
        {
            Debug.Log("Statement Failed:" + lrsResponse.errMsg);
        }
    }

    public void GetStatement()
    {
        transform.localScale = defScale;
        bool isExist = false;
        //Build out full Statement details based on verb completed to get the user learning data
        var query = new StatementsQuery();
        query.verbId = new Uri("https://w3id.org/xapi/dod-isd/verbs/completed");
        query.since = System.DateTime.Parse(dateTimeString);

        lrsResponse = lrs.QueryStatements(query);

        if (lrsResponse.success) //Get Statement Success
        {
            if (isStudent)
            {
                studentData(nameStudent);
            }
            else
            {
                mentorData();
            }
        }
        else //Get Statement fails
        {
            Debug.Log("Statement Failed:" + lrsResponse.errMsg);
        }
    }

    public void studentData(string emailStudent)
    {
        if (StudentMode)
            return;
        MentorMode = false;
        StudentMode = true;
        transform.localScale = defScale;
        DataSet.Clear();
        bool isExist = false;
        for (int i = 0; i < lrsResponse.content.statements.Count; i++)
        {
            if (lrsResponse.content.statements[i] != null)
            {
                if (lrsResponse.content.statements[i].actor.account != null)
                {
                    if (lrsResponse.content.statements[i].actor.account.name == emailStudent)
                    {
                        string namequiz = lrsResponse.content.statements[i].ToJObject()["object"]["definition"]["name"]["en-US"].ToObject<string>();
                        string[] namequizsplit = namequiz.Split(' ');
                        for (int j = 0; j < DataSet.Count; j++)
                        {
                            if (DataSet[j].GroupName == namequizsplit[0])
                            {
                                
                                XYBarValues values = new XYBarValues();
                                if (namequizsplit.Length > 2)
                                    values.XValue = namequizsplit[1] + " " + namequizsplit[2];
                                else
                                    values.XValue = namequiz;
                                values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                                DataSet[j].ListOfBars[1] = values;
                                isExist = true;
                                break;
                            }
                            else
                                isExist = false;
                        }
                        if (!isExist)
                        {
                            BarGraphDataSet bar = new BarGraphDataSet();
                            XYBarValues values = new XYBarValues();
                            bar.GroupName = namequizsplit[0];
                            bar.ListOfBars = new List<XYBarValues>(new XYBarValues[2]);
                            for (int k = 0; k < bar.ListOfBars.Count; k++)
                            {
                                if (k == 0)
                                {
                                    bar.ListOfBars[k] = new XYBarValues();
                                    if (namequizsplit.Length > 2)
                                    {
                                        bar.ListOfBars[k].XValue = namequizsplit[1] + " " + namequizsplit[2];
                                    }
                                    else
                                        bar.ListOfBars[k].XValue = namequiz;
                                    bar.ListOfBars[k].YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                                }
                                else
                                {
                                    bar.ListOfBars[k] = new XYBarValues();
                                    if (namequizsplit.Length > 2)
                                        bar.ListOfBars[k].XValue = namequizsplit[1] + " " + namequizsplit[2];
                                    else
                                        bar.ListOfBars[k].XValue = namequiz;
                                    bar.ListOfBars[k].YValue = 0;
                                }
                            }
                            DataSet.Add(bar);
                        }
                    }
                }
            }
        }
        DataSet2 = DataSet;
        barGraphGenerator.GeneratBarGraph(DataSet2);
        transform.localScale = scaleSizeStudent;
    }

    public void mentorData()
    {
        if (MentorMode)
            return;
        StudentMode = false;
        MentorMode = true;
        transform.localScale = defScale;
        DataSet.Clear();
        bool isExist = false;
        bool isExistMentor = false;
        for (int i = 0; i < lrsResponse.content.statements.Count; i++)
        {
            if (lrsResponse.content.statements[i] != null)
            {

                if (lrsResponse.content.statements[i].actor.account != null)
                {
                    string namequiz = lrsResponse.content.statements[i].ToJObject()["object"]["definition"]["name"]["en-US"].ToObject<string>();

                    for (int j = 0; j < DataSet.Count; j++)
                    {
                        if (!DataSet[j].ListOfBars.Any(f => f.XValue == namequiz))
                        {
                            XYBarValues values = new XYBarValues();
                            values.XValue = namequiz;
                            DataSet[j].ListOfBars.Add(values);
                        }
                        if (DataSet[j].email == lrsResponse.content.statements[i].actor.account.name)
                        {
                            XYBarValues values = new XYBarValues();
                            values.XValue = namequiz;
                            values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                            int idNotFound = DataSet[j].ListOfBars.FindIndex(g => g.XValue == values.XValue);
                            DataSet[j].ListOfBars[idNotFound] = values;
                        }
                    }
                    if (!DataSet.Any(f => f.email == lrsResponse.content.statements[i].actor.account.name))
                    {
                        XYBarValues values = new XYBarValues();
                        BarGraphDataSet bar = new BarGraphDataSet();
                        bar.GroupName = lrsResponse.content.statements[i].actor.name;
                        bar.email = lrsResponse.content.statements[i].actor.account.name;
                        values.XValue = namequiz;
                        values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                        bar.ListOfBars = new List<XYBarValues>();
                        bar.ListOfBars.Add(values);
                        DataSet.Add(bar);
                    }
                }
            }
        }
        DataSet2 = DataSet;
        barGraphGenerator.GeneratBarGraph(DataSet2);
        transform.localScale = scaleSizeMentor;
    }

    public void BackButton()
    {
        if(isStudent)
        {
            studentData(nameStudent);
        }
        else
        {
            mentorData();
        }
    }

    public void BarChart()
    {
        barGraphGenerator.barPrefab = barChartPrefab;
        if(MentorMode)
        { 
            MentorMode = false;
            mentorData();
        }
        else if(StudentMode)
        {
            StudentMode = false;
            studentData(nameStudent);
        }
    }

    public void ScatterPlot()
    {
        barGraphGenerator.barPrefab = scatterPlotPrefab;
        if (MentorMode)
        {
            MentorMode = false;
            mentorData();
        }
        else if (StudentMode)
        {
            StudentMode = false;
            studentData(nameStudent);
        }
    }
}
