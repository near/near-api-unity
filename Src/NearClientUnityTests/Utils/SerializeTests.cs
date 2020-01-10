using NearClientUnity;
using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Action = NearClientUnity.Action;

namespace NearClientUnityTests.Utils
{
    [TestFixture]
    public class SerializeTests
    {
        [Test]
        public async Task SerializeAndSignMultiActionTx()
        {
            var keyStore = new InMemoryKeyStore();
            var keyPair = KeyPair.FromString("ed25519:2wyRcSwSuHtRVmkMCGjPwnzZmQLeXLzLLyED1NDMt4BjnKgQL6tF85yBx6Jr26D2dUNeC716RBoTxntVHsegogYw");
            await keyStore.SetKeyAsync("test", "test.near", keyPair);
            var publicKey = keyPair.GetPublicKey();

            var actions = new[]
            {
                Action.CreateAccount(),
                Action.DeployContract(new byte[] {1, 2, 3}),
                Action.FunctionCall("qqq", new byte[] {1, 2, 3}, 1000, 1000000),
                Action.Transfer(123),
                Action.Stake(1000000, publicKey),
                Action.AddKey(publicKey, AccessKey.FunctionCallAccessKey("zzz", new []{"www"}, null)),
                Action.DeleteKey(publicKey),
                Action.DeleteAccount("123")
            };

            var blockHash = new ByteArray32() { Buffer = Base58.Decode("244ZQ9cgj3CQ6bWBdytfrJMuMQ1jdXLFGnr4HhvtCTnM") };
            var signedTransaction = await SignedTransaction.SignTransactionAsync("123", 1, actions, blockHash, new InMemorySigner(keyStore), "test.near", "test");

            const string expected = "Fo3MJ9XzKjnKuDuQKhDAC6fra5H2UWawRejFSEpPNk3Y";
            var actual = Base58.Encode(signedTransaction.Item1);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task SerializeAndSignTransferTx()
        {
            var keyStore = new InMemoryKeyStore();

            var keyPair = KeyPair.FromString("ed25519:3hoMW1HvnRLSFCLZnvPzWeoGwtdHzke34B2cTHM8rhcbG3TbuLKtShTv3DvyejnXKXKBiV7YPkLeqUHN1ghnqpFv");

            await keyStore.SetKeyAsync("test", "test.near", keyPair);

            var actions = new[] { Action.Transfer(1) };

            var blockHash = Base58.Decode("244ZQ9cgj3CQ6bWBdytfrJMuMQ1jdXLFGnr4HhvtCTnM");

            var signedTransaction = await SignedTransaction.SignTransactionAsync("whatever.near", 1, actions, new ByteArray32() { Buffer = blockHash }, new InMemorySigner(keyStore), "test.near", "test");

            const string expectedBase64String = "lpqDMyGG7pdV5IOTJVJYBuGJo9LSu0tHYOlEQ+l+HE8i3u7wBZqOlxMQDtpuGRRNp+ig735TmyBwi6HY0CG9AQ==";
            var actualBase64String = Convert.ToBase64String(signedTransaction.Item2.Signature.Data.Buffer);

            Assert.AreEqual(expectedBase64String, actualBase64String);

            const string expectedHexString = "09000000746573742e6e65617200917b3d268d4b58f7fec1b150bd68d69be3ee5d4cc39855e341538465bb77860d01000000000000000d00000077686174657665722e6e6561720fa473fd26901df296be6adc4cc4df34d040efa2435224b6986910e630c2fef601000000030100000000000000000000000000000000969a83332186ee9755e4839325525806e189a3d2d2bb4b4760e94443e97e1c4f22deeef0059a8e9713100eda6e19144da7e8a0ef7e539b20708ba1d8d021bd01";

            var serialized = signedTransaction.Item2.ToByteArray();

            var actualHexString = BitConverter.ToString(serialized).Replace("-", "").ToLower();

            Assert.AreEqual(expectedHexString, actualHexString);
        }

        [Test]
        public void SerializeTransferTx()
        {
            var actions = new[] { Action.Transfer(1) };
            var blockHash = Base58.Decode("244ZQ9cgj3CQ6bWBdytfrJMuMQ1jdXLFGnr4HhvtCTnM");
            var transaction = new Transaction()
            {
                SignerId = "test.near",
                PublicKey = new PublicKey("Anu7LYDfpLtkP7E16LT9imXF694BdQaa9ufVkQiwTQxC"),
                Nonce = 1,
                ReceiverId = "whatever.near",
                Actions = actions,
                BlockHash = new ByteArray32() { Buffer = blockHash }
            };
            var serialized = transaction.ToByteArray();
            var actualHexString = BitConverter.ToString(serialized).Replace("-", "").ToLower();
            const string expectedHexString = "09000000746573742e6e65617200917b3d268d4b58f7fec1b150bd68d69be3ee5d4cc39855e341538465bb77860d01000000000000000d00000077686174657665722e6e6561720fa473fd26901df296be6adc4cc4df34d040efa2435224b6986910e630c2fef6010000000301000000000000000000000000000000";
            Assert.AreEqual(expectedHexString, actualHexString);

            var deserialized = Transaction.FromByteArray(serialized);

            var expectedSerialized = serialized;
            var actualSerialized = deserialized.ToByteArray();
            Assert.AreEqual(expectedSerialized, actualSerialized);
        }
    }
}