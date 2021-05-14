using i5.VIAProMa.DataModel.API;
using System;

public class IssueEditedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue selection changed arguments
    /// </summary>
    /// <param name="issue">The issue which was selected or deselected</param>
    /// <param name="selected">True if the issue was selected, false if deselected</param>
    public IssueEditedArgs(String issueName, String newName, String newDescription)
    {
        IssueName = issueName;
        NewName = newName;
        NewDescription = newDescription;
    }

    /// <summary>
    /// The name of the issue that has been edited
    /// </summary>
    public String IssueName { get; private set; }

    /// <summary>
    /// Give the new name of the issue
    /// </summary>
    public String NewName { get; private set; }

    /// <summary>
    /// Give the new description of the issue
    /// </summary>
    public String NewDescription { get; private set; }
}
