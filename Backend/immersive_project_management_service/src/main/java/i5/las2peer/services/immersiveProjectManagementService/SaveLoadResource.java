package i5.las2peer.services.immersiveProjectManagementService;

import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.File;
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
    @GET
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Get Projects",
            notes = "Returns the list of projects which are saved on the server")
    @ApiResponses(
            value = { @ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE") })
    public Response getProjects() {
            File saveDir = new File("saveData");
            File[] saveFiles = saveDir.listFiles((d, name) -> name.endsWith(".json"));
            String[] names = new String[saveFiles.length];
            for (int i=0;i<saveFiles.length;i++)
            {
                String fullName = saveFiles[i].getName();
                names[i] = fullName.substring(0, fullName.length() - 5); // remove .json ending
            }
            try {
                String result = Utilities.toUnityCompatibleArray(names);
                return Response.ok().entity(result).build();
            }
            catch (IOException e)
            {
                return Response.serverError().entity(e).build();
            }
    }


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