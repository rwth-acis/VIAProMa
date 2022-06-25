using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class CitationNetwork : MonoBehaviour
    {
        public Paper Base { get; private set; }
        public CitationNetworkNode Head { get; private set; }

        public List<Paper> Papers 
        { 
            get 
            {
                List<Paper> papers = Head.GetAllPapers();
                List<Paper> papersWithoutDup = new List<Paper>();
                foreach(Paper paper in papers)
                {
                    if (!papersWithoutDup.Contains(paper))
                    {
                        papersWithoutDup.Add(paper);
                    }
                }
                return papersWithoutDup;
            } 
        }

        public async static Task<CitationNetwork> CreateNetwork(Paper _base)
        {
            CitationNetwork network = new CitationNetwork();
            network.Base = _base;

            List<Paper> relevantPapers = await GetAllReferences(_base);
            CitationNetworkNode head = new CitationNetworkNode();
            head.Content = _base;
            network.Head = head;
            
            foreach(Paper relevantPaper in relevantPapers)
            {
                head.Children.Add(new CitationNetworkNode
                {
                    Content = relevantPaper
                });
            }

            bool changesOccured = true;

            // Get potentially relevant papers
            while (changesOccured)
            {
                changesOccured = false;
                List<(List<Paper>, Paper)> potentialPapers = new List<(List<Paper>, Paper)>();
                for(int i = 0; i < network.Papers.Count; i++)
                {
                    potentialPapers.Add((await GetAllReferences(network.Papers[i]), network.Papers[i]));
                }

                // Check if potential papers overlap -> relevant paper
                for(int i = 0; i < potentialPapers.Count; i++)
                {
                    for(int j = i + 1; j < potentialPapers.Count; j++)
                    {
                        List<Paper> overlap = GetOverlap(potentialPapers[i].Item1, potentialPapers[j].Item1);
                        if (overlap.Count > 0)
                        {
                            Debug.Log("found overlap" + $"({i}, {j})");
                            changesOccured = true;

                            CitationNetworkNode node1 = network.GetNode(potentialPapers[i].Item2);
                            CitationNetworkNode node2 = network.GetNode(potentialPapers[j].Item2);
                            for(int overlapIndex = 0; overlapIndex < overlap.Count; overlapIndex++)
                            {
                                CitationNetworkNode newNode = new CitationNetworkNode
                                {
                                    Content = overlap[overlapIndex]
                                };
                                if(!node1.Children.Exists(n => n.Content.Equals(overlap[overlapIndex])))
                                {
                                    Debug.Log("added node to 1" + $"({i}, {j})");
                                    node1.Children.Add(newNode);
                                    changesOccured = true;
                                }
                                if(overlap[overlapIndex] == null)
                                {
                                    Debug.Log("why here" + overlapIndex);
                                }
                                if(!node2.Children.Exists(n => n.Content.Equals(overlap[overlapIndex])))
                                {
                                    Debug.Log("added node to 2" + $"({i}, {j})");
                                    node2.Children.Add(newNode);
                                    changesOccured = true;
                                }

                            }
                        }
                    }



                }
                
            }
            return network;

        }

        private CitationNetworkNode GetNode(Paper paper)
        {
            return Head.GetNode(paper);
        }

        /// <summary>
        /// Gets the overlapping papers of two <seealso cref="List{Paper}"/>.
        /// </summary>
        /// <param name="list1">First list</param>
        /// <param name="list2">Second list</param>
        /// <returns>The overlap of both lists.</returns>
        private static List<Paper> GetOverlap(List<Paper> list1, List<Paper> list2)
        {
            List<Paper> overlap = new List<Paper>();
            foreach(Paper paper in list1)
            {
                if (list2.Contains(paper))
                {
                    if (paper != null)
                        overlap.Add(paper);
                }
            }
            return overlap;
        }

        private static async Task<List<Paper>> GetAllReferences(Paper basePaper)
        {
            // Get all papers of references (as Tasks first for better effiecency)
            List<Task<Paper>> referencesTasks = new List<Task<Paper>>();
            foreach (string refDOI in basePaper.References)
            {
                if(!String.IsNullOrEmpty(refDOI))
                    referencesTasks.Add(Communicator.GetPaper(refDOI));
                await Task.Delay(10);
            }
            List<Paper> references = new List<Paper>();
            foreach (Task<Paper> referencesTask in referencesTasks)
            {
                Paper reference = await referencesTask;
                references.Add(reference);
            }

            return references;
        }

        public override string ToString()
        {
            return Head.ToString();
        }
    }

    public class CitationNetworkNode
    {
        public Paper Content { get; set; }

        public List<CitationNetworkNode> Children { get; set; } = new List<CitationNetworkNode>();

        public bool IsLeaf()
        {
            return Children == null || Children.Count == 0;
        }

        public CitationNetworkNode GetNode(Paper paper)
        {
            if (Content.Equals(paper)) return this;
            if (IsLeaf()) return null;
            for(int i = 0; i < Children.Count; i++)
            {
                CitationNetworkNode node = Children[i].GetNode(paper);
                if(node != null)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all papers down from a node.
        /// </summary>
        /// <returns></returns>
        public List<Paper> GetAllPapers()
        {
            if (IsLeaf())
            {
                List<Paper> papers = new List<Paper>();
                papers.Add(Content);
                return papers;
            }
            else
            {

                List<Paper> papers = new List<Paper>();
                for(int i = 0; i < Children.Count; i++)
                {
                    papers.AddRange(Children[i].GetAllPapers());
                }
                return papers;
            }

        }

        public override string ToString()
        {
            if (IsLeaf())
            {
                return "| "+ Content.DOI + " |";
            }

            string output = Content.DOI + " (";
            foreach(CitationNetworkNode node in Children)
            {
                output += node.ToString();
            }
            return output + ")";
        }
    }

}
