
using DeviceId;
using Rssdp;

public class AutoDiscoveryService : BackgroundService
{
  private ILogger<AutoDiscoveryService> _logger;
  private IServiceProvider _services;
  private SsdpDevicePublisher _Publisher;
  
  public AutoDiscoveryService(ILogger<AutoDiscoveryService> logger, IServiceProvider services)
  {
    _logger = logger;
    _logger.LogInformation("AutoDiscoveryService Initialized.");
    _services = services;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await DoWork(stoppingToken);
  }

  private async Task DoWork(CancellationToken stoppingToken)
  {
    _logger.LogInformation("AutoDiscoveryService is working.");
    // using (var scope = _services.CreateScope())
    // {
    PublishDevice();
    await Task.Delay(10000);
    await SearchForDevices();
    await Task.Delay(10000);




    // }
    _logger.LogInformation("AutoDiscoveryService is stopping.");
  }

  public void PublishDevice()
  {
      // As this is a sample, we are only setting the minimum required properties.
      var deviceDefinition = new SsdpRootDevice()
      {
          CacheLifetime = TimeSpan.FromMinutes(30), //How long SSDP clients can cache this info.
          Location = new Uri("http://mydevice/descriptiondocument.xml"), // Must point to the URL that serves your devices UPnP description document. 
          DeviceTypeNamespace = "my-namespace",
          DeviceType = "MyCustomDevice",
          FriendlyName = "Custom Device 1",
          Manufacturer = "Me",
          ModelName = "MyCustomDevice",
          Uuid = GetPersistentUuid() // This must be a globally unique value that survives reboots etc. Get from storage or embedded hardware etc.
      };
    _Publisher = new SsdpDevicePublisher();
    _Publisher.AddDevice(deviceDefinition);
  }

  private string GetPersistentUuid()
  {
    string deviceId = new DeviceIdBuilder()
      .AddMachineName()
      .AddOsVersion()
      .AddFileToken("example-device-token.txt")
      .ToString();
    _logger.LogInformation($"Current DeviceId: '{deviceId}'");
    return deviceId;
  }

  //Call this method from somewhere to begin the search.
public async Task SearchForDevices()
{
    // This code goes in a method somewhere.
    using (var deviceLocator = new SsdpDeviceLocator())
    {
        var foundDevices = await deviceLocator.SearchAsync(); // Can pass search arguments here (device type, uuid). No arguments means all devices.

        foreach (var foundDevice in foundDevices)
        {
          try{

            // Device data returned only contains basic device details and location ]
            // of full device description.
            _logger.LogInformation("Found " + foundDevice.Usn + " at " + foundDevice.DescriptionLocation.ToString());

            // Can retrieve the full device description easily though.
            var fullDevice = await foundDevice.GetDeviceInfo();
            _logger.LogInformation(fullDevice.FriendlyName);
          }
          catch (Exception exc){
            _logger.LogError(exc, $"Failed to get device info for {foundDevice}");
          }
        }
    }
}

}

public static class AutoDiscoveryServiceExtensions
{
  public static IServiceCollection AddAutoDiscoveryService(this IServiceCollection services)
  {
    return services.AddHostedService<AutoDiscoveryService>();
  }
}