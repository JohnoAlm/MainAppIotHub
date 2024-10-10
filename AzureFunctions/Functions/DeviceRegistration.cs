using IotHubResources.Handlers;
using IotHubResources.Managers;
using IotHubResources.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.Functions
{
    // todo Publicera upp till en Azure Function.
    public class DeviceRegistration
    {
        private readonly ILogger<DeviceRegistration> _logger;
        private readonly IotDeviceRegistrationManager _iotDeviceRegistrationManager;

        public DeviceRegistration(ILogger<DeviceRegistration> logger, IotDeviceRegistrationManager iotDeviceRegistrationManager)
        {
            _logger = logger;
            _iotDeviceRegistrationManager = iotDeviceRegistrationManager;
        }

        [Function("DeviceRegistration")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var iotDeviceRegistrationRequest = JsonConvert.DeserializeObject<IotDeviceRegistrationRequest>(body);

            if (iotDeviceRegistrationRequest == null || string.IsNullOrEmpty(iotDeviceRegistrationRequest.DeviceId) || string.IsNullOrEmpty(iotDeviceRegistrationRequest.DeviceName))
                return new BadRequestObjectResult("Invalid request. 'deviceId' or 'deviceName' is missing");

            var result = await _iotDeviceRegistrationManager.RegisterDeviceAsync(iotDeviceRegistrationRequest.DeviceId, iotDeviceRegistrationRequest.DeviceName);

            var response = new IotDeviceRegistrationResponse
            {
                DeviceId = result.Device?.Id,
                ConnectionString = result.ConnectionString,
                DeviceName = result.Twin?.Properties.Desired["deviceName"].ToString()
            };

            return new OkObjectResult(response);
        }
    }
}
