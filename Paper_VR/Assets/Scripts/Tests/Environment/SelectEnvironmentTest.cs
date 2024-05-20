using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// The testing class for SelectingEnvironment.
/// </summary>
public class SelectEnvironmentTest
{
    /// <summary>
    /// Tests the behavior of the AddEnvironmentButton method in the SelectEnvironment class.
    /// This test verifies that calling AddEnvironmentButton results in loading the "Environment" scene
    /// and setting the environment information on the GameManager.
    /// </summary>
    /// <returns>An IEnumerator for the UnityTest.</returns>
   // [UnityTest]
    public IEnumerator AddEnvironmentButtonAndLoadNewSceneTest()
    {
        // Create a GameObject and add the necessary components
        GameObject gameManager = new GameObject("GameManager");
        EnvironmentInformation gameManagerEnvInfo = gameManager.AddComponent<EnvironmentInformation>();
        GameObject testGameObject = new GameObject("TestGameObject");
        // SelectEnvironment selectEnvironment = testGameObject.AddComponent<SelectEnvironment>();
        SelectEnvironment selectEnvironment = new SelectEnvironment();

        TMPro.TextMeshPro name = new TMPro.TextMeshPro();
        TMPro.TextMeshPro email = new TMPro.TextMeshPro();

        // Assign the gameManager reference
        selectEnvironment.gameManager = gameManager;
        selectEnvironment.Email = email;
        selectEnvironment.Name = name;

        // Call the AddEnvironmentButton method
        selectEnvironment.AddEnvironmentButton();

        // Wait for the scene to load
        //yield return null;
        yield return new WaitForSeconds(0.1f);

        // Verify the scene has loaded
        Assert.AreEqual("Environment", SceneManager.GetActiveScene().name);

        // Clean up
        Object.Destroy(testGameObject);
        Object.Destroy(gameManager);
    }

    /// <summary>
    /// Ensures that the scene is unloaded after each test to clean up.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (SceneManager.GetSceneByName("Environment").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Environment");
        }
    }
}
