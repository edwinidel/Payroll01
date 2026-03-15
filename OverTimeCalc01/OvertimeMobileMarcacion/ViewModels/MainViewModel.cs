using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OvertimeMobileMarcacion.Services;
using OvertimeMobileMarcacion.Models;

namespace OvertimeMobileMarcacion.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isSingleEmployeeMode = true;

    [ObservableProperty]
    private string employeeCode = string.Empty;

    [ObservableProperty]
    private string apiUrl = "http://localhost:5031";

    [ObservableProperty]
    private string token = string.Empty;

    [ObservableProperty]
    private double allowedLatitude = 9.0;

    [ObservableProperty]
    private double allowedLongitude = -79.5;

    [ObservableProperty]
    private double allowedRadius = 100;

    [RelayCommand]
    private void SelectSingleEmployeeMode() => IsSingleEmployeeMode = true;

    [RelayCommand]
    private void SelectMultiEmployeeMode() => IsSingleEmployeeMode = false;

    [RelayCommand]
    private async Task MarkEntry() => await PerformMarcacion("Entrada");

    [RelayCommand]
    private async Task MarkExit() => await PerformMarcacion("Salida");

    [RelayCommand]
    private async Task MarkInicioComida() => await PerformMarcacion("Inicio Comida");

    [RelayCommand]
    private async Task MarkFinComida() => await PerformMarcacion("Fin Comida");

    private async Task PerformMarcacion(string type)
    {
        var locationService = new Services.LocationService();
        var location = await locationService.GetCurrentLocation();
        if (location == null || !locationService.IsWithinAllowedLocation(location, AllowedLatitude, AllowedLongitude, AllowedRadius))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Ubicación no válida", "OK");
            return;
        }

        string? photoBase64 = null;
        if (!IsSingleEmployeeMode)
        {
            var cameraService = new Services.CameraService();
            photoBase64 = await cameraService.CapturePhotoAsBase64();
            if (photoBase64 == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo capturar la foto", "OK");
                return;
            }
        }

        var marcacion = new Models.MarcacionDto
        {
            EmployeeCode = EmployeeCode,
            Id = type,
            Hora = DateTime.Now,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            PhotoBase64 = photoBase64
        };

        var apiService = new Services.ApiService();
        var success = await apiService.SendMarcacion(marcacion, ApiUrl, Token);
        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", $"Marcación {type} registrada", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Error al enviar marcación", "OK");
        }
    }
}