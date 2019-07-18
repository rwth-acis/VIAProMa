package i5.las2peer.services.immersiveProjectManagementService;

import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.IssueStatus;

import javax.ws.rs.core.Response;
import javax.ws.rs.core.UriBuilder;
import java.net.URI;

/**
 * Created by bened on 18.07.2019.
 */
public class GitHubConnector {

    private static String baseUrl = "https://api.github.com";

    public static Response requestIssuesInRepository(String owner, String repository, IssueStatus status, int page, int per_page)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("repos/{owner}/{repo}/issues")
                .queryParam("state", "all")
                .queryParam("page", page)
                .queryParam("per_page", per_page);

        URI uri = uriBuilder.build(owner, repository);
        return Utilities.getResponse(uri);
    }
}
