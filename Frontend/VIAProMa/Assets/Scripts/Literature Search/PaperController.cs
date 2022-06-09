using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperController : Singleton<PaperController>
    {
        [SerializeField] private GameObject paperListView;
        private List<GameObject> paperResultsList = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ShowResults(List<Paper> results, Transform transform = null)
        {
            if(transform == null)
            {
                transform = this.transform;
            }
            ClearResults();
            for(int i = 0; i < results.Count; i++)
            {
                GameObject displayInstance = Instantiate(paperListView, transform.position + new Vector3(transform.position.x + i * .4f + .3f, 0, 0), transform.rotation);
                paperResultsList.Add(displayInstance);
                PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponent<PaperDataDisplay>();
                remoteDataDisplay.Setup(results[i]);
            }
        }

        public void ClearResults()
        {
            for(int i = 0; i < paperResultsList.Count; i++)
            {
                Destroy(paperResultsList[i]);
            }
            paperResultsList.Clear();
        }
    }

}
