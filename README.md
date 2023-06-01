[![version](https://img.shields.io/github/license/texttechnologylab/NewsInTimeAndSpace)]()

# NewsInTimeAndSpace
Logo?
![alt text]([http://url/to/img.png](https://private-user-images.githubusercontent.com/134629212/242628798-c034c673-6284-427b-ac7f-7a2b455f14b6.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJrZXkxIiwiZXhwIjoxNjg1NjI5ODc4LCJuYmYiOjE2ODU2Mjk1NzgsInBhdGgiOiIvMTM0NjI5MjEyLzI0MjYyODc5OC1jMDM0YzY3My02Mjg0LTQyN2ItYWM3Zi03YTJiNDU1ZjE0YjYucG5nP1gtQW16LUFsZ29yaXRobT1BV1M0LUhNQUMtU0hBMjU2JlgtQW16LUNyZWRlbnRpYWw9QUtJQUlXTkpZQVg0Q1NWRUg1M0ElMkYyMDIzMDYwMSUyRnVzLWVhc3QtMSUyRnMzJTJGYXdzNF9yZXF1ZXN0JlgtQW16LURhdGU9MjAyMzA2MDFUMTQyNjE4WiZYLUFtei1FeHBpcmVzPTMwMCZYLUFtei1TaWduYXR1cmU9ZTYyNmVjODI3YjI3ZDc0ZTY1OWY0YzdlYzcxZjUzZTUyZjQ3MjUyM2ZjNTI2ZWEzNmNlZTljYWRjOWQ0OTA0MSZYLUFtei1TaWduZWRIZWFkZXJzPWhvc3QifQ.3cZwFgWqEVXAp0qy0QpsDvzMhEJuNbISlt1riAZPfKY
))
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
