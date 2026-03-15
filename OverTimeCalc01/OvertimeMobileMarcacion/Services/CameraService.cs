using Microsoft.Maui.Media;

namespace OvertimeMobileMarcacion.Services;

public class CameraService
{
    public async Task<string?> CapturePhotoAsBase64()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo == null) return null;

            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}