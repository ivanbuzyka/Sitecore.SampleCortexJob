using Sitecore.XConnect;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XcExport.Cortex.Workers
{
    public class ExportContactsToMsSql : Sitecore.Processing.Engine.Abstractions.IDistributedWorker<Sitecore.XConnect.Contact>
    {
        private readonly Sitecore.XConnect.IXdbContext _xdbContext;

        public ExportContactsToMsSql(Sitecore.XConnect.IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public void Dispose() => _xdbContext.Dispose();

        public async Task ProcessBatchAsync(IReadOnlyList<Contact> batch, CancellationToken token)
        {
            var contacts = new List<MsSqlStorage.Model.ContactEntity>();
            // Selected data is alrteady in batch collection 
            foreach (var contact in batch)
            {
                var contactEmails = contact.GetFacet<Sitecore.XConnect.Collection.Model.EmailAddressList>();
                var contactPersonalInformation = contact.GetFacet<Sitecore.XConnect.Collection.Model.PersonalInformation>();
                var c = new MsSqlStorage.Model.ContactEntity()
                {
                    Email = contactEmails == null ? "N/A" : contactEmails.PreferredEmail.SmtpAddress,
                    ContactId = contact.Id,
                    FirstName = contactPersonalInformation == null ? "N/A" : contactPersonalInformation.FirstName,
                    LastName = contactPersonalInformation == null ? "N/A" : contactPersonalInformation.LastName,
                    Interactions = new List<MsSqlStorage.Model.InteractionEntity>()
                };

                var interactions = contact.Interactions;

                foreach (var interaction in interactions)
                {
                    var ipAddress = interaction.GetFacet<Sitecore.XConnect.Collection.Model.IpInfo>();

                    c.Interactions.Add(
                        new MsSqlStorage.Model.InteractionEntity(
                            string.IsNullOrEmpty(interaction.UserAgent) ? string.Empty : interaction.UserAgent,
                            interaction.Duration,
                            interaction.Id,
                            interaction.Contact.Id,
                            interaction.StartDateTime,
                            ipAddress == null || string.IsNullOrEmpty(ipAddress.IpAddress) ? string.Empty : ipAddress.IpAddress,
                            ipAddress == null || string.IsNullOrEmpty(ipAddress.City) ? string.Empty : ipAddress.City
                            )
                        );
                }

                contacts.Add(c);
            }

            Storage.MsSqlStorage.WriteContacts(contacts);

            await Task.FromResult(1);
        }
    }
}
