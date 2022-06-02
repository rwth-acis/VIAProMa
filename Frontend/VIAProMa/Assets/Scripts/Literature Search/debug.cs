using i5.VIAProMa.LiteratureSearch;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
    [SerializeField] private GameObject displayPrefab;

    private GameObject displayInstance;
    private ObjectManipulator handlerOnCopy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDebugClick()
    {
        List<string> title = new List<string>();
        title.Add("this is a test title");
        List<Author> author = new List<Author>();
        author.Add(new Author()
        {
            family = "dirkson",
            given = "dirk"
        });
        Paper paper = new Paper("pub", "abst", "123498765", "testpaper", "69-420", 42, title, author, "google.com", DateTime.Now);

        displayInstance = Instantiate(displayPrefab, this.transform);
        PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponent<PaperDataDisplay>();
        remoteDataDisplay.Setup(paper);
    }
}
