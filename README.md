[![version](https://img.shields.io/github/license/texttechnologylab/NewsInTimeAndSpace)]()

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
docker-compose up -d
```
### Data import

After the server is running, new data is imported from gdelt every 15 minutes. If you want to import data from a specific time period, you can do so by querying the server with the following URL: `http://%SERVER_IP%/update/%DATE%`. The date has to be in the format `YYYY-MM-DD`. 

# Cite
If you want to use the *News in Time and Space* please quote this as follows:

CITE

# BibTeX
```
@InProceedings{Gagel:Hustedt:Lüttig:Berg:Abrami:Mehler:2023,
  author         = {Gagel, Julian and Hustedt, Jasper and Lüttig, Timo and Berg, Theresa and Abrami, Giuseppe and Mehler, Alexander},
  title          = {News in Time and Space: Global Event Exploration in Virtual Reality},
  booktitle      = {TBA},
  year           = {2023},
  pages          = {3},
  url            = {https://github.com/texttechnologylab/NewsInTimeAndSpace}
}

```
