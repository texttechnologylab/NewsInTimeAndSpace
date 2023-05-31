/*
* MongoDBConnectionHandler
*
* @date    28.11.2023
*
* @author  Jasper Husted, Timo Lüttig
* @version 1.0
*
* Handles the Database connection. 
* (Part of this class a copied from an older project)
*/
package org.texttechnologylab.timemachines.mongodb;

import com.mongodb.MongoClient;
import com.mongodb.MongoClientOptions;
import com.mongodb.MongoCredential;
import com.mongodb.ServerAddress;
import com.mongodb.client.MongoDatabase;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.texttechnologylab.timemachines.functions.Helper;

import java.util.Properties;

public class MongoDBConnectionHandler {

    private Logger logger = LoggerFactory.getLogger(MongoDBConnectionHandler.class);

    private static MongoDBConnectionHandler instance = null;
    private MongoCredential credential;
    private MongoClient mongoClient;
    public MongoDatabase database;

    /**
     * Constructs ConnectionHandler with given properties file
     * 
     * @param properties
     */
    private MongoDBConnectionHandler(String properties) {
        connectDB(properties);
    }

    /**
     * Constructs ConnectionHandler with pre-defined properties file
     */
    private MongoDBConnectionHandler() {
        connectDB("server/MongoDBConfig.cfg");
    }

    /**
     * creates a MongoDBConnections on first call, following calls return the same
     * instance
     * 
     * @return MongoDBConnectionHandler Instance
     */
    public static MongoDBConnectionHandler getInstance() {
        if (instance == null) {
            instance = new MongoDBConnectionHandler();
        }
        return instance;
    }

    /**
     * creates a MongoDBConnections on first call, following calls return the same
     * instance
     * 
     * @param properties file to use for MongoDB Connections
     * @return MongoDBConnectionHandler Instance
     */
    public static MongoDBConnectionHandler getInstance(String properties) {
        if (instance == null) {
            instance = new MongoDBConnectionHandler(properties);
        }
        return instance;
    }

    /**
     * establishes a MongoDBConnetion with details from file at fileName
     * 
     * @param fileName connection details in file
     */
    private void connectDB(String fileName) {
        // Cfg einlesen
        Properties prop = Helper.readProperties(fileName);
        try {
            this.credential = MongoCredential.createCredential(prop.getProperty("remote_user"),
                    prop.getProperty("remote_database"), (prop.getProperty("remote_password")).toCharArray());
            this.mongoClient = new MongoClient(
                    new ServerAddress(prop.getProperty("remote_host"),
                            Integer.parseInt(prop.getProperty("remote_port"))),
                    credential, MongoClientOptions.builder().build());
            this.database = mongoClient.getDatabase(prop.getProperty("remote_database"));

        } catch (Exception e) {
            logger.error("Verbindung konnte nicht hergestellt werden, Verbindungsangaben falsch?", e);
            System.exit(0);
        }

    }
}
