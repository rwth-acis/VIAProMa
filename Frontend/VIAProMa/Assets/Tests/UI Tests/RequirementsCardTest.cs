using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Tests
{
    public class RequirementsCardTest : MonoBehaviour
    {
        [SerializeField] IssueDataDisplay dataDisplay;

        public int requirementId = 2127;

        private void Awake()
        {
            if (dataDisplay == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(dataDisplay));
            }
        }

        // Update is called once per frame
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
                }
            }
        }
    }
}
