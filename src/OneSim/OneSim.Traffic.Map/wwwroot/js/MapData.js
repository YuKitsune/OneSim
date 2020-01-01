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
        this.controllers = [];
    }
    
    refreshPilots() {
        console.log("Refreshing Pilots...");
        $.get(this.statusFeedUrl + "/TrafficData/Pilots",
            function(data) {
                try {
                    this.pilots = data;
                    for (var i = 0; i < this.pilots.length; i++) {
                        var marker = new mapboxgl.Marker()
                            .setLngLat([this.pilots[i].Longitude, this.pilots[i].Latitude])
                            .addTo(map);
                    }
                } catch (e){
                    console.error("Failed to download pilots.");
                    console.error(e);
                }                
            });
    }
    
    
}