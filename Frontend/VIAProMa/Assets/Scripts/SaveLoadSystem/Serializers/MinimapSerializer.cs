using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.Visualizations.Minimap;
using UnityEngine;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    class MinimapSerializer : MonoBehaviour, ISerializable
    {
        private const string colorKey = "minimap_color";
        private const string widthKey = "minimap_width";
        private const string heightKey = "minimap_height";

        private MinimapController minimapController;

        private void Awake()
        {
            minimapController = GetComponent<MinimapController>();
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            Vector3 colorVector = SerializedObject.TryGet(colorKey, serializedObject.Vector3s, gameObject, out bool found);
            if (found)
            {
                minimapController.Color = new Color(colorVector.x, colorVector.y, colorVector.z);
            }
            float width = SerializedObject.TryGet(widthKey, serializedObject.Floats, gameObject, out found);
            if (found)
            {
                minimapController.Width = width;
            }
            float height = SerializedObject.TryGet(heightKey, serializedObject.Floats, gameObject, out found);
            if (found)
            {
                minimapController.Height = height;
            }

        }

        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Vector3s.Add(colorKey, new Vector3(
                minimapController.Color.r,
                minimapController.Color.g,
                minimapController.Color.b));
            serializedObject.Floats.Add(widthKey, minimapController.Width);
            serializedObject.Floats.Add(heightKey, minimapController.Height);

            return serializedObject;
        }
    }
}
