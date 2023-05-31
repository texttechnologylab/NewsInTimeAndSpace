/*
* Events
*
* @date    16.02.2023
*
* @author  Jasper Hustedt, Timo Lüttig
* @version 1.0
*
* Provides methods used by all Api calls requesting events
*
*/
package org.texttechnologylab.timemachines.functions;

import com.mongodb.client.model.Filters;
import com.mongodb.client.model.Projections;
import org.bson.Document;
import org.bson.conversions.Bson;
import org.json.simple.JSONObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.texttechnologylab.timemachines.App;
import org.texttechnologylab.timemachines.mongodb.MongoDBConnectionHandler;
import spark.Request;
import spark.Response;

import java.util.ArrayList;

import static com.mongodb.client.model.Sorts.descending;

public class Events {
	private Events() {
	}

	private static Logger logger = LoggerFactory.getLogger(Events.class);

	/**
	 * Method to request an event from a database via its ID.
	 * @param req
	 * @param res
	 * @return event as JSONObject with id from request parameters
	 */
	public static JSONObject getEventByID(Request req, Response res) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		String id = req.params("id");

		JSONObject obj = new JSONObject();
		Bson filter = Filters.eq("GLOBALEVENTID", id);
		Bson projection = Projections.fields(Projections.excludeId(), Projections.include());
		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter)
				.projection(projection).into(list);
		list.forEach(a -> a.replace("Date", Helper.formatDate(a.getDate("Date"))));
		obj.put("results", list);
		logger.info(obj.toJSONString());
		return (obj);
	}

	/**
	 * Methods to request events from database via its IDs.
	 * @param req
	 * @param res
	 * @return events as JSONObjects with id from request parameters
	 */
	public static JSONObject getEventsByIDs(Request req, Response res) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		String[] ids = req.queryParamsValues("id");

		JSONObject obj = new JSONObject();
		Bson filter = Filters.in("GLOBALEVENTID", ids);
		Bson projection = Projections.fields(Projections.excludeId(), Projections.exclude("Themes"));
		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter)
				.projection(projection).into(list);
		list.forEach(a -> a.replace("Date", Helper.formatDate(a.getDate("Date"))));
		obj.put("results", list);
		logger.info(obj.toJSONString());
		return (obj);
	}

	/**
	 * Method to request events from a database via filters.
	 * @param req
	 * @param res
	 * @return all events matching the filter from req
	 */
	public static JSONObject getEvents(Request req, Response res) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		Bson filter = Helper.createBsonFilter(req);

		String limit = req.queryParams("limit");
		int limitInt = 0;
		if (limit != null)
			limitInt = Integer.parseInt(limit);

		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter)
				.sort(descending("Date")).projection(Projections.excludeId()).limit(limitInt).into(list);

		list.forEach(a -> a.replace("Date", Helper.formatDate(a.getDate("Date"))));
		JSONObject obj = new JSONObject();
		obj.put("results", list);

		return (obj);
	}

	/**
	 * Method to request events from a database via filters.
	 * @param req
	 * @param res
	 * @return all event Ids matching the filter from req
	 */
	public static JSONObject getEventIDs(Request req, Response res) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		Bson filter = Helper.createBsonFilter(req);

		String limit = req.queryParams("limit");
		int limitInt = 0;
		if (limit != null)
			limitInt = Integer.parseInt(limit);

		Bson projection;
		projection = Projections.fields(Projections.excludeId(), Projections.include("GLOBALEVENTID"));

		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter)
				.sort(descending("Date")).limit(limitInt).projection(projection).into(list);

		JSONObject obj = new JSONObject();
		obj.put("results", list);

		return (obj);
	}
}
