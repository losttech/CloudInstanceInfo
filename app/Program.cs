namespace LostTech.Cloud {
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    class Program {
        static async Task Main() {
            var provider = new AnyCloudInstanceInfoProvider();
            var info = await provider.GetInstanceInfoAsync();
            string json = JsonSerializer.Serialize(info, jsonSerializerOptions);
            Console.WriteLine(json);
        }

        static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
            WriteIndented = true,
        };
    }
}
