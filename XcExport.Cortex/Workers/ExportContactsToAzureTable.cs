using Microsoft.WindowsAzure.Storage.Table;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XcExport.Cortex.AzureStorage.Model;
using XcExport.Cortex.JsonStorage.Model;
using XcExport.Cortex.Storage;

namespace XcExport.Cortex.Workers
{
    class ExportContactsToAzureTable : IDistributedWorker<Contact>
    {
        private readonly IXdbContext _xdbContext;

        public ExportContactsToAzureTable(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public void Dispose() => _xdbContext.Dispose();

        public async Task ProcessBatchAsync(IReadOnlyList<Contact> batch, CancellationToken token)
        {
            var tableClient = AzureStorageTable.GetAzTableClient();
            var azureTableBatchOperation = new TableBatchOperation();
            var portionSize = 70;
            var portionCounter = 0;

            // Selected data is alrteady in batch collection 
            foreach (var contact in batch)
            {
                if (portionCounter >= portionSize)
                {
                    await tableClient.ExecuteBatchAsync(azureTableBatchOperation);
                    azureTableBatchOperation.Clear();
                    portionCounter = 0;
                }

                var contactEmails = contact.GetFacet<EmailAddressList>();
                var contactPersonalInformation = contact.GetFacet<PersonalInformation>();
                azureTableBatchOperation.InsertOrReplace(new AzureStorage.Model.ContactEntity
                (
                    contactEmails == null ? "N/A" : contactEmails.PreferredEmail.SmtpAddress,
                    contact.Id,
                    contactPersonalInformation == null ? "N/A" : contactPersonalInformation.FirstName,
                    contactPersonalInformation == null ? "N/A" : contactPersonalInformation.LastName
                ));

                portionCounter++;
            }

            await tableClient.ExecuteBatchAsync(azureTableBatchOperation);

            await Task.FromResult(1);
        }
    }
}
