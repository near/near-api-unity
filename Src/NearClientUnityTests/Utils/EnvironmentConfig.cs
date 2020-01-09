using System;
namespace NearClientUnityTests.Utils
{
    public class EnvironmentConfig
    {

        public string NetworkId;
        public Uri NodeUrl;
        public string MasterAccount;

        public EnvironmentConfig(string networkId, Uri nodeUrl, string masterAccount)
        {
            NetworkId = networkId;
            NodeUrl = nodeUrl;
            MasterAccount = masterAccount;
        }

        public static EnvironmentConfig GetConfig(Environment env)
        {
            switch (env)
            {
                case Environment.Production:
                case Environment.Development:
                    {
                        return new EnvironmentConfig("default", new Uri("https://rpc.nearprotocol.com"), TestUtils.TestAccountName);
                    }
                case Environment.Local:
                    {
                        return new EnvironmentConfig("local", new Uri("http://localhost:3030"), TestUtils.TestAccountName);
                    }
                case Environment.Test:
                    {
                        return new EnvironmentConfig("local", new Uri("http://localhost:3030"), TestUtils.TestAccountName);
                    }
                case Environment.TestRemote:
                case Environment.CI:
                    {
                        return new EnvironmentConfig("shared-test", new Uri("http://shared-test.nearprotocol.com:3030"), TestUtils.TestAccountName);
                    }
                case Environment.CIStaging:
                    {
                        return new EnvironmentConfig("shared-test-staging", new Uri("http://staging-shared-test.nearprotocol.com:3030"), TestUtils.TestAccountName);
                    }
                default:
                    {
                        return new EnvironmentConfig("local", new Uri("http://localhost:3030"), TestUtils.TestAccountName);
                    }
            }
        }
    }

    public enum Environment
    {
        Production,
        Development,
        Local,
        Test,
        TestRemote,
        CI,
        CIStaging        
    }
}
