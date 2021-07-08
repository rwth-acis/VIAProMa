using i5.VIAProMa.Utilities;
using UnityEngine;

namespace i5.VIAProMa.UI.AppBar
{
    /// <summary>
    /// Spawner for an app bar with a configuration button
    /// </summary>
    public class AppBarConfigurableSpawner : AppBarSpanwer
    {
        [SerializeField] private GameObject configurationWindow;

        private IWindow configurationWindowInterface;

        /// <summary>
        /// Checks the component's setup and initializes it
        /// </summary>
        protected override void Awake()
        {
            if (configurationWindow == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationWindow));
            }
            configurationWindowInterface = configurationWindow?.GetComponent<IWindow>();
            if (configurationWindowInterface == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IWindow), configurationWindow);
            }

            base.Awake();
        }

        /// <summary>
        /// Sets up the spawned instance
        /// Adds the configuration window to the configuration actions
        /// </summary>
        protected override void Setup()
        {
            base.Setup();
            AppBarConfiguration configurationActions = instance.GetComponent<AppBarConfiguration>();
            configurationActions.ConfigurationWindow = configurationWindowInterface;
        }

        /// <summary>
        /// Called in the editor if the component's setup in the inspector is changed
        /// Checks if the configuration window has an IWindow component and otherwise it rejects the configurationWindow object
        /// </summary>
        private void OnValidate()
        {
            if (configurationWindow != null)
            {
                configurationWindowInterface = configurationWindow?.GetComponent<IWindow>();
                if (configurationWindowInterface == null)
                {
                    configurationWindow = null;
                }
            }
        }
    }
}