# NewsInTimeAndSpace
![NiTS logo](https://github.com/texttechnologylab/NewsInTimeAndSpace/assets/8282694/11cc5dc4-242b-4d70-a538-2f89f94039db)

# Abstract
We present News in Time and Space (NiTS), a virtual reality application for visualization, filtering and interaction with geo-based
events based on GDELT. The application can be used via VR glasses as well as a desktop solution for collaborative use by multiple
users using Ubiq. The aim of NiTS is to provide a broad overview of global events and trends over time, creating a valuable resource
for users seeking to understand and analyze world events.
<!--The application ’NiTS’, ’News in Time and Space’, filters news report information from the GDELT database and visualizes it on a
globe in either Virtual Reality or a desktop-designed version. This can be explored by a single user, but also includes support of adding
a multi-person extension. Our aim is to provide a comprehensive picture of global events and trends over time making it a valuable
resource to users seeking to understand and analyze world events.-->

# Screenshots
![Screenshot](https://github.com/texttechnologylab/NewsInTimeAndSpace/assets/134629212/19df0482-3b0b-49bd-a013-6743c5fd825b)\
*Filters 'Germany' and 'United States' in the Filterpool*\
![grafik](https://github.com/texttechnologylab/NewsInTimeAndSpace/assets/134629212/daab7c6c-83f2-4df5-ae20-cda2d6c0a35b)\
*Detailed view of an event based in Bavaria, Germany*

# Demonstration
A video demonstration of NiTS can be found [here](https://anonymfile.com/z8N6J/news-in-time-and-space-demonstration-eng.mp4).

# Installation

After you have cloned the project, you need to complete the configurations below:

## Database

### Configuration

For MongoDB configuration, you need to customize the `MongoDBConfig.cfg` file in the `server` folder with your credentials. Example:

```bash
remote_host = databasehost
remote_database = databasename
remote_user = databaseuser
remote_password = secret
remote_port = 27017
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
docker-compose up -d
```

The base server URL can be changed inside the 'WebRequestHandler.cs' script.
### Data import

After the server is running, new data is imported from gdelt every 15 minutes. If you want to import data from a specific time period, you can do so by querying the server with the following URL: `http://%SERVER_IP%/update/%DATE%`. The date has to be in the format `YYYY-MM-DD`. 
