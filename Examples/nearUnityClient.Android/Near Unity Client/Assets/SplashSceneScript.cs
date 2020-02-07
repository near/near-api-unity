using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneScript : MonoBehaviour
{
    private bool finished = false;
    
    void Start()
    {
        StartCoroutine(DoFade());
    }

    IEnumerator DoFade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        yield return finished = true;
    }

    private void LateUpdate()
    {
        if(finished == true && NearPersistentManager.Instance.WalletAccount.IsSignedIn() == false)
        {
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }
        if (finished == true && NearPersistentManager.Instance.WalletAccount.IsSignedIn() == true)
        {
            SceneManager.LoadScene("Near", LoadSceneMode.Single);
        }
    }
   
}
