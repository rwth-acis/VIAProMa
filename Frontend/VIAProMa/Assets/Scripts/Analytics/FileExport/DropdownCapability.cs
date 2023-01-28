using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.UI;
using i5.VIAProMa.UI.DropdownMenu;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.UI.ListView.Strings;
using i5.VIAProMa.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.Shelves.IssueShelf
{
    public class DropdownCapability : MonoBehaviour, IWindow
    {
        [Header("References")]
        [Header("UI Elements")]
        [SerializeField] private StringDropdownMenu sourceSelection;
        [SerializeField] private StringListView listView;

        private Project[] projects;
        private Category[] categories;

        public event EventHandler SourceChanged;
        public event EventHandler WindowOpened;
        public event EventHandler WindowClosed;

        private bool isConfiguring = true;

        public ExportSelection ChosenExportSelection;

        public IDropdownConfiguration DropdownConfiguration { get; private set; }

        public bool WindowEnabled
        { // not needed for configuration window => does not have an effect
            get; set;
        }
        public bool WindowOpen { get; private set; } = true;

        public bool ExternallyInitialized { get; set; } = false;

        private void Awake()
        {
            
            if (sourceSelection == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(sourceSelection));
            }
            sourceSelection.ItemSelected += SetExportSelection; 
        }

        private void Start()
        {
            if (!ExternallyInitialized)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            // populate the source dropdown menu with the available data sources
            List<StringData> sources = new List<StringData>();
            foreach (ExportSelection source in Enum.GetValues(typeof(ExportSelection)))
            {
                sources.Add(new StringData(source.GetDescription()));
            }
            sourceSelection.Items = sources;

            //sourceSelection.ItemSelected += SourceSelected; Whatever this does??
            SetExportSelection(ExportSelection.JSON); // first entry of dropdown box is Requirements Bazaar, so set this as the default 

            isConfiguring = false;
        }

        /*
        private async void SourceSelected(object sender, EventArgs e)
        {
            ExportSelection SelectedFormat = (ExportSelection)sourceSelection.SelectedItemIndex;
            SetExportSelection(SelectedFormat);
            SourceChanged?.Invoke(this, EventArgs.Empty); // important: invoke it only if the user changes the source
        }
        */

        public void SetExportSelection(object Sender, EventArgs eventArgs)
        {
            SetExportSelection(Enum.Parse<ExportSelection>(listView.SeletedItem.text.ToUpper()));
        }
        public void SetExportSelection(ExportSelection SelectedFormat)
        {
            ChosenExportSelection = SelectedFormat;
            sourceSelection.SelectedItemIndex = (int)SelectedFormat;
        }

        public void Open()
        {
            gameObject.SetActive(true);
            WindowOpen = true;
            WindowOpened?.Invoke(this, EventArgs.Empty);
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            // do not set position and eulerAngles since the configuration window should be fixed
        }

        public void Close()
        {
            WindowOpen = false;
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}