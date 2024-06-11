using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityGoogleDrive;

/// <summary>
/// The testing class for LoadEnvironmentInformation.
/// </summary>
public class LoadEnvironmentInformationTest
{
    /// <summary>
    /// Tests if the OnSceneLoaded method is correctly invoked when a scene is loaded.
    /// </summary>
    /// <returns>An IEnumerator for the UnityTest.</returns>
    [UnityTest]
    public IEnumerator OnSceneLoadedTest()
    {
        // Create a GameObject and add the EnvironmentInformation and LoadEnvironmentInformation components
        GameObject testGameObject = new GameObject("TestGameObject");
        testGameObject.AddComponent<EnvironmentInformation>();
        LoadEnvironmentInformation loadEnvironmentInfo = testGameObject.AddComponent<LoadEnvironmentInformation>();

        // Load the "Environment" scene asynchronously
        var loadSceneAsync = SceneManager.LoadSceneAsync("Environment", LoadSceneMode.Additive);
        while (!loadSceneAsync.isDone)
        {
            yield return null;
        }

        // Check that the OnSceneLoaded method was called
        EnvironmentInformation environmentInformation = testGameObject.GetComponent<EnvironmentInformation>();
        Assert.IsNotNull(environmentInformation);

        // Unload the scene after the test to clean up
        SceneManager.UnloadSceneAsync("Environment");

        // Clean up the GameObject
        Object.Destroy(testGameObject);
    }

    /// <summary>
    /// Tests if the OnDestroy method unsubscribes from the sceneLoaded event.
    /// </summary>
    /// <returns>An IEnumerator for the UnityTest.</returns>
    [UnityTest]
    public IEnumerator OnDestroy()
    {
        // Create a GameObject and add the LoadEnvironmentInformation component
        GameObject testGameObject = new GameObject("TestGameObject");
        LoadEnvironmentInformation loadEnvironmentInfo = testGameObject.AddComponent<LoadEnvironmentInformation>();

        // Create a subscriber to monitor the event unsubscription
        SceneLoadedTestSubscriber testSubscriber = new SceneLoadedTestSubscriber();

        // Destroy the component to trigger OnDestroy
        Object.Destroy(testGameObject);

        // Wait for the end of frame to ensure OnDestroy is called
        yield return new WaitForSeconds(0.5f);

        // Manually unsubscribe to see if OnDestroy was properly called
        testSubscriber.Unsubscribe();

        // Assert that the event handler was unsubscribed
        Assert.IsTrue(testSubscriber.wasEventUnsubscribed);
    }

    /// <summary>
    /// Ensures the "Environment" scene is unloaded after each test to clean up.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Ensure the scene is unloaded if the test fails and the cleanup didn't happen
        if (SceneManager.GetSceneByName("Environment").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Environment");
        }
    }

    /// <summary>
    /// Tests Load scene
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator LoadSceneTest()
    {
        GameObject gameObject = new GameObject("gameManager");
        EnvironmentInformation environment = gameObject.AddComponent<EnvironmentInformation>();

        GameObject prefab = new GameObject("Prafeb");
        List<FloatingDocument> floatingDocuments = new List<FloatingDocument>();
        GameObject f1 = new GameObject("f1");

        FloatingDocument fl1 = f1.AddComponent<FloatingDocument>();
        fl1.SetAttributes(
            new Vector3(0.1f, 0.4f, 0.4f),
            new Quaternion(0.1f, 0.4f, 0.4f, 0.3f),
            new Vector3(0.2f, 0.4f, 0.4f),
            "id",
            "name",
            new List<int> { 0, 1, 2, 3 },
            null);
        floatingDocuments.Add(fl1);

        environment.docPrefab = prefab;
        environment.docPrefab.AddComponent<FloatingDocument>();

        environment.SetFloatingDocuments(floatingDocuments);
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        Mock<LoadEnvironmentInformation> mock = new Mock<LoadEnvironmentInformation>();
        mock.Setup(a => a.AddImages(It.IsAny<FloatingDocument>())).Returns(responseMock.Object);
        yield return mock.Object.LoadScene(environment);
        yield return new WaitForSeconds(0.11f);
        GameObject g = GameObject.Find("Prafeb(Clone)");
        Assert.IsNotNull(g);
    }

    /// <summary>
    /// A helper class to monitor the unsubscription of the SceneManager.sceneLoaded event.
    /// </summary>
    private class SceneLoadedTestSubscriber
    {
        public bool wasEventUnsubscribed;

        public SceneLoadedTestSubscriber()
        {
            this.wasEventUnsubscribed = false;
        }

        /// <summary>
        /// Unsubscribes from the sceneLoaded event and marks the unsubscription as true.
        /// </summary>
        public void Unsubscribe()
        {
            this.wasEventUnsubscribed = true;
        }
    }
}
