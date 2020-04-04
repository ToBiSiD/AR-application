using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

/*
 * https://github.com/ChrisMaire/unity-native-sharing
 */

public class Controller : MonoBehaviour
{
    [Header("BackGround Music")]
    public AudioSource BackGroundMusic;

    [Header("WolfRunning Music")]
    public AudioSource WolfRun;

    [Header("Wolf")]
    public GameObject Wolf;

    //buttons
    [Header("Menu Button")]
    public GameObject Menu;
    public Sprite[] MenuSprites;

    [Header("Tap Button")]
    public GameObject Tap;
    [Header("Tap Panel")]
    public GameObject TapPanel;

    [Header("Music On/Off Button")]
    public GameObject MusicButton;
    public Sprite[] MusicSprites;

    [Header("How to Play Button")]
    public GameObject InfoButton;

    [Header("Support Button")]
    public GameObject SupportButton;

    [Header("Share Button")]
    public GameObject shareButton;

    [Header("Panel with Photo")]
    public GameObject photoPanel;

    [Header("ForPhoto")]
    public GameObject forPhoto;

    [Header("Photo")]
    public Image photo;

    [Header("search")]
    public GameObject search;



    private bool isProcessing = false;
    private string screenshotName;
    private string screenshotPath;

    
    void Start()
    {
        BackGroundMusic.Play();
    }


    void Update()
    {

        if (DefaultTrackableEventHandler.wolfTracked)
            Wolf.SetActive(true);
        else Wolf.SetActive(false);
        if (forPhoto.activeSelf)
            search.SetActive(false);
        if (Wolf.activeSelf)
        {
            if (WolfRun.isPlaying)
            {
                BackGroundMusic.Pause();
                Wolf.GetComponent<Animator>().SetTrigger("Run");
            }
            else
            {
                BackGroundMusic.UnPause();
                Wolf.GetComponent<Animator>().SetTrigger("Stay");
            }
        }

        if(AudioListener.volume > 0)
            MusicButton.GetComponent<Image>().sprite = MusicSprites[1];
        else MusicButton.GetComponent<Image>().sprite = MusicSprites[0];

        if (Input.GetKey("escape"))
            Application.Quit();
    }

    // Quit app
    private void OnApplicationQuit()
    {
        if (File.Exists(screenshotPath))
            File.Delete(screenshotPath);
    }

    // Play "Run" music (button)
    public void RunningMusicOn()
    {
        WolfRun.Play();
    }

    //Menu On/Off
    public void ShowButtons()
    {
        if (MusicButton.activeSelf == false )
            Menu.GetComponent<Image>().sprite = MenuSprites[1];
        else Menu.GetComponent<Image>().sprite = MenuSprites[0];
        MusicButton.SetActive(!MusicButton.activeSelf);
        InfoButton.SetActive(!InfoButton.activeSelf);
        SupportButton.SetActive(!SupportButton.activeSelf);
    }

    //Music Off/On
    public void MusicTurn()
    {
        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0;
            MusicButton.GetComponent<Image>().sprite = MusicSprites[0];
        }

        else
        {
            AudioListener.volume = 1;
            MusicButton.GetComponent<Image>().sprite = MusicSprites[1];
        }
    }

    //native mail agent
    public void Support()
    {
        Application.OpenURL(string.Format("mailto:vb@marevo.vision?subject=Test My APP&body=testing native mail agent"));
    }

    //new Scene with pink background
   public void HowToPlay()
    {
        SceneManager.LoadScene(2);
    }


    //Take Photo
    public void OnMakePhoto()
    {
        MakeScreenshot();

    }
    //Share Photo 
    public void OnShareButtonClick( string text)
    {
        forPhoto.SetActive(false);
        photoPanel.SetActive(false);
        Menu.GetComponent<Image>().sprite = MenuSprites[0];
        Menu.SetActive(true);
        TapPanel.SetActive(true);
        Tap.SetActive(true);

        StartCoroutine(delayedScreenShare(text));
    }

    //Back to find marker
    public void OnBackButtonClick()
    {
        forPhoto.SetActive(false);
        photoPanel.SetActive(false);
        Menu.GetComponent<Image>().sprite = MenuSprites[0];
        Menu.SetActive(true);
        TapPanel.SetActive(true);
        Tap.SetActive(true);
        File.Delete(screenshotPath);
    }

    private void MakeScreenshot()
    {
#if UNITY_ANDROID
        if(!isProcessing)
        {
            StartCoroutine(TakeAndSaveScreenshot());
        }
#else
		Debug.Log("No ScreenShot set up for this platform.");
#endif
    }


   //Coroutine to take Photo
   IEnumerator TakeAndSaveScreenshot()
    {
        Menu.SetActive(false);
        TapPanel.SetActive(false);
        Tap.SetActive(false);
        photoPanel.SetActive(false);
        MusicButton.SetActive(false);
        SupportButton.SetActive(false);
        InfoButton.SetActive(false);

        isProcessing = true;

        yield return new WaitForEndOfFrame();

        screenshotName = "photo"+System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss")+".png";
        //its to load on screen and share
        screenshotPath = Path.Combine(Application.temporaryCachePath,screenshotName);
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        //get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        forPhoto.SetActive(true);
        isProcessing = false;
        photoPanel.SetActive(true);
        //convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();

        //save image in dir of app
        File.WriteAllBytes(screenshotPath,imageBytes);
        //save image to gallery (maybe it should be in couratine with nativeShare)
        NativeGallery.SaveImageToGallery(imageBytes, "testAR_Kucherenko", screenshotName,null);
        //photo.sprite = Sprite.Create(screenImage, new Rect(0, 0, screenImage.width, screenImage.height), new Vector2(0.5f, 0.5f), 100f);
        byte[] data = File.ReadAllBytes(screenshotPath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        photo.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
    }

    //corputine to delete photo
    IEnumerator DeletePhoto()
    {
        yield return new WaitForSeconds(.5f);
        File.Delete(screenshotPath);
    }

    // coroutine to share photo
    IEnumerator delayedScreenShare(string text)
    {
        yield return new WaitForSeconds(.001f);
        new NativeShare().SetSubject("It`s my wolf").SetText("Hey,dude, it`s my wolf!").SetTitle("Share ur Wolf!").AddFile(Path.Combine( Application.temporaryCachePath,screenshotName)).Share();
        //if i need to save img after Sharing
        //NativeGallery.SaveImageToGallery(imageBytes, "testAR_Kucherenko", screenshotName, null);

        yield return new  WaitForSeconds(.5f);
        StartCoroutine(DeletePhoto());
    }

}
