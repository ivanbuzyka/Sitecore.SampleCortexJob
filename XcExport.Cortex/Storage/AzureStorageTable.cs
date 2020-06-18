using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace XcExport.Cortex.Storage
{
    public static class AzureStorageTable
    {
        public const string StorageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=ibustatic1;AccountKey=sTqazckdEmOesyhNUoGLSWZNmCLpxRzumRlysxNcg3h166uGRr9+F1FgHBzEABmY2XXrPnBfR+zx9RpU1iroog==;EndpointSuffix=core.windows.net";

        public const string TableName = "CortexDataStore";

        public static CloudTable GetAzTableClient()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageAccountConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                        
            CloudTable table = tableClient.GetTableReference(TableName);

            return table;
        }
    }
}
