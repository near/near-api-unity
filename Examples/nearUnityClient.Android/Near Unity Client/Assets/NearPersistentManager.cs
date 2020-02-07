using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NearClientUnity;
using NearClientUnity.Utilities;
using NearClientUnity.KeyStores;
using NearClientUnity.Providers;

public class NearPersistentManager : MonoBehaviour
{
    public static NearPersistentManager Instance { get; private set; }
    public WalletAccount WalletAccount { get; set; }
    public Near Near { get; set; }

    void Start()
    {
        Near = new Near(config: new NearConfig()
        {
            NetworkId = "default",
            NodeUrl = "https://rpc.nearprotocol.com",
            ProviderType = ProviderType.JsonRpc,
            SignerType = SignerType.InMemory,
            KeyStore = new InMemoryKeyStore(),
            ContractName = "myContractId",
            WalletUrl = "https://wallet.nearprotocol.com"
        });
        WalletAccount = new WalletAccount(
        Near,
        "",
        new AuthService(),
        new AuthStorage());
    }    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class AuthService : IExternalAuthService
{
    public bool OpenUrl(string url)
    {
        try
        {
            Application.OpenURL(url);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class AuthStorage : IExternalAuthStorage
{  
    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void Add(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public string GetValue(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }
}