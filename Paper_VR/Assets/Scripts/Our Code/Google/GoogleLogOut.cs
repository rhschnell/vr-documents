using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGoogleDrive;

/// <summary>
/// class that deals with logging out
/// </summary>
public class GoogleLogOut : MonoBehaviour
{
    private GoogleDriveSettings settings;
    private GoogleDriveAbout.GetRequest request;
    GoogleLogin GoogleLogin = new GoogleLogin();

    /// <summary>
    /// Logs the user out, then triggers a new login
    /// </summary>
    public void LogOut()
    {
        bool once = true;
        this.settings.DeleteCachedAuthTokens();
        while (!this.settings.IsAnyAuthTokenCached() && once)
        {
            once = false;
            this.StartCoroutine(this.LogIn());
        }
    }

    /// <summary>
    /// logs the user back in
    /// </summary>
    /// <returns>waits until the request is finished to continue</returns>
    public IEnumerator LogIn()
    {
        AuthController.CancelAuth();
        this.request = GoogleDriveAbout.Get();
        this.request.Fields = new List<string> { "user" };
        yield return this.request.Send();

        GoogleLogin.name = this.request.ResponseData.User.DisplayName;
        GoogleLogin.email = this.request.ResponseData.User.EmailAddress;
        this.StartCoroutine(this.GoogleLogin.FindId());
    }

    // Start is called before the first frame update
    void Awake()
    {
        this.settings = GoogleDriveSettings.LoadFromResources();
    }
}
