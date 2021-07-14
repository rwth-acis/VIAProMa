using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.Visualizations.BuildingProgressBar;
using UnityEngine;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    /// <summary>
    /// Serializes the data of the building progress bar
    /// </summary>
    [RequireComponent(typeof(MockupEdiorGameObject))]
    public class MockupItemSerializer : MonoBehaviour, ISerializable
    {
        private const string categoryKey = "category";
        private const string indexKey = "index";

        private MockupEdiorGameObject mockupItem;

        /// <summary>
        /// Initializes the component
        /// </summary>
        private void Awake()
        {
            mockupItem = GetComponent<MockupEdiorGameObject>();
        }

        /// <summary>
        /// Applies the settings which are stored in the serializedObject to the building progress bar
        /// </summary>
        /// <param name="serializedObject">The serialized object with the save data</param>
        public void Deserialize(SerializedObject serializedObject)
        {
            string category = SerializedObject.TryGet(categoryKey, serializedObject.Strings, gameObject, out bool found);
            if (found)
            {
                mockupItem.category = category;
            }
            int index = SerializedObject.TryGet(indexKey, serializedObject.Integers, gameObject, out bool found2);
            if (found2)
            {
                mockupItem.index = index;
            }
            mockupItem.SpawnChildObject();
        }

        /// <summary>
        /// Writes the data of the progress bar into a save data object
        /// </summary>
        /// <returns>The saved data of the building progress bar component</returns>
        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Strings.Add(categoryKey, mockupItem.category);
            serializedObject.Integers.Add(indexKey, mockupItem.index);
            return serializedObject;
        }
    }
}