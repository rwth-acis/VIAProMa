using i5.VIAProMa.SaveLoadSystem.Core;
using UnityEngine;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    [RequireComponent(typeof(ImportedModel))]
    public class ImportedModelSerializer : MonoBehaviour, ISerializable
    {
        private const string webLinkKey = "importedModel_webLink";
        private const string ownerKey = "importedModel_owner";

        private ImportedModel imported_model;

        private void Awake()
        {
            imported_model = GetComponent<ImportedModel>();
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            string web_link = SerializedObject.TryGet(webLinkKey, serializedObject.Strings, gameObject, out bool found_web_link);
            if (found_web_link)
            {
				imported_model.WebLink = web_link;
            }
            string owner = SerializedObject.TryGet(ownerKey, serializedObject.Strings, gameObject, out bool found_owner);
            if (found_owner)
            {
				imported_model.Owner = owner;
            }
        }

        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Strings.Add(webLinkKey, imported_model.WebLink);
            serializedObject.Strings.Add(webLinkKey, imported_model.Owner);
            return serializedObject;
        }
    }
}