/*
  * Extras
  *
  * @date    16.02.2023
  *
  * @author Jasper Hustedt, Timo Lüttig
  * @version 1.0
  *
  * Provides methods used by all Api calls requesting extras
  *
  */
package org.texttechnologylab.timemachines.functions;

import com.mongodb.client.FindIterable;
import com.mongodb.client.model.Filters;
import com.mongodb.client.model.Projections;
import org.bson.Document;
import org.bson.conversions.Bson;
import org.json.simple.JSONObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.texttechnologylab.timemachines.App;
import org.texttechnologylab.timemachines.mongodb.MongoDBConnectionHandler;
import org.texttechnologylab.timemachines.cameo.CameoCodeTranslator;
import spark.Request;
import spark.Response;

import java.util.*;

public class Extras {
	private Extras() {
	}

	private static Logger logger = LoggerFactory.getLogger(Extras.class);

	// nur Event Typen und Akteure abfragen

	/**
	 * Method to request events with only their actors field from a server via filters.
	 * @param req
	 * @param res
	 * @return Json structure containing all actors in events with the given filters
	 *         from req
	 */
	public static JSONObject getActors(Request req, Response res) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		Bson filter = Helper.createBsonFilter(req);

		String limit = req.queryParams("limit");
		int limitInt = 0;
		if (limit != null)
			limitInt = Integer.parseInt(limit);

		Bson projections = Projections.include("Actors");
		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter)
				.projection(projections).limit(limitInt).into(list);

		logger.info("Events amount: " + list.size());
		JSONObject obj = new JSONObject();
		obj.put("results", countActors(list));

		return (obj);
	}

	/**
	 * Method to request events with only their type field from a server via filters.
	 * @param req
	 * @param res
	 * @return Json structure containing all types in events with the given filters
	 *         from req
	 */
	public static JSONObject getTypes(Request req, Response res) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		Bson filter = Helper.createBsonFilter(req);

		String limit = req.queryParams("limit");
		int limitInt = 0;
		if (limit != null)
			limitInt = Integer.parseInt(limit);

		Bson projections = Projections.include("Type");
		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter)
				.projection(projections).limit(limitInt).into(list);

		String typeFilter = req.queryParams("type_filter");
		if (typeFilter == null)
			typeFilter = "EXACT";
		System.out.println("Events amount: " + list.size());
		JSONObject obj = new JSONObject();
		obj.put("results", countTypes(list, typeFilter));

		return (obj);
	}

	/**
	 * Method to count, group and sort actors.
	 * @param list containing actors
	 * @return distinct actors from list with count
	 */
	public static List<Document> countActors(List<Document> list) {
		Map<String, Integer> actorCounts = new HashMap<>();
		Map<String, Object> actorLocations = new HashMap<>();
		Map<String, String> actorTypes = new HashMap<>();
		ArrayList<Document> groups = new ArrayList<>();
		int counter1 = 0;
		for (Document doc : list) {
			ArrayList<Document> actors = doc.get("Actors", ArrayList.class);
			if (actors == null)
				actors = new ArrayList<>();
			counter1++;
			if (counter1 % 10000 == 0)
				logger.info(list.indexOf(doc) + " / " + list.size());
			for (Document actor : actors) {
				if (actorCounts.containsKey(actor.getString("Name")))
					actorCounts.replace(actor.getString("Name"), actorCounts.get(actor.getString("Name")) + 1);
				else {
					actorCounts.put(actor.getString("Name"), 1);
					actorLocations.put(actor.getString("Name"), actor.get("Location"));
					actorTypes.put(actor.getString("Name"), actor.getString("Type"));
				}
			}
		}
		for (String actor : actorCounts.keySet()) {
			Document newdoc = new Document();
			newdoc.append("Count", actorCounts.get(actor));
			newdoc.append("Actor", actor);
			newdoc.append("Location", actorLocations.get(actor));
			newdoc.append("Type", actorTypes.get(actor));
			groups.add(newdoc);
		}
		logger.info("Actors amount: " + groups.size());
		groups.sort(Comparator.comparing(a -> a.getInteger("Count"), Comparator.reverseOrder()));
		return groups;
	}

	/**
	 * Method to count, group and sort types.
	 * @param list containing types
	 * @return distinct types from list with count
	 * @return
	 */
	public static List<Document> countTypes(List<Document> list, String typeFilter) {
		ArrayList<Document> groups = new ArrayList<>();
		for (Document doc : list) {
			boolean added = false;
			for (Document group : groups) {
				if (typeFilter.equals("EXACT") && group.getString("Type").equals(doc.getString("Type"))) {
					group.replace("Count", group.getInteger("Count") + 1);
					added = true;
					break;
				} else if (typeFilter.equals("BASE") && Helper.getFirstTwoCharsOfString(group.getString("BaseType"))
						.equals(Helper.getFirstTwoCharsOfString(doc.getString("Type")))) {
					group.replace("Count", group.getInteger("Count") + 1);
					added = true;
					break;
				}
			}
			if (!added) {
				Document newdoc = new Document();
				newdoc.append("Count", 1);
				String type = doc.getString("Type");
				if (typeFilter.equals("EXACT")) {
					newdoc.append("Type", doc.getString("Type"));
					if (doc.getString("Type") == null || CameoCodeTranslator.getTypeValue(doc.getString("Type")) == null)
						continue;
					newdoc.append("Type_Name", CameoCodeTranslator.getTypeValue(doc.getString("Type")));
				}
				newdoc.append("BaseType", Helper.getFirstTwoCharsOfString(type));
				newdoc.append("BaseType_Name", CameoCodeTranslator.getTypeValue(Helper.getFirstTwoCharsOfString(type)));
				groups.add(newdoc);
			}
		}
		groups.sort(Comparator.comparing(a -> a.getInteger("Count"), Comparator.reverseOrder()));
		return groups;
		//test123
	}
}
