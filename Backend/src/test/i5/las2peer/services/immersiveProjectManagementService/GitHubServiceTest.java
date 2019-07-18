package i5.las2peer.services.immersiveProjectManagementService;

import i5.las2peer.api.p2p.ServiceNameVersion;
import i5.las2peer.connectors.webConnector.WebConnector;
import i5.las2peer.p2p.LocalNode;
import i5.las2peer.p2p.LocalNodeManager;
import i5.las2peer.security.UserAgentImpl;
import i5.las2peer.testing.MockAgentFactory;
import org.junit.After;
import org.junit.Before;

import java.io.ByteArrayOutputStream;
import java.io.PrintStream;

/**
 * Created by bened on 18.07.2019.
 */
public class GitHubServiceTest {

    private static final String testPass = "adamspass";
    private static final String mainPath = "resources/";
    private static LocalNode node;
    private static WebConnector connector;
    private static ByteArrayOutputStream logStream;
    private static UserAgentImpl testAgent;

    /**
     * Called before a test starts.
     * <p>
     * Sets up the node, initializes connector and adds user agent that can be used throughout the test.
     *
     * @throws Exception
     */
    @Before
    public void startServer() throws Exception {
        // start node
        node = new LocalNodeManager().newNode();
        node.launch();

        // add agent to node
        testAgent = MockAgentFactory.getAdam();
        testAgent.unlock(testPass); // agents must be unlocked in order to be stored
        node.storeAgent(testAgent);

        // start service
        // during testing, the specified service version does not matter
        node.startService(new ServiceNameVersion(ImmersiveProjectManagementService.class.getName(), "1.0.0"), "a pass");

        // start connector
        connector = new WebConnector(true, 0, false, 0); // port 0 means use system defined port
        logStream = new ByteArrayOutputStream();
        connector.setLogStream(new PrintStream(logStream));
        connector.start(node);
    }

    /**
     * Called after the test has finished. Shuts down the server and prints out the connector log file for reference.
     *
     * @throws Exception
     */
    @After
    public void shutDownServer() throws Exception {
        if (connector != null) {
            connector.stop();
            connector = null;
        }
        if (node != null) {
            node.shutDown();
            node = null;
        }
        if (logStream != null) {
            System.out.println("Connector-Log:");
            System.out.println("--------------");
            System.out.println(logStream.toString());
            logStream = null;
        }
    }
}
