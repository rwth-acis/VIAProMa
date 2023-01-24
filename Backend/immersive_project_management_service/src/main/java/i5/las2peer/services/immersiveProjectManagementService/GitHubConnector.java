package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.IssueStatus;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub.GitHubIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub.GitHubRepository;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;

import javax.ws.rs.core.Response;
import javax.ws.rs.core.UriBuilder;
import java.io.IOException;
import java.net.URI;
import java.Utilities.timestamp;

/**
 * Created by bened on 18.07.2019.
 */
public class GitHubConnector {

    private static int requestCounter = 0;
    private static Timestamp last_timestamp = null; 

    private static String baseUrl = "https://api.github.com";


    public static void increaseCounter() 
    {
        requestCounter += 1;
    }

    public static void resetCounter() 
    {
        requestCounter = 0;
    }

    private static boolean increaseRequestCounter()
    {
        long now = System.currentTimeMillis();
        long then = last_timestamp.getTime();
        long hours = TimeUnit.MILLISECONDS.toHours(now - then);

        if(requestCounter == 0 || abs(hours) >= 1)
        {
            last_timestamp = new Timestamp(now);
            resetCounter();
            increaseCounter();
            return true;
        }
        else 
        {
            increaseCounter();
            if(requestCounter > 5000)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public static Response requestRepository(String owner, String repository)
    {
        String statusString;

        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                        .path("repos/{owner}/{repo}");

        URI uri = uriBuilder.build(owner, repository);

        if(increaseRequestCounter())
        {
            return Utilities.getResponse(uri);
        }
        else
        {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
        }
    }

    public static APIResult<GitHubRepository> getRepository(String owner, String repositoryName)
    {
        try {
            if(increaseRequestCounter())
            {
                Response response = requestRepository(owner, repositoryName);

                if (response.getStatus() != 200 && response.getStatus() != 201) {
                    return new APIResult<GitHubRepository>(response.readEntity(String.class), response.getStatus());
                }

                String origJson = response.readEntity(String.class);

                // parse JSON data
                ObjectMapper mapper = new ObjectMapper();
                GitHubRepository repository = mapper.readValue(origJson, GitHubRepository.class);
                return new APIResult<GitHubRepository>(response.getStatus(), repository);
            }
            else
            {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
            }
        }
        catch (IOException e)
        {
            return new APIResult<GitHubRepository>(e.getMessage(), 500);
        }
    }

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
        if(increaseRequestCounter())
        {
            return Utilities.getResponse(uri);
        }
    }

    public static APIResult<GitHubIssue[]> getIssuesInRepository(String owner, String repository, int page, int per_page)
    {
        try {
            if(increaseRequestCounter())
            {
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
            else
            {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
            }
        }
        catch (IOException e)
        {
            return new APIResult<GitHubIssue[]>(e.getMessage(), 500);
        }
    }

    public static Response requestIssue(String owner, String repository, int issueNumber)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("repos/{owner}/{repo}/issues/{issueNumber}");

        URI uri = uriBuilder.build(owner, repository, issueNumber);

        if(increaseRequestCounter())
        {
            return Utilities.getResponse(uri);
        }
        else
        {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
        }
    }

    public static APIResult<GitHubIssue> getIssue(String owner, String repository, int issueNumber)
    {
        try {

            if(increaseRequestCounter())
            {
                Response response = requestIssue(owner, repository, issueNumber);

                if (response.getStatus() != 200 && response.getStatus() != 201) {
                    return new APIResult<GitHubIssue>(response.readEntity(String.class), response.getStatus());
                }

                String origJson = response.readEntity(String.class);

                // parse JSON data
                ObjectMapper mapper = new ObjectMapper();
                GitHubIssue issue = mapper.readValue(origJson, GitHubIssue.class);
                return new APIResult<GitHubIssue>(response.getStatus(), issue);
            }
            else
            {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
            }
        }
        catch (IOException e)
        {
            return new APIResult<GitHubIssue>(e.getMessage(), 500);
        }
    }

    public static Response requestIssue(int repositoryId, int issueNumber)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                        .path("repositories/{repositoryId}/issues/{issueNumber}");

        URI uri = uriBuilder.build(repositoryId, issueNumber);

        if(increaseRequestCounter())
        {
            return Utilities.getResponse(uri);
        }
        else
        {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
        }
    }

    public static APIResult<GitHubIssue> getIssue(int repositoryId, int issueNumber)
    {
        try {

            if(increaseRequestCounter())
            {
                Response response = requestIssue(repositoryId, issueNumber);

                if (response.getStatus() != 200 && response.getStatus() != 201) {
                    return new APIResult<GitHubIssue>(response.readEntity(String.class), response.getStatus());
                }

                String origJson = response.readEntity(String.class);

                // parse JSON data
                ObjectMapper mapper = new ObjectMapper();
                GitHubIssue issue = mapper.readValue(origJson, GitHubIssue.class);
                return new APIResult<GitHubIssue>(response.getStatus(), issue);
            }
            else
            {
             return new APIResult<GitHubIssue>("Rate Limit exceeded.", 403);
            }
        }
        catch (IOException e)
        {
            return new APIResult<GitHubIssue>(e.getMessage(), 500);
        }
    }
}
