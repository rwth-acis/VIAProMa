using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    public class FloatEffect : MonoBehaviour
    {
        [SerializeField] private float height = 0.1f;
        [SerializeField] private float frequency = 1f;

        public float Height { get => height; set => height = value; }
        public float Frequency { get => frequency; set => frequency = value; }
        public Vector3 StartPosition { get; set; }

        private void Awake()
        {
            StartPosition = transform.localPosition;
        }

        private void Update()
        {
            float offset = height * 0.5f * (1f + Mathf.Sin(2f * Mathf.PI * frequency * Time.time));
            transform.localPosition = StartPosition + new Vector3(0, offset, 0);
        }
    }
}