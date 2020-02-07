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
using System.Web.Util;

namespace NearClientUnity
{
    public class WalletAccount
    {
        private const string LocalStorageKeySuffix = "_wallet_auth_key";
        private const string LoginWalletUrlSuffix = "/login/";
        private const string PendingAccessKeyPrefix = "pending_key";

        private dynamic _authData = new ExpandoObject();
        private string _authDataKey;
        private IExternalAuthService _authService;
        private IExternalAuthStorage _authStorage;
        private KeyStore _keyStore;        
        private string _networkId;
        private string _walletBaseUrl;

        public WalletAccount(Near near, string appKeyPrefix, IExternalAuthService authService, IExternalAuthStorage authStorage)
        {
            _networkId = near.Config.NetworkId;
            _walletBaseUrl = near.Config.WalletUrl;
            appKeyPrefix = string.IsNullOrEmpty(appKeyPrefix) || string.IsNullOrWhiteSpace(appKeyPrefix)
                ? "default"
                : appKeyPrefix;
            _authDataKey = $"{appKeyPrefix}{LocalStorageKeySuffix}";
            _keyStore = (near.Connection.Signer as InMemorySigner).KeyStore;
            _authService = authService;
            _authStorage = authStorage;

           
            if (_authStorage.HasKey(_authDataKey))
            {
                _authData.AccountId = _authStorage.GetValue(_authDataKey);
            }
            else
            {
                _authData.AccountId = null;
            }
        }

        public IExternalAuthStorage NearAuthStorage => _authStorage;

        public async Task CompleteSignIn(string url)
        {
            HttpEncoder.Current = HttpEncoder.Default;
            Uri uri = new Uri(url);
            string publicKey = HttpUtility.ParseQueryString(uri.Query).Get("public_key");
            string accountId = HttpUtility.ParseQueryString(uri.Query).Get("account_id");
            _authData.AccountId = accountId;

            try
            {
                _authStorage.Add(_authDataKey, accountId);                
                await MoveKeyFromTempToPermanent(accountId, publicKey);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetAccountId()
        {
            return _authData.AccountId ?? "";
        }

        public bool IsSignedIn()
        {
            if (_authData.AccountId == null) return false;
            return true;
        }

        public async Task<bool> RequestSignIn(string contractId, string title, Uri successUrl, Uri failureUrl, Uri appUrl)
        {
            if (!string.IsNullOrWhiteSpace(GetAccountId())) return true;
            if (await _keyStore.GetKeyAsync(_networkId, GetAccountId()) != null) return true;

            var accessKey = KeyPair.FromRandom("ed25519");

            var url = new UriBuilder(_walletBaseUrl + LoginWalletUrlSuffix);

            url.Query = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "title", title },
                { "contract_id", contractId },
                { "success_url", successUrl.AbsoluteUri },
                { "failure_url", failureUrl.AbsoluteUri },
                { "app_url", appUrl.AbsoluteUri},
                { "public_key", accessKey.GetPublicKey().ToString() },
            }).ReadAsStringAsync().Result;

            await _keyStore.SetKeyAsync(_networkId, PendingAccessKeyPrefix + accessKey.GetPublicKey(), accessKey);
            return _authService.OpenUrl(url.Uri.AbsoluteUri);
        }

        public void SignOut()
        {
            _authData = new ExpandoObject();
            _authData.AccountId = null;
            _authStorage.DeleteKey(_authDataKey);
        }

        private async Task MoveKeyFromTempToPermanent(string accountId, string publicKey)
        {
            var pendingAccountId = PendingAccessKeyPrefix + publicKey;
            KeyPair keyPair;
            try
            {
                keyPair = await _keyStore.GetKeyAsync(_networkId, pendingAccountId);
            }
            catch (Exception)
            {
                throw new Exception("Wallet account error: no KeyPair");
            }

            try
            {
                await _keyStore.SetKeyAsync(_networkId, accountId, keyPair);
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                await _keyStore.RemoveKeyAsync(_networkId, PendingAccessKeyPrefix + publicKey);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}