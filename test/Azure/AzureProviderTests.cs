namespace LostTech.Cloud.Azure {
    using System;
    using System.Threading.Tasks;

    using RichardSzalay.MockHttp;

    using Xunit;
    public class AzureProviderTests {
        [Fact]
        public async Task CorrectlyParsesReponse() {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://127.0.0.1/metadata/instance?api-version=2019-06-01")
                    .Respond("application/json", SampleResponse);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://127.0.0.1");
            var provider = new AzureCloudInstanceInfoProvider(httpClient);
            var info = await provider.GetInstanceInfoAsync();
            Assert.Equal(0, info.AttachedDevices.Count);
            Assert.Equal("Standard_A1_v2", info.Size);
            Assert.Equal("Microsoft.Compute/AzurePublicCloud", info.Environment);
        }

        const string SampleResponse = @"
{
  ""compute"": {
    ""azEnvironment"": ""AzurePublicCloud"",
    ""customData"": """",
    ""location"": ""centralus"",
    ""name"": ""negasonic"",
    ""offer"": ""lampstack"",
    ""osType"": ""Linux"",
    ""placementGroupId"": """",
    ""plan"": {
        ""name"": ""5-6"",
        ""product"": ""lampstack"",
        ""publisher"": ""bitnami""
    },
    ""platformFaultDomain"": ""0"",
    ""platformUpdateDomain"": ""0"",
    ""provider"": ""Microsoft.Compute"",
    ""publicKeys"": [],
    ""publisher"": ""bitnami"",
    ""resourceGroupName"": ""myrg"",
    ""resourceId"": ""/subscriptions/xxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx/resourceGroups/myrg/providers/Microsoft.Compute/virtualMachines/negasonic"",
    ""sku"": ""5-6"",
    ""storageProfile"": {
        ""dataDisks"": [
          {
            ""caching"": ""None"",
            ""createOption"": ""Empty"",
            ""diskSizeGB"": ""1024"",
            ""image"": {
              ""uri"": """"
            },
            ""lun"": ""0"",
            ""managedDisk"": {
              ""id"": ""/subscriptions/xxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx/resourceGroups/macikgo-test-may-23/providers/Microsoft.Compute/disks/exampledatadiskname"",
              ""storageAccountType"": ""Standard_LRS""
            },
            ""name"": ""exampledatadiskname"",
            ""vhd"": {
    ""uri"": """"
            },
            ""writeAcceleratorEnabled"": ""false""
          }
        ],
        ""imageReference"": {
    ""id"": """",
          ""offer"": ""UbuntuServer"",
          ""publisher"": ""Canonical"",
          ""sku"": ""16.04.0-LTS"",
          ""version"": ""latest""
        },
        ""osDisk"": {
    ""caching"": ""ReadWrite"",
          ""createOption"": ""FromImage"",
          ""diskSizeGB"": ""30"",
          ""diffDiskSettings"": {
        ""option"": ""Local""
          },
          ""encryptionSettings"": {
        ""enabled"": ""false""
          },
          ""image"": {
        ""uri"": """"
          },
          ""managedDisk"": {
        ""id"": ""/subscriptions/xxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx/resourceGroups/macikgo-test-may-23/providers/Microsoft.Compute/disks/exampleosdiskname"",
            ""storageAccountType"": ""Standard_LRS""
          },
          ""name"": ""exampleosdiskname"",
          ""osType"": ""Linux"",
          ""vhd"": {
        ""uri"": """"
          },
          ""writeAcceleratorEnabled"": ""false""
        }
    },
    ""subscriptionId"": ""13f56399-bd52-4150-9748-7190aae1ff21"",
    ""tags"": ""Department:IT;Environment:Prod;Role:WorkerRole"",
    ""version"": ""7.1.1902271506"",
    ""vmId"": ""13f56399-bd52-4150-9748-7190aae1ff21"",
    ""vmScaleSetName"": """",
    ""vmSize"": ""Standard_A1_v2"",
    ""zone"": ""1""
  },
  ""network"": {
    ""interface"": [
      {
        ""ipv4"": {
            ""ipAddress"": [
              {
                ""privateIpAddress"": ""10.1.2.5"",
              ""publicIpAddress"": ""X.X.X.X""
              }
          ],
          ""subnet"": [
            {
                ""address"": ""10.1.2.0"",
              ""prefix"": ""24""
            }
          ]
        },
        ""ipv6"": {
            ""ipAddress"": []
        },
        ""macAddress"": ""000D3A36DDED""
      }
    ]
  }
}";
    }
}
