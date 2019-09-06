using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{

    public class AxisController : MonoBehaviour
    {
        [SerializeField] private float length = 1f;
        [SerializeField] private TextMeshPro labelPrefab;
        [SerializeField] private TextMeshPro titleLabel;

        public float Length
        {
            get => length;
            set
            {
                length = value;
            }
        }

        public void Setup(Axis axis, float length)
        {
            Length = length;
        }

        private void UpdateAxis()
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                length
                );
        }
    }
}
