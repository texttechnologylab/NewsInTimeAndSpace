
/*
* Actor
*
* @date    05.03.2023
*
* @author  Timo Lüttig
* @version 1.0
*
* Object representation for gdlet actor
*/
package org.texttechnologylab.timemachines.gdelt.objects;

import org.bson.Document;

public class Actor {

    private String name;
    private String typeCode;
    private Coordinate location;

    /**
     * Constructs actor for given parameters
     * 
     * @param name     Name as String for actor
     * @param typeCode TypeCode as String for actor
     * @param location Location as Coordinate for actor
     */
    public Actor(String name, String typeCode, Coordinate location) {
        this.name = name;
        this.typeCode = typeCode;
        this.location = location;
    }

    /**
     * Constructs Actor at number from gdelt event
     * 
     * @param doc    Document representatipon of gdelt event
     * @param number Numnber of the actor to create
     */
    public Actor(Document doc, String number) {
        this.name = doc.getOrDefault("Actor" + number + "Name", "").toString();
        this.typeCode = doc.getOrDefault("Actor" + number + "Type1Code", "").toString();
        this.location = new Coordinate(doc, "Actor" + number);
    }

    /**
     * creates Document with all informations from Actor 
     * @return Actor as Document
     */
    public Document getActor() {
        Document actor = new Document();
        actor.put("Name", this.name);
        actor.put("Type", this.typeCode);
        actor.put("Location", this.location.getGeoJson());
        return actor;
    }

}
