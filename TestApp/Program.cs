using IotHubResources.Handlers;
using IotHubResources.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace TestApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Console.ReadKey();

            var drr = new IotDeviceRegistrationRequest
            {
                DeviceId = "ec420d92-d7ea-4f5c-8abf-0774b9db9ac1",
                DeviceName = "Dev App"
            };

            using var http = new HttpClient();
            var result = await http.PostAsJsonAsync("https://olivera-iot-fa.azurewebsites.net/api/DeviceRegistration?code=q2QFeKdG9GTfjHxf48nRBOjaJyp5XHqR6Y-0l_XbFh22AzFuoFPhrA%3D%3D", drr);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<IotDeviceRegistrationResponse>(content);

            DeviceClient client = DeviceClient.CreateFromConnectionString(response!.ConnectionString);

            var twinCollection = new TwinCollection();
            twinCollection["deviceName"] = response.DeviceName;
            twinCollection["deviceType"] = "console app";

            await client.UpdateReportedPropertiesAsync(twinCollection);

            Console.ReadKey();
        }
    }
}
