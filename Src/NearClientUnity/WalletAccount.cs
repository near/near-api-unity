using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace NearClientUnity
{
    public class WalletAccount
    {
        private const string LoginWalletUrlSuffix = "/login/";
        private const string LocalStorageKeySuffix = "_wallet_auth_key";
        private const string PendingAccessKeyPrefix = "pending_key";

        private string _walletBaseUrl;
        private string _authDataKey;
        private KeyStore _keyStore;
        private dynamic _authData = new ExpandoObject();
        private string _networkId;
        private IExternalAuthService _authService;
        public AppSettingsSection _nearLocalStorage;

        public WalletAccount(Near near, string appKeyPrefix, IExternalAuthService authService)
        {
            _networkId = near.Config.NetworkId;
            _walletBaseUrl = near.Config.WalletUrl;
            appKeyPrefix = string.IsNullOrEmpty(appKeyPrefix) || string.IsNullOrWhiteSpace(appKeyPrefix)
                ? "default"
                : appKeyPrefix;
            _authDataKey = $"{appKeyPrefix}{LocalStorageKeySuffix}";
            _keyStore = (near.Connection.Signer as InMemorySigner).KeyStore;
            _authService = authService;

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config";
            Configuration libConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            _nearLocalStorage = (libConfig.GetSection("appSettings") as AppSettingsSection);
            if(_nearLocalStorage.Settings.Count > 0)
            {
                _authData.AccountId = _nearLocalStorage.Settings[_authDataKey].Value ?? null;
            }
            else
            {
                _authData.AccountId = null;
            }                        
        }

        public bool IsSignedIn()
        {
            if (_authData.AccountId == null) return false;
            return true;
        }

        public string GetAccountId()
        {
            return _authData.AccountId ?? "";
        }

        public async Task<bool> RequestSignIn(string contractId, string title,Uri successUrl, Uri failureUrl, Uri appUrl)
        {
            if (!string.IsNullOrWhiteSpace(GetAccountId())) return true;
            if (await _keyStore.GetKeyAsync(_networkId, GetAccountId()) != null) return true;

            var accessKey = KeyPair.FromRandom("ed25519");

            var url = new UriBuilder(_walletBaseUrl + LoginWalletUrlSuffix);

            url.Query = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "title", title },
                { "contract_id", contractId },
                { "success_url", successUrl.AbsolutePath },
                { "failure_url", failureUrl.AbsolutePath },
                { "app_url", appUrl.AbsolutePath},
                { "'public_key'", accessKey.GetPublicKey().ToString() },
            }).ReadAsStringAsync().Result;

            await _keyStore.SetKeyAsync(_networkId, PendingAccessKeyPrefix + accessKey.GetPublicKey(), accessKey);
            return _authService.OpenUrl(url.Uri.AbsolutePath);

        }

        public async Task CompleteSignIn(string url)
        {
            Uri uri = new Uri(url);
            string publicKey = HttpUtility.ParseQueryString(uri.Query).Get("public_key");
            string accountId = HttpUtility.ParseQueryString(uri.Query).Get("account_id");
            _authData.AccountId = accountId;
            
            try
            {
                _nearLocalStorage.Settings.Add(_authDataKey, accountId);
                await MoveKeyFromTempToPermanent(accountId, publicKey);               
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private async Task MoveKeyFromTempToPermanent(string accountId, string publicKey)
        {
            var pendingAccountId = PendingAccessKeyPrefix + publicKey;
            KeyPair keyPair;
            try
            {
                keyPair = await _keyStore.GetKeyAsync(_networkId, pendingAccountId);
            }
            catch(Exception)
            {
                throw new Exception("Wallet account error: no KeyPair");
            }

            try
            {
                await _keyStore.SetKeyAsync(_networkId, accountId, keyPair);
            }
            catch(Exception e)
            {
                throw e;
            }

            try
            {
                await _keyStore.RemoveKeyAsync(_networkId, PendingAccessKeyPrefix + publicKey);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void SignOut()
        {
            _authData = new ExpandoObject();
            _nearLocalStorage.Settings.Remove(_authDataKey);
        }
    }
}