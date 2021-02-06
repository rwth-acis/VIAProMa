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
        private const string colorKey = "note_color";

        private HaftnotizenVisualController noteVisual;

        private void Awake()
        {
            noteVisual = GetComponent<HaftnotizenVisualController>();
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            string returnedText = SerializedObject.TryGet(textKey, serializedObject.Strings, gameObject, out bool found);
            if(found){
                noteVisual.Text = serializedObject.Strings[textKey];
            }

            string returnedColor = SerializedObject.TryGet(colorKey, serializedObject.Strings, gameObject, out found);
            if(found){
                noteVisual.ColorTag = serializedObject.Strings[colorKey];
            }
        }

        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Strings.Add(textKey,  noteVisual.Text);
            serializedObject.Strings.Add(colorKey,  noteVisual.ColorTag);
            return serializedObject;
        }
    }
}
