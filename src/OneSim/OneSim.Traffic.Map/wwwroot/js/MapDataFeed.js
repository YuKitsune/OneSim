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
        this.connection = new signalR.HubConnectionBuilder().withUrl(statusFeedUrl + "/TrafficDataHub").build();

        // Setup the messages
        this.connection.on("NewTrafficDataAvailable", this.refreshPilots);
    }
    
    refreshPilots() {
        console.log("Refreshing Pilots...");
        let self = this;
        $.get(this.statusFeedUrl + "/TrafficData/Pilots",
            function(data) {
                try {
                    self.processNewPilots(data);
                } catch (e){
                    console.error("Failed to process pilots.");
                    console.error(e);
                }                
            });
    }
    
    processNewPilots(data) {
        
        // Clean up old pilots
        for (let i = 0; i < this.pilots.length; i++) {
            
            // Remove from map
            this.pilots[i].remove();
            
            // Remove event listeners
            this.pilots[i].element.removeEventListener("click", this.pilotClicked);
        }
        
        // Add our new pilots
        for (let i = 0; i < data.length; i++) {

            // Create the element
            let element = this.createMarkerElementFor(data[i].Callsign);

            // Create the marker and add to the map
            let marker = new mapboxgl.Marker(element)
                .setLngLat([data[i].Longitude, data[i].Latitude])
                .setRotation(data[i].Heading)
                .addTo(map);
            
            // Add the marker
            this.markers.push(marker);
        }

        // Update the pilots list after all the processing
        this.pilots = data;
    }
    
    createMarkerElementFor(callsign) {
        
        // Create the HTML element
        let el = document.createElement('div');
        el.className = 'aircraftMarker';
        
        // Store the callsign
        el.setAttribute('data-callsign', callsign);
        
        // Add the event listener
        el.addEventListener("click", this.pilotClicked);
        
        return el;
    }
    
    pilotClicked(element) {
        let callsign = element.currentTarget.getAttribute("data-callsign");
        console.log("User clicked " + callsign);
    }
    
    
}