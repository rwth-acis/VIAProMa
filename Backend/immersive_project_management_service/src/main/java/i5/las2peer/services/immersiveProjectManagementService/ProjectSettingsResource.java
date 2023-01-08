package i5.las2peer.services.immersiveProjectManagementService;

import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.*;
import java.net.HttpURLConnection;
import java.nio.file.Files;
import java.nio.file.Paths;

/**
 * ProjectSettingsResource
 * <p>
 * This is the backend service which saves and returns project specific settings
 */
// TODO Adjust the following configuration
@Api(value = "Project Settings", description = "")
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
@Path("/project-settings")
public class ProjectSettingsResource {

    @POST
    @Path("/{project}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Sets the analytics settings",
            notes = "")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_CREATED,
                    message = "Saved settings")})
    public Response setSettings(@PathParam("project") String project, String requestBody) {
        try (PrintWriter out = new PrintWriter(getProjectDirectory(project).getPath() + "/settings.json")) {
            out.println(requestBody);
        } catch (FileNotFoundException e) {
            return Response.serverError().entity(e.getMessage()).build();
        }
        return Response.ok().entity(requestBody).build();
    }

    @GET
    @Path("/{project}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Sets the analytics settings",
            notes = "")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "OK")})
    public Response getSettings(@PathParam("project") String project, String text) {
        String result;
        try {
            result = new String(Files.readAllBytes(Paths.get(getProjectDirectory(project).getPath() + "/settings.json")));
        } catch (Exception e) {
            return Response.serverError().entity(e.getMessage()).build();
        }
        return Response.ok().entity(result).build();
    }
    File getProjectDirectory(String project) {
        File parentDir = new File("projects/" + project + "/");
        if (!parentDir.exists())
            parentDir.mkdirs();
        return parentDir;
    }


}


