using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.WebConnection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Tests
{
    /// <summary>
    /// Tests the issue cards
    /// If the user presses space, a requirement from the Requirements Bazaar is loaded and displayed on the given dataDisplay
    /// </summary>
    public class RequirementsCardTest : MonoBehaviour
    {
        [SerializeField] IssueDataDisplay dataDisplay;

        public int requirementId = 2127;

        /// <summary>
        /// Checks the setup
        /// </summary>
        private void Awake()
        {
            if (dataDisplay == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(dataDisplay));
            }
        }

        /// <summary>
        /// If the user presses space, a request to the Requirements Bazaar is started
        /// </summary>
        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ApiResult<Issue> res = await RequirementsBazaar.GetRequirement(requirementId);
                if (res.Successful)
                {
                    dataDisplay.Setup(res.Value);
                }
                else
                {
                    Debug.LogError("Error fetching the requirement: (Code " + res.ResponseCode + ") " + res.ErrorMessage);
                    dataDisplay.Setup(null);
                }
            }
        }
    }
}
