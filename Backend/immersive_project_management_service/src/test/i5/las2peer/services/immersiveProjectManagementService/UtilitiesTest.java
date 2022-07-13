package i5.las2peer.services.immersiveProjectManagementService;

import org.junit.Assert;
import org.junit.Test;

/**
 * Created by bened on 07.07.2019.
 */
public class UtilitiesTest {

    @Test
    public void testToUnityArray()
    {
        try
        {
            final String expected = "{\"array\":[0,1,2,3,4]}";

            int[] array = {0, 1, 2, 3, 4};
            String json = Utilities.toUnityCompatibleArray(array);

            System.out.println("Result of 'testToUnityArray': " + json);
            Assert.assertEquals(expected, json.trim());
        }
        catch (Exception e)
        {
            e.printStackTrace();
            Assert.fail(e.toString());
        }
    }

    @Test
    public void testFromUnityArray()
    {
        try {
            final int[] expexted = {0, 1, 2, 3, 4};

            final String json = "{\"array\":[0,1,2,3,4]}";

            int[] result = Utilities.fromUnityCompatibleArray(json, int[].class);
            Assert.assertEquals(expexted.length, result.length);

            for (int i=0;i<expexted.length;i++)
            {
                Assert.assertEquals(expexted[i], result[i]);
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
            Assert.fail(e.toString());
        }
    }

    @Test
    public void testRemoveSpecialSymbols()
    {
        final String text = "Hello! ~World+ ?!#//\\";
        final String expected = "Hello World ";

        String result = Utilities.RemoveSpecialSymbols(text);
        System.out.println("Result of 'testRemoveSpecialSymbols': " + result);
        Assert.assertEquals(expected, result);
    }
}
