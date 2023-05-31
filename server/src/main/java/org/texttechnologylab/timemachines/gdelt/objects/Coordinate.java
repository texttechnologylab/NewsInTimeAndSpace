/*
* Coordinate
*
* @date    05.03.2023
*
* @author  Timo Lüttig
* @version 1.0
*
* Object representation for coordinates
*/
package org.texttechnologylab.timemachines.gdelt.objects;

import java.util.Arrays;

import org.bson.Document;

public class Coordinate {

    /**
     * Enum for all available types currently only Point
     */
    private enum Type {
        POINT {
            @Override
            public String toString() {
                return "Point";
            }
        }
    }

    private Type type;
    private Double longitude;
    private Double latitude;

    /**
     * Constructs coordinate from doc at given field
     * 
     * @param doc   Document to parse coordinate from
     * @param field Which field to parse in doc
     */
    public Coordinate(Document doc, String field) {
        this.type = Type.POINT;
        try {
            this.longitude = Double.parseDouble(doc.getOrDefault(field + "Geo_Long", 0.0).toString());
            this.latitude = Double.parseDouble(doc.getOrDefault(field + "Geo_Lat", 0.0).toString());
        } catch (Exception e) {
            this.longitude = 0.0;
            this.latitude = 0.0;
        }
    }

    /**
     * constructs coordinate from lat and long
     * 
     * @param type      which Type of coordinate
     * @param longitude longitude of coordinate
     * @param latitude  latiture of coordinate
     */
    public Coordinate(Type type, Double longitude, Double latitude) {
        this.type = type;
        this.latitude = latitude;
        this.longitude = longitude;
    }

    /**
     * translate the coordinate to geoJson
     * 
     * @return geoJson representation of coordinate
     */
    public Document getGeoJson() {
        Document geoJson = new Document();
        geoJson.put("type", this.type.toString());
        Double[] coordinates = { this.longitude, this.latitude };
        geoJson.put("coordinates", Arrays.asList(coordinates));
        return geoJson;
    }
}
