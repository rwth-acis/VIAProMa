package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;

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
        ObjectMapper mapper = new ObjectMapper();
        ObjectWriter writer = mapper.writer().withRootName("array");
        return writer.writeValueAsString(obj);
    }
}
