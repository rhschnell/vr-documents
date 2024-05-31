using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityGoogleDrive;

/// <summary>
/// class that deals with logging out
/// </summary>
public class GoogleLogOut : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the settings for the google drive
    /// </summary>
    public virtual GoogleDriveSettings Settings { get; set; }

    /// <summary>
    /// Gets or sets the google login
    /// </summary>
    public virtual GoogleLogin GoogleLogin { get; set; }

    /// <summary>
    /// Gets or sets the coroutine runner
    /// </summary>
    public virtual ICoroutineRunner CoroutineRunner { get; set; }

    /// <summary>
    /// Opens a new tab with the given url, only works in WebGL
    /// </summary>
    /// <param name="url">the url path to open</param>
    public void OpenIt(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
             OpenNewTab(url);
#endif
    }

    /// <summary>
    /// Logs the user out, then triggers a new login
    /// </summary>
    public void LogOut()
    {
        bool once = true;
        this.Settings.DeleteCachedAuthTokens();
        this.OpenIt(" ");
        while (!this.Settings.IsAnyAuthTokenCached() && once)
        {
            once = false;
            this.CoroutineRunner.StartCoroutine(this.LogIn());
        }
    }

    /// <summary>
    /// logs the user back in
    /// </summary>
    /// <returns>waits until the request is finished to continue</returns>
    public IEnumerator LogIn()
    {
        yield return this.CoroutineRunner.StartCoroutine(this.GoogleLogin.UpdateInfo(new GoogleDriveAbout.GetRequest()));
        this.CoroutineRunner.StartCoroutine(this.GoogleLogin.FindId(new GoogleDriveFiles.ListRequest(), false));
    }

    /// <summary>
    /// Imports the javascript function to open a new tab
    /// </summary>
    /// <param name="url">the url to change the current page to</param>
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);

    // Start is called before the first frame update
    void Awake()
    {
        this.Settings = GoogleDriveSettings.LoadFromResources();
        this.CoroutineRunner = this.CoroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
        this.GoogleLogin = new GoogleLogin();
    }
}
