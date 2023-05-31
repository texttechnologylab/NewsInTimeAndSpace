[![version](https://img.shields.io/github/license/texttechnologylab/NewsInTimeAndSpace)]()

# NewsInTimeAndSpace
Logo?


# Abstract


# Installation

## Database

### Configuration

For the MongoDB configuration you have to create a file called `MongoDBConfig.cfg` in the `server` folder. The file should look like this:

```bash
remote_host = 
remote_database = 
remote_user = 
remote_password = 
remote_port = 27021
```

### Data
Our Application is relying on the DB to provide translations for CAMEO codes. To import use following command:

```bash
mongoimport -c CameoCodes -d %remot_database% --mode upsert --file ./server/EventCameoCodes.json --jsonArray
```
## Server

To Start the Server with Docker, run the following commands:

```bash 
cd server
mvn clean package
docker build -t newsintimeandspace .
docker run -p 8080:8080 newsintimeandspace
```
### Data import

After the server is running, new data is imported from gdelt every 15 minutes. If you want to import data from a specific time period, you can do so by querying the server with the following URL: `http://%SERVER_IP%/update/%DATE%`. The date has to be in the format `YYYY-MM-DD`. 

# Cite
If you want to use the *News in Time and Space* please quote this as follows:

CITE

# BibTeX
```
@InProceedings{NAMES:Abrami:Mehler:2023,
  author         = {NAMES and Abrami, Giuseppe and Mehler, Alexander},
  title          = {TITLE},
  booktitle      = {TITLE},
  year           = {2023},
  pages          = {TBA},
  url            = {TBA}
}

```
