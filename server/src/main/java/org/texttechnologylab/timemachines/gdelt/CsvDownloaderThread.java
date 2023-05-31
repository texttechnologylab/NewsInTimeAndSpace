/*
* CsvDownloaderThread
*
* @date    03.12.2022
*
* @author  Timo Lüttig
* @version 1.0
*
* Thread for gdelt csv downloads
*/
package org.texttechnologylab.timemachines.gdelt;

import java.time.LocalDateTime;

public class CsvDownloaderThread implements Runnable {

    private LocalDateTime dateTime;

    /**
     * Constructs a new Thread to donwload gdelt event at dateTime
     * 
     * @param dateTime
     */
    public CsvDownloaderThread(LocalDateTime dateTime) {
        this.dateTime = dateTime;
    }

    /**
     * Start the Thread an download events and graph into datebase
     */
    @Override
    public void run() {
        CsvDownloader downloader = new CsvDownloader();
        downloader.getEvents(dateTime);
        downloader.getGraph(dateTime);
    }

}
