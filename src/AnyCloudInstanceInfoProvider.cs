namespace LostTech.Cloud {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LostTech.Cloud.AWS;
    using LostTech.Cloud.Azure;
    public sealed class AnyCloudInstanceInfoProvider: ICloudInstanceInfoProvider {
        static readonly ICloudInstanceInfoProvider[] providers = {
            new AzureCloudInstanceInfoProvider(),
            new AwsCloudInstanceInfoProvider(),
        };

        public async Task<CloudInstanceInfo> GetInstanceInfoAsync(CancellationToken cancellation = default) {
            using var stillPendingCancellation = new CancellationTokenSource();
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation, stillPendingCancellation.Token);
            var queries = providers.Select(provider => provider.GetInstanceInfoAsync(linkedCancellation.Token)).ToList();
            var failures = new List<Exception>();
            while(queries.Count > 0) {
                var finished = await Task.WhenAny(queries).ConfigureAwait(false);
                queries.Remove(finished);
                if (finished.Status == TaskStatus.RanToCompletion) {
                    stillPendingCancellation.Cancel();
                    return finished.Result;
                } else {
                    failures.Add(finished.Exception);
                }
            }

            throw new AggregateException(failures);
        }
    }
}
