# Google API implementation in the application


We use the google API to use the google drive functionality. We use a Unity Package to handle the actual calling of the API. See [here](https://github.com/elringus/unity-google-drive) which package is used.


## Setup
To work with the google API, you need a google cloud application. Since we created a proof of concept, we didn't publish the google cloud application we used. This means you can only use a limited amount of accounts that we have to manually input. If you want to use your own google cloud application, this is possible. The package README explains how you set this up, but we will repeat it here.
### Setup your own Google Cloud Application
- In the Unity editor, press `Edit -> Project Settings -> Google Drive`
- Click **Create Google Drive API app** button; web-browser will open URL to setup the app:
  - Select **Create a new project** and click continue;
    - Fill in all of the details of your project and then continue to the next step
  - On Dashboard click the **≡ menu icon** and select **APIs & Services**
  - From the menu on the left side of your screen select **OAuth consent screen** tab, choose **User Type** and click **Create**;
  - Insert necessary information and click **Save and Continue** (After this you will see **Scopes** tab, at this point just click **Cancel**)
  - Navigate to **Credentials** on the menu on the left side of your screen tab and click **Create credentials** -> **OAuth client ID**;
  - Select **Web application** for 'Application type', give your app a name and enter the following restrictions:
    - Authorised JavaScript origins: enter host names which will serve WebGL builds *(not required for platforms other than WebGL)*;
    - Authorised redirect URIs:
      - Add redirect URI for the local loopback requests: **http://localhost**;
      - Add full URIs to the WebGL builds locations *(not required for platforms other than WebGL)*.
    - Final result may [look like that](https://i.gyazo.com/9d28c9b1e0201cb92ed6d8f3fc6dcfaf.png).
  - Click **Save**;
  - Close the appeared popup and click [**Download JSON** button](https://i.gyazo.com/d6b620221f1326aada98b02e011b9094.png) to get the credentials JSON file.
- Return to Unity editor, open Google Drive settings and click **Parse generic credentials JSON file**; select the downloaded credentials JSON file;


If you want to make sure it works with all google accounts, you will need to publish the application. For this you will need
- An official link to the app's Privacy Policy,
- A YouTube video showing how you plan to use the Google user data you get from scopes,
- A written explanation telling Google why you need access to sensitive and/or restricted user data
- All your domains are verified in Google Search Console.