/*
* App
*
* @date    04.01.2023
*
* @author  Jasper Husted, Timo Lüttig
* @version 1.0
*
* Provides starting point for Spark application
*/
package org.texttechnologylab.timemachines;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.texttechnologylab.timemachines.functions.*;

import org.texttechnologylab.timemachines.gdelt.CsvDownloaderTask;
import org.texttechnologylab.timemachines.mongodb.MongoDBConnectionHandler;

import com.mongodb.client.model.IndexOptions;

import org.bson.Document;
import spark.Request;
import spark.Response;
import spark.servlet.SparkApplication;

import java.util.*;

import static spark.Spark.*;

public class App implements SparkApplication {

	public static String collection = "events";

	private static Logger logger = LoggerFactory.getLogger(App.class);

	/**
	 * Main methond to start application without webserver.
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		App app = new App();
		app.init();
	}

	/**
	 * Implement SparkApplication and init to run in tomcat Server.
	 */
	@Override
	public void init() {
		logger.info("starting server...");

		// configure MongoDB
		MongoDBConnectionHandler.getInstance().database.getCollection(collection)
				.createIndex(new Document("Location", "2dsphere"));
		MongoDBConnectionHandler.getInstance().database.getCollection(collection)
				.createIndex(new Document("Actors.Location", "2dsphere"));
		MongoDBConnectionHandler.getInstance().database.getCollection(collection)
				.createIndex(new Document("GLOBALEVENTID", 1), new IndexOptions().unique(true));
		MongoDBConnectionHandler.getInstance().database.getCollection(collection).createIndex(new Document("date", 1));

		// start downloader periodically
		CsvDownloaderTask eventDownloader = new CsvDownloaderTask();
		new Timer().schedule(eventDownloader, 0, 900000);

		// set up generell spark response
		after((request, response) -> {
			// allow all origins (SwaggerUi from different URI)
			response.header("Access-Control-Allow-Origin", "*");
			// all answers should be in json format
			response.type("application/json");
		});

		// root response
		get("/", (Request req, Response res) -> {
			return "{\"success\":\"true\"}";
		});

		path("/events", () -> {
			get("", Events::getEvents); // Filter, and return whole Events
			get("/ids", Events::getEventIDs); // Filter, and return Event IDs

		    	get("/event/:id", Events::getEventByID); // Get whole Event by ID
			get("/multiple", Events::getEventsByIDs); // Get whole Events by IDs

		});

		path("/groups", () -> {
			get("/country", Groups::getGroupsCountryAll);
			get("/country/ids", Groups::getGroupsCountryId);
		    	get("/country/test", Groups::testGroups);
			get("/region", Groups::getGroupsRegionAll);
			get("/region/ids", Groups::getGroupsRegionId);
			get("/city", Groups::getGroupsCityAll);
			get("/city/ids", Groups::getGroupsCityId);
		});

		path("/extra", () -> {
			get("/types", Extras::getTypes);
			get("/actors", Extras::getActors);
		});

		path("/update", () -> {
			put("/:date", Update::donwloadDate);
		});

		logger.info("finished init()");
	}
}
