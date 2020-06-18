using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using XcExport.Cortex.Storage;

namespace XcExport.Cortex.Workers
{
    public class ExportInteractionsToAzureTable : IDistributedWorker<Interaction>
    {
        private readonly IXdbContext _xdbContext;

        public ExportInteractionsToAzureTable(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public void Dispose() => _xdbContext.Dispose();

        public async Task ProcessBatchAsync(IReadOnlyList<Interaction> batch, CancellationToken token)
        {
            var tableClient = AzureStorageTable.GetAzTableClient();
            var azureTableBatchOperation = new TableBatchOperation();
            var portionSize = 70;
            var portionCounter = 0;


            foreach (var interaction in batch)
            {
                // this is related to Azure Table storage, bach size is limited there, therefore it flushes every "portionSize" the batches to table storage
                if (portionCounter >= portionSize)
                {
                    await tableClient.ExecuteBatchAsync(azureTableBatchOperation);
                    azureTableBatchOperation.Clear();
                    portionCounter = 0;
                }

                var ipAddress = interaction.GetFacet<IpInfo>();

                azureTableBatchOperation.InsertOrReplace(
                    new AzureStorage.Model.InteractionEntity(
                        interaction.UserAgent,
                        interaction.Duration,
                        interaction.Id,
                        interaction.Contact.Id,
                        interaction.StartDateTime,
                        ipAddress == null ? string.Empty : ipAddress.IpAddress,
                        ipAddress == null ? string.Empty : ipAddress.City
                        )
                    );

                portionCounter++;


                await tableClient.ExecuteBatchAsync(azureTableBatchOperation);

                await Task.FromResult(1);
            }
        }
    }
}
