using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ExportXConnectDataToTableStorage.Cortex.AzureStorage.Model
{
    public class IntractionEntity: TableEntity
    {
        public static string PartitionKey = "CortexInteractions.IntractionEntity";
        public IntractionEntity(string userAgent, TimeSpan duration, Guid? interactionId, Guid? contactId, DateTime startDateTime, string ipInfo, string city)
        {
            base.PartitionKey = PartitionKey;
            base.RowKey = interactionId.ToString();

            UserAgent = userAgent;
            Duration = duration;
            InteractionId = interactionId;
            ContactId = contactId;
            StartDateTime = startDateTime;
            IpInfo = ipInfo;
            City = city;
        }

        public IntractionEntity() {}

        public string UserAgent { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid? InteractionId { get; set; }
        public Guid? ContactId { get; set; }
        public DateTime StartDateTime { get; set; }
        public string IpInfo { get; set; }
        public string City { get; set; }
    }
}
