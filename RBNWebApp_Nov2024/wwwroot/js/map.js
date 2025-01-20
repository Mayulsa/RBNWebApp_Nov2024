// Add this to your wwwroot/js/map.js file
let map = null;
let isGoogleMapsLoaded = false;

// Function to check if Google Maps is loaded
function isGoogleMapsReady() {
    return typeof google !== 'undefined' && typeof google.maps !== 'undefined';
}

// Initialize map with retry mechanism
window.initializeMap = async function (markers) {
    try {
        // Wait for Google Maps to be loaded
        let retries = 0;
        while (!isGoogleMapsReady() && retries < 10) {
            await new Promise(resolve => setTimeout(resolve, 500));
            retries++;
        }

        if (!isGoogleMapsReady()) {
            console.error('Google Maps failed to load');
            return;
        }

        // Get the map container
        const mapElement = document.getElementById("map");
        if (!mapElement) {
            console.error('Map container not found');
            return;
        }

        // Default center (Costa Rica)
        const defaultCenter = { lat: 9.7489, lng: -83.7534 };

        // Create the map
        map = new google.maps.Map(mapElement, {
            zoom: 7,
            center: defaultCenter,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            mapTypeControl: true,
            streetViewControl: true,
            fullscreenControl: true
        });

        // Create bounds object
        const bounds = new google.maps.LatLngBounds();

        // Check if markers array exists and has items
        if (markers && markers.length > 0) {
            // Add markers
            markers.forEach(markerData => {
                if (typeof markerData.lat === 'number' && typeof markerData.lng === 'number') {
                    const position = { lat: markerData.lat, lng: markerData.lng };

                    // Create marker
                    const marker = new google.maps.Marker({
                        position: position,
                        map: map,
                        title: markerData.title || 'Unknown location'
                    });

                    // Create info window content
                    const content = `
                        <div class="info-window">
                            <h5 class="mb-1">${markerData.title || 'Unknown'}</h5>
                            <p class="mb-0"><strong>Role:</strong> ${markerData.role || 'N/A'}</p>
                        </div>
                    `;

                    // Create info window
                    const infoWindow = new google.maps.InfoWindow({
                        content: content,
                        maxWidth: 200
                    });

                    // Add click listener
                    marker.addListener('click', () => {
                        infoWindow.open(map, marker);
                    });

                    // Extend bounds
                    bounds.extend(position);
                }
            });

            // Fit map to bounds
            map.fitBounds(bounds);

            // Set minimum zoom level
            const listener = google.maps.event.addListener(map, 'idle', function () {
                if (map.getZoom() > 15) {
                    map.setZoom(15);
                }
                google.maps.event.removeListener(listener);
            });
        }

    } catch (error) {
        console.error('Error initializing map:', error);
    }
};