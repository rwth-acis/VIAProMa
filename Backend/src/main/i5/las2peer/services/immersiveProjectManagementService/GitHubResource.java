package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.CrossIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub.GitHubIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Category;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazContributors;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Project;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;
import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.util.ArrayList;
import java.util.Arrays;

/**
 * RequirementsBazaarResource
 *
 * This is the backend service which connects to the data source Requirements Bazaar
 *
 *
 *
 */
// TODO Adjust the following configuration
@Api(value="Requirements Bazaar", description = "Requirements Bazaar resources")
@SwaggerDefinition(
        info = @Info(
                title = "Immersive Project Management Service",
                version = "1.0.0",
                description = "Backend service of the immersive analytics project management framework",
                termsOfService = "",
                contact = @Contact(
                        name = "Benedikt Hensen",
                        url = "",
                        email = ""),
                license = @License(
                        name = "MIT",
                        url = "")))
@Path("/gitHub")
public class GitHubResource
{
    public GitHubResource() throws  Exception
    {
    }

    @GET
    @Path("/repos/{owner}/{repositoryName}/issues")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Get all issues in a GitHub repository",
            notes = "Returns the list issues")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getIssuesInRepository(@PathParam("owner") String owner,
                                                    @PathParam("repositoryName") String repositoryName,
                                                    @QueryParam("page") int page,
                                                    @QueryParam("per_page") int itemsPerPage) {
        // if items per page is 0, it was probably not set
        // default the items per page to 5 in order to avoid errors when requesting items with a page size of 0
        if (itemsPerPage == 0)
        {
            itemsPerPage = 5;
        }
        APIResult<GitHubIssue[]> res = GitHubConnector.getIssuesInRepository(owner, repositoryName, page, itemsPerPage);
        if (res.hasError())
        {
            return  Response.status(res.getCode()).entity(res.getErrorMessage()).build();
        }
        try {
            CrossIssue[] issues = CrossIssue.fromGitHubIssues(res.getValue(), owner, repositoryName);
            String result = Utilities.toUnityCompatibleArray(issues);
            return Response.ok().entity(result).build();
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }

    @GET
    @Path("/repos/{owner}/{repositoryName}/issues/{issueNumber}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Get an issue in a GitHub repository by its number",
            notes = "Returns the issue")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getIssue(@PathParam("owner") String owner,
                                          @PathParam("repositoryName") String repositoryName,
                                          @PathParam("issueNumber") int issueNumber) {

        APIResult<GitHubIssue> res = GitHubConnector.getIssue(owner, repositoryName, issueNumber);
        if (res.hasError())
        {
            return  Response.status(res.getCode()).entity(res.getErrorMessage()).build();
        }
        try {
            CrossIssue issues = CrossIssue.fromGitHubIssue(res.getValue(), owner, repositoryName);
            String result = Utilities.toUnityCompatibleArray(issues);
            return Response.ok().entity(result).build();
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }
}
