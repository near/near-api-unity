using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class Near
    {
        private readonly AccountCreator _accountCreator;
        private readonly NearConfig _config;
        private readonly Connection _connection;

        public Near(NearConfig config)
        {
            _config = config;

            dynamic providerArgs = new ExpandoObject();
            providerArgs.Url = config.NodeUrl;
            dynamic signerArgs = new ExpandoObject();
            signerArgs.KeyStore = config.KeyStore;
            var connectionConfig = new ConnectionConfig()
            {
                NetworkId = config.NetworkId,
                Provider = new ProviderConfig()
                {
                    Type = config.ProviderType,
                    Args = providerArgs
                },
                Signer = new SignerConfig()
                {
                    Type = config.SignerType,
                    Args = signerArgs
                }
            };
            _connection = Connection.FromConfig(connectionConfig);

            if (config.MasterAccount != null)
            {
                // TODO: figure out better way of specifiying initial balance.
                var initialBalance = config.InitialBalance > 0
                    ? config.InitialBalance
                    : new UInt128(1000 * 1000) * new UInt128(1000 * 1000);
                _accountCreator =
                    new LocalAccountCreator(new Account(_connection, config.MasterAccount), initialBalance);
            }
            else if (config.HelperUrl != null)
            {
                _accountCreator = new UrlAccountCreator(_connection, config.HelperUrl);
            }
            else
            {
                _accountCreator = null;
            }
        }

        public AccountCreator AccountCreator => _accountCreator;
        public NearConfig Config => _config;
        public Connection Connection => _connection;

        public static async Task<Near> ConnectAsync(dynamic config)
        {
            // Try to find extra key in `KeyPath` if provided.
            if (config.KeyPath == null) return new Near(config);
            try
            {
                var accountKeyFile = await UnencryptedFileSystemKeyStore.ReadKeyFile(config.keyPath);
                if (accountKeyFile[0] != null)
                {
                    // TODO: Only load key if network ID matches
                    var keyPair = accountKeyFile[1];
                    var keyPathStore = new InMemoryKeyStore();
                    await keyPathStore.SetKeyAsync(config.NetworkId, accountKeyFile[0], keyPair);
                    if (config.MasterAccount == null)
                    {
                        config.MasterAccount = accountKeyFile[0];
                    }

                    config.KeyStore = new MergeKeyStore(new KeyStore[] { config.KeyStore, keyPathStore });
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"Failed to load master account key from {config.KeyPath}: {error}");
            }

            return new Near(config);
        }

        public async Task<Account> AccountAsync(string accountId)
        {
            var account = new Account(_connection, accountId);
            await account.FetchStateAsync();
            return account;
        }

        public async Task<Account> CreateAccountAsync(string accountId, PublicKey publicKey)
        {
            if (_accountCreator == null)
            {
                throw new Exception(
                    "Must specify account creator, either via masterAccount or helperUrl configuration settings.");
            }

            await _accountCreator.CreateAccountAsync(accountId, publicKey);
            return new Account(_connection, accountId);
        }
    }
}