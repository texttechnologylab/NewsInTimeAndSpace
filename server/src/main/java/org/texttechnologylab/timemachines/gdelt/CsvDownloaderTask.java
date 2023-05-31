/*
* CsvDownloaderTask
*
* @date    03.12.2022
*
* @author  Timo Lüttig
* @version 1.0
*
* Task for gdelt csv downloads
*/
package org.texttechnologylab.timemachines.gdelt;

import java.util.TimerTask;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class CsvDownloaderTask extends TimerTask {

    private static Logger logger = LoggerFactory.getLogger(CsvDownloaderTask.class);

    /**
     * start a task for downloading latest events from gdelt
     */
    @Override
    public void run() {
        try {
            new CsvDownloader().getLatestGdelt();
        } catch (Exception e) {
            logger.error(e.toString());
        }
    }

}
