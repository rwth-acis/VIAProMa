<p align="center">
  <img src="https://raw.githubusercontent.com/rwth-acis/las2peer/master/img/logo/bitmap/las2peer-logo-128x128.png" />
</p>
<h1 align="center">las2peer-Template-Project</h1>

![Java CI with Gradle](https://github.com/rwth-acis/las2peer-template-project/workflows/Java%20CI%20with%20Gradle/badge.svg?branch=master)
[![codecov](https://codecov.io/gh/rwth-acis/las2peer-template-project/branch/master/graph/badge.svg)](https://codecov.io/gh/rwth-acis/las2peer-template-project)
[![Dependencies](https://img.shields.io/librariesio/github/rwth-acis/las2peer-template-project)](https://libraries.io/github/rwth-acis/las2peer-template-project)

This project can be used as a starting point for your las2peer service development.
It contains everything needed to start las2peer service development, you do not need to add any dependencies manually.  

For documentation on the las2peer service API, please refer to the [wiki](https://github.com/rwth-acis/las2peer-Template-Project/wiki).

Please follow the instructions of this ReadMe to setup your basic service development environment.  

## Preparations

### Java

las2peer uses **Java 17**.


## Quick Setup of your Service Development Environment

*If you never used las2peer before, it is recommended that you first visit the
[Step by Step - First Service](https://github.com/rwth-acis/las2peer-Template-Project/wiki/Step-By-Step:-First-Service)
tutorial for a more detailed guidance on how to use this template.*  

Follow these four steps to setup your project:  
1. If you use Eclipse (for our guides we are using version 2020-12), import this project (as Gradle -> Existing Gradle Project). Please make sure, that Java 17 is available in Eclipse. During the import process, the .classpath files will be generated automatically.
2. The service source code can be found at `i5.las2peer.services.templateService.TemplateService`.  
(Optional: Change [gradle.properties](gradle.properties)
according to the service you want to build. Rename your build directory structure according to the names you gave.)
3. Compile your service with `./gradlew clean jar`. This will also build the service jar.  
4. Generate documentation, run your JUnit tests and generate service and user agent with `./gradlew clean build` (If this did not run check that the policy files are working correctly).  

The jar file with your service will be in "template_project/export/" and "service/" and the generated agent XML files in "etc/startup/".
You can find the JUnit reports in the folder "template_project/build/reports/tests/".  

If you decide to change the dependencies of your project, please make sure to refresh the Gradle project in Eclipse by right-clicking on your project and then choosing Gradle -> Refresh Gradle Project.
Also run "gradle cleanAll" to remove all previously added libraries.

## Next Steps

Please visit the [Wiki](https://github.com/rwth-acis/las2peer-Template-Project/wiki/) of this project.
There you will find guides and tutorials, information on las2peer concepts and further interesting las2peer knowledge.  
