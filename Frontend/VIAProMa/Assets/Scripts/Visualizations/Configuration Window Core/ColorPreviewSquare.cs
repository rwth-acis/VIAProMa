using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ColorConfigWindow
{
    [RequireComponent(typeof(Interactable))]
    public class ColorPreviewSquare : MonoBehaviour
    {
        [SerializeField] private Renderer previewQuad;

        private Interactable interactable;

        public Color Color
        {
            get
            {
                return previewQuad.material.color;
            }
            set
            {
                previewQuad.material.color = value;
            }
        }

        public ConfigurationColorChooser ColorChooser
        {
            get; set;
        }

        private void Awake()
        {
            if (previewQuad == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(previewQuad));
            }
            interactable = GetComponent<Interactable>();
        }

        public void Select()
        {
            if (ColorChooser != null)
            {
                ColorChooser.SelectedColor = Color;
            }
        }
    }
}