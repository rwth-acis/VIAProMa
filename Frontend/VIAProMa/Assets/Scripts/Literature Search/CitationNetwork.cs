using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class CitationNetwork : MonoBehaviour
    {
        private static readonly int _cutoff = 3;

        private readonly double _weigthEdges = 1;
        private readonly double _weigthYear = 1;
        private readonly double _weigthCitations = 1;

        public Paper Base { get; private set; }
        public CitationNetworkNode Head { get; private set; }
        public bool StepPossible { get; private set; } = true;

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

            int loops = 0;
            // Get potentially relevant papers
            while (network.StepPossible && loops < _cutoff)
            {
                loops++;
                network = await network.CalculateNextIteration();

            }

            for(int i = 0; i < network.Head.Children.Count; i++)
            {
                if(network.Head.Children[i].Children.Count == 0)
                {
                    network.Head.Children.RemoveAt(i);
                    i--;
                }
            }

            return network;

        }

        public async Task<CitationNetwork> CalculateNextIteration()
        {
            if (StepPossible)
            {
                this.StepPossible = false;
                List<(List<Paper>, Paper)> potentialPapers = new List<(List<Paper>, Paper)>();
                for (int i = 0; i < this.Papers.Count; i++)
                {
                    potentialPapers.Add((await Communicator.GetAllReferences(this.Papers[i]), this.Papers[i]));
                }


                // Check if potential papers overlap -> relevant paper
                for (int i = 0; i < potentialPapers.Count; i++)
                {
                    for (int j = i + 1; j < potentialPapers.Count; j++)
                    {
                        List<Paper> overlap = GetOverlap(potentialPapers[i].Item1, potentialPapers[j].Item1);
                        if (overlap.Count > 0)
                        {
                            CitationNetworkNode node1 = this.GetNode(potentialPapers[i].Item2);
                            CitationNetworkNode node2 = this.GetNode(potentialPapers[j].Item2);
                            for (int overlapIndex = 0; overlapIndex < overlap.Count; overlapIndex++)
                            {
                                CitationNetworkNode newNode = new CitationNetworkNode
                                {
                                    Content = overlap[overlapIndex]
                                };
                                if (!(node1 is null) && !node1.Children.Exists(n => n.Content.Equals(overlap[overlapIndex])))
                                {
                                    node1.Children.Add(newNode);
                                    this.StepPossible = true;
                                }
                                if (!(node2 is null) && !node2.Children.Exists(n => n.Content.Equals(overlap[overlapIndex])))
                                {
                                    node2.Children.Add(newNode);
                                    this.StepPossible = true;
                                }

                            }
                        }
                    }



                }
            }

            return this;
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
            if(basePaper is null)
            {
                return new List<Paper>();
            }
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

        public List<CitationNetworkNode> GetNodes()
        {
            List<CitationNetworkNode> nodes = new List<CitationNetworkNode>();
            foreach(CitationNetworkNode node in Head.GetAllNodes())
            {
                if (!nodes.Contains(node))
                    nodes.Add(node);
            }
            return nodes;
        }

        public List<(CitationNetworkNode, CitationNetworkNode)> GetConnections()
        {
            return Head.GetConnections();
        }

        public override string ToString()
        {
            return Head.ToString();
        }

        public Dictionary<string, double> CalculateRanks()
        {
            Dictionary<string, double> results = new Dictionary<string, double>();

            List<(CitationNetworkNode, CitationNetworkNode)> connections = GetConnections();
            List<Paper> papers = Papers;
            Debug.Log(papers.Count);
            for(int i = 0; i < papers.Count; i++)
            {
                Paper paper = papers[i];
                double rank = 0;
                int edges = 0;
                foreach((CitationNetworkNode, CitationNetworkNode) connection in connections)
                {
                    if(paper.Equals(connection.Item1.Content) || paper.Equals(connection.Item2.Content))
                    {
                        edges++;
                    }
                }
                rank += edges * _weigthEdges;

                rank += paper.ReferencedByCount / 100 * _weigthCitations;

                rank += (paper.Created.Year/Base.Created.Year) * _weigthYear;

                results.Add(paper.DOI, rank);
            }

            return results;
        }
    }

}
