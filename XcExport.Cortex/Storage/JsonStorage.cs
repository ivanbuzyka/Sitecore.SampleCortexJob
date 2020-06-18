using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using XcExport.Cortex.JsonStorage.Model;

namespace XcExport.Cortex.Storage
{
    public static class JsonStorage
    {
        private const string InteractionsJsonFilePath = @"App_Data\interactions.json";
        private const string ContactsJsonFilePath = @"App_Data\contacts.json";

        public static void WriteInteractionsWithOverwrite(IEnumerable<InteractionEntity> entities)
        {            
            File.WriteAllText(InteractionsJsonFilePath, JsonConvert.SerializeObject(entities));
        }

        public static void WriteContactsWithOverwrite(IEnumerable<ContactEntity> entities)
        {
            File.WriteAllText(ContactsJsonFilePath, JsonConvert.SerializeObject(entities));
        }
    }
}
