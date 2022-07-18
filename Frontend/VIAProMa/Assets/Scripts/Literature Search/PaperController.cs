using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperController : Singleton<PaperController>
    {
        private enum Visualizations
        {
            Scale,
            Depth,
            Size
        }

        [SerializeField] private GameObject paperListView;
        [SerializeField] private GameObject paperNetworkView;
        [SerializeField] private GameObject linePrefab;

        private List<GameObject> _paperResultsList = new List<GameObject>();
        private List<GameObject> _paperNetworkList = new List<GameObject>();
        private List<GameObject> _connectionsList  = new List<GameObject>();

        private Visualizations _currentVisualization = Visualizations.Depth;
        private readonly float _weigthDepth = .03f; 

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
                _paperResultsList.Add(displayInstance);
                PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponent<PaperDataDisplay>();
                remoteDataDisplay.Setup(results[i]);
            }
        }

        public void ClearResults()
        {
            for(int i = 0; i < _paperResultsList.Count; i++)
            {
                Destroy(_paperResultsList[i]);
            }
            _paperResultsList.Clear();
        }

        public IEnumerator ShowNetwork(CitationNetwork network, Transform transform = null)
        {
            ClearNetwork();

            float itemHeight = .1f;
            float itemWidth = .225f;

            float heightOffset = .1f;
            float zOffset = -.1f;

            Dictionary<string, double> ranks = network.CalculateRanks();

            if(transform == null)
            {
                transform = this.transform;
            }
            List<CitationNetworkNode> nodes = network.GetNodes();

            List<int> years = GetAllYears(nodes);
            List<int> yearEntries = new List<int>();
            foreach(int _ in years)
            {
                yearEntries.Add(0);
            }

            GameObject emptyParent = new GameObject();
            
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

                

                xPos = (((int)((yearEntries[yearIndex]) / 2 + 1)) * itemWidth);

                if(yearEntries[yearIndex] % 2 == 0)
                {
                    xPos = -xPos;
                }
                yearEntries[yearIndex]++;
                


                GameObject displayInstance = Instantiate(paperNetworkView);
                displayInstance.transform.parent = emptyParent.transform;

                double rank = 0;
                if (ranks.ContainsKey(node.Content.DOI))
                {
                    rank = ranks[node.Content.DOI];
                }
                switch (_currentVisualization)
                {
                    case Visualizations.Depth:
                        displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset - (float) rank * _weigthDepth);
                        break;
                    default:
                        displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset);
                        break;
                }

                displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset);

                _paperNetworkList.Add(displayInstance);
                PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponentInChildren<PaperDataDisplay>();
                remoteDataDisplay.Setup(node.Content);
                PaperNetworkItem nodeItem = displayInstance?.GetComponentInChildren<PaperNetworkItem>();
                nodeItem.Node = node;
            }
            emptyParent.transform.position = transform.position;
            emptyParent.transform.rotation = transform.rotation;

            yield return null;

            List<(CitationNetworkNode, CitationNetworkNode)> connections = network.GetConnections();
            for(int i = 0; i < connections.Count; i++)
            {
                GameObject connection = Instantiate(linePrefab);
                NetworkLine line = connection.GetComponent<NetworkLine>();
                line.SetLine(GetNode(connections[i].Item1).transform, connections[i].Item1.Content.DOI, 
                    GetNode(connections[i].Item2).transform, connections[i].Item2.Content.DOI);
                _connectionsList.Add(connection);
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

            for(int i = 0; i < _paperNetworkList.Count; i++)
            {
                if (node.Content.DOI.Equals(_paperNetworkList[i].GetComponentInChildren<PaperNetworkItem>().Node.Content.DOI)){
                    obj = _paperNetworkList[i];
                }
            }

            return obj;
        }

        public void ClearNetwork()
        {
            for (int i = 0; i < _paperNetworkList.Count; i++)
            {
                Destroy(_paperNetworkList[i]);
            }
            _paperNetworkList.Clear();
            for (int i = 0; i < _connectionsList.Count; i++)
            {
                Destroy(_connectionsList[i]);
            }
            _connectionsList.Clear();
        }
        
        public void HighlightNode(CitationNetworkNode node)
        {
            ResetNodeColors();

            for (int i = 0; i < _connectionsList.Count; i++)
            {
                NetworkLine line = _connectionsList[i].GetComponent<NetworkLine>();
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
            for (int i = 0; i < _connectionsList.Count; i++)
            {
                NetworkLine line = _connectionsList[i].GetComponent<NetworkLine>();
                
                line.ChangeColor(Color.white);
                
            }
        }
    }

}
