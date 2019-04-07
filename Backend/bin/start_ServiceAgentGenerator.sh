#!/bin/bash

# this scripts generates a xml file for the specified ServiceClass with the desired ServicePass
# pls run the script form the root folder of your deployment, e. g. ./bin/start_ServiceAgentGenerator.sh

java -cp "lib/*" i5.las2peer.tools.ServiceAgentGenerator "$@"
