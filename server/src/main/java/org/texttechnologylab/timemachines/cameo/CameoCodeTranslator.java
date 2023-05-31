/*
 * CameoCodeTranslator
 *
 * @date 16.02.2023
 *
 * @author Jasper Husted, Timo Lüttig
 * 
 * @version 1.0
 *
 * Class for translations between cameo codes an their description
 *
 */
package org.texttechnologylab.timemachines.cameo;

import java.util.HashMap;
import java.util.Map;

import org.bson.Document;
import org.texttechnologylab.timemachines.mongodb.MongoDBConnectionHandler;

import com.mongodb.client.MongoCursor;

public class CameoCodeTranslator {
    private static final String CAMEO_CODES = "CameoCodes";
    private static Map<String, String> cameoMap;

    private CameoCodeTranslator() {
    }

    /**
     * Gets the mapping from the database if no mapping is "chached"
     * 
     * @return map for all cameo codes an their description
     */
    private static Map<String, String> getCameoCodeMap() {
        if (cameoMap == null) {
            cameoMap = new HashMap<String, String>();
            MongoCursor<Document> result = MongoDBConnectionHandler.getInstance().database.getCollection(CAMEO_CODES)
                    .find().cursor();
            while (result.hasNext()) {
                Document doc = result.next();
                cameoMap.put(doc.getString("code"), doc.getString("value"));
            }
        }
        return cameoMap;
    }

    /**
     * 
     * @param code CameoCode to translate
     * @return description of given code
     */
    public static String getTypeValue(String code) {
        return getCameoCodeMap().get(code);
    }
}
