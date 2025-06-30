# Google Maps Integration

## Overview

The CycleRoutes application now includes Google Maps integration to provide a visual route overview alongside the Street View navigation. This feature displays your entire route on an interactive map with real-time position tracking.

## Features

### Route Visualization
- **Complete Route Display**: Shows the entire GPX route as a red polyline on the map
- **Start/End Markers**: Green marker (S) for route start, red marker (E) for route end
- **Current Position Indicator**: Blue marker that tracks your current position as you navigate

### Interactive Map
- **Auto-fit to Route**: Map automatically adjusts zoom and center to show the entire route
- **Position Tracking**: As you advance through route points, the map smoothly pans to follow your current location
- **Standard Google Maps Controls**: Zoom, pan, and map type switching available

## Configuration

### API Key Setup
To enable Google Maps functionality, you need to configure a Google Maps API key:

1. **Get an API Key**:
   - Go to the [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one
   - Enable the "Maps JavaScript API" and "Maps Embed API"
   - Create an API key with appropriate restrictions

2. **Configure the Application**:
   - Add your API key to `appsettings.json`:
   ```json
   {
     "GoogleMaps": {
       "ApiKey": "YOUR_API_KEY_HERE"
     }
   }
   ```

3. **API Restrictions** (Recommended):
   - Restrict the API key to your domain/localhost
   - Only enable the required APIs: Maps JavaScript API, Maps Embed API

### Platform Support
- **Web Application**: Full Google Maps integration with interactive map
- **MAUI Application**: Maps not available (displays status message)
- **WebAssembly**: Maps not available (displays status message)

## Usage

### Loading a Route
1. Upload a GPX file or select a saved route
2. The map will automatically initialize and display the route
3. The Street View and map will both show the starting position

### Navigation
- **Manual Navigation**: Use Previous/Next buttons or click the progress bar
- **Auto-advance**: Enable auto-advance to automatically progress through route points
- **Position Sync**: Both Street View and map position update simultaneously

### Map Controls
- **Zoom**: Use mouse wheel or map controls to zoom in/out
- **Pan**: Click and drag to explore different areas of the route
- **Map Type**: Use Google Maps controls to switch between map, satellite, terrain views

## Technical Implementation

### Services
- `IGoogleMapsService`: Interface for Google Maps functionality
- `GoogleMapsService`: Web implementation with full Google Maps integration
- Platform-specific implementations for MAUI and WebAssembly (stub implementations)

### Key Components
- **Map Container**: `<div id="routeMap">` element hosts the Google Maps instance
- **JavaScript Integration**: Dynamic script loading and map initialization
- **Position Updates**: Real-time marker position updates via JavaScript interop

### Map Features
- **Route Polyline**: GPX track displayed as red line with 3px stroke
- **Markers**: Custom SVG markers for start (green), end (red), and current position (blue)
- **Bounds Fitting**: Automatic zoom level adjustment to show entire route
- **Error Handling**: Graceful fallback when API key is missing or invalid

## Troubleshooting

### Map Not Loading
- **Check API Key**: Ensure the API key is correctly configured in appsettings.json
- **Verify APIs**: Confirm "Maps JavaScript API" is enabled in Google Cloud Console
- **Check Restrictions**: Ensure API key restrictions allow your domain/localhost
- **Browser Console**: Check for JavaScript errors related to Google Maps

### Position Not Updating
- **Route Navigation**: Ensure you're in navigation mode (started following route)
- **JavaScript Errors**: Check browser console for script errors
- **API Limits**: Verify you haven't exceeded API usage quotas

### Common Error Messages
- "No API key configured": Add GoogleMaps:ApiKey to appsettings.json
- "API key is invalid": Check API key and enabled services in Google Cloud Console
- "Google Maps not available": Platform doesn't support maps (MAUI/WebAssembly)

## Performance Considerations

- **Map Loading**: Initial map load may take 1-2 seconds depending on route complexity
- **API Calls**: Map uses Google Maps JavaScript API (not per-request billing)
- **Memory Usage**: Large routes (1000+ points) may use significant browser memory
- **Network**: Initial API script loading requires internet connection

## Future Enhancements

Potential improvements for future versions:
- **Offline Maps**: Integration with offline mapping solutions
- **Route Editing**: Click-to-modify route points on the map
- **Elevation Profile**: Display elevation changes along the route
- **Multiple Routes**: Support for displaying multiple routes simultaneously
- **Custom Map Styles**: Cycling-specific map styling options
