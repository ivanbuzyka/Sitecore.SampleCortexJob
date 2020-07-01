using System;
using System.Collections.Generic;
using System.Text;
using XcExport.Cortex.MsSqlStorage.Model;
using System.Data.SqlClient;

namespace XcExport.Cortex.Storage
{
    public static class MsSqlStorage
    {
        public const string ConnectionString = "user id=sa;password=Sitecore12345;data source=LT-IBU01-T-DE;Initial Catalog=agi_export";
        public static void WriteContacts(IEnumerable<ContactEntity> contacts)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                foreach(var contact in contacts)
                {
                    var contactExists = false;

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "select ContactId from Contacts where ContactId = @contactId";
                        command.Parameters.AddWithValue("@contactId", contact.ContactId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                contactExists = true;
                            }
                        }
                    }

                    if(!contactExists)
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            //ToDo: check whether contact already exists
                            command.CommandText = "INSERT INTO Contacts(ContactId,Email,FirstName,LastName) VALUES(@param1,@param2,@param3,@param4)";

                            command.Parameters.AddWithValue("@param1", contact.ContactId);
                            command.Parameters.AddWithValue("@param2", contact.Email);
                            command.Parameters.AddWithValue("@param3", contact.FirstName);
                            command.Parameters.AddWithValue("@param4", contact.LastName);

                            command.ExecuteNonQuery();
                        }
                    }

                    // Insert Interactions
                    foreach(var interaction in contact.Interactions)
                    {
                        var interactionExists = false;
                        // check for existing interaction
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "select InteractionId from Interactions where InteractionId = @interactionId";
                            command.Parameters.AddWithValue("@interactionId", interaction.InteractionId);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    interactionExists = true;
                                }
                            }
                        }

                        if(!interactionExists)
                        {
                            using (SqlCommand command = connection.CreateCommand())
                            {
                                //ToDo: check whether contact already exists
                                command.CommandText = "INSERT INTO Interactions(InteractionId,UserAgent,Duration,ContactId,StartDateTime,IpInfo,City) VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7)";

                                command.Parameters.AddWithValue("@param1", interaction.InteractionId);
                                command.Parameters.AddWithValue("@param2", interaction.UserAgent);
                                command.Parameters.AddWithValue("@param3", interaction.Duration.Ticks);
                                command.Parameters.AddWithValue("@param4", contact.ContactId);
                                command.Parameters.AddWithValue("@param5", interaction.StartDateTime);
                                command.Parameters.AddWithValue("@param6", interaction.IpInfo);
                                command.Parameters.AddWithValue("@param7", interaction.City);

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }                
            }
        }
    }
}
