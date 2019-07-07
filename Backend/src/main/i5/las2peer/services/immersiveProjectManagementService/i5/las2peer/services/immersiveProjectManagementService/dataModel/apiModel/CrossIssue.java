package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel;

import i5.las2peer.services.immersiveProjectManagementService.RequirementsBazaarAdapter;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
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
    private IssueStatus status;
    private CrossUser[] developers;

    public CrossIssue()
    {

    }

    public CrossIssue(DataSource source, int id, String name, String description, int projectId, IssueStatus status, CrossUser[] developers) {
        this.source = source;
        this.id = id;
        this.name = name;
        this.description = description;
        this.projectId = projectId;
        this.status = status;
        this.developers = developers;
    }

    public DataSource getSource() {
        return source;
    }

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

    public IssueStatus getStatus() {
        return status;
    }

    public CrossUser[] getDevelopers() {
        return developers;
    }

    /**
     * Generates a CrossIssue object from a Requirement (from the Requirements Bazaar)
     * @param req The requirement from the requirements bazaar
     * @return corresponding CrossIssue
     */
    public static CrossIssue FromRequirement(Requirement req)
    {
        // we need the contributors in order to determine the developers and the issue status
        APIResult<ReqBazContributors> contrRes = RequirementsBazaarAdapter.GetRequirementContributors(req.id);
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
                    developers[i] = CrossUser.FromReqBazUser(contributors.developers[i]);
                }
                developers[developers.length - 1] = CrossUser.FromReqBazUser(contributors.leadDeveloper);
            }
            else
            {
                developers = CrossUser.FromReqBazUsers(contributors.developers);
            }
            CrossIssue issue = new CrossIssue(DataSource.REQUIREMENTS_BAZAAR,
                    req.id,
                    req.name,
                    req.description,
                    req.projectId,
                    DetermineIssueStatusFromRequirement(req, contributors),
                    developers
                    );
            return  issue;
        }
        else
        {
            return null;
        }
    }

    public static CrossIssue[] FromRequirements(Requirement[] reqs)
    {
        CrossIssue[] issues = new CrossIssue[reqs.length];
        for (int i=0;i<reqs.length;i++)
        {
            issues[i] = FromRequirement(reqs[i]);
        }
        return issues;
    }

    /**
     * Determines the issue status based on a requirement and its contributors
     * @param req The requirement from the Requirements Bazaar
     * @param contributors The contributors of the requirement
     * @return The status of the corresponding issue
     */
    private static IssueStatus DetermineIssueStatusFromRequirement(Requirement req, ReqBazContributors contributors) {
        if (req.realized != "")
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
}