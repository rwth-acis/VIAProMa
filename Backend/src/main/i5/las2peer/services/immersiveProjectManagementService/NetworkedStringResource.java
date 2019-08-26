package i5.las2peer.services.immersiveProjectManagementService;

import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.net.HttpURLConnection;
import java.util.Dictionary;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by bened on 26.08.2019.
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
@Path("/networkedStrings")
public class NetworkedStringResource {

    private static Map<Short, String> stringDictionary = new HashMap<>();
    private static short idPointer = 0;
    private final short maxId = 100;

    public NetworkedStringResource() {
    }

    @POST
    @Produces(MediaType.TEXT_PLAIN)
    @ApiOperation(
            value = "Registers a string entry with the given id",
            notes = "Returns the id of the string")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response registerString() {

        stringDictionary.put(idPointer, "");
        short id = idPointer;
        idPointer++;
        idPointer %= maxId;

        System.out.println("Registered entry " + id);
        System.out.println("Dictionary now has " + stringDictionary.size() + " entries");

        return Response.ok().entity(id).build();
    }

    @GET
    @Path("/{id}")
    @Produces(MediaType.TEXT_PLAIN)
    @ApiOperation(
            value = "Gets a string with the given id",
            notes = "Returns the string with the id")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response getString(@PathParam("id") short id) {
        System.out.println("Received get request for id " + id);
        if (stringDictionary.containsKey(id)) {
            return Response.ok().entity(stringDictionary.get(id)).build();
        } else {
            return Response.ok().entity("").build();
        }
    }

    @PUT
    @Path("/{id}")
    @Consumes( MediaType.APPLICATION_JSON )
    @Produces(MediaType.TEXT_PLAIN)
    @ApiOperation(
            value = "Sets a string with the given id",
            notes = "Returns the string with the id")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response setString(@PathParam("id") short id, String text) {
        System.out.println("Received put request for " + id + " and text " + text);
        System.out.println("Dictionary has " + stringDictionary.size() + " entries");
        if (text.equals("<<empty>>"))
        {
            text = "";
        }
        if (stringDictionary.containsKey(id)) {
            stringDictionary.put(id, text);
            return Response.ok().entity(stringDictionary.get(id)).build();
        } else {
            return Response.serverError().entity("The id for this string does not exist. It needs to be registered first.").build();
        }
    }

    @DELETE
    @Path("/{id}")
    @Consumes( MediaType.TEXT_PLAIN )
    @Produces(MediaType.TEXT_PLAIN)
    @ApiOperation(
            value = "Deletes the entry with the given id",
            notes = "Returns the string with the id")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response setString(@PathParam("id") short id) {
        if (stringDictionary.containsKey(id)) {
            stringDictionary.remove(id);
            System.out.println("Removed entry " + id);
            System.out.println("Dictionary now has " + stringDictionary.size() + " entries");
            return Response.ok().build();
        } else {
            return Response.serverError().entity("The id for this string does not exist.").build();
        }
    }
}
