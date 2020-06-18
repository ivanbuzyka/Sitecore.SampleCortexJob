using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace XcExport.Cortex.Workers
{
    public class ExportInteractionsToJson : IDistributedWorker<Interaction>
    {
        private readonly IXdbContext _xdbContext;

        public ExportInteractionsToJson(IXdbContext xdbContext, IReadOnlyDictionary<string, string> options)
        {
            _xdbContext = xdbContext;
        }

        public void Dispose() => _xdbContext.Dispose();

        public async Task ProcessBatchAsync(IReadOnlyList<Interaction> batch, CancellationToken token)
        {
            var interactionEntities = new List<JsonStorage.Model.InteractionEntity>();

            foreach (var interaction in batch)
            {
                var ipAddress = interaction.GetFacet<IpInfo>();

                interactionEntities.Add(
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

                XcExport.Cortex.Storage.JsonStorage.WriteInteractionsWithOverwrite(interactionEntities);

                await Task.FromResult(1);
            }
        }
    }
}
