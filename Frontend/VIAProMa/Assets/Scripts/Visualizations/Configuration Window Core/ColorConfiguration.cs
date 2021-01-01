using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ColorConfigWindow
{
    [CreateAssetMenu(fileName = "ColorConfiguration", menuName = "Scriptable Objects/Color Configuration File", order = 1)]
    public class ColorConfiguration : ScriptableObject
    {
        [SerializeField] private List<Color> colors;

        public List<Color> Colors
        {
            get => colors;
        }
    }
}