namespace LostTech.Cloud {
    using System.Threading;
    using System.Threading.Tasks;
    public interface ICloudInstanceInfoProvider {
        Task<CloudInstanceInfo> GetInstanceInfoAsync(CancellationToken cancellation = default);
    }
}
