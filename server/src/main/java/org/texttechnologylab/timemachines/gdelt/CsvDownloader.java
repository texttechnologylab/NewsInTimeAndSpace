/*
* CsvDonwloader
*
* @date    03.12.2022
*
* @author  Timo Lüttig
* @version 1.0
*
* Donwloader for all gdelt Files
*/
package org.texttechnologylab.timemachines.gdelt;

import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;
import java.time.ZoneOffset;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import org.bson.Document;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.texttechnologylab.timemachines.gdelt.objects.Event;
import org.texttechnologylab.timemachines.gdelt.objects.Graph;
import org.texttechnologylab.timemachines.mongodb.MongoDBConnectionHandler;

import com.mongodb.MongoBulkWriteException;
import com.mongodb.client.model.InsertManyOptions;
import com.mongodb.client.model.UpdateOptions;
import com.opencsv.CSVParser;
import com.opencsv.CSVParserBuilder;
import com.opencsv.CSVReader;
import com.opencsv.CSVReaderBuilder;

public class CsvDownloader {

    private Logger logger = LoggerFactory.getLogger(getClass());

    private String gdeltBaseURLString = "http://data.gdeltproject.org/gdeltv2/";
    private String gdeltEventString = ".export.CSV.zip";
    private String gdeltGraphString = ".gkg.csv.zip";
    private String gdeltMentionString = ".mentions.CSV.zip";
    private String gdeltLatest = "lastupdate.txt";

    private int maxAttempts = 3;

    private static final String TIMESTAMP_FORMAT = "yyyyMMddHHmmss";
    private static final String COLLECTION = "events";

    /**
     * Donwload and insert a gdelt events export into the database
     * 
     * @param url url for gdlet event export
     */
    private void downloadExport(String url) {
        ArrayList<Document> docs = new ArrayList<>();
        try {
            CSVReader reader = downloadGdeltZip(url);
            for (String[] line : reader) {
                docs.add(new Event(line).getDoc());
            }
            reader.close();
            MongoDBConnectionHandler.getInstance().database.getCollection(COLLECTION).insertMany(docs,
                    new InsertManyOptions().ordered(false));
            logger.info("insert of events complete");
        } catch (MongoBulkWriteException e) {
            // identify dublicate ID and ignore
            if (e.getCode() != -3) {
                logger.error("MongoDB Error while downloading CSV", e);
            }
        } catch (Exception e) {
            logger.error("Error while downloading CSV", e);
        }
    }

    /**
     * Donwload and insert a gdelt events export into the database
     * 
     * @param url url for gdlet event export
     */
    private void downloadGraph(String url) {
        UpdateOptions options = new UpdateOptions();
        options.upsert(false);
        try {
            CSVReader reader = downloadGdeltZip(url);
            for (String[] line : reader) {
                Document doc = new Graph(line).getGraph();

                Document update = new Document();
                update.append("$set", doc);
                Document filter = new Document();
                filter.append("Source", doc.getOrDefault("Source", ""));

                MongoDBConnectionHandler.getInstance().database.getCollection(COLLECTION).updateMany(filter, update,
                        options);
                logger.debug("inserted new graph line");
            }
            reader.close();
        } catch (IOException e) {
            logger.error("Error while downloading CSV", e);
        }
        logger.info("graph update complete");
    }

    /**
     * Trys to donwload a gdelt csv-zip. If the response code is not 200 this method
     * will attempt to donwload the csv maxAttempts every 10 minutes
     * 
     * @param urlString url as String to gdelt export
     * @return CSV reader for given urlString. This reader has to be closed in
     *         calling method
     * @throws MalformedURLException
     */
    private CSVReader downloadGdeltZip(String urlString) throws MalformedURLException {
        InputStreamReader isr = null;
        CSVReader reader = null;

        if (logger.isInfoEnabled()) {
            logger.info(String.format("GET: %s", urlString));
        }

        URL gdelt = new URL(urlString);

        try {
            int responseCode = 0;
            int attempts = 0;
            do {
                if (attempts != 0) {
                    logger.info("Gdlet export is not (yet) available -- trying again in 10 minutes");
                    try {
                        Thread.sleep(600000);
                    } catch (InterruptedException e) {
                        logger.error("Error while sleeping", e);
                    }
                }
                HttpURLConnection huc = (HttpURLConnection) gdelt.openConnection();
                huc.setRequestMethod("HEAD");
                responseCode = huc.getResponseCode();
                attempts++;
            } while (responseCode != HttpURLConnection.HTTP_OK && attempts <= maxAttempts);

            ZipInputStream zip = new ZipInputStream(gdelt.openStream());
            ZipEntry entry = zip.getNextEntry();
            if (entry != null) {
                isr = new InputStreamReader(zip);
                CSVParser parser = new CSVParserBuilder().withSeparator('\t').withQuoteChar((char) 0xFFFF).build();
                reader = new CSVReaderBuilder(isr).withCSVParser(parser).build();
            }
        } catch (IOException e) {
            logger.error("Error while unpacking zip", e);
        }
        return reader;
    }

    /**
     * get latest export intervall from gdelt latest export and donwload the events
     * export and graph
     */
    public void getLatestGdelt() {
        String urlString = gdeltBaseURLString + gdeltLatest;
        try {
            CSVParser parser = new CSVParserBuilder().withSeparator(' ').withQuoteChar((char) 0xFFFF).build();
            URL gdelt = new URL(urlString);
            InputStreamReader isr = new InputStreamReader(gdelt.openStream(), StandardCharsets.UTF_8);
            CSVReader reader = new CSVReaderBuilder(isr).withCSVParser(parser).build();
            for (String[] line : reader) {
                if (line[2].contains(gdeltEventString)) {
                    downloadExport(line[2]);
                }
                if (line[2].contains(gdeltGraphString)) {
                    downloadGraph(line[2]);
                }
            }
        } catch (Exception e) {
            logger.error("failed to get newest GDELT", e);
        }
    }

    // jasper test
    public static void main(String[] args) {
        new CsvDownloader().getLatestGdelt();
    }

    /**
     * format a timestamp to use with gdelt downloads
     * 
     * @param ldt
     * @return ldt as String to use with gdelt download
     */
    private String formatGdeltTimestamp(LocalDateTime ldt) {
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern(TIMESTAMP_FORMAT).withZone(ZoneOffset.UTC);
        return formatter.format(ldt);
    }

    /**
     * Donwload all events from dateTime into datebase
     * 
     * @param dateTime
     */
    public void getEvents(LocalDateTime dateTime) {
        downloadExport(gdeltBaseURLString + formatGdeltTimestamp(dateTime) + gdeltEventString);
    }

    /**
     * Donwload graph from dateTime into datebase
     * 
     * @param dateTime
     */
    public void getGraph(LocalDateTime dateTime) {
        downloadGraph(gdeltBaseURLString + formatGdeltTimestamp(dateTime) + gdeltGraphString);
    }
}
