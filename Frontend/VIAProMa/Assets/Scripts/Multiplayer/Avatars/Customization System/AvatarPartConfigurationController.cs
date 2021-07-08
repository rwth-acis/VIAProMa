using i5.VIAProMa.Multiplayer.Avatars.Customization.Configurator;
using i5.VIAProMa.Utilities;
using System;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization
{
    /// <summary>
    /// Controls the configuration of one single avatar part
    /// </summary>
    public class AvatarPartConfigurationController : MonoBehaviour, IConfigurationController
    {
        [Tooltip("The available options for this part")]
        [SerializeField] private AvatarPartCollection[] avatarPartCollections;

        public event EventHandler ModelChanged;
        public event EventHandler ConfigurationChanged;

        private SkinnedMeshRenderer partRenderer;

        private int avatarIndex;

        private int modelIndex;
        private int materialIndex;
        private int colorIndex;

        public int AvatarIndex
        {
            get => avatarIndex;
            set
            {
                // if an avatar was selected for which no variants exist => take the standard options of the first avatar
                if (value >= avatarPartCollections.Length)
                {
                    avatarIndex = 0;
                }
                else
                {
                    avatarIndex = value;
                    modelIndex = Mathf.Clamp(modelIndex, 0, avatarPartCollections[avatarIndex].avatarParts.Length - 1);
                    materialIndex = 0;
                    colorIndex = 0;
                    ConfigurationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The index of the 3D model which is displayed on this part
        /// </summary>
        public int ModelIndex
        {
            get => modelIndex;
            set
            {
                modelIndex = value;
                materialIndex = 0;
                colorIndex = 0;
                ModelChanged?.Invoke(this, EventArgs.Empty);
                ConfigurationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The index of the material which is displayed on this part
        /// </summary>
        public int MaterialIndex
        {
            get => materialIndex;
            set
            {
                materialIndex = value;
                colorIndex = 0;
                ConfigurationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The index of the color variation which is displayed on this part
        /// </summary>
        public int ColorIndex
        {
            get => colorIndex;
            set
            {
                colorIndex = value;
                ConfigurationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The available avatar part options for this part
        /// </summary>
        public AvatarPart[] AvatarParts { get => avatarPartCollections[avatarIndex].avatarParts; }

        /// <summary>
        /// The availalbe avatar part materials for the currently displayed model
        /// </summary>
        public AvatarPartMaterial[] AvatarPartMaterials { get => avatarPartCollections[avatarIndex].avatarParts[modelIndex].PartMaterials; }

        /// <summary>
        /// The available color variations for the currently displayed model and material
        /// </summary>
        public Color[] AvatarPartColors
        {
            get
            {
                if (avatarPartCollections[avatarIndex].avatarParts[modelIndex].MaterialVariationCount == 0)
                {
                    return new Color[0];
                }
                else
                {
                    return avatarPartCollections[avatarIndex].avatarParts[modelIndex]?.GetAvatarPartMaterial(materialIndex)?.Colors;
                }
            }
        }

        public ColorItem[] AvatarPartColorsAsItems
        {
            get
            {
                Color[] colors = AvatarPartColors;
                ColorItem[] res = new ColorItem[colors.Length];
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = new ColorItem(colors[i]);
                }
                return res;
            }
        }

        /// <summary>
        /// The currently displayed part 3D model
        /// </summary>
        public AvatarPart CurrentPart { get => avatarPartCollections[avatarIndex].avatarParts[modelIndex]; }

        /// <summary>
        /// The curerntly displayed part material
        /// </summary>
        public AvatarPartMaterial CurrentMaterial { get => avatarPartCollections[avatarIndex].avatarParts[modelIndex].GetAvatarPartMaterial(materialIndex); }

        /// <summary>
        /// The currently displayed color variation
        /// </summary>
        public Color CurrentColor { get => avatarPartCollections[avatarIndex].avatarParts[modelIndex].GetAvatarPartMaterial(materialIndex).GetColor(colorIndex); }

        /// <summary>
        /// Initializes the component, checks the setup
        /// </summary>
        private void Awake()
        {
            // check setup
            partRenderer = GetComponent<SkinnedMeshRenderer>();
            if (partRenderer == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(partRenderer), gameObject);
            }
            if (avatarPartCollections.Length == 0)
            {
                SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(avatarPartCollections));
            }
            for (int i = 0; i < avatarPartCollections.Length; i++)
            {
                for (int j = 0; j < avatarPartCollections[i].avatarParts.Length; j++)
                {
                    if (avatarPartCollections[i].avatarParts[j] == null)
                    {
                        SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(avatarPartCollections) + i, j);
                    }
                    else if (avatarPartCollections[i].avatarParts[j].MaterialVariationCount == 0)
                    {
                        SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, "MaterialVariationCount");
                    }
                }
            }
        }

        /// <summary>
        /// Applies the selected configuration indices to the renderer on the avatar
        /// </summary>
        public void ApplyConfiguration()
        {
            // handle case that no material variation was given (e.g. for the case "do not show this part")
            if (avatarPartCollections[avatarIndex].avatarParts[ModelIndex].Mesh == null
                && avatarPartCollections[avatarIndex].avatarParts[ModelIndex].MaterialVariationCount == 0)
            {
                SetConfiguration(null, null, Color.white);
            }
            // otherwise set the part's appearance according to the indices
            else if (ModelIndex < avatarPartCollections[avatarIndex].avatarParts.Length
                && MaterialIndex < avatarPartCollections[avatarIndex].avatarParts[ModelIndex].MaterialVariationCount)
            {
                AvatarPartMaterial avatarMat = avatarPartCollections[avatarIndex].avatarParts[ModelIndex].GetAvatarPartMaterial(MaterialIndex);
                Color avatarMatColor = avatarMat.Material.color; // avatar part does not necessarily have color variations => use default color

                if (ColorIndex < avatarMat.ColorVariationCount)
                {
                    avatarMatColor = avatarMat.GetColor(ColorIndex);
                }

                SetConfiguration(avatarPartCollections[avatarIndex].avatarParts[ModelIndex].Mesh, avatarMat.Material, avatarMatColor);
            }
        }

        /// <summary>
        /// Applies a specific (already processed and extracted) configuration to the avatar
        /// </summary>
        /// <param name="mesh">The mesh to show</param>
        /// <param name="material">The material which is applied to the mesh's renderer</param>
        /// <param name="color">The color which is set on the material's _color parameter</param>
        private void SetConfiguration(Mesh mesh, Material material, Color color)
        {
            partRenderer.sharedMesh = mesh;
            partRenderer.material = material;
            partRenderer.material.color = color;
        }
    }
}