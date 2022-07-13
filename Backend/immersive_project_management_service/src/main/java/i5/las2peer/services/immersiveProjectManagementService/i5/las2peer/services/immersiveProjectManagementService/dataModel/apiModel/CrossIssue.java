package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel;

import com.fasterxml.jackson.annotation.JsonIgnore;
import i5.las2peer.services.immersiveProjectManagementService.RequirementsBazaarConnector;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub.GitHubIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazContributors;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;

import java.util.Arrays;

/**
 * Created by bened on 07.07.2019.
 */
public class CrossIssue {
    private DataSource source;
    private int id;
    private String name;
    private String description;
    private int projectId;
    private CrossUser creator;
    private IssueStatus status;
    private String creationDate;
    private String lastUpdated;
    private String closedDate;
    private CrossUser[] developers;
    private CrossUser[] commenters;

    public CrossIssue()
    {

    }

    public CrossIssue(DataSource source, int id, String name, String description, int projectId, CrossUser creator, IssueStatus status, String creationDate, String lastUpdated, String closedDate, CrossUser[] developers, CrossUser[] commenters) {
        this.source = source;
        this.id = id;
        this.name = name;
        this.description = description;
        this.projectId = projectId;
        this.creator = creator;
        this.status = status;
        this.creationDate = creationDate;
        this.lastUpdated = lastUpdated;
        this.closedDate = closedDate;
        this.developers = developers;
        this.commenters = commenters;
    }

    @JsonIgnore
    public DataSource getDataSource() {
        return source;
    }

    public int getSource() {return source.ordinal();}

    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }

    public int getProjectId() {
        return projectId;
    }

    public CrossUser getCreator() {
        return creator;
    }

    @JsonIgnore
    public IssueStatus getIssueStatus() {
        return status;
    }

    public int getStatus() {return status.ordinal(); }

    public String getCreationDate() { return creationDate; }

    public String getLastUpdated() { return lastUpdated; }

    public String getClosedDate() { return closedDate; }

    public CrossUser[] getDevelopers() {
        return developers;
    }

    public CrossUser[] getCommenters() { return commenters; }

    /**
     * Generates a CrossIssue object from a Requirement (from the Requirements Bazaar)
     * @param req The requirement from the requirements bazaar
     * @return corresponding CrossIssue
     */
    public static CrossIssue fromRequirement(Requirement req)
    {
        // we need the contributors in order to determine the developers and the issue status
        APIResult<ReqBazContributors> contrRes = RequirementsBazaarConnector.getRequirementContributors(req.getId());
        if (contrRes.successful())
        {
            ReqBazContributors contributors = contrRes.getValue();
            CrossUser[] developers;
            // construct the list of developers; this also includes the lead developer
            if (contributors.leadDeveloper != null && !Arrays.asList(contributors.developers).contains(contributors.leadDeveloper)) // also add the lead developer to the list of developers
            {
                developers = new CrossUser[contributors.developers.length + 1];
                for (int i=0;i<contributors.developers.length;i++)
                {
                    developers[i] = CrossUser.fromReqBazUser(contributors.developers[i]);
                }
                developers[developers.length - 1] = CrossUser.fromReqBazUser(contributors.leadDeveloper);
            }
            else
            {
                developers = CrossUser.fromReqBazUsers(contributors.developers);
            }
            CrossIssue issue = new CrossIssue(DataSource.REQUIREMENTS_BAZAAR,
                    req.getId(),
                    req.getName(),
                    req.getDescription(),
                    req.getProjectId(),
                    CrossUser.fromReqBazUser(req.getCreator()),
                    determineIssueStatusFromRequirement(req, contributors),
                    req.getCreationDate(),
                    req.getLastUpdatedDate(),
                    req.getRealized(),
                    developers,
                    CrossUser.fromReqBazUsers(contributors.commentCreator)
                    );
            return  issue;
        }
        else
        {
            return null;
        }
    }

    public static CrossIssue[] fromRequirements(Requirement[] reqs)
    {
        CrossIssue[] issues = new CrossIssue[reqs.length];
        for (int i=0;i<reqs.length;i++)
        {
            issues[i] = fromRequirement(reqs[i]);
        }
        return issues;
    }

    /**
     * Determines the issue status based on a requirement and its contributors
     * @param req The requirement from the Requirements Bazaar
     * @param contributors The contributors of the requirement
     * @return The status of the corresponding issue
     */
    private static IssueStatus determineIssueStatusFromRequirement(Requirement req, ReqBazContributors contributors) {
        if (req.getRealized() != null)
        {
            return  IssueStatus.CLOSED;
        }
        else
        {
            if (contributors.leadDeveloper != null || contributors.developers.length > 0)
            {
                return IssueStatus.IN_PROGRESS;
            }
            else
            {
                return  IssueStatus.OPEN;
            }
        }
    }

    public static CrossIssue fromGitHubIssue(GitHubIssue gitHubIssue, int repositoryId)
    {
        CrossIssue issue = new CrossIssue(
                DataSource.GITHUB,
                gitHubIssue.getNumber(),
                gitHubIssue.getTitle(),
                gitHubIssue.getBody(),
                repositoryId,
                CrossUser.fromGitHubUser(gitHubIssue.getUser()),
                gitHubIssue.getIssueStatus(),
                gitHubIssue.getCreated_at(),
                gitHubIssue.getUpdated_at(),
                gitHubIssue.getClosed_at(),
                CrossUser.fromGitHubUsers(gitHubIssue.getAssignees()),
                null);
        return issue;
    }

    public static CrossIssue[] fromGitHubIssues(GitHubIssue[] gitHubIssues, int repositoryId)
    {
        CrossIssue[] issues = new CrossIssue[gitHubIssues.length];
        for (int i=0;i<gitHubIssues.length;i++)
        {
            issues[i] = CrossIssue.fromGitHubIssue(gitHubIssues[i], repositoryId);
        }
        return issues;
    }

    @Override
    public boolean equals(Object obj) {
        if (!(obj instanceof CrossIssue))
        {
            return false;
        }
        CrossIssue other = (CrossIssue)obj;
        return this.source == other.source && this.id == other.id;
    }
}