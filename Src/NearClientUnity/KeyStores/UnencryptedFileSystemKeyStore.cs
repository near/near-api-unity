using NearClientUnity.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearClientUnity.KeyStores
{
    public class UnencryptedFileSystemKeyStore : KeyStore
    {
        private readonly string _keyDir;

        public UnencryptedFileSystemKeyStore(string keyDir)
        {
            _keyDir = keyDir;
        }

        public static async Task EnsureDir(string path)
        {
            await Task.Run(() => Directory.CreateDirectory(path));
        }

        public static async Task<dynamic> LoadJsonFile(string path)
        {
            using (var sourceStream = new FileStream(path,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                var sb = new StringBuilder();

                var buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    var text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                dynamic result = JObject.Parse(sb.ToString());
                return result;
            }
        }

        public static async Task<dynamic[]> ReadKeyFile(string path)
        {
            var accountInfo = await LoadJsonFile(path);
            string privateKey = accountInfo.PrivateKey;
            if (privateKey == null && accountInfo.SecretKey != null) privateKey = accountInfo.SecretKey;
            return new dynamic[] { accountInfo.AccountId, KeyPair.FromString(privateKey) };
        }

        public override async Task ClearAsync()
        {
            foreach (var network in await GetNetworksAsync())
            {
                foreach (var account in await GetAccountsAsync(network))
                {
                    await RemoveKeyAsync(network, account);
                }

                await RemoveNetworkAsync(network);
            }
        }

        public override async Task<string[]> GetAccountsAsync(string networkId)
        {
            return await Task.Factory.StartNew(() =>
            {
                if (!Directory.Exists($"{_keyDir}/{networkId}")) return new string[0];
                return Directory.GetFiles($"{_keyDir}/{networkId}", "*.json")
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
            });
        }

        public override async Task<KeyPair> GetKeyAsync(string networkId, string accountId)
        {
            if (!File.Exists(GetKeyFilePath(networkId, accountId))) return null;
            var accountKeyPair = await ReadKeyFile(GetKeyFilePath(networkId, accountId));
            return accountKeyPair[1];
        }

        public override async Task<string[]> GetNetworksAsync()
        {
            return await Task.Factory.StartNew(() =>
            {
                if (!Directory.Exists(_keyDir)) return new string[0];
                var directory = new DirectoryInfo(_keyDir);
                var networks = directory.GetDirectories().Select(subDirectory => subDirectory.Name).ToList();
                return networks.ToArray();
            });
        }

        public override async Task RemoveKeyAsync(string networkId, string accountId)
        {
            var filePath = GetKeyFilePath(networkId, accountId);
            if (File.Exists(filePath))
            {
                await Task.Factory.StartNew(() =>
                {
                    File.Delete(filePath);
                });
            }
        }

        public async Task RemoveNetworkAsync(string networkId)
        {
            var dirPath = GetNetworkDirPath(networkId);
            if (Directory.Exists(dirPath))
            {
                await Task.Factory.StartNew(() =>
                {
                    Directory.Delete(dirPath);
                });
            }
        }

        public override async Task SetKeyAsync(string networkId, string accountId, KeyPair keyPair)
        {
            await EnsureDir($"{_keyDir}/{networkId}");
            var content = new AccountInfo() { AccountId = accountId, PrivateKey = keyPair.ToString() };
            var jsonString = JsonConvert.SerializeObject(content);
            var encodedJsonString = Encoding.Unicode.GetBytes(jsonString);

            using (var sourceStream = new FileStream(GetKeyFilePath(networkId, accountId),
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedJsonString, 0, encodedJsonString.Length);
            };
        }

        private string GetKeyFilePath(string networkId, string accountId)
        {
            return $"{_keyDir}/{networkId}/{accountId}.json";
        }

        private string GetNetworkDirPath(string networkId)
        {
            return $"{_keyDir}/{networkId}/";
        }
    }
}