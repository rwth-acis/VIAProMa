using i5.VIAProMa.Multiplayer.Poll;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Shelves.Widgets
{
    /// <summary>
    /// Object on the Poll Shelf
    /// </summary>
    public class PollObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro question;
        private int pollIndex;

        public int PollIndex
        {
            get
            {
                return pollIndex;
            }
            set
            {
                pollIndex = value;
                question.text = PollHandler.Instance.savedPolls[pollIndex]?.Question ?? "";
            }
        }

        /// <summary>
        /// Load Poll Visualization
        /// </summary>
        public void SelectPoll()
        {
            PollHandler.Instance.GenerateSynchronizedPollDisplay(pollIndex + 1);
        }
    }
}

