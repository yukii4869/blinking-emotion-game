using UnityEngine;

public class Webcam : MonoBehaviour {
  [SerializeField] private Material webcamMaterial;
  [SerializeField] private string webcamName = "Logi C270 HD WebCam";

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    WebCamDevice[] devices = WebCamTexture.devices;

    // Log out each webcam
    Debug.Log("Available webcams:");
    foreach (WebCamDevice webcam in devices) {
      Debug.Log(webcam.name);
    }

    // We get the webcam by name.
    // Note: on PC you can have multiple, but on mobile you can't.
    // Can just call new() to get the default.
    WebCamTexture webcamTexture = new(webcamName, 1920, 1080, 30);

    // Set our material texture to be our webcam texture.
    webcamMaterial.mainTexture = webcamTexture;

    // Start the webcam.
    webcamTexture.Play();
    
  }
}