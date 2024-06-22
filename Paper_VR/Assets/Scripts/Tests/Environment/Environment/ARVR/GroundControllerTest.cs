using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

/// <summary>
/// This test class tests the GroundController class.
/// </summary>
public class GroundControllerTest : MonoBehaviour
{
    private GameObject groundController;
    private GroundController controller;
    private GameObject ARstatus;
    private GameObject cameraObject;

    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Create an instance of the ARstatus class
        this.ARstatus = new GameObject("ARstatus");
        this.ARstatus.AddComponent<ARstatus>();

        // Create an instance of the Camera class
        this.cameraObject = new GameObject("Camera");
        this.cameraObject.AddComponent<Camera>();

        // Tag the camera as MainCamera
        this.cameraObject.tag = "MainCamera";

        // Create a new GameObject with GroundController attached
        this.groundController = new GameObject("GroundControllerTestObject");
        this.controller = this.groundController.AddComponent<GroundController>();

        // Setup required components and variables
        this.controller.ground = new GameObject[1]; // Assuming you want to test with 1 ground object
        this.controller.ground[0] = new GameObject("GroundObject");
        this.controller.ground[0].AddComponent<MeshRenderer>(); // Add renderer component for visibility control
        this.controller.leftController = new GameObject("LeftController");
        this.controller.rightController = new GameObject("RightController");

        // Give the left and right controllers a child object
        GameObject leftControllerChild = new GameObject("LeftControllerChild");
        GameObject rightControllerChild = new GameObject("RightControllerChild");
        leftControllerChild.transform.parent = this.controller.leftController.transform;
        rightControllerChild.transform.parent = this.controller.rightController.transform;

        // Setup button and text components
        this.controller.switchGroundButton = this.groundController.AddComponent<Button>();
        this.controller.switchGroundButton.image = this.controller.switchGroundButton.gameObject.AddComponent<Image>();
        this.controller.switchGroundText = new GameObject("SwitchGroundText").AddComponent<TextMeshProUGUI>();

        // Setup sprites
        this.controller.offButtonSprite = Sprite.Create(new Texture2D(10, 10), new Rect(0, 0, 10, 10), Vector2.zero);
        this.controller.onButtonSprite = Sprite.Create(new Texture2D(10, 10), new Rect(0, 0, 10, 10), Vector2.zero);
    }

    /// <summary>
    /// Cleans up the test environment.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        // Clean up objects after each test
        Object.DestroyImmediate(this.groundController);
        Object.DestroyImmediate(this.controller.ground[0]);
        Object.DestroyImmediate(this.controller.leftController);
        Object.DestroyImmediate(this.controller.rightController);
        Object.DestroyImmediate(this.controller.switchGroundText.gameObject);
        Object.DestroyImmediate(this.cameraObject);
    }

    /// <summary>
    /// Tests if the ground is not visible and if the text has changed.
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    [UnityTest]
    public IEnumerator ToggleGroundVisibility_Test()
    {
        // Ground should start as visible
        Assert.IsFalse(this.controller.arStatus.isAR);

        // Simulate button click
        this.controller.OnSwitchClick();

        // Wait for end of frame to allow for UI update
        yield return new WaitForSeconds(0.5f);

        // Ground should be invisible after toggle
        Assert.IsTrue(this.controller.arStatus.isAR);

        // Additional assertions can be made on button sprite and text changes
        Assert.AreEqual("Ground OFF (AR)", this.controller.switchGroundText.text);
    }

    /// <summary>
    /// Tests if there is always only one instance of the ARstatus class.
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    [UnityTest]
    public IEnumerator CreateNewARstatus()
    {
        // Create a new ARstatus object
        GameObject newARstatus = new GameObject("NewARstatus");
        newARstatus.AddComponent<ARstatus>();

        // Check that the new ARstatus object is destroyed
        yield return new WaitForSeconds(0.5f);

        Debug.Log(newARstatus);

        // Convert newARstatus to an string
        string value = newARstatus.ToString();

        Assert.AreEqual("null", value);

        yield return null;
    }
}