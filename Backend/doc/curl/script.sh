#!/bin/sh
URL='http://localhost:8080/example'
echo "curl test script for las2peer service"

echo "test authentication with test user alice"
curl -v -X GET $URL/validate --user alice:pwalice
echo 
echo "PRESS RETURN TO CONTINUE..."
read

echo "more curl commandlines..."


