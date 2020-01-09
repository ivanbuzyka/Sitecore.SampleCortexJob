using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExportXConnectDataToTableStorage.Cortex.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace ExportXConnectDataToTableStorage.Cortex
{
    class ExportToAzTableStorageUsingInteractions : IDistributedWorker<Interaction>
    {
        private readonly IXdbContext _xdbContext;

        public ExportToAzTableStorageUsingInteractions(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public void Dispose() => _xdbContext.Dispose();

        public async Task ProcessBatchAsync(IReadOnlyList<Interaction> batch, CancellationToken token)
        {
            var tableClient = GetAzTableClient();            
            var azureTableBatchOperation = new TableBatchOperation();
            var portionSize = 70;
            var portionCounter = 0;

            // Selected data is alrteady in batch collection 
            foreach (var interaction in batch)
            {                
                // this is related to Azure Table storage, bach size is limited there, therefore it flushes every "portionSize" the batches to table storage
                if(portionCounter >= portionSize)
                {
                    await tableClient.ExecuteBatchAsync(azureTableBatchOperation);
                    azureTableBatchOperation.Clear();
                    portionCounter = 0;
                }

                // getting ipAddress facet
                var ipAddress = interaction.GetFacet<IpInfo>();                
                azureTableBatchOperation.InsertOrReplace(
                    new IntractionEntity(
                        interaction.UserAgent,
                        interaction.Duration,
                        interaction.Id,
                        interaction.Contact.Id,
                        interaction.StartDateTime,
                        ipAddress.IpAddress,
                        ipAddress.City
                        )
                    );
                
                portionCounter++;
            }

            await tableClient.ExecuteBatchAsync(azureTableBatchOperation);

            await Task.FromResult(1);
        }

        public static CloudTable GetAzTableClient()
        {
            //completely Azure Storage Table business, connection strings quickly and dirty hard-coded
            var debugConnString = "DefaultEndpointsProtocol=https;AccountName=nameofstorageaccount;AccountKey=storage-account-key-goes-here;EndpointSuffix=core.windows.net";
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(debugConnString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table. Hard-coded for now
            CloudTable table = tableClient.GetTableReference("CortexDataStore");

            return table;
        }
    }
}
