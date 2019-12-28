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
        $.get(this.statusFeedUrl + "/Status/Pilots".replace("//", "/"),
            function(data) {
                try {
                    this.pilots = JSON.parse(data);
                } catch (e){
                    console.error("Failed to download pilots.");
                    console.error(e);
                }                
            });
    }
    
    
}