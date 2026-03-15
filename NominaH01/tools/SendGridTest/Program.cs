using System.Text;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

// Use explicit project settings path for local test
var projectSettingsPath = "/Users/edwin/Documents/Projects/2FA01/2FA";
var config = new ConfigurationBuilder()
    .SetBasePath(projectSettingsPath)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

// Allow overriding via environment variables in CI
var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? config["SendGridSettings:ApiKey"];
var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM") ?? config["SendGridSettings:FromEmail"];
var fromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME") ?? config["SendGridSettings:EmailName"] ?? "2FA Test";

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("SendGrid ApiKey not found in configuration. Set SendGridSettings:ApiKey in appsettings or environment.");
    return 1;
}
if (string.IsNullOrEmpty(fromEmail))
{
    Console.WriteLine("FromEmail not found in configuration.");
    return 1;
}

var client = new SendGridClient(apiKey);
var msg = new SendGridMessage()
{
    From = new EmailAddress(fromEmail, fromName),
    Subject = "[2FA] Test email",
    HtmlContent = "<strong>This is a test email from the 2FA project.</strong>"
};

// send to the From address to self-verify
msg.AddTo(fromEmail);

Console.WriteLine($"Sending test email to {fromEmail}...");
var resp = await client.SendEmailAsync(msg);
Console.WriteLine($"Response: {resp.StatusCode} - {resp.Headers}");
var body = await resp.Body.ReadAsStringAsync();
Console.WriteLine(body);
return 0;
