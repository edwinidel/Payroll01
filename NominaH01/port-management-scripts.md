# Port Management Scripts for .NET Development

## Quick Port Release Script

Create this script to quickly release port 7094:

```bash
#!/bin/bash
# Kill any process using port 7094
lsof -ti:7094 | xargs kill -9 2>/dev/null || echo "Port 7094 is now available"

# Alternative method using netstat
# netstat -tulpn | grep :7094 | awk '{print $7}' | cut -d/ -f1 | xargs kill -9 2>/dev/null || echo "Port 7094 is available"
```

## Check Port Usage

To see what's using port 7094:

```bash
# Method 1: Using lsof
lsof -i:7094

# Method 2: Using netstat
netstat -tulpn | grep :7094

# Method 3: Using ss (modern replacement for netstat)
ss -tulpn | grep :7094
```

## VS Code Tasks Configuration

Add this to your `.vscode/tasks.json` to create automated port cleanup:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Kill Port 7094",
            "type": "shell",
            "command": "lsof -ti:7094 | xargs kill -9 2>/dev/null || echo 'Port 7094 is available'",
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "silent",
                "focus": false,
                "panel": "shared"
            }
        },
        {
            "label": "Clean Build",
            "dependsOn": "Kill Port 7094",
            "command": "dotnet clean",
            "group": "build"
        }
    ]
}
```

## Launch Configuration

Update your `.vscode/launch.json` to automatically clean up:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web) - Clean Port First",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "Kill Port 7094",
            "program": "${workspaceFolder}/2FA/bin/Debug/net8.0/2FA.dll",
            "args": [],
            "cwd": "${workspaceFolder}/2FA",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development",
                "ASPNETCORE_URLS": "https://localhost:7094;http://localhost:5094"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/2FA/Views"
            }
        }
    ]
}
```

## VS Code Extension Recommendations

1. **.NET Install Tool** - Automatically manages .NET SDK versions
2. **C# Dev Kit** - Provides better debugging experience
3. **Error Lens** - Shows errors inline

## Additional Solutions

### 1. Graceful Shutdown

Ensure your application shuts down gracefully by adding this to your `Program.cs`:

```csharp
var host = CreateHostBuilder(args).Build();

// Add graceful shutdown
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application is shutting down gracefully...");
});

await host.RunAsync();
```

### 2. Port Configuration

Modify your `appsettings.json` to use dynamic ports:

```json
{
  "Urls": "https://localhost:0;http://localhost:0"
}
```

Or in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Use random available ports
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(0); // HTTP
    options.ListenLocalhost(0, listenOptions => listenOptions.UseHttps()); // HTTPS
});

var app = builder.Build();
```

### 3. Process Management

Add this to your debugging workflow:

```bash
# Before starting debug
pkill -f "2FA.dll"  # Kill any existing instances
sleep 1

# Check if port is free
if lsof -i:7094 >/dev/null 2>&1; then
    echo "Port 7094 is still in use. Trying to free it..."
    lsof -ti:7094 | xargs kill -9
    sleep 2
fi
```

## Prevention Tips

1. **Always stop debugging properly** - Use the stop button in VS Code
2. **Check running processes** before starting a new debug session
3. **Use different ports** for different configurations
4. **Add pre-launch tasks** to your launch configuration
5. **Set up graceful shutdown** in your application