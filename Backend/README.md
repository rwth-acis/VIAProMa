![las2peer](https://rwth-acis.github.io/las2peer/logo/vector/las2peer-logo.svg)

# las2peer-Template-Project

[![Build Status](https://travis-ci.org/rwth-acis/las2peer-Template-Project.svg?branch=master)](https://travis-ci.org/rwth-acis/las2peer-Template-Project)

This project can be used as a starting point for your las2peer service development.
It contains everything needed to start las2peer service development, you do not need to add any dependencies manually.  

For documentation on the las2peer service API, please refer to the [wiki](https://github.com/rwth-acis/las2peer-Template-Project/wiki).

Please follow the instructions of this ReadMe to setup your basic service development environment.  


## 1. Enable Strong Encryption

This step is **not** required with the following Java versions:
* any **Java 9**, **Java 8u162** or later, **Java 7u181** or later, **Java 6u191** or later

If you use an older Oracle Java version, you have to enable strong encryption for this software.

Please put the files to '[...]/lib/security/' of your java runtime installation (replacing the existing files).

The policy files can be downloaded via Oracle: [JCE for Java 8](http://www.oracle.com/technetwork/java/javase/downloads/jce8-download-2133166.html "JCE-8")


## 2. Quick Setup of your Service Development Environment

*If you never used las2peer before, it is recommended that you first visit the
[Step by Step - First Service](https://github.com/rwth-acis/las2peer-Template-Project/wiki/Step-By-Step:-First-Service)
tutorial for a more detailed guidance on how to use this template.*  

Follow these five steps to setup your project:  
1. If you use Eclipse, import this project or just create a new project in the same folder.  
2. Run "ant get_deps" once to pull all dependencies (You can skip this but Eclipse will complain about missing libraries until you build the first time).  
3. The service source code can be found at "i5.las2peer.services.servicePackage.TemplateService".  
(3.5 Optional: Change "etc/ant_configuration/service.properties" and "etc/ant_configuration/user.properties"
according to the service you want to build. Rename your build directory structure according to the names you gave in 2.,
you have to also correct the package declaration and the 'testTemplateService' constant in your source code files.)  
4. Compile your service with "ant jar" or just "ant" (default target). This will also build the service jar.  
5. Generate documentation, run your JUnit tests and generate service and user agent with "ant all" (If this did not run check that the policy files are working correctly).  

The jar file with your service will be in "export/" and "service/" and the generated agent XML files in "etc/startup/".
You can find the JUnit reports in the folder "export/test_reports/".  

If you decide to change the dependencies of your project, please make sure to run "ant clean_all" to remove all previously
added libraries first.  


## 3. Next Steps

Please visit the [Wiki](https://github.com/rwth-acis/las2peer-Template-Project/wiki/) of this project.
There you will find guides and tutorials, information on las2peer concepts and further interesting las2peer knowledge.  
