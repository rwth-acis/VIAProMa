using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI.KeyboardInput
{
    public class AutocompleteItem : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textLabel;

        private string text;
        private Keyboard keyboard;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                UpdateDisplay();
            }
        }

        private void Awake()
        {
            if (textLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(textLabel));
            }
            else
            {
                Text = "";
            }
        }

        public void Setup(Keyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        public void Select()
        {
            if (keyboard != null)
            {
                // store the text in a temporary variable: when changing the text of the keyboard, the text of this item will already be changed
                string tmpText = string.Copy(text);
                keyboard.Text = tmpText;
                keyboard.CursorPos = tmpText.Length;
            }
            else
            {
                Debug.LogError("User selected auto complete item but it is not yet set up");
            }
        }

        private void UpdateDisplay()
        {
            textLabel.text = text;
            gameObject.SetActive(!string.IsNullOrEmpty(text));
        }
    }
}