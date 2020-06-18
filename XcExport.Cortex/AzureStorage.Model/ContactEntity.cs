using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace XcExport.Cortex.AzureStorage.Model
{
    public class ContactEntity : TableEntity
    {
        public static string PartitionKey = "Cortex.ContactEntity";
        public ContactEntity(string email, Guid? contactId, string firstName, string lastName)
        {
            base.PartitionKey = PartitionKey;
            RowKey = contactId.ToString();

            Email = email;
            FirstName = firstName;
            ContactId = contactId;
            LastName = lastName;
        }

        public ContactEntity() { }

        public string Email { get; set; }
        public Guid? ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
