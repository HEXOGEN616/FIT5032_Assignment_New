/**
* This is a simple JavaScript demonstration of how to call MapBox API to load the maps.
* I have set the default configuration to enable the geocoder and the navigation control.
* https://www.mapbox.com/mapbox-gl-js/example/popup-on-click/
*
* @author Jian Liew <jian.liew@monash.edu>
*/
const TOKEN = "pk.eyJ1Ijoic3ByaW50YWwiLCJhIjoiY2psamowbjZvMGNqZzN3cGg3M3l2MTM0ayJ9.6YSw6HC812NIKZ-Wu6gETA";
var locations = [];
$(".coordinates").each(function () {
    var longitude = $(".longitude", this).text().trim();
    var latitude = $(".latitude", this).text().trim();
    var description = $(".description", this).text().trim();
    // Create a point data structure to hold the values.
    var point = {
        "latitude": latitude,
        "longitude": longitude,
        "description": description
    };
    // Push them all into an array.
    locations.push(point);
});
var data = [];
for (i = 0; i < locations.length; i++) {
    var feature = {
        "type": "Feature",
        "properties": {
            "description": locations[i].description,
            "icon": "circle-15"
        },
        "geometry": {
            "type": "Point",
            "coordinates": [locations[i].longitude, locations[i].latitude]
        }
    };
    data.push(feature)
}
mapboxgl.accessToken = TOKEN;
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v10',
    zoom: 11,
    center: [locations.length === 0 ? 145.04583700 : locations[0].longitude, locations.length === 0 ? -37.87682300 : locations[0].latitude]
});
map.on('load', function () {
    // Add a layer showing the places.
    map.addLayer({
        "id": "places",
        "type": "symbol",
        "source": {
            "type": "geojson",
        "data": {
            "type": "FeatureCollection",
            "features": data
        }
},
    "layout": {
        "icon-image": "{icon}",
        "icon-allow-overlap": true
    }
});
map.addControl(new MapboxGeocoder({
    accessToken: mapboxgl.accessToken
}));;
map.addControl(new mapboxgl.NavigationControl());
// When a click event occurs on a feature in the places layer, open a popup at the
// location of the feature, with description HTML from its properties.



map.on('click', 'places', function (e) {
    var coordinates = e.features[0].geometry.coordinates.slice();
    var description = e.features[0].properties.description;
    // Ensure that if the map is zoomed out such that multiple
    // copies of the feature are visible, the popup appears
    // over the copy being pointed to.
    while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
        coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
    }
    new mapboxgl.Popup()
        .setLngLat(coordinates)
        .setHTML(description)
        .addTo(map);
});

$('.button').click(function (e) {
    var lng = $(this).parent().parent().children(".longitude").html();
    var lat = $(this).parent().parent().children(".latitude").html();
    map.flyTo({ center: [lng, lat] });
    })

map.on("click", function (e) {
   // document.getElementById('info').innerHTML =

        // e.lngLat is the longitude, latitude geographical position of the event
        //JSON.stringify(e.lngLat);

    var coodinates = JSON.stringify(e.lngLat);
    var cp = coodinates.replace('{"lng":', '').replace('"lat":', '').replace('}', '');
    var co = cp.split(',');
    if(document.getElementById('lng')) {
        document.getElementById('lng').value = co[0];
        document.getElementById('lat').value = co[1];
    }
        
    
    

    //map.addLayer({
    //    "id": "point",
    //    "type": "symbol",
    //    "source": {
    //        "type": "geojson",
    //        "data": {
    //            "type": "FeatureCollection",
    //            "features": [{
    //                "type": "Feature",
    //                "geometry": {
    //                    "type": "Point",
    //                    "coordinates": [co[0], co[1]]
    //                }
    //            }]
    //        }
    //    },
    //    "layout": {
    //        "icon-image": "{icon}",
    //        "icon-allow-overlap": true
    //    }
    //});
});
// Change the cursor to a pointer when the mouse is over the places layer.
map.on('mouseenter', 'places', function () {
    map.getCanvas().style.cursor = 'pointer';
});
// Change it back to a pointer when it leaves.
map.on('mouseleave', 'places', function () {
    map.getCanvas().style.cursor = '';
});
});