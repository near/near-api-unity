using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInButtonHandler : MonoBehaviour
{
    public async void RequestSignIn()
    {
        await NearPersistentManager.Instance.WalletAccount.RequestSignIn(
            "signInContract",
            "Near Unity Client",
            new Uri("nearclientunity://nearprotocol.com/success"),
            new Uri("nearclientunity://nearprotocol.com/fail"),
            new Uri("nearclientios://nearprotocol.com")
            );
    }
}
