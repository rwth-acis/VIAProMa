package i5.las2peer.services.immersiveProjectManagementService;

import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.PrintWriter;
import java.net.HttpURLConnection;
import java.nio.file.Files;
import java.nio.file.Paths;

// TODO Adjust the following configuration
@Api(value="Save Load", description = "Save Data resources")
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
@Path("/saveData")
public class SaveLoadResource
{
    @POST
    @Path("/{saveName}")
    @Consumes(MediaType.APPLICATION_JSON)
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Saves the data",
            notes = "Returns whether the operation was successful")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response saveData(@PathParam("saveName") String saveName, String text) {

        saveName = saveName.replaceAll("[^a-zA-Z0-9]+","");

        try (PrintWriter out = new PrintWriter("saveData/" + saveName + ".json"))
        {
            out.println(text);
        }
        catch (FileNotFoundException e)
        {
            System.out.println(e);
        }

        return Response.ok().build();
    }

    @GET
    @Path("/{saveName}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Loads the data file with the given name",
            notes = "Returns the content of the save file"
    )
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response loadData (@PathParam("saveName") String saveName)
    {
        try
        {
            String content = new String(Files.readAllBytes(Paths.get("saveData/" + saveName + ".json")));
            return Response.ok().entity(content).build();
        }
        catch (FileNotFoundException e)
        {
            return Response.ok().entity("{ }").build();
        }
        catch (IOException e)
        {
            return Response.serverError().entity(e).build();
        }
    }
}