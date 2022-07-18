using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperDataDisplay : DataDisplay<Paper>
    {
        [SerializeField] private TextMeshPro keyField;
        [SerializeField] private TextMeshPro titleField;
        [SerializeField] private TextMeshPro yearField;
        [SerializeField] private TextMeshPro authorField;

        [SerializeField] private TextMeshPro publisherField;
        [SerializeField] private TextMeshPro typeField;
        [SerializeField] private TextMeshPro pagesField;
        [SerializeField] private TextMeshPro referencedByCountField;
        [SerializeField] private TextMeshPro abstractField;

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

        public override bool Equals(object other)
        {
            PaperDataDisplay display = other as PaperDataDisplay;
            if(display == null)
            {
                return false;
            }
            return display.Content.Equals(Content);
        }
    }
}
