using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{

    /// <summary>
    /// Class for the visualization of paper data structures.
    /// </summary>
    public class PaperController : Singleton<PaperController>
    {
        /// <summary>
        /// Enums for visualizations
        /// </summary>
        private enum Visualizations
        {
            Depth,
            Size,
            Scale, 
        }

        [Tooltip("Prefab of the paper list item.")]
        [SerializeField] private GameObject paperListView;
        [Tooltip("Prefab of the paper network item.")]
        [SerializeField] private GameObject paperNetworkView;
        [Tooltip("Prefab of the network line.")]
        [SerializeField] private GameObject linePrefab;

        /// <summary>
        /// List of search result gameobjects.
        /// </summary>
        private List<GameObject> _paperResultsList = new List<GameObject>();
        /// <summary>
        /// List of network node gameobjects.
        /// </summary>
        private List<GameObject> _paperNetworkList = new List<GameObject>();
        /// <summary>
        /// List of network edge gameobjects.
        /// </summary>
        private List<GameObject> _connectionsList  = new List<GameObject>();

        /// <summary>
        /// Currently selected visualization of citation network
        /// </summary>
        private Visualizations _currentVisualization = Visualizations.Depth;
        /// <summary>
        /// Weight for scaling the visualization with depth.
        /// </summary>
        private readonly float _weightDepth = 1.5f;
        /// <summary>
        /// Weight for scaling the visualization with scale.
        /// </summary>
        private readonly float _weightScale = 1.4f;
        /// <summary>
        /// Weight for scaling the visualization with size.
        /// </summary>
        private readonly float _weightSize = 20;

        /// <summary>
        /// Visualizes the search results.
        /// </summary>
        /// <param name="results">List of search result papers.</param>
        /// <param name="transform">Parent transform of the visualized list.</param>
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

        /// <summary>
        /// Clears the currently visualized results.
        /// </summary>
        public void ClearResults()
        {
            for(int i = 0; i < _paperResultsList.Count; i++)
            {
                Destroy(_paperResultsList[i]);
            }
            _paperResultsList.Clear();
        }

        /// <summary>
        /// Visualizes a citation network.
        /// </summary>
        /// <param name="network">Citation network to visualize.</param>
        /// <param name="transform">Parent transform of the paper.</param>
        /// <returns></returns>
        public IEnumerator ShowNetwork(CitationNetwork network, Transform transform = null)
        {
            // Clear previous network
            ClearNetwork();

            // Initializations
            float itemHeight = .1f;
            float itemWidth = .25f;

            float heightOffset = .1f;
            float zOffset = -.1f;

            if(_currentVisualization == Visualizations.Scale)
            {
                itemHeight = .15f;
                itemWidth = .28f;
            }


            (Dictionary<string, double>, double, double) rankCalc = network.CalculateRanks();
            Dictionary<string, double> ranks = rankCalc.Item1;
            double minRank = rankCalc.Item2;
            double maxRank = rankCalc.Item3;

            if (transform == null)
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
            
            // Loop through all nodes and create node objects
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

                

                xPos = (((int)( ((float)(yearEntries[yearIndex])) / 2 + .5)) * itemWidth);

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

                // Affect the transform of the node objects according to the selected visualization.
                switch (_currentVisualization)
                {
                    case Visualizations.Depth:
                        // Avoid devision by zero.
                        float z;
                        if (maxRank == minRank)
                        {
                            z = 0;
                        }
                        else
                        {
                            z = (float)((rank - minRank)/(maxRank - minRank) * _weightDepth) - _weightDepth / 2;

                        }
                        displayInstance.transform.position += new Vector3(xPos, height + heightOffset, -z);
                        break;
                    case Visualizations.Scale:
                        {
                            displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset);
                            // Avoid devision by zero.
                            double scale;
                            if (maxRank == minRank)
                            {
                                scale = 1;
                            }
                            else
                            {
                                scale = ((rank - minRank) / (maxRank - minRank)) * _weightScale + _weightScale / 2;

                            }
                            Vector3 oldScale = displayInstance.transform.localScale;
                            Vector3 newScale = new Vector3(oldScale.x * (float)scale, oldScale.y * (float)scale, oldScale.z);
                            displayInstance.transform.localScale = newScale;
                        }
                        break;
                    case Visualizations.Size:
                        {
                            // Avoid devision by zero.
                            double scale;
                            if (maxRank == minRank)
                            {
                                scale = 1;
                            }
                            else
                            {
                                scale = 1 + (rank - minRank) / (maxRank - minRank) * _weightSize;

                            }
                            Vector3 oldScale = displayInstance.GetComponentInChildren<PaperNetworkItem>().gameObject.transform.localScale;
                            Vector3 newScale = new Vector3(oldScale.x, oldScale.y, oldScale.z * (float)scale);
                            displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset);
                            displayInstance.GetComponentInChildren<PaperNetworkItem>().gameObject.transform.localScale = newScale;
                        }
                        break;
                    default:
                        displayInstance.transform.position += new Vector3(xPos, height + heightOffset, zOffset);
                        break;
                }

                _paperNetworkList.Add(displayInstance);
                PaperDataDisplay remoteDataDisplay = displayInstance?.GetComponentInChildren<PaperDataDisplay>();
                remoteDataDisplay.Setup(node.Content);
                PaperNetworkItem nodeItem = displayInstance?.GetComponentInChildren<PaperNetworkItem>();
                nodeItem.Node = node;

                yield return null;
            }
            emptyParent.transform.position = transform.position;
            emptyParent.transform.rotation = transform.rotation;
            emptyParent.transform.parent = transform;

            List<(CitationNetworkNode, CitationNetworkNode)> connections = network.GetConnections();
            for(int i = 0; i < connections.Count; i++)
            {
                GameObject connection = Instantiate(linePrefab);
                connection.transform.parent = emptyParent.transform;
                NetworkLine line = connection.GetComponent<NetworkLine>();
                line.SetLine(GetNode(connections[i].Item1).transform, connections[i].Item1.Content.DOI, 
                    GetNode(connections[i].Item2).transform, connections[i].Item2.Content.DOI);
                _connectionsList.Add(connection);

                yield return null;
            }

        }

        /// <summary>
        /// Gets all the years of a list of citation network nodes.
        /// </summary>
        /// <param name="nodes">List of citation network nodes.</param>
        /// <returns>Sorted list of years.</returns>
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

        /// <summary>
        /// Gets the GameObject of a citation network node.
        /// </summary>
        /// <param name="node">Citation netork node.</param>
        /// <returns>Returns the GameObject if it exists, otherwise null.</returns>
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

        /// <summary>
        /// Clears the visualization of the currently displayed citation network node.
        /// </summary>
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
        
        /// <summary>
        /// Highlights a node in the visualized citation network node.
        /// </summary>
        /// <param name="node">Node to highlight.</param>
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

        /// <summary>
        /// Sets all edges back to white color.
        /// </summary>
        private void ResetNodeColors()
        {
            for (int i = 0; i < _connectionsList.Count; i++)
            {
                NetworkLine line = _connectionsList[i].GetComponent<NetworkLine>();
                
                line.ChangeColor(Color.white);
                
            }
        }

        /// <summary>
        /// Changes the current visualization.
        /// </summary>
        /// <param name="vis">Index of visualization.</param>
        public void ChangeVisualization(int vis)
        {
           _currentVisualization = (Visualizations) vis;

        }
    }

}
