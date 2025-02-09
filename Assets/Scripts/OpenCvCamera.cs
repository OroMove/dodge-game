using UnityEngine;
using OpenCvSharp;
using System;
using UnityEngine.UI;
using UnityEngine.Android;

public class OpenCvCamera : MonoBehaviour
{
    public RawImage rawImage; // Assign this in the Inspector
    private WebCamTexture webCamTexture;
    private Mat rgbaMat;
    private Texture2D texture;
    private bool permissionGranted = false;

    void Start()
    {
        // Request camera permission on Android
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            return;
        }
#endif

        InitializeCamera();
    }

    void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        string frontCamName = "";

        // Find the front-facing camera
        foreach (var device in devices)
        {
            if (device.isFrontFacing)
            {
                frontCamName = device.name;
                break;
            }
        }

        if (!string.IsNullOrEmpty(frontCamName))
        {
            // Initialize WebCamTexture
            webCamTexture = new WebCamTexture(frontCamName, 1280, 720, 30);
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;
            webCamTexture.Play();

            // Initialize OpenCvSharp Mat and Texture2D
            rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC4);
            texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
        }
        else
        {
            Debug.LogError("No front camera found!");
        }
    }

    void Update()
    {
#if PLATFORM_ANDROID
        // Check if permission was just granted
        if (!permissionGranted && Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            permissionGranted = true;
            InitializeCamera();
            return;
        }
#endif

        if (webCamTexture != null && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            // Handle Android rotation
            int width = webCamTexture.width;
            int height = webCamTexture.height;

            // Adjust RawImage rotation and scaling based on device orientation
            float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f;
            rawImage.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            float rotation = -webCamTexture.videoRotationAngle;
            rawImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, rotation);

            // Convert WebCamTexture to Mat
            Color32[] colors = webCamTexture.GetPixels32();
            byte[] bytes = new byte[colors.Length * 4];

            for (int i = 0; i < colors.Length; i++)
            {
                bytes[i * 4] = colors[i].r;
                bytes[i * 4 + 1] = colors[i].g;
                bytes[i * 4 + 2] = colors[i].b;
                bytes[i * 4 + 3] = colors[i].a;
            }

            Mat mat = new Mat(height, width, MatType.CV_8UC4, bytes);

            // Rotate matrix if needed based on device orientation
            if (webCamTexture.videoRotationAngle == 90 || webCamTexture.videoRotationAngle == 270)
            {
                Mat rotatedMat = new Mat();
                Cv2.Rotate(mat, rotatedMat, RotateFlags.Rotate90Clockwise);
                mat.Dispose();
                mat = rotatedMat;
            }

            // Perform any OpenCV processing here if needed
            // Example: Cv2.CvtColor(mat, rgbaMat, ColorConversionCodes.BGR2RGBA);

            // Convert the processed Mat back to Texture2D
            int bufferSize = mat.Cols * mat.Rows * 4; // 4 bytes per pixel for RGBA

            // Ensure texture size matches the mat size
            if (texture.width != mat.Cols || texture.height != mat.Rows)
            {
                texture = new Texture2D(mat.Cols, mat.Rows, TextureFormat.RGBA32, false);
            }

            texture.LoadRawTextureData(mat.Data, bufferSize);
            texture.Apply();

            // Display the texture on the RawImage
            rawImage.texture = texture;

            // Clean up
            mat.Dispose();
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
        if (rgbaMat != null)
        {
            rgbaMat.Dispose();
        }
    }
}