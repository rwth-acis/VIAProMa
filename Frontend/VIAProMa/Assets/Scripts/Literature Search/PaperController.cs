using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperController : Singleton<PaperController>
    {
        [SerializeField] private GameObject paperListView;
        [SerializeField] private GameObject paperNetworkView;
        [SerializeField] private GameObject linePrefab;
        private List<GameObject> paperResultsList = new List<GameObject>();
        private List<GameObject> paperNetworkList = new List<GameObject>();
        private List<GameObject> connectionsList  = new List<GameObject>();

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
                GameObject displayInstance = Instantiate(paperListView, transform.position + new Vector3(i * .4f + .6f, 0, 0), transform.rotation);
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

        public IEnumerator ShowNetwork(CitationNetwork network, Transform transform = null)
        {
            ClearNetwork();
            if(transform == null)
            {
                transform = this.transform;
            }

            // Instantiate all nodes
            List<CitationNetworkNode> nodes = network.GetNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                CitationNetworkNode node = nodes[i];
                if(GetNode(node) != null)
                {
                    continue;
                }
                GameObject displayInstance = Instantiate(paperNetworkView, 
                    transform.position + new Vector3(i*.1f, 1f - (network.Base.Created.Year - node.Content.Created.Year) * .1f, 0),
                    transform.rotation);
                paperNetworkList.Add(displayInstance);
                PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponent<PaperDataDisplay>();
                remoteDataDisplay.Setup(node.Content);
                CitationNetworkNode objectNode = displayInstance?.GetComponent<CitationNetworkNode>();
                objectNode.Content = node.Content;
                objectNode.Children = node.Children;
            }

            yield return null;

            List<(CitationNetworkNode, CitationNetworkNode)> connections = network.GetConnections();
            for(int i = 0; i < connections.Count; i++)
            {
                GameObject connection = Instantiate(linePrefab);
                NetworkLine line = connection.GetComponent<NetworkLine>();
                line.SetLine(GetNode(connections[i].Item1).transform.position, GetNode(connections[i].Item2).transform.position);
                connectionsList.Add(connection);
            }
        }

        private GameObject GetNode(CitationNetworkNode node)
        {
            GameObject obj = null;

            for(int i = 0; i < paperNetworkList.Count; i++)
            {
                if (node.Content.DOI.Equals(paperNetworkList[i].GetComponent<CitationNetworkNode>().Content.DOI)){
                    obj = paperNetworkList[i];
                }
            }

            return obj;
        }

        public void ClearNetwork()
        {
            for (int i = 0; i < paperNetworkList.Count; i++)
            {
                Destroy(paperNetworkList[i]);
            }
            paperNetworkList.Clear();
            for (int i = 0; i < connectionsList.Count; i++)
            {
                Destroy(connectionsList[i]);
            }
            connectionsList.Clear();
        }
    }

}
