using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComeBackTOMainScene : MonoBehaviour
{
    private void OnMouseDown()
    {
        SceneManager.LoadScene(1);
    }
    private void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();
    }
}
