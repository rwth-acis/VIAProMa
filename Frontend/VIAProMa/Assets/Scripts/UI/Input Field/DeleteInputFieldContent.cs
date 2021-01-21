using UnityEngine;

namespace i5.VIAProMa.UI.InputFields
{
    [RequireComponent(typeof(InputField))]
    public class DeleteInputFieldContent : MonoBehaviour
    {
        private InputField inputField;

        public void DeleteContent()
        {
            inputField.Text = "";
        }
    }
}