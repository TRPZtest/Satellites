# Satellites

Hello
This application makes http requests and saving json data to file.
Also it checks if satellite information is available in the file and compare hash sums of the API response and local data.
This applictation makes HttpRequests in parallel and you can change size of requests batch in appsettings.json file. Unsuccessful requests will be sent again a limited number of times which you can set in appsettings.json.
