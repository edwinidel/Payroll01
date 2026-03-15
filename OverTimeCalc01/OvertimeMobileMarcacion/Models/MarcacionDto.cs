namespace OvertimeMobileMarcacion.Models;

public class MarcacionDto
{
    public string? EmployeeCode { get; set; }
    public string? Id { get; set; }
    public DateTime Hora { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PhotoBase64 { get; set; }
}