# CycleRoutes - GPX Route Viewer with Google Street View

A Blazor web application for viewing cycling routes with Google Street View integration. Import GPX files and explore your routes with immersive street-level navigation before you ride.

## Features

- 📁 **GPX File Import**: Upload and parse GPX route files
- 🗺️ **Google Street View Integration**: View routes with automatic heading calculation
- 📱 **Responsive Design**: Works on desktop and mobile devices
- 💾 **Route Management**: Save and manage multiple routes
- 🧭 **Point-by-Point Navigation**: Step through route points with Street View
- ⚡ **Real-time Route Stats**: Distance, elevation, and estimated duration

## Getting Started

### Prerequisites

- .NET 9.0 or later
- Modern web browser with internet access

### Running the Application

1. Clone the repository and navigate to the web project:
   ```bash
   cd CycleRoutes.Web
   dotnet run
   ```

2. Open your browser and navigate to `http://localhost:5178`

### Using the Application

1. **Import a Route**:
   - Navigate to the "Routes" page
   - Click "Choose File" and select a GPX file
   - The route will be automatically parsed and saved

2. **View Route in Street View**:
   - Select a saved route from the list
   - Click "Start Following Route" to begin navigation
   - Use Previous/Next buttons to move through route points
   - Street View automatically orients based on your direction of travel

3. **Route Management**:
   - Save multiple routes for quick access
   - View route statistics (distance, points, duration)
   - Delete routes you no longer need

## Configuration

### Google Maps API Key Setup

The application includes automatic API key validation to help you troubleshoot configuration issues:

1. **Get a Google Maps API Key**:
   - Visit [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one
   - Enable the **Maps Embed API** and **Geocoding API**
   - Create credentials (API Key)
   - Restrict the key to your domain for security

2. **Configure the API Key**:
   - Open `appsettings.json` or `appsettings.Development.json`
   - Add your API key:
   ```json
   {
     "GoogleMaps": {
       "ApiKey": "your-api-key-here"
     }
   }
   ```

3. **API Key Status Messages**:
   - **"API key is valid"**: Everything is working correctly
   - **"API key is invalid or has insufficient permissions"**: Check your key and API enablement
   - **"No API key configured"**: Add your key to appsettings.json
   - **"API key not yet validated"**: Validation is in progress

The app automatically validates your API key on startup and shows helpful error messages if there are issues.

## Sample Route

A sample GPX file (`sample-london-loop.gpx`) is included in the repository for testing. This route takes you through central London landmarks.

## Technical Details

### Architecture

- **Frontend**: Blazor Server + WebAssembly hybrid
- **Backend**: ASP.NET Core
- **File Parsing**: Custom XML-based GPX parser
- **Mapping**: Google Street View embeds

### Project Structure

- `CycleRoutes.Web`: Main web application
- `CycleRoutes.Shared`: Shared components and services
- `CycleRoutes.Web.Client`: WebAssembly client components

### Key Components

- **RouteService**: Handles GPX parsing and route management
- **StreetViewService**: Google Street View URL generation
- **GpxParser**: Custom GPX file parser
- **Routes.razor**: Main route viewing component

## Development

### Building the Solution

```bash
dotnet build CycleRoutes.slnx
```

### Project Dependencies

- Microsoft.AspNetCore.Components.Web
- System.Text.Json
- Microsoft.AspNetCore.Components.WebAssembly.Server

## Future Enhancements

- 🗺️ Interactive route map overlay
- 📍 Real-time GPS tracking
- 🚴‍♂️ Route planning and editing
- 📊 Advanced route analytics
- 🌤️ Weather integration
- 📲 Progressive Web App (PWA) support

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is open source and available under the MIT License.

## Support

For issues or questions, please open an issue on the repository.
