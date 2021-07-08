using UnityEngine;

namespace i5.VIAProMa.Visualizations
{
    /// <summary>
    /// Interface for objects where the color can be changed
    /// </summary>
    public interface IColorChangeable
    {
        /// <summary>
        /// The main color which can be changed on the accompanying GameObject
        /// </summary>
        Color Color { get; set; }
    }
}