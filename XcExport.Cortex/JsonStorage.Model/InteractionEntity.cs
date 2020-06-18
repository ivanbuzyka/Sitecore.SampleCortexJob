using System;

namespace XcExport.Cortex.JsonStorage.Model
{
    public class InteractionEntity
    {
        public InteractionEntity(string userAgent, TimeSpan duration, Guid? interactionId, Guid? contactId, DateTime startDateTime, string ipInfo, string city)
        {
            UserAgent = userAgent;
            Duration = duration;
            InteractionId = interactionId;
            ContactId = contactId;
            StartDateTime = startDateTime;
            IpInfo = ipInfo;
            City = city;
        }

        public InteractionEntity() { }

        public string UserAgent { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid? InteractionId { get; set; }
        public Guid? ContactId { get; set; }
        public DateTime StartDateTime { get; set; }
        public string IpInfo { get; set; }
        public string City { get; set; }
    }
}
