/*
  * Helper
  *
  * @date    16.02.2023
  *
  * @author Jasper Hustedt, Timo Lüttig
  * @version 1.0
  *
  * Provides helper methods used in different API interactions
  *
  */
package org.texttechnologylab.timemachines.functions;

import com.mongodb.client.model.Filters;
import com.mongodb.client.model.geojson.Point;
import com.mongodb.client.model.geojson.Position;
import org.bson.conversions.Bson;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import spark.Request;

import java.io.FileInputStream;
import java.text.SimpleDateFormat;
import java.time.LocalDate;
import java.util.Date;
import java.util.Properties;
import java.util.TimeZone;

public class Helper {
	private Helper() {
	}

	private static Logger logger = LoggerFactory.getLogger(Helper.class);

	/**
	 * Format a date into a String.
	 * 
	 * @param date the date value to be formatted into a string
	 * @return the date represented as String following die format
	 *         "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"
	 */
	public static String formatDate(java.util.Date date) {
		SimpleDateFormat sdf;
		sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
		sdf.setTimeZone(TimeZone.getTimeZone("CET"));
		String text;
		if (date != null)
			text = sdf.format(date);
		else
			text = sdf.format(new Date());
		return text;
	}

	/**
	 * Constructs a BsonFilter from a request.
	 * 
	 * @param req the request from a spark API call
	 * @return Bson filter conataining infos from req
	 */
	public static Bson createBsonFilter(Request req) {
	    Bson filter = Filters.ne("GLOBALEVENTID", null);

	    String[] actors1 = req.queryParamsValues("actor");
	    if (actors1 != null) {
		Bson actorsOrFilter = Filters.eq("_id", null);
		for (String actor : actors1) {
		    if (actor.contains("~"))
		    {
			String[] splitActors = actor.split("~");
			actorsOrFilter = Filters.or(actorsOrFilter,
					Filters.and(
					Filters.in("Actors.Name", splitActors[0].toUpperCase()),
					Filters.in("Actors.Name", splitActors[1].toUpperCase())));
			continue;
		    }
		    actorsOrFilter = Filters.or(actorsOrFilter, Filters.in("Actors.Name", actor.toUpperCase()));
		}
		filter = Filters.and(filter, actorsOrFilter);
	    }
	    String[] eventTypes = req.queryParamsValues("type");
	    if (eventTypes != null) {
		Bson eventTypesFilter = Filters.eq("_id", null); // ACHTUNG hier muss Filters.eq stehen weil das immer
		// false sein muss für den OR Filter
		for (String event_type : eventTypes) {
		    if (event_type != null)
			eventTypesFilter = Filters.or(eventTypesFilter, Filters.in("Type", event_type));
		}
		filter = Filters.and(filter, eventTypesFilter);
	    }

	    String radius = req.queryParams("radius");
	    String latitude = req.queryParams("lat");
	    String longitude = req.queryParams("long");
	    if (latitude != null && latitude.length() != 0 && Double.parseDouble(latitude) != -1 && longitude != null
			    && longitude.length() != 0 && Double.parseDouble(longitude) != -1 && radius != null)
			filter = Filters.and(filter,
				    Filters.nearSphere("Location",
						    new Point(new Position(Double.parseDouble(longitude), Double.parseDouble(latitude))),
						    Double.parseDouble(radius) * 1000, 0.0));
	    else if (latitude != null && latitude.length() != 0 && Double.parseDouble(latitude) != -1 && longitude != null
			    && longitude.length() != 0 && Double.parseDouble(longitude) != -1)
		    filter = Filters.and(filter, Filters.nearSphere("Location",
				    new Point(new Position(Double.parseDouble(longitude), Double.parseDouble(latitude))), 0.0, 0.0));

	    String startTime = req.queryParams("startTime");
	    if (startTime != null)
		    filter = Filters.and(filter, Filters.gte("Date", LocalDate.parse(startTime)));
	    String endTime = req.queryParams("endTime");
	    if (endTime != null)
		    filter = Filters.and(filter, Filters.lte("Date", LocalDate.parse(endTime)));

	    String goldsteinScaleHigh = req.queryParams("GoldsteinScaleHigh");
	    if (goldsteinScaleHigh != null)
		    filter = Filters.and(filter, Filters.lte("GoldsteinScale", Double.parseDouble(goldsteinScaleHigh)));
	    String goldsteinScaleLow = req.queryParams("GoldsteinScaleLow");
	    if (goldsteinScaleLow != null)
		    filter = Filters.and(filter, Filters.gte("GoldsteinScale", Double.parseDouble(goldsteinScaleLow)));

	    String toneMax = req.queryParams("ToneMax");
	    if (toneMax != null)
		    filter = Filters.and(filter, Filters.lte("AvgTone", Double.parseDouble(toneMax)));
	    String toneMin = req.queryParams("ToneMin");
	    if (toneMin != null)
		    filter = Filters.and(filter, Filters.gte("AvgTone", Double.parseDouble(toneMin)));

	    return filter;
	}

	/**
	 * Reads Properties of a cfg file.
	 * @param fileName location of properties file
	 * @return Properties object loaded from fileName
	 */
	public static Properties readProperties(String fileName) {
		Properties prop = new Properties();
		try (FileInputStream fis = new FileInputStream(fileName)) {
			prop.load(fis);
			return prop;
		} catch (Exception e) {
			e.printStackTrace();
			logger.error(String.format("Can't load %s", fileName), e);
			System.exit(0);
		}
		return prop;
	}

	/**
	 * Get first two chars of a string, if string exists and string longer than 2 chars.
	 * @param string simple string
	 * @return first to characters of given string
	 */
	public static String getFirstTwoCharsOfString(String string) {
		return string == null ? "" : string.length() < 2 ? string : string.substring(0, 2);
	}
}
