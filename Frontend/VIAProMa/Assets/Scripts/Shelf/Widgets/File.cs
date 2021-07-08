using i5.VIAProMa.Shelves.ProjectLoadShelf;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Shelves.Widgets
{
    /// <summary>
    /// Adapter class which maps project information to the file GameObject
    /// </summary>
    public class File : MonoBehaviour
    {
        /// <summary>
        /// text mesh of the title on the back of the file
        /// </summary>
        [SerializeField] TextMeshPro titleOnBack;
        /// <summary>
        /// text mesh on the top of the file
        /// </summary>
        [SerializeField] TextMeshPro titleOnTop;

        private string projectTitle;

        /// <summary>
        /// The title of the project's title how it is displayed on the text labels of the file
        /// </summary>
        public string ProjectTitle
        {
            get
            {
                return projectTitle;
            }
            set
            {
                projectTitle = value;
                titleOnBack.text = projectTitle;
                titleOnTop.text = projectTitle;
            }
        }

        public ProjectLoader ProjectLoader
        {
            get; set;
        }

        public void SelectFile()
        {
            ProjectLoader.LoadProject(ProjectTitle);
        }
    }
}