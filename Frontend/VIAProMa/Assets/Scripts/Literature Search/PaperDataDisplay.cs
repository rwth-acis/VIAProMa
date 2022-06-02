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
            }
            else
            {
                keyField.text = "Loading error";
                titleField.text = "";
                yearField.text = "";
                authorField.text = "";
            }
        }

    }
}
