using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XcExport.Cortex.JsonStorage.Model;

namespace XcExport.Cortex.Workers
{
    class ExportContactsToJson : IDistributedWorker<Contact>
    {
        private readonly IXdbContext _xdbContext;

        public ExportContactsToJson(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public void Dispose() => _xdbContext.Dispose();

        public async Task ProcessBatchAsync(IReadOnlyList<Contact> batch, CancellationToken token)
        {
            var contacts = new List<ContactEntity>();
            // Selected data is alrteady in batch collection 
            foreach (var contact in batch)
            {
                var contactEmails = contact.GetFacet<EmailAddressList>();
                var contactPersonalInformation = contact.GetFacet<PersonalInformation>();
                var c = new ContactEntity()
                {
                    Email = contactEmails == null ? "N/A" : contactEmails.PreferredEmail.SmtpAddress,
                    ContactId = contact.Id,
                    FirstName = contactPersonalInformation == null ? "N/A" : contactPersonalInformation.FirstName,
                    LastName = contactPersonalInformation == null ? "N/A" : contactPersonalInformation.LastName,
                    Interactions = new List<InteractionEntity>()
                };

                var interactions = contact.Interactions;                               

                foreach (var interaction in interactions)
                {                    
                    var ipAddress = interaction.GetFacet<IpInfo>();

                    c.Interactions.Add(
                        new JsonStorage.Model.InteractionEntity(
                            interaction.UserAgent,
                            interaction.Duration,
                            interaction.Id,
                            interaction.Contact.Id,
                            interaction.StartDateTime,
                            ipAddress == null ? string.Empty : ipAddress.IpAddress,
                            ipAddress == null ? string.Empty : ipAddress.City
                            )
                        );                 
                }

                contacts.Add(c);
            }

            //  CAUTION: caution, since cortex jobs is executed in batches, it is possible that
            //  there are more than one batch, therefore Json file will be overwritten and only 
            //  contents of last batch will be saved
            XcExport.Cortex.Storage.JsonStorage.WriteContactsWithOverwrite(contacts);

            await Task.FromResult(1);
        }
    }
}
