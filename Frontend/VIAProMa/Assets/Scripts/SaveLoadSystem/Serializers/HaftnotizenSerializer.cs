using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.UI.InputFields;
using UnityEngine;
using TMPro;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    [RequireComponent(typeof(HaftnotizenVisualController))]
    public class HaftnotizenSerializer : MonoBehaviour, ISerializable
    {
        private const string textKey = "haftnotiz_text";

        private HaftnotizenVisualController contentText;

        private void Awake()
        {
            contentText = GetComponent<HaftnotizenVisualController>();
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            string returnedText = SerializedObject.TryGet(textKey, serializedObject.Strings, gameObject, out bool found);
            if(found){
                    contentText.Text = serializedObject.Strings[textKey];
            }
        }

        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Strings.Add(textKey,  contentText.Text);
            return serializedObject;
        }
    }
}
