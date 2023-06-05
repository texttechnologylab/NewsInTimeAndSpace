#
# Build stage
#
FROM maven AS build
COPY src /home/app/src
COPY pom.xml /home/app
RUN mvn -f /home/app/pom.xml clean package

#
#  Package stage
#
FROM tomcat:9
COPY --from=build /home/app/target/*.war /usr/local/tomcat/webapps/news.war
# tmp: insert config
# ADD MongoDBConfig.cfg /usr/local/tomcat/server/


