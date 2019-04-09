package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import i5.las2peer.restMapper.RESTService;
import i5.las2peer.restMapper.annotations.ServicePath;
import io.swagger.annotations.*;
import net.minidev.json.JSONArray;

import javax.ws.rs.*;
import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Invocation;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.IOException;
import java.net.HttpURLConnection;

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
			WebTarget webTarget = client.target("https://api.github.com/repos/rwth-acis/RequirementsBazaar/stats/punch_card");
			Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
			Response response = invocationBuilder.get();
			String origJson = response.readEntity(String.class);

			ObjectMapper mapper = new ObjectMapper();
			int[][] commitTimeData = mapper.readValue(origJson, int[][].class);

			String result = unityCompatibleArray(commitTimeData);

			return Response.ok().entity(result).build();
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

	// TODO your own service methods, e. g. for RMI

	private String unityCompatibleArray(Object obj) throws JsonProcessingException
	{
		ObjectMapper mapper = new ObjectMapper();
		ObjectWriter writer = mapper.writer().withRootName("array");
		return writer.writeValueAsString(obj);
	}

}
