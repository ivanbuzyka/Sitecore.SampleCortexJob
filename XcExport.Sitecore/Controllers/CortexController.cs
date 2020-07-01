using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Tasks.Options.DataSources.DataExtraction;
using Sitecore.Processing.Tasks.Options.DataSources.Search;
using Sitecore.XConnect;
using Sitecore.XConnect.Client.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace XcExport.Sitecore.Controllers
{
    public class CortexController : Controller
    {
        private readonly ITaskManager _taskManager;

        public CortexController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task<ActionResult> RegisterInteractionsExportToJson()
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
                    new InteractionDataSourceOptionsDictionary(expandOptions, 1000, 1000);

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    interactionDataSourceOptionsDictionary,
                    new DistributedWorkerOptionsDictionary(
                        "XcExport.Cortex.Workers.ExportInteractionsToJson, XcExport.Cortex", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return Json(new { TaskId = taskId.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> RegisterContactsExportToJson()
        {
            using (IXdbContext client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                var contactFacets = client.Model.Facets.Where(c => c.Target == EntityType.Contact).Select(x => x.Name);

                var searchRequest = client.Contacts
                    .Where(c => c.Interactions.Any())
                    .WithExpandOptions(new ContactExpandOptions(contactFacets.ToArray())
                    {
                        Interactions = new RelatedInteractionsExpandOptions(interactionFacets.ToArray())
                        {
                            StartDateTime = DateTime.Now.AddDays(-3),
                            EndDateTime = DateTime.Now,
                            Limit = int.MaxValue
                        }
                    })
                    .GetSearchRequest();

                var dataSourceOptions = new ContactSearchDataSourceOptionsDictionary(searchRequest, 1000, 1000);

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    dataSourceOptions,
                    new DistributedWorkerOptionsDictionary(
                        "XcExport.Cortex.Workers.ExportContactsToJson, XcExport.Cortex", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return Json(new { TaskId = taskId.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> RegisterContactsExportToMsSql()
        {
            using (IXdbContext client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                var contactFacets = client.Model.Facets.Where(c => c.Target == EntityType.Contact).Select(x => x.Name);

                var searchRequest = client.Contacts
                    .Where(c => c.Interactions.Any())
                    .WithExpandOptions(new ContactExpandOptions(contactFacets.ToArray())
                    {
                        Interactions = new RelatedInteractionsExpandOptions(interactionFacets.ToArray())
                        {
                            //StartDateTime = DateTime.Now.AddDays(-3),
                            //EndDateTime = DateTime.Now,
                            Limit = int.MaxValue
                        }
                    })
                    .GetSearchRequest();

                var dataSourceOptions = new ContactSearchDataSourceOptionsDictionary(searchRequest, 1000, 1000);

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    dataSourceOptions,
                    new DistributedWorkerOptionsDictionary(
                        "XcExport.Cortex.Workers.ExportContactsToMsSql, XcExport.Cortex", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return Json(new { TaskId = taskId.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> RegisterInteractionsExportToAzureTable()
        {
            using (IXdbContext client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                var contactFacets = client.Model.Facets.Where(c => c.Target == EntityType.Contact).Select(x => x.Name);

                // Expand Options: https://doc.sitecore.com/developers/92/sitecore-experience-platform/en/expand-options-overview.html
                var expandOptions = new InteractionExpandOptions(interactionFacets.ToArray())
                {
                    Contact = new RelatedContactExpandOptions(contactFacets.ToArray())
                };

                InteractionDataSourceOptionsDictionary interactionDataSourceOptionsDictionary =
                    new InteractionDataSourceOptionsDictionary(expandOptions, 1000, 1000);

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    interactionDataSourceOptionsDictionary,
                    new DistributedWorkerOptionsDictionary(
                        "XcExport.Cortex.Workers.ExportInteractionsToAzureTable, XcExport.Cortex", workerOptions),
                    null,
                    TimeSpan.FromHours(1));

                return Json(new { TaskId = taskId.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> RegisterContactsExportToAzureTable()
        {
            using (IXdbContext client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                var contactFacets = client.Model.Facets.Where(c => c.Target == EntityType.Contact).Select(x => x.Name);

               var searchRequest = client.Contacts
                   .Where(c => c.Interactions.Any())
                   .WithExpandOptions(new ContactExpandOptions(contactFacets.ToArray())
                   {
                       Interactions = new RelatedInteractionsExpandOptions(interactionFacets.ToArray())
                       {
                           StartDateTime = DateTime.Now.AddDays(-3),
                           EndDateTime = DateTime.Now,
                           Limit = int.MaxValue
                       }
                   })
                   .GetSearchRequest();

                var dataSourceOptions = new ContactSearchDataSourceOptionsDictionary(searchRequest, 1000, 1000);

                var workerOptions = new Dictionary<string, string>();

                var taskId = await _taskManager.RegisterDistributedTaskAsync(
                    dataSourceOptions,
                    new DistributedWorkerOptionsDictionary(
                        "XcExport.Cortex.Workers.ExportContactsToAzureTable, XcExport.Cortex", workerOptions),
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