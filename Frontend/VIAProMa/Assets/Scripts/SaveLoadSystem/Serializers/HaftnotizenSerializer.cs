using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.UI.InputFields;
using UnityEngine;
using TMPro;
using i5.VIAProMa.Visualizations.Haftnotizen;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    [RequireComponent(typeof(HaftnotizenVisualController))]
    public class HaftnotizenSerializer : MonoBehaviour, ISerializable
    {
        private const string textKey = "note_text";

        private HaftnotizenVisualController noteText;

        private void Awake()
        {
            noteText = GetComponent<HaftnotizenVisualController>();
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            string returnedText = SerializedObject.TryGet(textKey, serializedObject.Strings, gameObject, out bool found);
            if(found){
                noteText.Text = serializedObject.Strings[textKey];
            }
        }

        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Strings.Add(textKey,  noteText.Text);
            return serializedObject;
        }
    }
}
