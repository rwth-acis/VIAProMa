using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.Visualizations.ColorConfigWindow;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Configuration window for the minimap (... button)
    /// </summary>
    public class MinimapConfiguration : ConfigurationWindow
    {
        [Tooltip("The UI which handles the minimap")]
        [SerializeField] private ConfigurationColorChooser colorChooser;

        public override bool WindowEnabled
        {
            get => base.WindowEnabled;
            set
            {
                base.WindowEnabled = value;
                colorChooser.UIEnabled = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            colorChooser.Setup(visualization);
        }

        public override void Open()
        {
            base.Open();
            transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
            transform.rotation = appBarSpawner.SpawnedInstance.transform.rotation;
        }
    }
}
