using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Class for the display of paper information on a GameObject.
    /// </summary>
    public class PaperDataDisplay : DataDisplay<Paper>
    {
        [Tooltip("Textmesh of the paper key.")]
        [SerializeField] private TextMeshPro keyField;
        [Tooltip("Textmesh of the paper title.")]
        [SerializeField] private TextMeshPro titleField;
        [Tooltip("Textmesh of the year of publication of the paper.")]
        [SerializeField] private TextMeshPro yearField;
        [Tooltip("Textmesh of the paper author.")]
        [SerializeField] private TextMeshPro authorField;

        [Tooltip("Textmesh of the paper publisher.")]
        [SerializeField] private TextMeshPro publisherField;
        [Tooltip("Textmesh of the paper type.")]
        [SerializeField] private TextMeshPro typeField;
        [Tooltip("Textmesh of the paper pages.")]
        [SerializeField] private TextMeshPro pagesField;
        [Tooltip("Textmesh of the paper reference count.")]
        [SerializeField] private TextMeshPro referencedByCountField;
        [Tooltip("Textmesh of the paper abstract.")]
        [SerializeField] private TextMeshPro abstractField;

        /// <summary>
        /// Checks the component's setup and fetches necessary references
        /// </summary>
        private void Awake()
        {
            if (keyField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(keyField));
            }
            if (titleField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(titleField));
            }
            if (yearField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yearField));
            }
            if (authorField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(authorField));
            }
            if (publisherField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(publisherField));
            }
            if (typeField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(typeField));
            }
            if (pagesField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pagesField));
            }
            if (referencedByCountField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(referencedByCountField));
            }
            if (abstractField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(abstractField));
            }
        }

        /// <summary>
        /// Updates the view of the paper data display when the content changes.
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();
            if(content != null)
            {
                keyField.text = content.Key;
                titleField.text = content.Title[0];
                yearField.text = content.Created.Year.ToString();
                string authors = "";
                foreach(Author author in content.Authors)
                {
                    authors += author.family + ", " + author.given + "; ";
                }
                authorField.text = authors;
                publisherField.text = content.Publisher ?? "Publisher unknown";
                typeField.text = content.Type ?? "Type unknow";
                pagesField.text = content.Page ?? "";
                referencedByCountField.text = content.ReferencedByCount.ToString();
                abstractField.text = content.Abstracttext ?? "no abstract contained";


            }
            else
            {
                keyField.text = "Loading error";
                titleField.text = "";
                yearField.text = "";
                authorField.text = "";

                publisherField.text = "";
                typeField.text = "";
                pagesField.text = "";
                referencedByCountField.text = "";
                abstractField.text = "";
            }
        }

        /// <summary>
        /// Gets the hash code of the object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Checks wether another object is the same data display.
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>true if other object is the same data display, else false.</returns>
        public override bool Equals(object other)
        {
            PaperDataDisplay display = other as PaperDataDisplay;
            if(display == null)
            {
                return false;
            }
            // 2 Data displays are equal if the are applied on the same gameobject.
            return GameObject.ReferenceEquals(display.gameObject, gameObject);
        }
    }
}
