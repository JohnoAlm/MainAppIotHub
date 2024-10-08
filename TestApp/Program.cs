using IotHubResources.Handlers;
using IotHubResources.Models;
using System.Net.Http.Json;

namespace TestApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ReadKey();

            var iotDeviceRegistrationRequest = new IotDeviceRegistrationRequest
            {
                DeviceId = "cfe51819-462c-4606-a6f1-aa2a3b0a29fd",
                DeviceName = "Dev App"
            };

            using var httpClient = new HttpClient();
            var result = await httpClient.PostAsJsonAsync("http://localhost:7057/api/DeviceRegistration", iotDeviceRegistrationRequest);

            Console.ReadKey();
        }
    }
}
