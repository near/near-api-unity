using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignOutButtonHandler : MonoBehaviour
{
    public void SignOut()
    {
        NearPersistentManager.Instance.WalletAccount.SignOut();
        if (NearPersistentManager.Instance.WalletAccount.IsSignedIn() == false)
        {
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }
    }
}
