using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    [Header("Apple")]
    public GameObject apple;
   
    IEnumerator Start()
    {
        apple.GetComponent<Animator>().Play("appleAnimation");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);

    }

  
}
