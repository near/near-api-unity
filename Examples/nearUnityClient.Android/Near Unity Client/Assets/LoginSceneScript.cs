using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoginSceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.deepLinkActivated += async (string url) => await CompleteSignIn(url);
    }
    
    async Task CompleteSignIn(string url)
    {        
        await NearPersistentManager.Instance.WalletAccount.CompleteSignIn(url);
        if(NearPersistentManager.Instance.WalletAccount.IsSignedIn() == true)
        {
            SceneManager.LoadScene("Near", LoadSceneMode.Single);
        }        
    }
}
