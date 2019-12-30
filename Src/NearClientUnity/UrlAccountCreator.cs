using NearClientUnity.Utilities;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class UrlAccountCreator : AccountCreator
    {
        private readonly Connection _connection;
        private readonly ConnectionInfo _helperConnection;

        public UrlAccountCreator(Connection connection, string helperUrl)
        {
            _connection = connection;
            _helperConnection = new ConnectionInfo
            {
                Url = helperUrl
            };
        }

        public override async Task CreateAccountAsync(string newAccountId, PublicKey publicKey)
        {
            // TODO: hit url to create account.
            throw new System.NotImplementedException();
        }
    }
}