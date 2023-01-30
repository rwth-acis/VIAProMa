using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.UI;
using i5.VIAProMa.UI.DropdownMenu;
using i5.VIAProMa.UI.ListView.Strings;
using i5.VIAProMa.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Analytics.FileExport
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
        { // Not needed for configuration window => does not have an effect.
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
            // Populate the source dropdown menu with the available data sources.
            List<StringData> sources = new List<StringData>();
            foreach (ExportSelection source in Enum.GetValues(typeof(ExportSelection)))
            {
                sources.Add(new StringData(source.GetDescription()));
            }
            sourceSelection.Items = sources;
            sourceSelection.ItemSelected += SetExportSelection;
            SetExportSelection(ExportSelection.JSON); // First entry of dropdown box is JSON, so set this as the default.

            isConfiguring = false;
        }
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
            // Do not set position and eulerAngles since the configuration window should be fixed.
        }

        public void Close()
        {
            WindowOpen = false;
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}