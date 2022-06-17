using i5.VIAProMa.UI.ListView.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    [Serializable]
    public class Paper : IListViewItemData
    {

        [SerializeField] private string key;
        [SerializeField] private string publisher;
        [SerializeField] private string abstractText;
        [SerializeField] private string doi;
        [SerializeField] private string type;
        [SerializeField] private string page;
        [SerializeField] private int referencedByCount;
        [SerializeField] private List<string> title;
        [SerializeField] private List<Author> authors;
        [SerializeField] private List<string> references;
        [SerializeField] private string url;
        [SerializeField] private DateTime created;

        public string Key { get => key; }
        public string Publisher { get => publisher; }
        public string Abstracttext { get => abstractText; }
        public string DOI { get => doi; }
        public string Type { get => type; }
        public string Page { get => page; }
        public int ReferencedByCount { get => referencedByCount; }
        public List<string> Title { get => title; }
        public List<Author> Authors { get => authors; }
        /// <summary>
        /// List of DOIs of the references. 
        /// </summary>
        public List<string> References { get => references; }
        public string URL { get => url; }
        public DateTime Created { get => created; }


        public Paper()
        {
        }

        /// <summary>
        /// Creates a paper object with given parameters.
        /// </summary>
        /// <param name="publisher">The publisher of the paper.</param>
        /// <param name="abstractText">The abstract of the paper.</param>
        /// <param name="doi">The DOI of the paper.</param>
        /// <param name="type">The type of the paper.</param>
        /// <param name="page">The pages of the paper.</param>
        /// <param name="referencedByCount">Count how often the paper was referenced.</param>
        /// <param name="title">The title of the paper.</param>
        /// <param name="authors">The authors of the paper.</param>
        /// <param name="url">The URL to the paper.</param>
        /// <param name="created">The date the paper was created.</param>
        /// <param name="references">The list of DOIs of the references.</param>
        public Paper(string publisher, string abstractText, string doi, string type, string page, int referencedByCount, 
            List<string> title, List<Author> authors, string url, DateTime created, List<string> references)
        {
            this.publisher = publisher;
            this.abstractText = abstractText;
            this.doi = doi;
            this.type = type;
            this.page = page;
            this.referencedByCount = referencedByCount;
            this.title = title;
            this.authors = authors;
            this.url = url;
            this.created = created;
            this.references = references;

            if(authors.Count > 1)
            {
                if(authors.Count == 2)
                {
                    key = authors[0].family.Substring(0, Math.Min(authors[0].family.Length, 2)) + authors[1].family.Substring(0, Math.Min(authors[0].family.Length, 2)) + (created.Year % 100);
                }
                else
                {
                    key = authors[0].family.Substring(0, 1) + authors[1].family.Substring(0, 1) + authors[2].family.Substring(0, 1) + (created.Year % 100);
                }
            }
            else if(authors.Count == 1)
            {
                key = authors[0].family.Substring(0, Math.Min(authors[0].family.Length, 4)) + (created.Year % 100);
            }
            else
            {
                key = (created.Year % 100).ToString();
            }
        }

        /// <summary>
        /// Checks the equality to the object <paramref name="obj"/>.
        /// Papers are equal if they have the same doi.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Is the object the same paper.</returns>
        public override bool Equals(object obj)
        {
            Paper paper = obj as Paper;
            if (paper == null)
            {
                return false;
            }
            return paper.doi == doi;
        }

        /// <summary>
        /// Checks the equality to the paper <paramref name="paper"/>.
        /// Papers are equal if they have the same doi.
        /// </summary>
        /// <param name="paper"></param>
        /// <returns>Is the paper the same paper.</returns>
        public bool Equals(Paper paper)
        {
            if (paper == null)
            {
                return false;
            }
            return paper.doi == doi;
        }

        /// <summary>
        /// Gets a hash code of the issue object
        /// </summary>
        /// <returns>A has code</returns>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}


