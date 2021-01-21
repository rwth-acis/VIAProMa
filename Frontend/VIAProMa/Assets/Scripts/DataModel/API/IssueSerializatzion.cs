using i5.VIAProMa.Utilities;
using i5.VIAProMa.WebConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.DataModel.API
{
    /// <summary>
    /// Compact representation of an issue
    /// It does not store the details but just the key information which are needed for retrieving the issue again from the backend
    /// </summary>
    [Serializable]
    public class IssueSerializatzion
    {
        [SerializeField] private int sourceIndex;
        [SerializeField] private int issueId;
        [SerializeField] private int projectId;

        public int SourceIndex { get => sourceIndex; }
        public DataSource Source { get => (DataSource)sourceIndex; }
        public int IssueId { get => issueId; }
        public int ProjectId { get => projectId; }

        public IssueSerializatzion(Issue fromIssue)
        {
            sourceIndex = (int)fromIssue.Source;
            issueId = fromIssue.Id;
            projectId = fromIssue.ProjectId;
        }

        public async Task<Issue> FetchFullIssue()
        {
            ApiResult<Issue> res;
            if (Source == DataSource.REQUIREMENTS_BAZAAR)
            {
                res = await RequirementsBazaar.GetRequirement(issueId);
            }
            else // GitHub
            {
                res = await GitHub.GetIssue(projectId, issueId);
            }
            if (res.Successful)
            {
                return res.Value;
            }
            else
            {
                return null;
            }
        }

        public static List<IssueSerializatzion> FromIssues(List<Issue> issues)
        {
            List<IssueSerializatzion> serializedIssues = new List<IssueSerializatzion>();
            for (int i = 0; i < issues.Count; i++)
            {
                serializedIssues.Add(new IssueSerializatzion(issues[i]));
            }
            return serializedIssues;
        }

        public static async Task<List<Issue>> FromSerializedIssues(List<IssueSerializatzion> serializedIssues)
        {
            List<Issue> issues = new List<Issue>();
            for (int i = 0; i < issues.Count; i++)
            {
                issues.Add(await serializedIssues[i].FetchFullIssue());
            }
            return issues;
        }
    }
}