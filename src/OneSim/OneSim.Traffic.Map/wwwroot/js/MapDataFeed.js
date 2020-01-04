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
        
        // Prepare some events
        this.onNewTrafficDataAvailable = (conn) => { this.refreshPilots(); };
        this.onPilotClicked = (e) => { this.pilotClicked(e); };

        // Connect SignalR
        this.connection = new signalR.HubConnectionBuilder().withUrl(statusFeedUrl + "/TrafficDataHub").withAutomaticReconnect().build();
        
        // Setup the messages
        this.connection.on("NewTrafficDataAvailable", this.onNewTrafficDataAvailable);
        
        // Start the connection
        this.connection.start({ withCredentials: false });
    }
    
    // Downloads and then processes the pilot data
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
    
    // Processes the pilot data, removing old markers and events, and making new markers and events
    processNewPilots(data) {
        
        // Clean up old markers
        for (let i = 0; i < this.markers.length; i++) {
                        
            // Remove from map
            this.markers[i].remove();
            
            // Remove event listeners
            this.markers[i].getElement().removeEventListener("click", this.onPilotClicked);
        }
        
        // Clear the markers array
        this.markers = [];
        
        // Add our new pilots
        for (let i = 0; i < data.length; i++) {

            // Create the element
            let element = this.createMarkerElementFor(data[i].Callsign, this.onPilotClicked);

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
    
    // Creates a HTML element for the marker for a specific pilot
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

    // Method called when a pilot has been clicked on by the user
    pilotClicked(element) {
        let callsign = element.currentTarget.getAttribute("data-callsign");
        this.connection.invoke("pilotSelected", callsign);
    }
}