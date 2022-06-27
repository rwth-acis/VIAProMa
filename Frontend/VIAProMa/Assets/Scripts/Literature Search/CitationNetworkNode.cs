using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class CitationNetworkNode : MonoBehaviour
    {
        public Paper Content { get; set; }

        public List<CitationNetworkNode> Children { get; set; } = new List<CitationNetworkNode>();

        public bool IsLeaf()
        {
            return Children == null || Children.Count == 0;
        }

        public CitationNetworkNode GetNode(Paper paper)
        {
            if (Content.Equals(paper))
            {
                return this;
            }
            if (IsLeaf()) 
            {
                return null; 
            }
            for (int i = 0; i < Children.Count; i++)
            {
                CitationNetworkNode node = Children[i].GetNode(paper);
                Debug.Log(node?.Content);
                if (!(node is null))
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
                for (int i = 0; i < Children.Count; i++)
                {
                    papers.AddRange(Children[i].GetAllPapers());
                }
                return papers;
            }

        }

        public int GetLayers()
        {
            if (IsLeaf())
            {
                return 1;
            }
            int max = 0;
            foreach (CitationNetworkNode node in Children)
            {
                int layers = node.GetLayers();
                if (layers > max)
                {
                    max = layers;
                }
            }
            return max + 1;

        }
        public override string ToString()
        {
            if (IsLeaf())
            {
                return "| " + Content.DOI + " |";
            }

            string output = Content.DOI + " (";
            foreach (CitationNetworkNode node in Children)
            {
                output += node.ToString();
            }
            return output + ")";
        }

        public override bool Equals(object other)
        {
            CitationNetworkNode node = other as CitationNetworkNode;
            if (node == null)
            {
                return false;
            }
            return Content.Equals(node.Content);
        }
        public bool Equals(CitationNetworkNode node)
        {
            if (node == null)
            {
                return false;
            }
            return Content.Equals(node.Content);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets all nodes below current node.
        /// </summary>
        /// <returns>All nodes below.</returns>
        public List<CitationNetworkNode> GetAllNodes()
        {

            List<CitationNetworkNode> nodes = new List<CitationNetworkNode>();
            nodes.Add(this);
            if (IsLeaf())
            {
                return nodes;
            }
            foreach(CitationNetworkNode node in Children)
            {
                nodes.AddRange(node.GetAllNodes());
            }
            return nodes;

        }

        public List<(CitationNetworkNode, CitationNetworkNode)> GetConnections()
        {
            List<(CitationNetworkNode, CitationNetworkNode)> connections = new List<(CitationNetworkNode, CitationNetworkNode)>();
            if (IsLeaf())
            {
                return connections;
            }
            foreach (CitationNetworkNode node in Children)
            {
                connections.Add((this, node));
            }
            foreach (CitationNetworkNode node in Children)
            {
                connections.AddRange(node.GetConnections());
            }
            return connections;
        }
    }

}
