#!/bin/bash

# this scripts generates an user agent as xml file in order to upload it via the startup folder
# pls run the script form the root folder of your deployment, e. g. ./bin/start_UserAgentGenerator.sh

java -cp "lib/*" i5.las2peer.tools.UserAgentGenerator "$@"
