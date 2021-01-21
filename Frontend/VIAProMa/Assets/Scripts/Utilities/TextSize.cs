using HoloToolkit.Unity;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    /// <summary>
    /// Script for measuring the size of text
    /// It uses an invisible textMesh in the scene, loads the text in there with the given settigns and measures its size
    /// </summary>
    public class TextSize : Singleton<TextSize>
    {
        /// <summary>
        /// The textMesh which is used to try out the text configurations
        /// </summary>
        [Tooltip("The textMesh which is used to try out the text configurations")]
        [SerializeField] private TextMeshPro textMesh;

        /// <summary>
        /// Instantiates the script, i.e. it creates the textMesh and gets its components
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (textMesh == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(textMesh));
            }

            // the textMesh should not be visible in the scene
            textMesh.gameObject.SetActive(false);

            if (textMesh == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(TextMeshPro), gameObject);
                return;
            }
            textMesh.transform.position = Vector3.zero;
            textMesh.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Measures the size of text in world units
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="fontSize">The font size of the text</param>
        /// <returns>The size of the text in the given font size in world units</returns>
        public Vector2 GetTextSize(string text, float fontSize)
        {
            textMesh.fontSize = fontSize;
            textMesh.text = text;
            textMesh.ForceMeshUpdate();
            Vector2 textSize = textMesh.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
            return textSize;
        }
    }
}