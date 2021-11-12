using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinCan;
using TinCan.LRSResponses;
using System;
using BarGraph.VittorCloud;
using i5.VIAProMa.UI;
using System.Linq;
using UnityEngine.Networking;

[Serializable]
public class xlistData
{
    public string email;
    public List<double> scoreMin = new List<double>();
    public List<double> scoreScaled = new List<double>();
    public List<double> scoreMax = new List<double>();
    public List<double> scoreRaw = new List<double>();
    public DateTime storedDate;
}
public class StatementSender : MonoBehaviour
{
    public static StatementSender instance;
    public List<xlistData> listDatas = new List<xlistData>();
    public List<float> score = new List<float>();
    public List<float> scoresort = new List<float>();
    public List<float> testsc = new List<float>();
    public bool isStudent;
    bool StudentMode;
    bool MentorMode;
    private RemoteLRS lrs;
    public List<BarGraphDataSet> exampleDataSet; // public data set for inserting data into the bar graph
    public List<BarGraphDataSet> exampleDataSet2; // public data set for inserting data into the bar graph
    BarGraphGenerator barGraphGenerator;
    StatementsResultLRSResponse lrsResponse;
    public string namaStudent;
    Vector3 defScale;
    public Vector3 scaleSizeMentor;
    public Vector3 scaleSizeStudent;
    public string dateTimeString = "2018-12-16";
    public GameObject barChartPrefab;
    public GameObject ScaterPrefab;
    // Use this for initialization

    private void Awake()
    {
        defScale = transform.localScale;
        instance = this;
    }
    void OnEnable()
    {
        var textFile = Resources.Load<TextAsset>("pipeline");
        //StartCoroutine(TestPostman(textFile.ToString()));
        //namaStudent = WindowManagerCILA.Instance.LoginMenu.cachedUserInfo.Email;
        namaStudent = "suchi.julidayani@rwth-aachen.de";
        lrs = new RemoteLRS(
        "https://lrs.tech4comp.dbis.rwth-aachen.de/data/xAPI",
        "79c09762728c78bab3309dd56899e0a5d67a4f76",
        "be4b2e4b958c2c5bd119fe1dd31dc466a85af8c4"
        );
        barGraphGenerator = GetComponent<BarGraphGenerator>();
        SendStatement();


        print(barGraphGenerator);

        //if the exampleDataSet list is empty then return.
        if (exampleDataSet2.Count == 0)
        {

            Debug.LogError("ExampleDataSet is Empty!");
            return;
        }

    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendStatement();
        }

    }

    public void CheckRole()
    {
        transform.localScale = defScale;
        string tempName = "";
        bool ada = false;
        //Build out full Statement details
        var query = new StatementsQuery();
        query.verbId = new Uri("http://id.tincanapi.com/verb/viewed");
        //query.since = System.DateTime.Parse(dateTimeString);
        query.limit = 500;
        //query.activityId = "https://moodle.tech4comp.dbis.rwth-aachen.demod/quiz/view.php?id=438";
        //Send the statement
        lrsResponse = lrs.QueryStatements(query);
        print(lrsResponse.success);
        if (lrsResponse.success) //Success
        {
            print(namaStudent);
            print(lrsResponse.content.statements.Count);
            for (int i = 0; i < lrsResponse.content.statements.Count; i++)
            {
                if (lrsResponse.content.statements[i] != null)
                {
                    print(lrsResponse.content.statements[i].ToJSON());
                    if (lrsResponse.content.statements[i].actor.account != null && lrsResponse.content.statements[i].actor.account.name.Contains("@"))
                    {
                        //print(lrsResponse.content.statements[i].actor.account.name);
                        if (lrsResponse.content.statements[i].actor.account.name == namaStudent)
                        {
                            if (lrsResponse.content.statements[i].context.extensions.ToJSON().Contains("student"))
                            {
                                print("student");
                                isStudent = true;
                                SendStatement();
                                return;
                            }
                            else
                            {
                                print("mentor");
                                isStudent = false;
                                SendStatement();
                                return;
                            }
                        }
                    }
                }
            }
        }
        else //Failure
        {
            Debug.Log("Statement Failed: " + lrsResponse.errMsg);
        }
        print("selesai");
    }
    public void SendStatement()
    {
        transform.localScale = defScale;
        string tempName = "";
        bool ada = false;
        //Build out full Statement details
        var query = new StatementsQuery();
        query.verbId = new Uri("https://w3id.org/xapi/dod-isd/verbs/completed");
        query.since = System.DateTime.Parse(dateTimeString);
        //query.activityId = "https://moodle.tech4comp.dbis.rwth-aachen.demod/quiz/view.php?id=438";
        //Send the statement
        lrsResponse = lrs.QueryStatements(query);
        print(lrsResponse.success);
        if (lrsResponse.success) //Success
        {
            if (isStudent)
            {
                StudentData(namaStudent);
            }
            else
            {
                ManagerData();
            }
        }
        else //Failure
        {
            Debug.Log("Statement Failed: " + lrsResponse.errMsg);
        }
        print("selesai");
    }
    public void StudentData(string emailStudent)
    {
        if (StudentMode)
            return;
        MentorMode = false;
        StudentMode = true;
        transform.localScale = defScale;
        exampleDataSet.Clear();
        bool ada = false;
        for (int i = 0; i < lrsResponse.content.statements.Count; i++)
        {
            if (lrsResponse.content.statements[i] != null)
            {
                if (lrsResponse.content.statements[i].actor.account != null)
                {
                    if (lrsResponse.content.statements[i].actor.account.name == emailStudent)
                    {
                        string namaquiz = lrsResponse.content.statements[i].ToJObject()["object"]["definition"]["name"]["en-US"].ToObject<string>();
                        string[] namaquizsplit = namaquiz.Split(' ');
                        for (int j = 0; j < exampleDataSet.Count; j++)
                        {
                            if (exampleDataSet[j].GroupName == namaquizsplit[0])
                            {
                                XYBarValues values = new XYBarValues();
                                if (namaquizsplit.Length > 2)
                                    values.XValue = namaquizsplit[1] + " " + namaquizsplit[2];
                                else
                                    values.XValue = namaquiz;
                                values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                                exampleDataSet[j].ListOfBars[1] = values;
                                ada = true;
                                break;
                            }
                            else
                                ada = false;
                        }
                        if (!ada)
                        {
                            BarGraphDataSet bar = new BarGraphDataSet();
                            XYBarValues values = new XYBarValues();
                            bar.GroupName = namaquizsplit[0];
                            bar.ListOfBars = new List<XYBarValues>(new XYBarValues[2]);
                            for (int k = 0; k < bar.ListOfBars.Count; k++)
                            {
                                if (k == 0)
                                {
                                    bar.ListOfBars[k] = new XYBarValues();
                                    if (namaquizsplit.Length > 2)
                                    {
                                        bar.ListOfBars[k].XValue = namaquizsplit[1] + " " + namaquizsplit[2];
                                    }
                                    else
                                        bar.ListOfBars[k].XValue = namaquiz;
                                    bar.ListOfBars[k].YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                                }
                                else
                                {
                                    bar.ListOfBars[k] = new XYBarValues();
                                    if (namaquizsplit.Length > 2)
                                        bar.ListOfBars[k].XValue = namaquizsplit[1] + " " + namaquizsplit[2];
                                    else
                                        bar.ListOfBars[k].XValue = namaquiz;
                                    bar.ListOfBars[k].YValue = 0;
                                }
                            }
                            exampleDataSet.Add(bar);
                        }
                    }
                }
            }
        }
        exampleDataSet2 = exampleDataSet;
        barGraphGenerator.GeneratBarGraph(exampleDataSet2);
        transform.localScale = scaleSizeStudent;
    }
    public void ManagerData()
    {
        if (MentorMode)
            return;
        StudentMode = false;
        MentorMode = true;
        transform.localScale = defScale;
        exampleDataSet.Clear();
        bool ada = false;
        bool ada2 = false;
        for (int i = 0; i < lrsResponse.content.statements.Count; i++)
        {
            if (lrsResponse.content.statements[i] != null)
            {
                if (lrsResponse.content.statements[i].actor.account != null)
                {
                    string namaquiz = lrsResponse.content.statements[i].ToJObject()["object"]["definition"]["name"]["en-US"].ToObject<string>();

                    for (int j = 0; j < exampleDataSet.Count; j++)
                    {
                        if (!exampleDataSet[j].ListOfBars.Any(f => f.XValue == namaquiz))
                        {
                            XYBarValues values = new XYBarValues();
                            values.XValue = namaquiz;
                            exampleDataSet[j].ListOfBars.Add(values);
                        }
                        if (exampleDataSet[j].GroupName == lrsResponse.content.statements[i].actor.account.name)
                        {

                            XYBarValues values = new XYBarValues();
                            values.XValue = namaquiz;
                            values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                            int idxfound = exampleDataSet[j].ListOfBars.FindIndex(g => g.XValue == values.XValue);
                            exampleDataSet[j].ListOfBars[idxfound] = values;
                        }
                    }
                    if (!exampleDataSet.Any(f => f.GroupName == lrsResponse.content.statements[i].actor.account.name))
                    {
                        XYBarValues values = new XYBarValues();
                        BarGraphDataSet bar = new BarGraphDataSet();
                        bar.GroupName = lrsResponse.content.statements[i].actor.account.name;
                        values.XValue = namaquiz;
                        values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                        bar.ListOfBars = new List<XYBarValues>();
                        bar.ListOfBars.Add(values);
                        exampleDataSet.Add(bar);
                    }
                }
            }
        }
        exampleDataSet2 = exampleDataSet;
        /*   float t = 0;
           foreach(var e in exampleDataSet) { 
               t = e.ListOfBars[0].YValue+ e.ListOfBars[1].YValue;
               score.Add(t);
               //print(i+" "+t);
           }
           BarGraphDataSet bar2 = new BarGraphDataSet();
           scoresort = new List<float>(score);
           scoresort.Sort();
           for(int m=0; m < scoresort.Count; m++) { 
               for (int l = 0; l < score.Count; l++)
               {

                   if (scoresort[m] == score[l])
                   {
                       if (testsc.Count > 0)
                       {
                           if (!testsc.Contains(l))
                           {
                               bar2 = exampleDataSet[l];
                               exampleDataSet2.Add(bar2);
                               testsc.Add(l);
                               //score.RemoveAt(l);
                               break;
                           }
                       }
                   else
                   {
                           bar2 = exampleDataSet[l];
                           exampleDataSet2.Add(bar2);
                           testsc.Add(l);
                           //score.RemoveAt(l);
                           break;
                       }
                   }
               }
           }*/
        print(barGraphGenerator);
        barGraphGenerator.GeneratBarGraph(exampleDataSet2);
        transform.localScale = scaleSizeMentor;
    }
    public void BackButton()
    {
        if (isStudent)
        {
            StudentData(namaStudent);
        }
        else
        {
            ManagerData();
        }
    }
    public void BarChart()
    {
        barGraphGenerator.barPrefab = barChartPrefab;
        if (MentorMode)
        {
            MentorMode = false;
            ManagerData();
        }
        else if (StudentMode)
        {
            StudentMode = false;
            StudentData(namaStudent);
        }
    }
    public void ScaterPlot()
    {
        barGraphGenerator.barPrefab = ScaterPrefab;
        if (MentorMode)
        {
            MentorMode = false;
            ManagerData();
        }
        else if (StudentMode)
        {
            StudentMode = false;
            StudentData(namaStudent);
        }
    }
}