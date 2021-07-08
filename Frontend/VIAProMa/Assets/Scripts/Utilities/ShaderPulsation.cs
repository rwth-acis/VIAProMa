using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    [RequireComponent(typeof(Renderer))]
    public class ShaderPulsation : MonoBehaviour
    {
        [Tooltip("The name of the property which should pulsate. The property is expected to be a color.")]
        [SerializeField] private string propertyName;

        [SerializeField] private Color color;
        [SerializeField] private float strength = 1f;
        [SerializeField] private float frequency = 1f;

        private Renderer rend;


        public Color Color { get => color; set => color = value; }
        public float Strength { get => strength; set => strength = value; }
        public float Frequency { get => frequency; set => frequency = value; }

        private void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        private void Update()
        {
            float factor = strength * 0.5f * (1f + Mathf.Sin(2f * Mathf.PI * frequency * Time.time));
            rend.material.SetColor(propertyName, factor * color);
        }
    }
}