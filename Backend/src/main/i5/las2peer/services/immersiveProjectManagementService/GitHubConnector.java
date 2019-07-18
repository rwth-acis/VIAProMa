package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.IssueStatus;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub.GitHubIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;

import javax.ws.rs.core.Response;
import javax.ws.rs.core.UriBuilder;
import java.io.IOException;
import java.net.URI;

/**
 * Created by bened on 18.07.2019.
 */
public class GitHubConnector {

    private static String baseUrl = "https://api.github.com";

    public static Response requestIssuesInRepository(String owner, String repository, int page, int per_page)
    {
        String statusString;

        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("repos/{owner}/{repo}/issues")
                .queryParam("state", "all")
                .queryParam("page", page)
                .queryParam("per_page", per_page);

        URI uri = uriBuilder.build(owner, repository);
        return Utilities.getResponse(uri);
    }

    public static APIResult<GitHubIssue[]> getIssuesInRepository(String owner, String repository, int page, int per_page)
    {
        try {

            Response response = requestIssuesInRepository(owner, repository, page, per_page);

            if (response.getStatus() != 200 && response.getStatus() != 201) {
                return new APIResult<GitHubIssue[]>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            GitHubIssue[] issues = mapper.readValue(origJson, GitHubIssue[].class);
            return new APIResult<GitHubIssue[]>(response.getStatus(), issues);
        }
        catch (IOException e)
        {
            return new APIResult<GitHubIssue[]>(e.getMessage(), 500);
        }
    }
}
