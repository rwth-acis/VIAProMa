using i5.VIAProMa.DataModel.API;
using System;

public class IssueDeletedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue deleted arguments
    /// </summary>
    /// <param name="issueName">The previous name of the issue that was deleted</param>
    /// <param name="projectID">Id if the project the issue belongs to</param>
    public IssueDeletedArgs(String issueName, int projectID)
    {
        IssueName = issueName;
        ProjectID = projectID;
    }

    /// <summary>
    /// The name of the issue that has been deleted
    /// </summary>
    public String IssueName { get; private set; }

    /// <summary>
    /// the projectID of the issue that has been deleted
    /// </summary>
    public int ProjectID { get; private set; }
}
