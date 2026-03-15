using Microsoft.Maui.Devices.Sensors;

namespace OvertimeMobileMarcacion.Services;

public class LocationService
{
    public async Task<Location?> GetCurrentLocation()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);
            return location;
        }
        catch (Exception ex)
        {
            // Log or handle
            return null;
        }
    }

    public bool IsWithinAllowedLocation(Location currentLocation, double allowedLat, double allowedLon, double radiusMeters)
    {
        var distance = Location.CalculateDistance(currentLocation.Latitude, currentLocation.Longitude, allowedLat, allowedLon, DistanceUnits.Kilometers) * 1000;
        return distance <= radiusMeters;
    }
}