### Installing with installer
* Double click and run the installer just like any software
* After installation, search for "Configure EdnaApi" in start menu to open appsettings.json file and change settings like port, log level etc
* After installation, search for "Restart EdnaApi" in start menu to restart the Edna api web server
* Uninstall the software in control panel or "Add or remove programs" menu just like any software
* The web api runs as a windows background service named "edna_api"

### Command to publish the web-api as a self-contained web server
https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained

```
dotnet publish --self-contained -r win-x64 .\src\EdnaApi\EdnaApi.csproj
```

### run dotnet server at custom port
* https://stackoverflow.com/questions/37365277/how-to-specify-the-port-an-asp-net-core-application-is-hosted-on
* Using command line arguments, by starting your .NET application with --urls=[url]
```
dotnet run --urls=http://localhost:5001/
```
* Using appsettings.json, by adding a Urls node
```json
{
  "Urls": "http://0.0.0.0:5001"
}
```

### Swagger integration
* Official docs - https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
* swagger ui available at ```/swagger``` path of the web application

### Create Installer
* First publish the web api as a self-contained web server
* Open `installerScripts/installerGenScript.iss` in Inno Setup
* Build installer exe file using Build->Compile in Inno Setup