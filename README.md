[![Conference](http://img.shields.io/badge/conference-HT--2023-4b44ce.svg)](https://ht.acm.org/ht2023/)
[![version](https://img.shields.io/github/license/texttechnologylab/NewsInTimeAndSpace)]()
[![latest](https://img.shields.io/github/v/release/texttechnologylab/NewsInTimeAndSpace)]()


# NewsInTimeAndSpace

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

The base server URL can be changed inside the 'ServerURL' gameobject inspector.
### Data import

After the server is running, new data is imported from gdelt every 15 minutes. If you want to import data from a specific time period, you can do so by querying the server with the following URL: `http://%SERVER_IP%/update/%DATE%`. The date has to be in the format `YYYY-MM-DD`. 


# Cite
If you want to use the project please quote this as follows:

Julian Gagel, Jasper Hustedt, Timo Lüttig, Theresa Berg, Giuseppe Abrami and Alexander Mehler. (2023). "News in Time and Space: Global Event Exploration in Virtual Reality." Proceedings of the 34th ACM Conference on Hypertext and Social Media. 

# Globe
Our globe is based on Sebastian Lagues game: https://github.com/SebLague/Geographical-Adventures

## BibTeX
```
@inproceedings{Gagel:et:al:2023,
  author    = {Gagel, Julian and Hustedt, Jasper and L\"{u}ttig, Timo and Berg, Theresa
               and Abrami, Giuseppe and Mehler, Alexander},
  title     = {News in Time and Space: Global Event Exploration in Virtual Reality},
  year      = {2023},
  isbn      = {9798400702327},
  publisher = {Association for Computing Machinery},
  address   = {New York, NY, USA},
  url       = {https://doi.org/10.1145/3603163.3609080},
  doi       = {10.1145/3603163.3609080},
  abstract  = {We present News in Time and Space (NiTS), a virtual reality application
               for visualization, filtering and interaction with geo-referenced
               events based on GDELT. It can be used both via VR glasses and
               as a desktop solution for shared use by multiple users with Ubiq.
               The aim of NiTS is to provide overviews of global events and trends
               in order to create a resource for their monitoring and analysis.},
  booktitle = {Proceedings of the 34th ACM Conference on Hypertext and Social Media},
  articleno = {7},
  numpages  = {3},
  location  = {Rome, Italy},
  series    = {HT '23},
  pdf       = {https://dl.acm.org/doi/pdf/10.1145/3603163.3609080}
}
```
