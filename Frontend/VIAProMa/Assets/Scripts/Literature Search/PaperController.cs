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
            float itemHeight = .1f;
            float itemWidth = .15f;

            float heightOffset = .1f;
            float zOffset = -.1f;
            ClearNetwork();
            if(transform == null)
            {
                transform = this.transform;
            }
            List<CitationNetworkNode> nodes = network.GetNodes();

            List<int> years = GetAllYears(nodes);
            List<int> yearEntries = new List<int>();
            foreach(int i in years)
            {
                yearEntries.Add(0);
            }
            
            
            for (int i = 0; i < nodes.Count; i++)
            {
                CitationNetworkNode node = nodes[i];
                if(GetNode(node) != null)
                {
                    continue;
                }

                int yearIndex = years.IndexOf(node.Content.Created.Year);


                float height = (yearIndex * itemHeight);
                float xPos;
                if(yearIndex == years.Count - 1)
                {
                    xPos = 0;
                }
                else
                {
                    xPos = (((int)((yearEntries[yearIndex]) / 2) + 1) * itemWidth);

                    if(yearEntries[yearIndex] % 2 == 0)
                    {
                        xPos = -xPos;
                    }
                    yearEntries[yearIndex]++;
                }


                GameObject displayInstance = Instantiate(paperNetworkView);
                displayInstance.transform.parent = transform;
                displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset);

                paperNetworkList.Add(displayInstance);
                PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponentInChildren<PaperDataDisplay>();
                remoteDataDisplay.Setup(node.Content);
                PaperNetworkItem nodeItem = displayInstance?.GetComponentInChildren<PaperNetworkItem>();
                nodeItem.Node = node;
            }

            yield return null;

            List<(CitationNetworkNode, CitationNetworkNode)> connections = network.GetConnections();
            for(int i = 0; i < connections.Count; i++)
            {
                GameObject connection = Instantiate(linePrefab);
                NetworkLine line = connection.GetComponent<NetworkLine>();
                line.SetLine(GetNode(connections[i].Item1).transform, connections[i].Item1.Content.DOI, 
                    GetNode(connections[i].Item2).transform, connections[i].Item2.Content.DOI);
                connectionsList.Add(connection);
            }
        }

        private List<int> GetAllYears(List<CitationNetworkNode> nodes)
        {
            List<int> years = new List<int>();

            foreach(CitationNetworkNode node in nodes)
            {
                if (!years.Contains(node.Content.Created.Year))
                {
                    years.Add(node.Content.Created.Year);
                }
            }
            years.Sort();
            return years;
        }

        private GameObject GetNode(CitationNetworkNode node)
        {
            GameObject obj = null;

            for(int i = 0; i < paperNetworkList.Count; i++)
            {
                if (node.Content.DOI.Equals(paperNetworkList[i].GetComponentInChildren<PaperNetworkItem>().Node.Content.DOI)){
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
        
        public void HighlightNode(CitationNetworkNode node)
        {
            ResetNodeColors();

            for (int i = 0; i < connectionsList.Count; i++)
            {
                NetworkLine line = connectionsList[i].GetComponent<NetworkLine>();
                if(line.StartDOI.Equals(node.Content.DOI))
                {
                    line.ChangeColor(Color.blue);
                }
                else if (line.EndDOI.Equals(node.Content.DOI))
                {
                    line.ChangeColor(Color.red);

                }
            }
        }

        private void ResetNodeColors()
        {
            for (int i = 0; i < connectionsList.Count; i++)
            {
                NetworkLine line = connectionsList[i].GetComponent<NetworkLine>();
                
                line.ChangeColor(Color.white);
                
            }
        }
    }

}
