/*
* Graph
*
* @date    05.03.2023
*
* @author  Timo Lüttig
* @version 1.0
*
* Object representation for gdlet graph
*/
package org.texttechnologylab.timemachines.gdelt.objects;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.w3c.dom.Node;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import java.io.IOException;
import java.io.StringReader;
import java.util.Arrays;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.apache.commons.text.StringEscapeUtils;
import org.bson.Document;

public class Graph {

    private final Logger logger = LoggerFactory.getLogger(getClass());

    private Document docRepresentation;

    private static final String[] GDELT_GRAPH_HEADERS = { "", "DATE", "SourceCollectionIdentifier", "SourceCommonName",
            "DocumentIdentifier", "Counts", "V2Counts", "Themes", "V2Themes", "Locations", "V2Locations", "Persons",
            "V2Persons", "Organizations", "V2Organizations", "V2Tone", "Dates", "GCAM", "SharingImage", "RelatedImages",
            "SocialImageEmbeds", "SocialVideoEmbeds", "Quotations", "AllNames", "Amounts", "TranslationInfo",
            "Extras" };

    /**
     * constructs graph object from gdelt csv
     * 
     * @param line one line from GDELT graph csv
     */
    public Graph(String[] line) {
        this.docRepresentation = buildGraphDocument(line);
    }

    /**
     * parse one line from a gdlet graph export
     * 
     * @param line one line from GDELT graph csv
     * @return Document containing all relevant graph informations
     */
    private Document buildGraphDocument(String[] line) {
        Document doc = new Document();
        Document result = new Document();

        if (line.length == GDELT_GRAPH_HEADERS.length) {
            for (int key = 0; key < line.length; key++) {
                doc.append(GDELT_GRAPH_HEADERS[key], line[key]);
            }
        } else {
            logger.info("Error while parsing graph line");

            if (logger.isDebugEnabled()) {
                logger.debug(Arrays.toString(line));
            }
        }
        parseExtras(doc);
        createArrays(doc);

        result.put("Source", doc.getOrDefault("DocumentIdentifier", ""));
        result.put("Themes", doc.get("Themes"));
        result.put("Counts", doc.get("Counts"));
        result.put("Media", new Document().append("type", "picture").append("content", doc.get("SharingImage")));
        result.put("Title", doc.get("PAGE_TITLE"));
        return result;
    }

    /**
     * Parse gedelt Extras and add content as fields to doc. This method also
     * removes the "EXTRAS" field from doc
     * 
     * @param doc
     */
    private void parseExtras(Document doc) {
        org.w3c.dom.Document extrasDoc;
        // add root element to gdelt xml like structure
        String extrasString = new StringBuilder().append("<root>").append(doc.getString("Extras")).append("</root>")
                .toString();
        // escape all xml specical chars
        extrasString = StringEscapeUtils.escapeXml11(extrasString);
        // revert replacement for tags
        extrasString = extrasString.replace("&lt;", "<");
        extrasString = extrasString.replace("&gt;", ">");

        try {
            extrasDoc = parseXMLString(extrasString);
            Node next = extrasDoc.getDocumentElement().getFirstChild();
            while (next != null) {
                doc.append(next.getNodeName(), StringEscapeUtils.unescapeXml(next.getTextContent()));
                next = next.getNextSibling();
            }
            doc.remove("Extras");
        } catch (ParserConfigurationException | SAXException | IOException e) {
            logger.error("error while parsing extras", e);
            System.out.println(extrasString);
        }

    }

    /**
     * Some Arrays are hidden in the gdlet csv and seperated with semicolon. Extract
     * these arrays and create real ones
     * 
     * @param doc Document containing hidden csv arrays
     * @return doc with real arrays
     */
    private Document createArrays(Document doc) {
        doc.forEach((key, value) -> {
            if (value.getClass().isInstance("")) {
                String valueString = (String) value;
                if (valueString.contains(";")) {
                    doc.put(key, Arrays.asList(valueString.split(";")));
                }
            }
        });
        return doc;
    }

    /**
     * parse the given xml from String to Document
     * 
     * @param xml
     * @return DOM for given xml String
     * @throws ParserConfigurationException
     * @throws IOException
     * @throws SAXException
     */
    private org.w3c.dom.Document parseXMLString(String xml)
            throws ParserConfigurationException, SAXException, IOException {
        DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
        DocumentBuilder builder = factory.newDocumentBuilder();
        InputSource is = new InputSource(new StringReader(xml));
        return builder.parse(is);
    }

    /**
     * 
     * @return Document representation
     */
    public Document getGraph() {
        return docRepresentation;
    }

}
