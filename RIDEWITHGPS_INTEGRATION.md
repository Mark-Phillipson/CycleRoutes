# RideWithGPS API Integration Plan

## Overview

This document outlines the integration plan for connecting CycleRoutes with the RideWithGPS API to access routes from cycling organizations like the San Fairy Ann Cycling Club.

**Date Created**: June 30, 2025  
**Status**: Planning Phase - Awaiting API Key Request Response  
**Target Organization**: [San Fairy Ann Cycling Club](https://ridewithgps.com/organizations/633-san-fairy-ann-cycling-club)

## API Availability ✅

RideWithGPS provides a comprehensive REST API that is actively maintained and well-documented.

### Key Resources
- **API Documentation**: https://ridewithgps.com/api
- **Developer GitHub**: https://github.com/ridewithgps/developers
- **Integrations Page**: https://ridewithgps.com/integrations
- **API Key Request**: Email `info@ridewithgps.com`
- **Developer Settings**: https://ridewithgps.com/settings/developers

## API Capabilities

### Available Endpoints
- **Routes** (`/api/v1/routes.json`) - Route management and retrieval
- **Users** (`/api/v1/users.json`) - User authentication and profiles
- **Trips** (`/api/v1/trips.json`) - Recorded ride data
- **Events** (`/api/v1/events.json`) - Event information
- **Sync** (`/api/v1/sync.json`) - Data synchronization

### Authentication Methods
- **OAuth** (Preferred) - Configurable from API client management
- **Basic Auth** - Using API key as username and user tokens as password
- **API Key Required** - For all requests via `apikey` parameter

### Data Format
- **JSON Only** - All requests and responses
- **Content-Type**: `application/json` or `.json` extension
- **Current Version**: `v1` (available at `https://ridewithgps.com/api/v1`)

## Route Data Structure

### Route Object Properties
```json
{
  "route": {
    "id": 999,
    "url": "https://ridewithgps.com/api/v1/routes/999.json",
    "name": "Route Name",
    "description": "Route description",
    "locality": "City",
    "administrative_area": "State/Province",
    "country_code": "US",
    "distance": 21453,
    "elevation_gain": 133,
    "elevation_loss": 134,
    "first_lat": 45.58787,
    "first_lng": -122.69944,
    "last_lat": 45.58824,
    "last_lng": -122.69944,
    "sw_lat": 45.52858,
    "sw_lng": -122.70294,
    "ne_lat": 45.58824,
    "ne_lng": -122.64282,
    "track_type": "loop",
    "terrain": "rolling",
    "difficulty": "easy",
    "unpaved_pct": 5,
    "surface": "mostly_paved",
    "created_at": "2024-01-16T17:46:23-08:00",
    "updated_at": "2024-01-22T16:42:37-08:00",
    "track_points": [...],
    "course_points": [...],
    "points_of_interest": [...]
  }
}
```

### Track Points Structure
```json
{
  "x": -122.69944,  // Longitude
  "y": 45.58708,    // Latitude
  "e": 25.5,        // Elevation
  "d": 87.9         // Distance
}
```

### Course Points (Turn-by-Turn)
```json
{
  "x": -122.69944,
  "y": 45.58708,
  "d": 87.9,
  "i": 0,
  "t": "Right",
  "n": "Turn right onto North Houghton Street"
}
```

### Points of Interest
```json
{
  "id": 1,
  "type": "convenience_store",
  "type_id": 24,
  "type_name": "Convenience Store",
  "name": "Seven Eleven",
  "description": "Get snacks on the way home.",
  "url": null,
  "lat": 45.561964,
  "lng": -122.68902
}
```

## Integration Architecture

### Service Interface Design

Create `IRideWithGpsService` in `CycleRoutes.Shared/Services/`:

```csharp
public interface IRideWithGpsService
{
    Task<IEnumerable<CycleRoute>> GetOrganizationRoutesAsync(int organizationId);
    Task<CycleRoute?> GetRouteDetailsAsync(int routeId);
    Task<IEnumerable<CycleRoute>> SearchRoutesAsync(string query, double? lat = null, double? lng = null);
    Task<bool> AuthenticateAsync(string email, string password);
    Task<User?> GetCurrentUserAsync();
}
```

### Implementation Projects

1. **CycleRoutes.Shared** - Service interface and models
2. **CycleRoutes.Web** - Server-side implementation
3. **CycleRoutes.Web.Client** - WebAssembly implementation
4. **CycleRoutes (MAUI)** - Mobile/desktop implementation

### Configuration Structure

```json
{
  "RideWithGps": {
    "ApiKey": "your-api-key-here",
    "BaseUrl": "https://ridewithgps.com/api/v1",
    "Version": "1",
    "Organizations": {
      "SanFairyAnn": 633
    }
  }
}
```

## Model Extensions

### Enhanced CycleRoute Model

```csharp
public class CycleRoute
{
    // Existing properties...
    
    // RideWithGPS Integration
    public int? RideWithGpsId { get; set; }
    public string? RideWithGpsUrl { get; set; }
    public string? Locality { get; set; }
    public string? AdministrativeArea { get; set; }
    public string? CountryCode { get; set; }
    public string? TrackType { get; set; } // "loop", "out_and_back", "point_to_point"
    public string? Terrain { get; set; } // "flat", "rolling", "hilly", "mountainous"
    public string? Difficulty { get; set; } // "easy", "moderate", "difficult"
    public int? UnpavedPercentage { get; set; }
    public string? Surface { get; set; } // "paved", "mostly_paved", "mixed", "gravel"
    public DateTime? RideWithGpsCreatedAt { get; set; }
    public DateTime? RideWithGpsUpdatedAt { get; set; }
    
    // Bounding box
    public double? SouthWestLat { get; set; }
    public double? SouthWestLng { get; set; }
    public double? NorthEastLat { get; set; }
    public double? NorthEastLng { get; set; }
    
    // Points of Interest
    public List<PointOfInterest>? PointsOfInterest { get; set; }
}
```

### New Models

```csharp
public class PointOfInterest
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class RideWithGpsUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? AuthToken { get; set; }
}
```

## Implementation Steps

### Phase 1: Foundation
1. **Request API Key** from `info@ridewithgps.com`
2. **Create service interface** in `CycleRoutes.Shared`
3. **Extend models** to support RideWithGPS data
4. **Add configuration** classes and settings

### Phase 2: Core Implementation
1. **Implement HTTP client** with authentication
2. **Create route parsing** from RideWithGPS format to CycleRoute
3. **Add error handling** and retry logic
4. **Implement caching** for route data

### Phase 3: UI Integration
1. **Add "Import from RideWithGPS"** button
2. **Create organization browser** for San Fairy Ann CC
3. **Implement route preview** with RideWithGPS metadata
4. **Add filtering** by difficulty, terrain, etc.

### Phase 4: Advanced Features
1. **Sync capabilities** for route updates
2. **User authentication** with RideWithGPS accounts
3. **Export routes** back to RideWithGPS
4. **Webhooks** for real-time updates

## API Usage Examples

### Authentication
```http
GET /api/v1/users/current.json?email={email}&password={password}&apikey={api_key}&version=1
```

### Get Routes
```http
GET /api/v1/routes.json?apikey={api_key}&version=1&page=1
Authorization: Basic {base64(api_key:auth_token)}
```

### Get Specific Route
```http
GET /api/v1/routes/{route_id}.json?apikey={api_key}&version=1
Authorization: Basic {base64(api_key:auth_token)}
```

### Organization Routes (Hypothetical)
```http
GET /api/v1/organizations/{org_id}/routes.json?apikey={api_key}&version=1
```

## Error Handling

### Expected HTTP Status Codes
- `200 OK` - Successful request
- `201 Created` - Resource created
- `400 Bad Request` - Malformed request
- `401 Not Authorized` - Authentication failed
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Error Response Format
```json
{
  "error": "Failed to authenticate the user"
}
```

## Pagination Support

```json
{
  "routes": [...],
  "meta": {
    "pagination": {
      "record_count": 207,
      "page_count": 11,
      "next_page_url": "https://ridewithgps.com/api/v1/routes.json?page=2"
    }
  }
}
```

## Security Considerations

1. **API Key Storage** - Use secure configuration management
2. **Token Expiration** - Handle auth token refresh
3. **Rate Limiting** - Implement request throttling
4. **HTTPS Only** - All API calls must use SSL
5. **User Data** - Respect privacy and data protection laws

## Testing Strategy

1. **Unit Tests** - Service layer and data parsing
2. **Integration Tests** - API connectivity and authentication
3. **Mock API** - For development without API key
4. **Load Testing** - Performance with large route datasets

## Next Steps

1. **✅ Research Complete** - API availability confirmed
2. **📧 Pending** - API key request to `info@ridewithgps.com`
3. **⏳ Waiting** - Response from RideWithGPS team
4. **🚧 Future** - Begin implementation once API access is granted

## Contact Information

- **RideWithGPS API Support**: developers@ridewithgps.com
- **San Fairy Ann CC Contact**: technical@sanfairyanncc.co.uk (Duncan Edwards)
- **API Key Requests**: info@ridewithgps.com

## Useful Links

- [RideWithGPS API Playground](https://ridewithgps.com/api)
- [San Fairy Ann Cycling Club](https://ridewithgps.com/organizations/633-san-fairy-ann-cycling-club)
- [San Fairy Ann Route Library](https://ridewithgps.com/clubs/633-san-fairy-ann-cycling-club/routes)
- [RideWithGPS Route Map View](https://ridewithgps.com/clubs/633-san-fairy-ann-cycling-club/routes/explore)

---

**Last Updated**: June 30, 2025  
**Review Date**: Upon API key approval  
**Implementation Target**: Q3 2025
