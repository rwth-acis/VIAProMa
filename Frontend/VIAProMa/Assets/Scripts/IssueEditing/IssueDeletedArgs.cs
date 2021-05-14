using i5.VIAProMa.DataModel.API;
using System;

public class IssueDeletedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue selection changed arguments
    /// </summary>
    /// <param name="issue">The issue which was selected or deselected</param>
    /// <param name="selected">True if the issue was selected, false if deselected</param>
    public IssueDeletedArgs(String issueName)
    {
        IssueName = issueName;
    }

    /// <summary>
    /// The name of the issue that has been deleted
    /// </summary>
    public String IssueName { get; private set; }
}
