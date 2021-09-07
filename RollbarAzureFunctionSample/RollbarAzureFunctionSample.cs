using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Rollbar;

namespace RollbarAzureFunctionSample
{
    public static class RollbarAzureFunctionSample
    {
        [FunctionName("RollbarAzureFunctionSample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            Microsoft.Extensions.Logging.ILogger log)
        {

            //Config Rollbar in Azure Function
            RollbarConfig _rollbarConfig = new RollbarConfig("YOUR_ACCESS_TOKEN") { Environment = "production" };
            _rollbarConfig.Server = new Rollbar.DTOs.Server { Host = "localhost", Root = "/Users/nkruger/Projects/RollbarAzureFunctionSample/RollbarAzureFunctionSample", Branch = "master" };
            RollbarLocator.RollbarInstance.Configure(_rollbarConfig);

            //Example Rollbar Info
            RollbarLocator.RollbarInstance.Info("C# HTTP trigger function processed a request with Rollbar.");

            //Basic Azure Logging
            log.LogInformation("C# HTTP trigger function processed a request.");

            try {

                //Standard Azure Function
                string name = req.Query["name"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered function executed successfully.";

                int intValue1 = 100;
                int intValue2 = 0;
                int intValue3 = intValue1 / intValue2;

                return new OkObjectResult(responseMessage);

            } catch (Exception ex) {
                //Log Error
                RollbarLocator.RollbarInstance.Error(ex);

                return new BadRequestObjectResult(ex);
            }


        }
    }
}
