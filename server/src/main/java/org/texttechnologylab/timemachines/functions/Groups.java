/*
* Groups
*
* @date    16.02.2023
*
* @author  Jasper Hustedt
* @version 1.0
*
* Provides methods used by all Api calls requesting event groups
*
*/
package org.texttechnologylab.timemachines.functions;

import com.mongodb.client.model.Accumulators;
import com.mongodb.client.model.Aggregates;
import com.mongodb.client.model.Field;
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

import javax.print.Doc;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.Objects;

public class Groups {
    	private Groups() {
	}

	private static long starttime;

	private static Logger logger = LoggerFactory.getLogger(Groups.class);

	// Neue Groups (grouped by country, region, "city/subregion") start functions

	/**
	 * Adds variables onlyIDs and grouping to method and forwards it.
	 * @param req
	 * @param res
	 * @return JSONObject containing events grouped by country 
	 */
	public static JSONObject getGroupsCountryAll(Request req, Response res) {
		return getGroups(req, res, false, "country");
	}

	/**
	 * Adds variables onlyIDs and grouping to method and forwards it.
	 * @param req
	 * @param res
	 * @return JSONObject containing event Ids grouped by country 
	 */
	public static JSONObject getGroupsCountryId(Request req, Response res) {
		return getGroups(req, res, true, "country");
	}

	/**
	 * Adds variables onlyIDs and grouping to method and forwards it.
	 * @param req
	 * @param res
	 * @return JSONObject containing events grouped by region 
	 */
	public static JSONObject getGroupsRegionAll(Request req, Response res) {
		return getGroups(req, res, false, "region");
	}

	/**
	 * Adds variables onlyIDs and grouping to method and forwards it.
	 * @param req
	 * @param res
	 * @return JSONObject containing event Ids grouped by region 
	 */
	public static JSONObject getGroupsRegionId(Request req, Response res) {
		return getGroups(req, res, true, "region");
	}

	/**
	 * Adds variables onlyIDs and grouping to method and forwards it.
	 * @param req
	 * @param res
	 * @return JSONObject containing events grouped by city 
	 */
	public static JSONObject getGroupsCityAll(Request req, Response res) {
		return getGroups(req, res, false, "city");
	}

	/**
	 * Adds variables onlyIDs and grouping to method and forwards it.
	 * @param req
	 * @param res
	 * @return JSONObject containing event Ids grouped by city 
	 */
	public static JSONObject getGroupsCityId(Request req, Response res) {
		return getGroups(req, res, true, "city");
	}

	// Neue Groups Main Functions

	/**
	 * Method to request events from database via filters for further grouping.
	 * 
	 * @param req Spark request 
	 * @param res Spark response
	 * @param onlyIDs flag if only the ids should be returned 
	 * @param grouping field to be grouped by 
	 * @return JSONOBject containg all gorups
	 */
	private static JSONObject getGroups(Request req, Response res, boolean onlyIDs, String grouping) {
		res.type("application/json");
		ArrayList<Document> list = new ArrayList<>();

		Bson filter = Helper.createBsonFilter(req);

		String limit = req.queryParams("limit");
		int limitInt = 0;
		if (limit != null)
			limitInt = Integer.parseInt(limit);
		starttime = System.currentTimeMillis();

		Bson projection = Projections.excludeId();
		if (onlyIDs)
			projection = Projections.include("GLOBALEVENTID", "Location_Name", "Country_Code", "Location","Actors");
		MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter).projection(projection).limit(limitInt).into(list);
		logger.info("New");
		logger.info(Float.toString((System.currentTimeMillis() - starttime) / 1000f));
		list.forEach(a -> a.replace("Date", Helper.formatDate(a.getDate("Date"))));

		JSONObject obj = new JSONObject();
		obj.put("results", createGroups(list, onlyIDs, grouping));

		return (obj);
	}

	public static JSONObject testGroups(Request req, Response res) {
	    res.type("application/json");
	    ArrayList<Document> list = new ArrayList<>();

	    //Bson filter = Helper.createBsonFilter(req);

	    /*String limit = req.queryParams("limit");
	    int limitInt = 0;
	    if (limit != null)
		limitInt = Integer.parseInt(limit);*/
	    starttime = System.currentTimeMillis();

	    /*Bson projection = Projections.excludeId();
	    if (onlyIDs)
		projection = Projections.include("GLOBALEVENTID", "Location_Name", "Country_Code", "Location","Actors");*/
	    //MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).find(filter).projection(projection).limit(limitInt).into(list);

	    MongoDBConnectionHandler.getInstance().database.getCollection(App.collection).aggregate(Arrays.asList(
		    /*Aggregates.project(Projections.fields(
				    new Document("Location_Name", new Document("$split", Arrays.asList("$Location_Name", ", "))),
				    new Document("Count", 1)
		    )),*/
		    //Aggregates.addFields(new Field<>("Count", 1), new Field<>("Location_Name", new Document("$split", Arrays.asList("$Location_Name", ", ")))),
		    new Document("$match", new Document("Location_Name.0", new Document("$exists", true))),
		    new Document("$set", new Document("Location_Name", new Document("$last", "$Location_Name"))),
		    Aggregates.group("$Location_Name", Accumulators.sum("Count", 1), Accumulators.push("data", "$$ROOT")),
		    new Document("$sort", new Document("Count", -1))
	    )).into(list);

	    logger.info("New");
	    logger.info(Float.toString((System.currentTimeMillis() - starttime) / 1000f));
	    //list.forEach(a -> a.replace("Date", Helper.formatDate(a.getDate("Date"))));

	    JSONObject obj = new JSONObject();
	    //obj.put("results", createGroups(list, onlyIDs, grouping));
	    obj.put("results", list);
	    return (obj);
	}

	/**
	 * Method to group and count events via a chosen aggregation parameter.
	 * Possible aggregation parameters contain "country", "region" and "city".
	 * @param list
	 * @param onlyIDs
	 * @param grouping
	 * @return
	 */
	private static ArrayList<Document> createGroups(ArrayList<Document> list, boolean onlyIDs, String grouping) {
		int numberOfDocuments = 0;
		ArrayList<Document> groups = new ArrayList<>();
		for (Document doc : list) {
			ArrayList<String> locationsDivided = new ArrayList<>(
					Arrays.asList(doc.getString("Location_Name").split(",")));
			locationsDivided.replaceAll(String::trim);
			boolean added = false;
			for (Document group_doc : new ArrayList<Document>(groups)) {
				if (grouping.equals("country") && !locationsDivided.isEmpty()
						&& locationsDivided.get(locationsDivided.size() - 1).equals(group_doc.getString("Country"))) {
					added = true;
					group_doc.replace("Count", group_doc.getInteger("Count") + 1);
					group_doc.get("Events", ArrayList.class).add(getDocumentForGroup(doc, onlyIDs));
					numberOfDocuments += 1;
				} else if (grouping.equals("region") && locationsDivided.size() >= 2
						&& locationsDivided.get(locationsDivided.size() - 2).equals(group_doc.getString("Region"))) {
					added = true;
					group_doc.replace("Count", group_doc.getInteger("Count") + 1);
					group_doc.get("Events", ArrayList.class).add(getDocumentForGroup(doc, onlyIDs));
					numberOfDocuments += 1;
				} else if (grouping.equals("city") && locationsDivided.size() == 3
						&& locationsDivided.get(0).equals(group_doc.getString("City"))) {
					added = true;
					group_doc.replace("Count", group_doc.getInteger("Count") + 1);
					group_doc.get("Events", ArrayList.class).add(getDocumentForGroup(doc, onlyIDs));
					numberOfDocuments += 1;
				}
			}
			if (grouping.equals("city") && locationsDivided.size() < 3
					|| grouping.equals("region") && locationsDivided.size() < 2
					|| grouping.equals("country") && locationsDivided.isEmpty() || locationsDivided.size() > 3)
				continue;
			if ((groups.isEmpty() || !added)) {
				Document newGroupDoc = new Document();
				newGroupDoc.append("Group_ID", list.indexOf(doc));
				newGroupDoc.append("Count", 1);
				if (locationsDivided.size() == 3) {
					newGroupDoc.append("City", grouping.equals("city") ? locationsDivided.get(0) : "");
					newGroupDoc.append("Region", grouping.equals("city") || grouping.equals("region") ? locationsDivided.get(1) : "");
					newGroupDoc.append("Country", locationsDivided.get(2));
				} else if (locationsDivided.size() == 2) {
					newGroupDoc.append("City", "");
					newGroupDoc.append("Region", grouping.equals("city") || grouping.equals("region") ? locationsDivided.get(0) : "");
					newGroupDoc.append("Country", locationsDivided.get(1));
				} else if (locationsDivided.size() == 1) {
					newGroupDoc.append("City", "");
					newGroupDoc.append("Region", "");
					newGroupDoc.append("Country", locationsDivided.get(0));
				}
				newGroupDoc.append("Country_Code", doc.getString("Country_Code"));

				Document geo = new Document().append("type", "Point").append("coordinates",
						new ArrayList<>(doc.get("Location", Document.class).get("coordinates", ArrayList.class)));
				newGroupDoc.append("Location", geo);
				ArrayList<Document> doclist = new ArrayList<>();
				doclist.add(getDocumentForGroup(doc, onlyIDs));
				newGroupDoc.append("Events", doclist);
				groups.add(newGroupDoc);
				numberOfDocuments += 1;
			}
		}
		logger.info(Float.toString((System.currentTimeMillis() - starttime) / 1000f));
		groups.sort(Comparator.comparing(a -> a.getInteger("Count"), Comparator.reverseOrder()));
		logger.info("Number of Documents: " + numberOfDocuments);
		logger.info(Float.toString((System.currentTimeMillis() - starttime) / 1000f));
		return addGroupConnections(groups, onlyIDs);
	}

	/**
	 * Method to find and add (superficial) connections between event actor coordinates and group locations.
	 * @param groups
	 * @param onlyIDs
	 * @return
	 */
	private static ArrayList<Document> addGroupConnections(ArrayList<Document> groups, boolean onlyIDs) {
		for (Document group : groups) {
			ArrayList<Integer> connections = new ArrayList<>();
			ArrayList<Document> events = group.get("Events", ArrayList.class);
			for (Document event : events) {
				ArrayList<Document> actors = event.get("Actors", ArrayList.class);
				for (Document actor : actors) {
					Document geoDoc = (Document) actor.get("Location");
					Double geoLong = (Double) geoDoc.get("coordinates", ArrayList.class).get(0);
					Double geoLat = (Double) geoDoc.get("coordinates", ArrayList.class).get(1);
					for (Document check_group : groups) {
						Document geoDocGroup = (Document) check_group.get("Location");
						Double geoLongGroup = (Double) geoDocGroup.get("coordinates", ArrayList.class).get(0);
						Double geoLatGroup = (Double) geoDocGroup.get("coordinates", ArrayList.class).get(1);
						if (!connections.contains(check_group.getInteger("Group_ID"))
								&& Objects.equals(geoLong, geoLongGroup) && Objects.equals(geoLat, geoLatGroup)) {
							connections.add(check_group.getInteger("Group_ID"));
						}
					}
				}
			}
			group.append("Connections", connections);
		}
		for (Document group : groups) {
			ArrayList<Document> events = group.get("Events", ArrayList.class);
			for (Document event : events) {
				if (onlyIDs)
					event.remove("Actors");
			}
		}
		logger.info(Float.toString((System.currentTimeMillis() - starttime) / 1000f));
		return groups;
	}

	/**
	 * Method to create event documents for groups.
	 * @param doc
	 * @param onlyIDs
	 * @return
	 */
	private static Document getDocumentForGroup(Document doc, boolean onlyIDs) {
		if (!onlyIDs)
			return doc;
		return (new Document("GLOBALEVENTID", doc.getString("GLOBALEVENTID")).append("Actors",
				doc.get("Actors", ArrayList.class)));
	}
}
