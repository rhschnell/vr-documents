using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityGoogleDrive;

/// <summary>
/// Class that deals with logging out.
/// </summary>
public class GoogleLogOut : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the settings for the google drive.
    /// </summary>
    public virtual GoogleDriveSettings Settings { get; set; }

    /// <summary>
    /// Gets or sets the google login.
    /// </summary>
    public virtual GoogleLogin GoogleLogin { get; set; }

    /// <summary>
    /// Gets or sets the coroutine runner.
    /// </summary>
    public virtual ICoroutineRunner CoroutineRunner { get; set; }

    /// <summary>
    /// Opens a new tab with the given url, only works in WebGL.
    /// </summary>
    /// <param name="url">the url path to open</param>
    public void OpenIt(string url)
    {
        // Call the platform-specific method to open a new tab
#if !UNITY_EDITOR && UNITY_WEBGL
             OpenNewTab(url);
#endif
    }

    /// <summary>
    /// Logs the user out, then triggers a new login.
    /// </summary>
    public void LogOut()
    {
        bool once = true;
        this.Settings.DeleteCachedAuthTokens();
        this.OpenIt(" ");

        // Check if any authentication token is cached and trigger a new login if not already running
        while (!this.Settings.IsAnyAuthTokenCached() && once)
        {
            once = false;
            this.CoroutineRunner.StartCoroutine(this.LogIn());
        }
    }

    /// <summary>
    /// Logs the user back in.
    /// </summary>
    /// <returns>Waits until the request is finished to continue.</returns>
    public IEnumerator LogIn()
    {
        // Start coroutine to update user information
        yield return this.CoroutineRunner.StartCoroutine(this.GoogleLogin.UpdateInfo(new GoogleDriveAbout.GetRequest()));

        // Start coroutine to find PaperVR folder ID
        this.CoroutineRunner.StartCoroutine(this.GoogleLogin.FindId(new GoogleDriveFiles.ListRequest(), false));
    }

    /// <summary>
    /// Imports the javascript function to open a new tab.
    /// </summary>
    /// <param name="url">The url to change the current page to.</param>
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);

    /// <summary>
    /// Initializes Google Drive settings and coroutine runner on awake.
    /// </summary
    void Awake()
    {
        // Load Google Drive settings from resources
        this.Settings = GoogleDriveSettings.LoadFromResources();

        // Get or add the CoroutineRunner component
        this.CoroutineRunner = this.CoroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();

        // Instantiate a new GoogleLogin object
        this.GoogleLogin = new GoogleLogin();
    }
}