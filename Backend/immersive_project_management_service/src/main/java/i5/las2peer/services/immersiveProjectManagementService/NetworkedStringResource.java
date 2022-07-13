package i5.las2peer.services.immersiveProjectManagementService;

import io.swagger.annotations.*;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.net.HttpURLConnection;
import java.time.LocalDate;
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

    private static Map<Short, String> idToStringDictionary = new HashMap<>();
    private static Map<String, Short> stringToIdDictionary = new HashMap<>();
    private static Map<Short, LocalDate> timeStamps = new HashMap<>();
    private static short idPointer = 0;

    public NetworkedStringResource() {
    }

    @POST
    @Consumes(MediaType.TEXT_PLAIN)
    @Produces(MediaType.TEXT_PLAIN)
    @ApiOperation(
            value = "Registers a string entry with the given id",
            notes = "Returns the id of the string")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "REPLACE THIS WITH YOUR OK MESSAGE")})
    public Response registerString(String text) {

        if (stringToIdDictionary.containsKey(text))
        {
            short id = stringToIdDictionary.get(text);
            return Response.ok().entity(id).build();
        }

        short id = idPointer;
        idToStringDictionary.put(idPointer, text);
        stringToIdDictionary.put(text, idPointer);

        idPointer++;
        if (idPointer == -1)
        {
            idPointer++; // skip -1 because it is reserved for the empty string
        }

        System.out.println("Registered entry " + id + " with value " + text);
        System.out.println("Dictionary now has " + idToStringDictionary.size() + " entries");

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
        if (idToStringDictionary.containsKey(id)) {
            return Response.ok().entity(idToStringDictionary.get(id)).build();
        } else {
            return Response.ok().entity("").build(); // includes -1 case
        }
    }
}
