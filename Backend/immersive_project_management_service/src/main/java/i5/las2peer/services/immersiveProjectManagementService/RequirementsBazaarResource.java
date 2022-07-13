package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.CrossIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Category;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazContributors;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazProject;
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
                title = "Immersive ReqBazProject Management Service",
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
@Path("/requirementsBazaar")
public class RequirementsBazaarResource
{
    public RequirementsBazaarResource() throws  Exception
    {
    }

    @GET
    @Path("/categories/{categoryId}/requirements")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Get all Requirements in a Requirements Bazaar category",
            notes = "Returns the list requirements")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getReqBazRequirementsInCategory(@PathParam("categoryId") int categoryId,
                                                    @QueryParam("page") int page,
                                                    @QueryParam("per_page") int itemsPerPage,
                                                    @QueryParam("search") String searchFilter) {
        // if items per page is 0, it was probably not set
        // default the items per page to 5 in order to avoid errors when requesting items with a page size of 0
        if (itemsPerPage == 0)
        {
            itemsPerPage = 5;
        }
        APIResult<Requirement[]> res = RequirementsBazaarConnector.getRequirementsInCategory(categoryId, page, itemsPerPage, searchFilter);
        if (res.hasError())
        {
            return  Response.status(res.getCode()).entity(res.getErrorMessage()).build();
        }
        try {
            CrossIssue[] issues = CrossIssue.fromRequirements(res.getValue());
            String result = Utilities.toUnityCompatibleArray(issues);
            return Response.ok().entity(result).build();
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }

    @GET
    @Path("/projects")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Gets the list of all project names and their Ids",
            notes = "Returns the list of all project names and their IDs")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getReqBazProjectNames() {
        int itemsPerPage = 100;
        APIResult<ReqBazProject[]> res;
        ArrayList<ReqBazProject> allProjects = new ArrayList<>();

        int page = 0;

        // go over each page and collect all projects
        do {
            res = RequirementsBazaarConnector.getProjects(page, itemsPerPage);
            if (res.successful())
            {
                allProjects.addAll(Arrays.asList(res.getValue()));
                page++;
            }
        }
        while (res.successful() && res.getValue().length == itemsPerPage);

        try {
            if (res.hasError())
            {
                return  Response.status(res.getCode()).entity(res.getErrorMessage()).build();
            }
            String result = Utilities.toUnityCompatibleArray(allProjects);
            return  Response.ok().entity(result).build();
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }

    @GET
    @Path("/projects/{projectId}/categories")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Gets the short version of all categories in a project",
            notes = "Returns the list of requirements in a project (in a shortened version)")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getReqBazShortCategoriesInProject(@PathParam("projectId") int projectId) {
        int itemsPerPage = 100;
        APIResult<Category[]> res;
        ArrayList<Category> allCategories = new ArrayList<>();

        int page = 0;

        // go over each page and collect all projects
        do {
            res = RequirementsBazaarConnector.getCategoriesInProject(projectId);
            if (res.successful())
            {
                allCategories.addAll(Arrays.asList(res.getValue()));
                page++;
            }
        }
        while (res.successful() && res.getValue().length == itemsPerPage);

        try {
            if (res.hasError()) {
                return Response.status(res.getCode()).entity(res.getErrorMessage()).build();
            }
            // filter the properties of a category so that it only returns the name, id and projectId
            //SimpleFilterProvider filterProvider = new SimpleFilterProvider();
            //filterProvider.addFilter("shortFilter", SimpleBeanPropertyFilter.filterOutAllExcept("id", "name", "projectId"));
            String result = Utilities.toUnityCompatibleArray(allCategories);
            return Response.ok().entity(result).build();
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }

    @GET
    @Path("/requirements/{requirementId}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Gets a requirement",
            notes = "Returns the requirement with the given id")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getRequirement(@PathParam("requirementId") int requirementId) {
        try {
            APIResult<Requirement> res = RequirementsBazaarConnector.getRequirement(requirementId);
            if (res.successful())
            {
                CrossIssue issue = CrossIssue.fromRequirement(res.getValue()); // convert to CrossIssue
                // serialize to json
                ObjectMapper mapper = new ObjectMapper();
                ObjectWriter writer = mapper.writer();
                String result = writer.writeValueAsString(issue);
                return Response.ok().entity(result).build();
            }
            else
            {
                return Response.status(res.getCode()).entity(res.getErrorMessage()).build();
            }
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }

    @GET
    @Path("/requirements/{requirementId}/contributors")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Gets the contributors of a requirement",
            notes = "Returns contributors of the requirement with the given id")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getRequirementContributors(@PathParam("requirementId") int requirementId) {
        try {
            APIResult<ReqBazContributors> res = RequirementsBazaarConnector.getRequirementContributors(requirementId);
            if (res.successful())
            {
                ObjectMapper mapper = new ObjectMapper();
                ObjectWriter writer = mapper.writer();
                String result = writer.writeValueAsString(res.getValue());
                return Response.ok().entity(result).build();
            }
            else
            {
                return Response.status(res.getCode()).entity(res.getErrorMessage()).build();
            }
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }

    @GET
    @Path("/projects/{projectId}/requirements")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Gets the requirements in a project",
            notes = "Returns the requirements in a project")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getRequirementsInProject(@PathParam("projectId") int projectId,
                                             @QueryParam("page") int page,
                                             @QueryParam("per_page") int itemsPerPage,
                                             @QueryParam("search") String searchFilter) {
        try {
            if (itemsPerPage == 0)
            {
                itemsPerPage = 5;
            }
            APIResult<Requirement[]> res = RequirementsBazaarConnector.getRequirementsInProject(projectId, page, itemsPerPage, searchFilter);
            if (res.successful())
            {
                CrossIssue[] issues = CrossIssue.fromRequirements(res.getValue()); // convert to CrossIssues
                // serialize to json
                String result = Utilities.toUnityCompatibleArray(issues);
                return Response.ok().entity(result).build();
            }
            else
            {
                return Response.status(res.getCode()).entity(res.getErrorMessage()).build();
            }
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e.getMessage()).build();
        }
    }
}