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
            
            foreach(Paper relevantPaper in relevantPapers)
            {
                head.Children.Add(new CitationNetworkNode
                {
                    Content = relevantPaper
                });
            }

            bool changesOccured = true;
            while (changesOccured)
            {
                changesOccured = false;

                // Get potentially relevant papers
                List<(List<Paper>, Paper)> potentialPapers = new List<(List<Paper>, Paper)>();
                for(int i = 0; i < network.Papers.Count; i++)
                {
                    potentialPapers.Add((await GetAllReferences(network.Papers[i]), network.Papers[i]));
                }

                // Check if potential papers overlap -> relevant paper
                for(int i = 0; i < potentialPapers.Count; i++)
                {
                    for(int j = i; j < potentialPapers.Count; i++)
                    {
                        List<Paper> overlap = GetOverlap(potentialPapers[i].Item1, potentialPapers[j].Item1);
                        if (overlap.Count > 0)
                        {
                            changesOccured = true;

                            CitationNetworkNode node1 = network.GetNode(potentialPapers[i].Item2);
                            CitationNetworkNode node2 = network.GetNode(potentialPapers[j].Item2);
                            foreach(Paper newRelevantPaper in overlap)
                            {
                                CitationNetworkNode newNode = new CitationNetworkNode
                                {
                                    Content = newRelevantPaper
                                };
                                node1.Children.Add(newNode);
                                node2.Children.Add(newNode);
                            }
                        }
                    }

                }
                
            }
            return network;
        }

        private CitationNetworkNode GetNode(Paper paper)
        {
            throw new NotImplementedException();
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
                referencesTasks.Add(Communicator.GetPaper(refDOI));
            }
            List<Paper> references = new List<Paper>();
            foreach (Task<Paper> referencesTask in referencesTasks)
            {
                references.Add(await referencesTask);
            }

            return references;
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
    }

}
