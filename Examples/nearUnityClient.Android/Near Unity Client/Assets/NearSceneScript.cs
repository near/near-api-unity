using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NearClientUnity;
using NearClientUnity.Utilities;
using NearClientUnity.KeyStores;
using NearClientUnity.Providers;

public class NearSceneScript : MonoBehaviour
{
    private string accountId;
    private string accountBalance;
    private string accountStorageUsed;
    private string accountStoragePaid;
    private bool finished = false;

    void Awake()
    {
        Task.Run(async() => await CollectInformation());
    }

    private Text GetTextObjectByName(string name)
    {
        var canvas = GameObject.Find("Canvas");
        var texts = canvas.GetComponentsInChildren<Text>();
        return texts.FirstOrDefault(textObject => textObject.name == name);
    }

    async Task CollectInformation()
    {
        var walletAccountId = NearPersistentManager.Instance.WalletAccount.GetAccountId();        
        Account account = await NearPersistentManager.Instance.Near.AccountAsync(walletAccountId);
        AccountState accountState = await account.GetStateAsync();
        accountId = walletAccountId;
        accountBalance = FormatNearAmount(accountState.Amount);
        accountStorageUsed = accountState.StorageUsage.ToString();
        accountStoragePaid = accountState.StoragePaidAt.ToString();
        finished = true;
    }

    private string FormatNearAmount(string amount)
    {
        int NEAR_NOMINATION_EXP = 24;
        return $"{amount.Substring(0, amount.Length - NEAR_NOMINATION_EXP)} Near";
    }

    private void LateUpdate()
    {
        if(finished == true)
        {
            GetTextObjectByName("textAccountId").text = $"Id: {accountId}";
            GetTextObjectByName("textAccountBalance").text = $"Balance: {accountBalance}";
            GetTextObjectByName("textAccountStorage").text = $"Storage (used/paid): {accountStorageUsed}/{accountStoragePaid}";
        }
    }
 }
