/*
 * MapData.js
 * Responsible for populating the map with data from the desired data feed.
 */

// Reference the map
var map = $("#map");

// Class responsible for managing and presenting data related to a particular data feed
class MapDataFeed {
    
    constructor(statusFeedUrl) {
        this.statusFeedUrl = statusFeedUrl;
        
        this.pilots = [];
        this.markers = [];
        this.controllers = [];

        // Connect SignalR
        this.connection = new signalR.HubConnectionBuilder().withUrl(statusFeedUrl + "/TrafficDataHub").withAutomaticReconnect().build();
        
        // Setup the messages
        this.connection.on("NewTrafficDataAvailable", () => this.refreshPilots(this));
        
        // Start the connection
        this.connection.start({ withCredentials: false });
    }
    
    refreshPilots(dataFeed = null) {
        if (dataFeed == null) dataFeed = this;
        console.log("Refreshing Pilots...");
        $.get(dataFeed.statusFeedUrl + "/TrafficData/Pilots",
            function(data) {
                try {
                    dataFeed.processNewPilots(data, dataFeed);
                } catch (e){
                    console.error("Failed to process pilots.");
                    console.error(e);
                }                
            });
    }
    
    processNewPilots(data, dataFeed = null) {
        if (dataFeed == null) dataFeed = this;
        
        // Clean up old markers
        for (let i = 0; i < dataFeed.markers.length; i++) {
                        
            // Remove from map
            dataFeed.markers[i].remove();
            
            // Remove event listeners
            dataFeed.markers[i].getElement().removeEventListener("click", dataFeed.pilotClicked);
        }
        
        // Clear the markers array
        dataFeed.markers = [];
        
        // Add our new pilots
        for (let i = 0; i < data.length; i++) {

            // Create the element
            let element = dataFeed.createMarkerElementFor(data[i].Callsign, dataFeed.pilotClicked);

            // Create the marker and add to the map
            let marker = new mapboxgl.Marker(element)
                .setLngLat([data[i].Longitude, data[i].Latitude])
                .setRotation(data[i].Heading)
                .addTo(map);
            
            // Add the marker
            dataFeed.markers.push(marker);
        }

        // Update the pilots list after all the processing
        dataFeed.pilots = data;
    }
    
    createMarkerElementFor(callsign, pilotClickCallback) {
        
        // Create the HTML element
        let el = document.createElement('div');
        el.className = 'aircraftMarker';
        
        // Store the callsign
        el.setAttribute('data-callsign', callsign);
        
        // Add the event listener
        el.addEventListener("click", pilotClickCallback);
        
        return el;
    }
    
    pilotClicked(element) {
        let callsign = element.currentTarget.getAttribute("data-callsign");
                
        console.log("User clicked " + callsign);
    }
}