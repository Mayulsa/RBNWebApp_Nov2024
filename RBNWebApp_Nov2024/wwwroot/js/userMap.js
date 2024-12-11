let map;
let markers = [];

window.initializeMap = function (users) {
    // Check if map div exists
    const mapElement = document.getElementById('map');
    if (!mapElement) {
        console.error('Map element not found');
        return;
    }

    try {
        // Initialize the map
        map = new google.maps.Map(mapElement, {
            zoom: 7,
            center: { lat: 8.9824, lng: -79.5197 }, // Panama center coordinates
            mapTypeControl: true,
            streetViewControl: true,
            fullscreenControl: true
        });

        const bounds = new google.maps.LatLngBounds();

        // Clear existing markers
        markers.forEach(marker => marker.setMap(null));
        markers = [];

        // Add markers for each user
        users.forEach(user => {
            // Convert string coordinates to numbers
            const lat = typeof user.latitude === 'string' ? parseFloat(user.latitude) : user.latitude;
            const lng = typeof user.longitude === 'string' ? parseFloat(user.longitude) : user.longitude;

            // Verify coordinates are valid numbers
            if (isNaN(lat) || isNaN(lng)) {
                console.error('Invalid coordinates for user:', user);
                return;
            }

            const position = { lat, lng };

            const marker = new google.maps.Marker({
                position,
                map,
                title: user.name,
                animation: google.maps.Animation.DROP
            });

            const infoWindow = new google.maps.InfoWindow({
                content: `
                    <div style="padding: 10px;">
                        <h5 style="margin: 0 0 5px 0;">${user.name}</h5>
                        <p style="margin: 0 0 3px 0;"><strong>Role:</strong> ${user.role}</p>
                        <p style="margin: 0;"><strong>Email:</strong> ${user.email}</p>
                    </div>
                `
            });

            marker.addListener('click', () => {
                infoWindow.open(map, marker);
            });

            bounds.extend(position);
            markers.push(marker);
        });

        // Fit map to show all markers if there are any
        if (markers.length > 0) {
            map.fitBounds(bounds);
        }
    } catch (error) {
        console.error('Error initializing map:', error);
    }
}

// Add error handler for Google Maps
window.gm_authFailure = function () {
    console.error('Google Maps authentication failed. Please check your API key.');
    document.getElementById('map').innerHTML =
        '<div class="alert alert-danger">Failed to load Google Maps. Please check the API key.</div>';
}