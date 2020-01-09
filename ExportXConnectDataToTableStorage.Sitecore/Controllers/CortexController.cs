using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Tasks.Options.DataSources.DataExtraction;
using Sitecore.XConnect;
using Sitecore.XConnect.Client.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExportXConnectDataToTableStorage.Sitecore.Controllers
{
    public class CortexController : Controller
    {
        private readonly ITaskManager _taskManager;

        public CortexController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task<ActionResult> RegisterExportToAzTableStorage()
        {
            using (IXdbContext client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                var contactFacets = client.Model.Facets.Where(c => c.Target == EntityType.Contact).Select(x => x.Name);

                var expandOptions = new InteractionExpandOptions(interactionFacets.ToArray())
                {
                    Contact = new RelatedContactExpandOptions(contactFacets.ToArray())
                };

                InteractionDataSourceOptionsDictionary interactionDataSourceOptionsDictionary =
                    new InteractionDataSourceOptionsDictionary(expandOptions, 1000, 100);                

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    interactionDataSourceOptionsDictionary,
                    new DistributedWorkerOptionsDictionary(
                        "ExportXConnectDataToTableStorage.Cortex.ExportToAzTableStorageUsingInteractions, ExportXConnectDataToTableStorage.Cortex", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return Json(new { TaskId = taskId.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<JsonResult> GetTaskStatus(Guid taskId)
        {
            var processingTaskProgress = await _taskManager.GetTaskProgressAsync(taskId);
            return Json(processingTaskProgress, JsonRequestBehavior.AllowGet);
        }        
    }
}