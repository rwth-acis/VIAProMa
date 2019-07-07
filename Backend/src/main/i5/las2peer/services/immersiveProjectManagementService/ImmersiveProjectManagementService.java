package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import i5.las2peer.restMapper.RESTService;
import i5.las2peer.restMapper.annotations.ServicePath;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.GitHubPunchCardHour;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel.CrossIssue;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Category;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazContributors;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Project;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;
import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Invocation;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/**
 * ImmersiveProjectManagementService
 * 
 * This is the backend service for the immersive analytics framework for project management.
 * 
 *
 * 
 */
// TODO Adjust the following configuration
@Api
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
@ServicePath("/resources")
public class ImmersiveProjectManagementService extends RESTService {


	/**
	 * Template of a get function.
	 * 
	 * @return Returns an HTTP response with the username as string content.
	 */
	@GET
	@Path("/githubPunchCard/{owner}/{repo}")
	@Produces(MediaType.APPLICATION_JSON)
	@ApiOperation(
			value = "GitHub Punch Card",
			notes = "Returns punch card data by GitHub, stating how many commits were made during each hour on each day")
	@ApiResponses(
			value = { @ApiResponse(
					code = HttpURLConnection.HTTP_OK,
					message = "REPLACE THIS WITH YOUR OK MESSAGE") })
	public Response getPunchCard(@PathParam("owner") String owner, @PathParam("repo") String repositoryName) {

		try {
			Client client = ClientBuilder.newClient();
			WebTarget webTarget = client.target("https://api.github.com/repos/" + owner + "/" + repositoryName + "/stats/punch_card");
			Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
			Response response = invocationBuilder.get();
			String origJson = response.readEntity(String.class);

			// parse JSON data
			ObjectMapper mapper = new ObjectMapper();
			int[][] commitTimeData = mapper.readValue(origJson, int[][].class);

			// transform 2D array to special data type

			List<GitHubPunchCardHour> entries = new ArrayList<GitHubPunchCardHour>();

			for (int i=0;i<commitTimeData.length;i++)
			{
				if (commitTimeData[i].length != 3)
				{
					return Response.serverError().entity("received malformed GitHub response (not 3 fields in one entry)").build();
				}
				GitHubPunchCardHour entry = new GitHubPunchCardHour();
				for (int j=0;j<commitTimeData[i].length;j++)
				{
					if (j==0)
					{
						if (commitTimeData[i][j] < 0 || commitTimeData[i][j] > 6)
						{
							return Response.serverError().entity("received malformed GitHub response (days out of range)").build();
						}
						entry.day = commitTimeData[i][j];
					}
					else if (j==1)
					{
						if (commitTimeData[i][j] < 0 || commitTimeData[i][j] > 23)
						{
							return Response.serverError().entity("received malformed GitHub response (hours out of range)").build();
						}
						entry.hour = commitTimeData[i][j];
					}
					else if (j==2)
					{
						entry.numberOfCommits = commitTimeData[i][j];
						// entry is finished that this point => add it to the list
						entries.add(entry);
					}
				}
			}

			// pack resulting array in a Unity-compatible object
			String result = Utilities.unityCompatibleArray(entries);

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
			value = "Get Projects",
			notes = "Returns the list of projects which are saved on the server")
	@ApiResponses(
			value = { @ApiResponse(
					code = HttpURLConnection.HTTP_OK,
					message = "REPLACE THIS WITH YOUR OK MESSAGE") })
	public Response getProjects() {
		String[] projects = new String[64];
		for (int i=0;i<64;i++)
		{
			projects[i] = "Project " + (i+1);
		}
		try {
			String result = Utilities.unityCompatibleArray(projects);
			return  Response.ok().entity(result).build();
		}
		catch (IOException e)
		{
			return Response.serverError().entity(e.getMessage()).build();
		}
	}


	/**
	 * Template of a post function.
	 * 
	 * @param myInput The post input the user will provide.
	 * @return Returns an HTTP response with plain text string content derived from the path input param.
	 */
	@POST
	@Path("/post/{input}")
	@Produces(MediaType.TEXT_PLAIN)
	@ApiResponses(
			value = { @ApiResponse(
					code = HttpURLConnection.HTTP_OK,
					message = "REPLACE THIS WITH YOUR OK MESSAGE") })
	@ApiOperation(
			value = "REPLACE THIS WITH AN APPROPRIATE FUNCTION NAME",
			notes = "Example method that returns a phrase containing the received input.")
	public Response postTemplate(@PathParam("input") String myInput) {
		String returnString = "";
		returnString += "Input " + myInput;
		return Response.ok().entity(returnString).build();
	}

	@GET
	@Path("/ping")
	@Produces(MediaType.TEXT_PLAIN)
	@ApiOperation(
			value = "Check if the server is online",
			notes = "Returns a string")
	@ApiResponses(
			value = { @ApiResponse(
					code = HttpURLConnection.HTTP_OK,
					message = "REPLACE THIS WITH YOUR OK MESSAGE") })
	public Response ping() {
		return Response.ok().entity("pong").build();
	}

	// region RequirementsBazaar

	@GET
	@Path("/requirementsBazaar/categories/{categoryId}/requirements")
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
	@QueryParam("items_per_page") int itemsPerPage) {
		// if items per page is 0, it was probably not set
		// default the items per page to 5 in order to avoid errors when requesting items with a page size of 0
		if (itemsPerPage == 0)
		{
			itemsPerPage = 5;
		}
		APIResult<Requirement[]> res = RequirementsBazaarAdapter.GetRequirementsInCategory(categoryId, page, itemsPerPage);
		if (res.hasError())
		{
			return  Response.status(res.getCode()).entity(res.getErrorMessage()).build();
		}
		try {
			String result = Utilities.unityCompatibleArray(res.getValue());
			return Response.ok().entity(result).build();
		}
		catch (IOException e)
		{
			return Response.serverError().entity(e.getMessage()).build();
		}
	}

	@GET
	@Path("/requirementsBazaar/projects")
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
		APIResult<Project[]> res;
		ArrayList<Project> allProjects = new ArrayList<>();

		int page = 0;

		// go over each page and collect all projects
		do {
			res = RequirementsBazaarAdapter.GetProjects(page, itemsPerPage);
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
			String result = Utilities.unityCompatibleArray(allProjects);
			return  Response.ok().entity(result).build();
		}
		catch (IOException e)
		{
			return Response.serverError().entity(e.getMessage()).build();
		}
	}

	@GET
	@Path("/requirementsBazaar/projects/{projectId}/categories")
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
			res = RequirementsBazaarAdapter.GetCategoriesInProject(projectId);
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
			String result = Utilities.unityCompatibleArray(allCategories);
			return Response.ok().entity(result).build();
		}
		catch (IOException e)
		{
			return Response.serverError().entity(e.getMessage()).build();
		}
	}

	@GET
	@Path("/requirementsBazaar/requirements/{requirementId}")
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
			APIResult<Requirement> res = RequirementsBazaarAdapter.GetRequirement(requirementId);
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
	@Path("/requirementsBazaar/requirements/{requirementId}/contributors")
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
			APIResult<ReqBazContributors> res = RequirementsBazaarAdapter.GetRequirementContributors(requirementId);
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


	// endregion

}
