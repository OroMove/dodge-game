namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEngine.SceneManagement;

    public class DocumentScannerLobby : MonoBehaviour
    {
        // Use this for initialization
        void Awake()
        {
            // Load the DocumentScannerScene additively
            SceneManager.LoadScene("DocumentScannerScene", LoadSceneMode.Additive);
        }

        void OnDestroy()
        {
            // Unload the DocumentScannerScene asynchronously
            SceneManager.UnloadSceneAsync("DocumentScannerScene");
        }

        // Handle button click events
        public void OnButton(string name)
        {
            NavigateTo(name);
        }

        // Navigate to the specified action in the DocumentScannerScene
        private void NavigateTo(string name)
        {
            // Get the DocumentScannerScene
            Scene scene = SceneManager.GetSceneByName("DocumentScannerScene");
            if (scene.isLoaded)
            {
                // Find the DocumentScannerScript in the scene
                DocumentScannerScript script = Object.FindFirstObjectByType<DocumentScannerScript>();
                if (script != null)
                {
                    script.Process(name);
                }
                else
                {
                    Debug.LogWarning("DocumentScannerScript not found in the scene.");
                }

                // Example of how to manipulate a RawImage (commented out)
                // GameObject gameObject = GameObject.Find("OutputImage");
                // RawImage rawImage = gameObject.GetComponent<RawImage>();
                // rawImage.texture = (Texture2D)Resources.Load("Letter");

                // Example of getting root game objects in the scene (commented out)
                // GameObject[] gameObjects = scene.GetRootGameObjects();
                // Debug.Log(gameObjects.Length);
            }
            else
            {
                Debug.LogWarning("DocumentScannerScene is not loaded.");
            }
        }
    }
}