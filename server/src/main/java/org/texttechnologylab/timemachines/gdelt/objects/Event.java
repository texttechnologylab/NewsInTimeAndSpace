
/*
* Event
*
* @date    05.03.2023
*
* @author  Timo Lüttig
* @version 1.0
*
* Object representation for gdlet event
*/
package org.texttechnologylab.timemachines.gdelt.objects;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Arrays;

import org.bson.Document;

public class Event {

    private final Logger logger = LoggerFactory.getLogger(getClass());

    private Document docRepresentation;

    private static final String[] GDELT_EVENT_HEADERS = { "_id", "SQLDATE", "MonthYear", "Year", "FractionDate",
            "Actor1Code", "Actor1Name", "Actor1CountryCode", "Actor1KnownGroupCode", "Actor1EthnicCode",
            "Actor1Religion1Code", "Actor1Religion2Code", "Actor1Type1Code", "Actor1Type2Code", "Actor1Type3Code",
            "Actor2Code", "Actor2Name", "Actor2CountryCode", "Actor2KnownGroupCode", "Actor2EthnicCode",
            "Actor2Religion1Code", "Actor2Religion2Code", "Actor2Type1Code", "Actor2Type2Code", "Actor2Type3Code",
            "IsRootEvent", "EventCode", "EventBaseCode", "EventRootCode", "QuadClass", "GoldsteinScale", "NumMentions",
            "NumSources", "NumArticles", "AvgTone", "Actor1Geo_Type", "Actor1Geo_FullName", "Actor1Geo_CountryCode",
            "Actor1Geo_ADM1Code", "Actor1Geo_ADM2Code", "Actor1Geo_Lat", "Actor1Geo_Long", "Actor1Geo_FeatureID",
            "Actor2Geo_Type", "Actor2Geo_FullName", "Actor2Geo_CountryCode", "Actor2Geo_ADM1Code", "Actor2Geo_ADM2Code",
            "Actor2Geo_Lat", "Actor2Geo_Long", "Actor2Geo_FeatureID", "ActionGeo_Type", "ActionGeo_FullName",
            "ActionGeo_CountryCode", "ActionGeo_ADM1Code", "ActionGeo_ADM2Code", "ActionGeo_Lat", "ActionGeo_Long",
            "ActionGeo_FeatureID", "DATEADDED", "SOURCEURL" };

    /**
     * Constructs a gdelt event from one csv line
     * 
     * @param line one line from csv gdelt export
     */
    public Event(String[] line) {
        this.docRepresentation = buildEventDocument(line);
    }

    /**
     * Constructs a gdelt event from one csv line
     * 
     * @param line one line from csv gdelt export
     * @return Event object created from line
     */
    private Document buildEventDocument(String[] line) {
        Document tmp = new Document();
        Document result = new Document();
        if (line.length == GDELT_EVENT_HEADERS.length) {
            for (int key = 0; key < line.length; key++) {
                tmp.append(GDELT_EVENT_HEADERS[key], line[key]);
            }
        } else {
            logger.info("Error while parsing event line");

            if (logger.isDebugEnabled()) {
                logger.debug(Arrays.toString(line));
            }
        }
        result.put("GLOBALEVENTID", tmp.getOrDefault("_id", ""));
        result.put("Location_Name", tmp.getOrDefault("ActionGeo_FullName", ""));
        result.put("Country_Code", tmp.getOrDefault("ActionGeo_CountryCode", ""));
        result.put("Location", new Coordinate(tmp, "Action").getGeoJson());
        result.put("Actors", getActors(tmp));
        result.put("Source", tmp.getOrDefault("SOURCEURL", ""));
        result.put("IsRootEvent", tmp.getOrDefault("IsRootEvent", ""));
        result.put("GoldsteinScale", Double.parseDouble((String) tmp.getOrDefault("GoldsteinScale", "0")));
        result.put("AvgTone", Double.parseDouble((String) tmp.getOrDefault("AvgTone", "0")));
        result.put("NumMentions", tmp.getOrDefault("NumMentions", ""));

        // parse SQLDATE with Format YYYYMMDD
        result.put("Date", LocalDate.parse(tmp.getString("SQLDATE"), DateTimeFormatter.BASIC_ISO_DATE));
        result.put("Type", tmp.getOrDefault("EventCode", "0"));

        return result;
    }

    /**
     * Extracts all actoros from one gdelt event
     * 
     * @param doc Document containing a gdelt event
     * @return ArrayList of all actors from event
     */
    private ArrayList<Document> getActors(Document doc) {
        ArrayList<Document> result = new ArrayList<>();
        String type;
        type = doc.getString("Actor1Geo_Type");
        if (!type.equals("0")) {
            result.add(new Actor(doc, "1").getActor());
        }
        type = doc.getString("Actor2Geo_Type");
        if (!type.equals("0")) {
            result.add(new Actor(doc, "2").getActor());
        }
        return result;
    }

    /**
     * 
     * @return Document representation
     */
    public Document getDoc() {
        return docRepresentation;
    }
}
