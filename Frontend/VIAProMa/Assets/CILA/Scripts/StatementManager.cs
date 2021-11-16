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
    public string dateTimeString = "2021-01-01";

    public List<BarGraphDataSet> DataSet;
    public List<BarGraphDataSet> DataSet2;
    BarGraphGenerator barGraphGenerator;
    Vector3 defScale;
    Vector3 defPosition;
    public Vector3 scalePositionMentor;
    public Vector3 scalePositionStudent;
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
        nameStudent = WindowManagerCILA.Instance.LoginMenu.cachedUserInfo.Email;
        lrs = new RemoteLRS(
           "https://lrs.tech4comp.dbis.rwth-aachen.de/data/xAPI",
        "79c09762728c78bab3309dd56899e0a5d67a4f76",
        "be4b2e4b958c2c5bd119fe1dd31dc466a85af8c4"
        );

        barGraphGenerator = GetComponent<BarGraphGenerator>();
        role();
        
        print(barGraphGenerator);

        // if the DataSet2 list is empty then return
        if(DataSet.Count == 0)
        {
            Debug.LogError("Dataset is Empty!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            role();
        }
    }
    public void role()
    {
        string role = CheckRole("http://id.tincanapi.com/verb/viewed");
        if(role == "student")
        {
            isStudent = true;
            GetStatement();
        }else if( role == "mentor")
        {
            isStudent = false;
            GetStatement();
        }
        else
        {
             role = CheckRole("https://w3id.org/xapi/dod-isd/verbs/completed");
            if (role == "student")
            {
                isStudent = true;
                GetStatement();
            }
            else if (role == "mentor")
            {
                isStudent = false;
                GetStatement();
            }
        }
    }
    public string CheckRole(string verb)
    {
        transform.localScale = defScale;
        bool isExist = false;
        //Build out full Statement details based on verb viewed to get the user role
        var query = new StatementsQuery();
        query.verbId = new Uri(verb);
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
                    if (lrsResponse.content.statements[i].actor.account != null && lrsResponse.content.statements[i].actor.account.name.Contains("@"))
                    {
                        if (lrsResponse.content.statements[i].actor.account.name.ToLower().Contains(nameStudent.ToLower()))
                        {
                            if (lrsResponse.content.statements[i].context.extensions.ToJSON().Contains("student"))
                            {
                                return "student";
                            }
                            else
                            {
                                return "mentor";
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
        return "";
    }

    public void GetStatement()
    {
        transform.localScale = defScale;
        bool isExist = false;
        //Build out full Statement details based on verb completed to get the user learning data
        var query = new StatementsQuery();
        query.verbId = new Uri("https://w3id.org/xapi/dod-isd/verbs/completed");
        query.since = System.DateTime.Parse(dateTimeString);
        query.limit = 5000;
        lrsResponse = lrs.QueryStatements(query);

        if (lrsResponse.success) //Get Statement Success
        {
                if (isStudent)
            {
                studentData(nameStudent);
            }
            else
            {
                ManagerData();
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
        StudentMode = true;
        MentorMode = false;
        transform.localScale = defScale;
        DataSet.Clear();
        bool isExist = false;
        List<string> quizList = new List<string>();
        for (int i = 0; i < lrsResponse.content.statements.Count; i++)
        {
            if (lrsResponse.content.statements[i] != null && lrsResponse.content.statements[i].actor.account != null)
            {
                if (lrsResponse.content.statements[i].actor.account.name.ToLower().Contains(nameStudent.ToLower()))
                {
                    string namequiz = lrsResponse.content.statements[i].ToJObject()["object"]["definition"]["name"]["en-US"].ToObject<string>();
                    string[] namequizSplit = namequiz.Split(' ');
                    string n;

                    if (namequizSplit.Length > 2)
                        n = namequizSplit[1] + " " + namequizSplit[2];
                    else
                        n = namequiz;
                    if (!quizList.Contains(n))
                    {
                        quizList.Add(n);
                    }
                    if (!DataSet.Any(f => f.GroupName == namequizSplit[0]))
                    {
                        print("bawah"+" "+ namequiz + lrsResponse.content.statements[i].result.score.raw);
                        XYBarValues values = new XYBarValues();
                        BarGraphDataSet bar = new BarGraphDataSet();
                        bar.GroupName = namequizSplit[0];
                        values.XValue = n;
                        values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                        bar.ListOfBars = new List<XYBarValues>();
                        bar.ListOfBars.Add(values);
                        DataSet.Add(bar);
                    }
                    else {
                        for (int j = 0; j < DataSet.Count; j++)
                        {
                            if (DataSet[j].GroupName == namequizSplit[0])
                            {
                                print("atas1" + " " + namequiz + lrsResponse.content.statements[i].result.score.raw);
                                if (!DataSet[j].ListOfBars.Any(f => f.XValue == n))
                                {
                                    print("atas2" + " " + namequiz + lrsResponse.content.statements[i].result.score.raw);
                                    XYBarValues values = new XYBarValues();
                                    values.XValue = n;
                                    values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                                    DataSet[j].ListOfBars.Add(values);
                                }
                                else
                                {
                                    print("tengah1" + " " + namequiz + lrsResponse.content.statements[i].result.score.raw);
                                    if ((float)lrsResponse.content.statements[i].result.score.raw > DataSet[j].ListOfBars.Find(f => f.XValue == n).YValue)
                                    {
                                        print("tengah" + " " + namequiz + lrsResponse.content.statements[i].result.score.raw);
                                        DataSet[j].ListOfBars.Find(f => f.XValue == n).YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                                    }
                                }
                            }
                        }
                        for (int j = 0; j < DataSet.Count; j++)
                        {
                            for (int k = 0; k < quizList.Count; k++)
                            {
                                if (!DataSet[j].ListOfBars.Any(f => f.XValue == quizList[k]))
                                {
                                    XYBarValues values = new XYBarValues();
                                    values.XValue = quizList[k];
                                    DataSet[j].ListOfBars.Add(values);
                                }
                            }
                        }
                        //if (DataSet[j].GroupName == lrsResponse.content.statements[i].actor.account.name)
                        //{
                        //    print(lrsResponse.content.statements[i].ToJSON());

                        //    XYBarValues values = new XYBarValues();
                        //    values.XValue = n;
                        //    values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                        //    int idxfound = DataSet[j].ListOfBars.FindIndex(g => g.XValue == values.XValue);
                        //    DataSet[j].ListOfBars[idxfound] = values;
                        //}
                    }
                }
            }
        }
        DataSet2 = DataSet;
        barGraphGenerator.GeneratBarGraph(DataSet2);
        transform.localScale = scaleSizeStudent;
        transform.localPosition = scalePositionStudent;
    }


    public void ManagerData()
    {
        if (MentorMode)
            return;
        StudentMode = false;
        MentorMode = true;
        transform.localScale = defScale;
        DataSet.Clear();
        bool ada = false;
        bool ada2 = false;
        List<string> quizList = new List<string>();
        for (int i = 0; i < lrsResponse.content.statements.Count; i++)
        {
            if (lrsResponse.content.statements[i] != null)
            {
                if (lrsResponse.content.statements[i].actor.account != null)
                {
                    string namaquiz = lrsResponse.content.statements[i].ToJObject()["object"]["definition"]["name"]["en-US"].ToObject<string>();
                    if (!quizList.Contains(namaquiz))
                    {
                        quizList.Add(namaquiz);
                    }
                    for (int j = 0; j < DataSet.Count; j++)
                    {
                        if (!DataSet[j].ListOfBars.Any(f => f.XValue == namaquiz))
                        {
                            print(DataSet[j].email + " " + namaquiz);
                            XYBarValues values = new XYBarValues();
                            values.XValue = namaquiz;
                            DataSet[j].ListOfBars.Add(values);
                        }
                        if (DataSet[j].email == lrsResponse.content.statements[i].actor.account.name)
                        {

                            XYBarValues values = new XYBarValues();
                            values.XValue = namaquiz;
                            values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                            int idxfound = DataSet[j].ListOfBars.FindIndex(g => g.XValue == values.XValue);
                            DataSet[j].ListOfBars[idxfound] = values;
                        }
                    }
                    if (!DataSet.Any(f => f.email == lrsResponse.content.statements[i].actor.account.name))
                    {
                        XYBarValues values = new XYBarValues();
                        BarGraphDataSet bar = new BarGraphDataSet();
                        bar.email = lrsResponse.content.statements[i].actor.account.name;
                        bar.GroupName = lrsResponse.content.statements[i].actor.name;
                        values.XValue = namaquiz;
                        values.YValue = (float)lrsResponse.content.statements[i].result.score.raw;
                        bar.ListOfBars = new List<XYBarValues>();
                        bar.ListOfBars.Add(values);
                        DataSet.Add(bar);
                    }
                    for (int j = 0; j < DataSet.Count; j++)
                    {
                        for (int k = 0; k < quizList.Count; k++)
                        {
                            if (!DataSet[j].ListOfBars.Any(f => f.XValue == quizList[k]))
                            {
                                XYBarValues values = new XYBarValues();
                                values.XValue = quizList[k];
                                DataSet[j].ListOfBars.Add(values);
                            }
                        }
                    }
                }
            }
        }
        DataSet2 = DataSet;
        
        print(barGraphGenerator);
        barGraphGenerator.GeneratBarGraph(DataSet2);
        transform.localScale = scaleSizeMentor;
        transform.localPosition = scalePositionMentor;
    }

    public void BackButton()
    {
        if(isStudent)
        {
            studentData(nameStudent);
        }
        else
        {
            ManagerData();
        }
    }

    public void BarChart()
    {
        barGraphGenerator.barPrefab = barChartPrefab;
        if(MentorMode)
        { 
            MentorMode = false;
            ManagerData();
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
            ManagerData();
        }
        else if (StudentMode)
        {
            StudentMode = false;
            studentData(nameStudent);
        }
    }
}
