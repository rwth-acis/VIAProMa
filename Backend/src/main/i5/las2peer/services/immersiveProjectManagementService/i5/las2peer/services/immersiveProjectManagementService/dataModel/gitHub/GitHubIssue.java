package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.IssueStatus;
import net.minidev.json.annotate.JsonIgnore;

/**
 * Created by bened on 18.07.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class GitHubIssue {
    private int id;
    private int number;
    private String state;
    private String title;
    private String body;
    private GitHubUser user;
    private GitHubLabel[] labels;
    private GitHubUser[] assignees;
    private boolean locked;



    public int getId() {
        return id;
    }

    public int getNumber() {
        return number;
    }

    public String getState() {
        return state;
    }

    public String getTitle() {
        return title;
    }

    public String getBody() {
        return body;
    }

    public GitHubUser getUser() {
        return user;
    }

    public GitHubLabel[] getLabels() {
        return labels;
    }

    public GitHubUser[] getAssignees() {
        return assignees;
    }

    public boolean isLocked() {
        return locked;
    }

    public IssueStatus getIssueStatus()
    {
        if (state == "closed")
        {
            return IssueStatus.CLOSED;
        }
        else
        {
            if (assignees != null && assignees.length > 0)
            {
                return IssueStatus.IN_PROGRESS;
            }
            else
            {
                return IssueStatus.OPEN;
            }
        }
    }
}
