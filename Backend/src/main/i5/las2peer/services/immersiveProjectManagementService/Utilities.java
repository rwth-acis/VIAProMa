package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import com.fasterxml.jackson.databind.ser.impl.SimpleFilterProvider;

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Invocation;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

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
    public static String unityCompatibleArray(Object obj) throws JsonProcessingException
    {
        return  unityCompatibleArray(obj, null);
    }

    public static String unityCompatibleArray(Object obj, SimpleFilterProvider filter) throws JsonProcessingException
    {
        ObjectMapper mapper = new ObjectMapper();
        if (filter != null)
        {
            mapper.setFilterProvider(filter);
        }
        ObjectWriter writer = mapper.writer().withRootName("array");
        return writer.writeValueAsString(obj);
    }

    public static Response GetResponse(String url)
    {
        Client client = ClientBuilder.newClient();
        WebTarget webTarget = client.target(url);
        Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
        Response response = invocationBuilder.get();
        return response;
    }
}
