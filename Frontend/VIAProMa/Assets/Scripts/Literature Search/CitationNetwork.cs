using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Class for a citation network.
    /// </summary>
    public class CitationNetwork : MonoBehaviour
    {
        /// <summary>
        /// Initial iterationsteps when creating the network.
        /// </summary>
        private static readonly int _cutoff = 3;

        private readonly double _weigthEdges = .3;
        private readonly double _weigthYear = 1;
        private readonly double _weigthCitations = 3;

        /// <summary>
        /// Base paper of the network.
        /// </summary>
        public Paper Base { get; private set; }

        /// <summary>
        /// Head node of the citation network.
        /// </summary>
        public CitationNetworkNode Head { get; private set; }

        /// <summary>
        /// Indicates whether another iteration step is possible.
        /// </summary>
        public bool StepPossible { get; private set; } = true;

        /// <summary>
        /// List of all papers in the citation network.
        /// </summary>
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

        /// <summary>
        /// Creates a citation network based on a base paper.
        /// </summary>
        /// <param name="_base">Base paper for creation.</param>
        /// <returns>A citationnetwork based on a paper.</returns>
        public async static Task<CitationNetwork> CreateNetwork(Paper _base)
        {
            CitationNetwork network = new CitationNetwork();
            network.Base = _base;

            List<Paper> relevantPapers = await Communicator.GetAllReferences(_base);
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

        /// <summary>
        /// Calculates the next iteration step for expanding the citation network.
        /// </summary>
        /// <returns>Task which yields the expanded network once calculated.</returns>
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

        /// <summary>
        /// Gets the node of a paper in the citation network.
        /// </summary>
        /// <param name="paper">Paper of the node.</param>
        /// <returns>The node of the paper if it exists, null otherwise.</returns>
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

        /// <summary>
        /// Gets all nodes of the citation network.
        /// </summary>
        /// <returns>A list of citation network nodes.</returns>
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

        /// <summary>
        /// Gets all edges in the citation network.
        /// </summary>
        /// <returns>A list of citation network node tuples.</returns>
        public List<(CitationNetworkNode, CitationNetworkNode)> GetConnections()
        {
            return Head.GetConnections();
        }

        /// <summary>
        /// Turns the citation network into a string.
        /// </summary>
        /// <returns>Citation network as string.</returns>
        public override string ToString()
        {
            return Head.ToString();
        }

        /// <summary>
        /// Calculates the ranks of all citation network nodes.
        /// </summary>
        /// <returns>A tuple of a dictonary, containing the ranks, and minimum and maximum rank.</returns>
        public (Dictionary<string, double>, double, double) CalculateRanks()
        {
            Dictionary<string, double> results = new Dictionary<string, double>();

            List<(CitationNetworkNode, CitationNetworkNode)> connections = GetConnections();
            List<Paper> papers = Papers;
            double max = double.MinValue;
            double min = double.MaxValue;
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

                rank += Mathf.Log10(paper.ReferencedByCount) * _weigthCitations;

                rank += (paper.Created.Year/Base.Created.Year) * _weigthYear;

                if(rank > max)
                {
                    max = rank;
                }
                if(rank < min)
                {
                    min = rank;
                }

                results.Add(paper.DOI, rank);
            }

            return (results, min, max);
        }
    }

}
