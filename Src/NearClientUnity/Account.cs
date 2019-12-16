using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using NearClientUnity.Providers;
using NearClientUnity.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NearClientUnity
{
    public class Account
    {
        private readonly Connection _connection;
        private readonly string _accountId;
        private AccountState _state;
        private AccessKey _accessKey;
        private bool _ready;
        private const int TxStatusRetryWait = 500;
        private const int TxStatusRetryNumber = 10;
        // Default amount of tokens to be send with the function calls. Used to pay for the fees
        // incurred while running the contract execution. The unused amount will be refunded back to
        // the originator.
        private const int DefaultFuncCallAmount = 2000000;


        public Account(Connection connection, string accountId)
        {
            _connection = connection;
            _accountId = accountId;
        }

        public async Task FetchStateAsync()
        {
            _accessKey = null;
            _state = await _connection.Provider.QueryAsync($"account/{_accountId}", "");
            var publicKey = await _connection.Signer.GetPublicKeyAsync(_accountId, _connection.NetworkId);
            if (publicKey == null) return;
            try
            {
                _accessKey =
                    await _connection.Provider.QueryAsync($"access_key/{_accountId}/{publicKey.ToString()}", "");
            }
            catch (Exception)
            {
                throw new Exception(
                    $"Failed to fetch access key for '{_accountId}' with public key {publicKey.ToString()}");
            }
        }

        protected async Task<bool> GetReadyStatusAsync()
        {
            if (_ready) return _ready;
            try
            {
                await FetchStateAsync();
                _ready = true;
            }
            catch (Exception)
            {
                _ready = false;
            }

            return _ready;
        }

        private void PrintLogs(string contractId, string[] logs)
        {
            foreach (var log in logs)
            {
                Console.WriteLine($"[{contractId}]: {log}");
            }
        }

        private async Task<FinalExecutionOutcome> RetryTxResultAsync(byte[] txHash, string accountId)
        {
            var waitTime = TxStatusRetryWait;
            for (var i = 0; i < TxStatusRetryNumber; i++)
            {
                try
                {
                    var result = await _connection.Provider.GetTxStatusAsync(txHash, accountId);
                    return result;
                }
                catch (Exception)
                {
                    await Task.Delay(waitTime);
                    waitTime *= TxStatusRetryWait;
                    i++;
                }
            }

            throw new Exception(
                $"Exceeded {TxStatusRetryNumber} status check attempts for transaction ${Base58.Encode(txHash)}");
        }

        private async Task<FinalExecutionOutcome> SignAndSendTransactionAsync(string receiverId, Action[] actions)
        {
            if (!await GetReadyStatusAsync())
            {
                throw new Exception($"Can not sign transactions, no matching key pair found in Signer.");
            }

            var status = await _connection.Provider.GetStatusAsync();

            var signTransaction = await SignedTransaction.SignTransactionAsync(receiverId, ++_accessKey.Nonce, actions,
                Base58.Decode(status.SyncInfo.LatestBlockHash), _connection.Signer, _accountId, _connection.NetworkId);
            FinalExecutionOutcome result;

            try
            {
                result = await _connection.Provider.SendTransactionAsync(signTransaction.Item2);
            }
            catch (Exception e)
            {
                var parts = e.Message.Split(':');
                if (parts.Length > 1 && parts[1] == " Request timed out.")
                {
                    result = await RetryTxResultAsync(signTransaction.Item1, _accountId);
                }
                else
                {
                    throw;
                }
            }

            var tempFlatLogs = new ExecutionOutcomeWithId[1 + result.Receipts.Length];
            tempFlatLogs[0] = result.Transaction;
            Array.Copy(result.Receipts, 0, tempFlatLogs, 1, result.Receipts.Length);

            var flatLogs = new List<string>();

            foreach (var t in tempFlatLogs)
            {
                flatLogs.AddRange(t.Outcome.Logs);
            }

            PrintLogs(signTransaction.Item2.Transaction.ReceiverId, flatLogs.ToArray());

            // ToDo: Add typed error handling

            return result;
        }

        public async Task<Account> CreateAndDeployContractAsync(string contractId, string publicKey, byte[] data,
            UInt128 amount)
        {
            var accessKey = AccessKey.FullAccessKey();
            var actions = new []
            {
                Action.CreateAccount(), Action.Transfer(amount), Action.AddKey(new PublicKey(publicKey), accessKey),
                Action.DeployContract(data)
            };

            await SignAndSendTransactionAsync(contractId, actions);

            var contractAccount = new Account(_connection, contractId);
            return contractAccount;
        }

        public async Task<Account> CreateAndDeployContractAsync(string contractId, PublicKey publicKey, byte[] data,
            UInt128 amount)
        {
            var accessKey = AccessKey.FullAccessKey();
            var actions = new[]
            {
                Action.CreateAccount(), Action.Transfer(amount), Action.AddKey(publicKey, accessKey),
                Action.DeployContract(data)
            };

            await SignAndSendTransactionAsync(contractId, actions);

            var contractAccount = new Account(_connection, contractId);
            return contractAccount;
        }

        public async Task<FinalExecutionOutcome> SendMoneyAsync(string receiverId, UInt128 amount)
        {
            var result = await SignAndSendTransactionAsync(receiverId, new[] {Action.Transfer(amount)});
            return result;
        }

        public async Task<FinalExecutionOutcome> CreateAccountAsync(string newAccountId, string publicKey,
            UInt128 amount)
        {
            var accessKey = AccessKey.FullAccessKey();
            var actions = new[]
                {Action.CreateAccount(), Action.Transfer(amount), Action.AddKey(new PublicKey(publicKey), accessKey)};
            var result = await SignAndSendTransactionAsync(newAccountId, actions);
            return result;
        }

        public async Task<FinalExecutionOutcome> CreateAccountAsync(string newAccountId, PublicKey publicKey,
            UInt128 amount)
        {
            var accessKey = AccessKey.FullAccessKey();
            var actions = new[]
                {Action.CreateAccount(), Action.Transfer(amount), Action.AddKey(publicKey, accessKey)};
            var result = await SignAndSendTransactionAsync(newAccountId, actions);
            return result;
        }

        public async Task<FinalExecutionOutcome> DeleteAccountAsync(string beneficiaryId)
        {
            var result =
                await SignAndSendTransactionAsync(_accountId, new[] {Action.DeleteAccount(beneficiaryId)});
            return result;
        }

        public async Task<FinalExecutionOutcome> DeployContractAsync(byte[] data)
        {
            var result = await SignAndSendTransactionAsync(_accountId, new[] {Action.DeployContract(data)});
            return result;
        }

        public async Task<FinalExecutionOutcome> FunctionCallAsync(string contractId, string methodName, dynamic args, int gas, UInt128 amount)
        {
            if (args == null) {
                args = new ExpandoObject();
            }

            var methodArgs = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(args));
            var result = await SignAndSendTransactionAsync(contractId, new Action[]{Action.FunctionCall(methodName,methodArgs, gas, amount)});
            return result;
        }

        public async Task<FinalExecutionOutcome> FunctionCallAsync(string contractId, string methodName, dynamic args, UInt128 amount)
        {
            if (args == null)
            {
                args = new ExpandoObject();
            }

            var methodArgs = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(args));
            var result = await SignAndSendTransactionAsync(contractId, new Action[] { Action.FunctionCall(methodName, methodArgs, DefaultFuncCallAmount, amount) });
            return result;
        }

        public async Task<FinalExecutionOutcome> AddKeyAsync(string publicKey, UInt128 amount,
            string methodName = "", string contractId = "")
        {
            AccessKey accessKey;
            if (string.IsNullOrEmpty(contractId) || string.IsNullOrWhiteSpace(contractId))
            {
                accessKey = AccessKey.FullAccessKey();
            } else
            {
                accessKey = AccessKey.FunctionCallAccessKey(contractId, (string.IsNullOrWhiteSpace(methodName) || string.IsNullOrEmpty(methodName)) ? Array.Empty<string>() : new[] {methodName}, amount);
            }
            var result = await SignAndSendTransactionAsync(_accountId, new[]{Action.AddKey(new PublicKey(publicKey), accessKey)});
            return result;
        }

        public async Task<FinalExecutionOutcome> AddKeyAsync(string publicKey,
            string methodName = "", string contractId = "")
        {
            AccessKey accessKey;
            if (string.IsNullOrEmpty(contractId) || string.IsNullOrWhiteSpace(contractId))
            {
                accessKey = AccessKey.FullAccessKey();
            }
            else
            {
                accessKey = AccessKey.FunctionCallAccessKey(contractId, (string.IsNullOrWhiteSpace(methodName) || string.IsNullOrEmpty(methodName)) ? Array.Empty<string>() : new[] { methodName });
            }
            var result = await SignAndSendTransactionAsync(_accountId, new[] { Action.AddKey(new PublicKey(publicKey), accessKey) });
            return result;
        }

        public async Task<FinalExecutionOutcome> DeleteKeyAsync(string publicKey)
        {
            var result = await SignAndSendTransactionAsync(_accountId, new[]{Action.DeleteKey(new PublicKey(publicKey))});
            return result;
        }

        public async Task<FinalExecutionOutcome> DeleteKeyAsync(PublicKey publicKey)
        {
            var result = await SignAndSendTransactionAsync(_accountId, new[] { Action.DeleteKey(publicKey) });
            return result;
        }

        public async Task<FinalExecutionOutcome> StakeAsync(string publicKey,UInt128 amount)
        {
            var result = await SignAndSendTransactionAsync(_accountId, new[]{Action.Stake(amount, new PublicKey(publicKey))});
            return result;
        }

        public async Task<FinalExecutionOutcome> StakeAsync(PublicKey publicKey, UInt128 amount)
        {
            var result = await SignAndSendTransactionAsync(_accountId, new[] { Action.Stake(amount, publicKey)});
            return result;
        }

        public async Task<dynamic> ViewFunctionAsync(string contractId, string methodName, dynamic args)
        {
            var response = await _connection.Provider.QueryAsync($"call/{contractId}/{methodName}", Base58.Encode(JsonConvert.SerializeObject(args)));
            
            var result = JObject.Parse(response);

            if (result.logs != null && result.logs.GetType() is ArraySegment<string> && result.logs.Length >0 )
            {
                PrintLogs(contractId, result.logs);
            }

            return result;
        }

        /// Returns array of {access_key: AccessKey, public_key: PublicKey} items.
        public async Task<dynamic>  GetAccessKeysAsync()
        {
            var response = await _connection.Provider.QueryAsync($"access_key/{_accountId}", "");
            var result = JObject.Parse(response);
            return result;
        }

        public async Task<dynamic> GetAccountDetailsAsync()
        {
            // TODO: update the response value to return all the different keys, not just app keys.
            // Also if we need this function, or getAccessKeys is good enough.
            var accessKeys = await GetAccessKeysAsync() as List<dynamic>;
            dynamic result = new ExpandoObject();
            var authorizedApps = new List<dynamic>();

            foreach (var key in accessKeys)
            {
                if (key.access_key.permission.FunctionCall == null) continue;
                var perm = key.access_key.permission.FunctionCall;
                dynamic authorizedApp = new ExpandoObject();
                authorizedApp.ContractId = perm.receiver_id;
                authorizedApp.Amount = perm.allowance;
                authorizedApp.PublicKey = key.public_key;
                authorizedApps.Add(authorizedApp);
            }

            result.AuthorizedApps = authorizedApps.ToArray();
            result.Transactions = Array.Empty<dynamic>();
            return result;
        }
    }
}