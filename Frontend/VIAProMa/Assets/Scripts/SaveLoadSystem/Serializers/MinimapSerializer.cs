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
        private const string objsKey = "minimap_objs";
        private MinimapController controller;

        private void Awake()
        {
            controller = GetComponent<MinimapController>();
        }

        public SerializedObject Serialize()
        {
            throw new NotImplementedException();
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            throw new NotImplementedException();
        }
    }
}
