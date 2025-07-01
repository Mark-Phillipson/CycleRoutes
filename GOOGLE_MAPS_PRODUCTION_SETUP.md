# Google Maps Production Setup Guide

This guide explains how to configure Google Maps APIs for production deployment, particularly addressing the common issue where Street View works in development but not in production.

## The Problem

Google Maps Embed API (used for Street View iframes) has security restrictions that require proper configuration of HTTP referrers in production environments. Without this configuration, you'll see:
- Blank/gray Street View panels
- Console errors about referrer restrictions
- "This page can't load Google Maps correctly" messages

## Solution Steps

### 1. Configure API Key Restrictions in Google Cloud Console

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Navigate to **APIs & Services** > **Credentials**
3. Find your Google Maps API key and click on it
4. Under **Application restrictions**, select **HTTP referrers (web sites)**
5. Add the following referrers:

   ```
   # For Azure App Service (replace with your actual domain)
   https://cycleroutes-web-*.azurewebsites.net/*
   
   # For custom domains (if applicable)
   https://yourdomain.com/*
   https://www.yourdomain.com/*
   
   # For development (keep this for local testing)
   http://localhost:*
   https://localhost:*
   
   # For any other environments
   https://your-staging-domain.com/*
   ```

### 2. Required APIs

Ensure these APIs are **enabled** in Google Cloud Console:

- **Maps Embed API** ✅ (for Street View and map iframes)
- **Maps JavaScript API** ✅ (for interactive route map)
- **Geocoding API** ✅ (for API key validation)
- **Street View Static API** (optional, for fallback images)

### 3. API Key Configuration

Your API key should have permissions for the above APIs. To check:

1. In Google Cloud Console, go to **APIs & Services** > **Enabled APIs**
2. Verify all required APIs are listed
3. If missing, click **+ ENABLE APIS AND SERVICES** and search for the missing APIs

### 4. Testing the Configuration

After configuring the referrers:

1. **Wait 5-10 minutes** for changes to propagate
2. Deploy your application to production
3. Open browser dev tools (F12) and check the Console tab
4. Look for any Google Maps related errors

### 5. Common URLs Used by CycleRoutes

The application uses these Google Maps URLs:

```
# Street View Embed
https://www.google.com/maps/embed/v1/streetview?key=API_KEY&location=LAT,LNG&heading=0&pitch=0&fov=90

# Fallback Map View
https://www.google.com/maps/embed/v1/view?key=API_KEY&center=LAT,LNG&zoom=18&maptype=roadmap

# Maps JavaScript API
https://maps.googleapis.com/maps/api/js?key=API_KEY&callback=initGoogleMaps

# Geocoding API (for validation)
https://maps.googleapis.com/maps/api/geocode/json?address=London&key=API_KEY
```

### 6. Troubleshooting

If Street View still doesn't work:

1. **Check browser console** for error messages
2. **Verify API key** is correctly set in production configuration
3. **Test API key** using the "Validate API Key" button in the app
4. **Check quotas** in Google Cloud Console > APIs & Services > Quotas
5. **Verify billing** is enabled for your Google Cloud project

### 7. Environment Variables

Ensure your production environment has:

```json
{
  "GoogleMaps": {
    "ApiKey": "your-actual-api-key-here"
  }
}
```

For Azure App Service, set this in:
- **Configuration** > **Application settings**
- Add: `GoogleMaps__ApiKey` = `your-api-key`

### 8. Security Best Practices

- ✅ Use HTTP referrer restrictions (not IP restrictions for web apps)
- ✅ Limit API key to only required APIs
- ✅ Monitor usage in Google Cloud Console
- ✅ Set up billing alerts
- ❌ Never commit API keys to source control
- ❌ Don't use the same API key for different environments

### 9. Alternative: Multiple API Keys

For better security, consider using different API keys for different environments:

```json
// Development
"GoogleMaps": { "ApiKey": "dev-api-key-with-localhost-referrer" }

// Production  
"GoogleMaps": { "ApiKey": "prod-api-key-with-domain-referrer" }
```

## Additional Resources

- [Google Maps Platform Documentation](https://developers.google.com/maps/documentation)
- [API Key Restrictions Guide](https://developers.google.com/maps/api-key-best-practices)
- [Maps Embed API Documentation](https://developers.google.com/maps/documentation/embed/get-started)

## Support

If you continue to experience issues:
1. Check the application logs for Street View service errors
2. Use browser dev tools to inspect network requests
3. Verify the generated iframe URLs are correct
4. Test the API key directly in Google's API explorer
