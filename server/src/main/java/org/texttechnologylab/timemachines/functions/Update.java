/*
  * Update
  *
  * @date    08.03.2023
  *
  * @author  Timo Lüttig
  * @version 1.0
  *
  * This Class provides the update funktion to load new events into the Database
  */
package org.texttechnologylab.timemachines.functions;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.json.simple.JSONObject;
import org.texttechnologylab.timemachines.gdelt.CsvDownloaderThread;

import spark.Request;
import spark.Response;

public class Update {

    // Limit the execution to 5 parallel thread to mange the database load
    static ExecutorService gdletDownloaderService = Executors.newFixedThreadPool(5);

    private Update() {
    }

    /**
     * 
     * @param req
     * @param rep
     * @return json response indicating if the update was sucessfully scheduled
     * 
     *         Load all Events and Graph export from GDLET from the given date
     */
    public static JSONObject donwloadDate(Request req, Response rep) {
        String dateString = req.params("date");
        JSONObject result = new JSONObject();
        try {
            LocalDate date = LocalDate.parse(dateString, DateTimeFormatter.ISO_DATE);
            LocalDateTime dateTime = date.atStartOfDay();
            while (dateTime.getDayOfMonth() == date.getDayOfMonth() && dateTime.compareTo(LocalDateTime.now()) < 0) {
                // add new Task to Scheduler
                gdletDownloaderService.submit(new CsvDownloaderThread(dateTime));
                dateTime = dateTime.plusMinutes(15);
            }
            rep.status(202);
            result.put("success", "true");
        } catch (Exception e) {
            rep.status(400);
            result.put("success", "false");
            result.put("error", "date is not well formated");
        }
        return result;
    }

}
