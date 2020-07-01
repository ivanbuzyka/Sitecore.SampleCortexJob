using System;
using System.Collections.Generic;
using System.Text;

namespace XcExport.Cortex.MsSqlStorage.Model
{
    public class ContactEntity
    {
        public ContactEntity(string email, Guid? contactId, string firstName, string lastName)
        {
            Email = email;
            ContactId = contactId;
            FirstName = firstName;
            LastName = lastName;
        }

        public ContactEntity() { }

        public string Email { get; set; }
        public Guid? ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<InteractionEntity> Interactions { get; set; }
    }
}
