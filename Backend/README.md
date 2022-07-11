<h1 align="center">Immersive Project Management Service</h1>

![Java CI with Gradle](https://github.com/rwth-acis/las2peer-template-project/workflows/Java%20CI%20with%20Gradle/badge.svg?branch=master)
[![codecov](https://codecov.io/gh/rwth-acis/las2peer-template-project/branch/master/graph/badge.svg)](https://codecov.io/gh/rwth-acis/las2peer-template-project)
[![Dependencies](https://img.shields.io/librariesio/github/rwth-acis/las2peer-template-project)](https://libraries.io/github/rwth-acis/las2peer-template-project)

The Immersive Project Management Service is the backend of VIAProMa. It helps the frontend to communicate with other services, e.g. [Requirments Bazaar](https://requirements-bazaar.org/projects) and GitHub. It is based on the [las2peer-Template-Project](https://github.com/rwth-acis/las2peer-template-project). For documentation on the las2peer service API, please refer to the [wiki](https://github.com/rwth-acis/las2peer-Template-Project/wiki).

Please follow the instructions of this ReadMe to setup the backend service.  

## Preparations

### Java

The backend of VIAProMa uses **Java 17**.


## Quick Setup of the Immersive Project Management Service

Follow these four steps to setup the backend service:  
1. If you use Eclipse (for our guides we are using version 2020-12), import this project (as Gradle -> Existing Gradle Project). Please make sure, that Java 17 is available in Eclipse. During the import process, the .classpath files will be generated automatically.
2. The service source code can be found at "/immersive_project_management_service/src/main/java/i5/las2peer/services/immersiveProjectManagementService".  
3. Compile the service with `./gradlew clean jar`. This will also build the service jar.  
4. Generate documentation, run JUnit tests and generate the service and user agent with `./gradlew clean build` (If this did not run check that the policy files are working correctly).  

The jar file with the service will be in "immersive_project_management_service/export/", and "service/" and the generated agent XML files in "etc/startup/".  

## Next Steps

### Run the backend

After the build, execute the corresponding “start_network” script in the “bin” folder of the backend. It contains two scripts “start_network.bat” and “start_network.sh”. On Windows, execute the “start_network.bat” file. On Linux or Mac, first go back to the backend folder and execute ./bin/start_network.sh from there.

### Run the frontend

After starting the backend, you can now run the frontend. If the backend is running successfully, you can see a green light besides the "Backend Server" label when you open the "Server Connection" on the top left of the main menu.


