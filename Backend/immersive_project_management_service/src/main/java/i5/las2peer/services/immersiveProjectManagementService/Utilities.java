package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import com.fasterxml.jackson.databind.ser.impl.SimpleFilterProvider;
import org.apache.commons.lang3.StringUtils;

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Invocation;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.UriBuilder;
import java.io.IOException;
import java.net.URI;

/**
 * Created by bened on 09.05.2019.
 */
public class Utilities {

    /**
     * Creates a JSON array which is compatible to Unity's JSON parser
     * @param obj The object to convert to a JSON string
     * @return Returns the JSON string which represents the given array
     * @throws JsonProcessingException Exception if the conversion to JSON failed
     */
    public static String toUnityCompatibleArray(Object obj) throws JsonProcessingException
    {
        return  toUnityCompatibleArray(obj, null);
    }

    public static String toUnityCompatibleArray(Object obj, SimpleFilterProvider filter) throws JsonProcessingException
    {
        ObjectMapper mapper = new ObjectMapper();
        if (filter != null)
        {
            mapper.setFilterProvider(filter);
        }
        ObjectWriter writer = mapper.writer().withRootName("array");
        return writer.writeValueAsString(obj);
    }

    public static <T> T fromUnityCompatibleArray(String json, Class<T> ref) throws IOException
    {
        ObjectMapper mapper = new ObjectMapper();
        JsonNode arrayNode = mapper.readTree(json).get("array");
        if (arrayNode.isArray())
        {
            return mapper.readValue(mapper.treeAsTokens(arrayNode), ref);
        }
        else
        {
            throw new IOException("provided JSON is not in the Unity-array format.");
        }
    }

    public static Response getResponse(String url)
    {
        Client client = ClientBuilder.newClient();
        WebTarget webTarget = client.target(url);
        Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
        Response response = invocationBuilder.get();
        return response;
    }

    public static Response getResponse(URI uri)
    {
        return getResponse(uri.toString());
    }

    public static String RemoveSpecialSymbols(String text)
    {
        if (text == null)
        {
            return null;
        }
        return text.replaceAll("[^a-zA-Z0-9\\s]", "");
    }

    public static void RemoveDuplicates()
    {

    }
}
