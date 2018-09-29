using OpenCVForUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;
    private Texture2D t;
    private ResistorOhmFinder.LineCount lineCount = ResistorOhmFinder.LineCount.four;

    public RawImage background, testImage, testImage2;
    public AspectRatioFitter fit;
    public TMPro.TextMeshProUGUI colorText, infoText;
    public RectTransform cursor, redLine;
    public ResistorSimulator simulator;
    public Image colorDisplayer;
    public TMPro.TMP_Dropdown linesDropDownMenu;
    public GameObject[] _4LineDisplayers;
    public GameObject[] _5LineDisplayers;
    public Texture2D testTexture2D;


    private string s = "";
    private void Start()
    {
        //List<ColorMethods.ColorName> a = new List<ColorMethods.ColorName>() { ColorMethods.ColorName.Blue, ColorMethods.ColorName.Brown, ColorMethods.ColorName.Gray, ColorMethods.ColorName.Red, ColorMethods.ColorName.Green };
        //simulator.Set(a, ResistorOhmFinder.LineCount.five);


        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera found.");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        if (backCam == null)
        {
            Debug.Log("Back camera not found.");
            return;
        }

        backCam.Play();
        background.texture = backCam;

        camAvailable = true;



    }
    public void OnDropDownChanged(int value)
    {
        if (value == 0)
        {
            lineCount = ResistorOhmFinder.LineCount.four;
            foreach (var item in _5LineDisplayers)
            {
                item.SetActive(false);
            }
            foreach (var item in _4LineDisplayers)
            {
                item.SetActive(true);
            }
        }
        else if (value == 1)
        {
            lineCount = ResistorOhmFinder.LineCount.five;
            foreach (var item in _5LineDisplayers)
            {
                item.SetActive(true);
            }
            foreach (var item in _4LineDisplayers)
            {
                item.SetActive(false);
            }
        }

    }
    private void Update()
    {
        //print(ColorMethods.GetResistorColorCode(new Color(0f, 4f/255f, 13f/255f)).ToString());

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var data = t.EncodeToPNG();
            File.WriteAllBytes("a.png", data);
            print("oldu");
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && Input.touches[0].position.x > Screen.width / 2)
        {
            Application.CaptureScreenshot("ss" + Time.fixedTime + ".png");
            //StartCoroutine(CaptureScreenshotCoroutine(Screen.width, Screen.height));
            s += "s";
        }

        if (!camAvailable)
        {
            return;
        }

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float zoomValue = 2.7f;
        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f * zoomValue, scaleY * zoomValue, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        Color color = backCam.GetPixel(backCam.width / 2, backCam.height / 2);

        ColorMethods.ColorName colorName = ColorMethods.GetResistorColorCode(color);
        colorDisplayer.color = GetPrimitiveColor(colorName);
        //colorText.text = "Color: " + colorName.ToString();

        infoText.text = ColorMethods.GetHSV(color).ToString() + s;

        cursor.anchoredPosition = new Vector3(1080 / 2, 1920 / 2, 0f);
        int width = 75;
        int heigth = 5;

        Color[] colorBlock = backCam.GetPixels(backCam.width / 2 - width / 2, backCam.height / 2 - (heigth - 1) / 2, width, heigth);
        Color[,] colorBlock2D = ArrayTo2DArray(colorBlock, width, heigth);
        //Color baseColor;
        //List<ColorData> colorData = ResistorOhmFinder.GetLines(colorBlock2D, out baseColor);
        //simulator.Set(colorData, baseColor);

        t = new Texture2D(width, heigth);
        t.SetPixels(0, 0, width, heigth, colorBlock);
        t.Apply();

        var t2 = MedianBlur(t, 5);
        t2.Apply();
        Color[] colorBlockBlurred = t2.GetPixels();
        Color[,] colorBlockBlurred2D = ArrayTo2DArray(colorBlockBlurred, width, heigth);
        //testTexture2D = MedianBlur(testTexture2D);
        testImage.texture = t;
        testImage2.texture = t2;

        List<ColorMethods.ColorName> colorData = ResistorOhmFinder.GetLines(colorBlockBlurred2D, lineCount);

        simulator.Set(colorData, lineCount);


        redLine.sizeDelta = new Vector2(width * zoomValue / 2 * 1.1f, 3);

        //redLine.sizeDelta = new Vector2(width * ratio / 2 * zoomValue * 1.1f, 3);
    }

    private Color[,] ArrayTo2DArray(Color[] array, int width, int height)
    {
        Color[,] resultData = new Color[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                resultData[i, j] = array[width * i + j];
            }
        }
        return resultData;
    }
    private Color GetPrimitiveColor(ColorMethods.ColorName colorName)
    {
        switch (colorName)
        {
            case ColorMethods.ColorName.Black:
                return Color.black;
            case ColorMethods.ColorName.Blue:
                return Color.blue;
            case ColorMethods.ColorName.Brown:
                return new Color(71f / 255f, 34f / 255f, 10f / 255f);//Brown
            case ColorMethods.ColorName.Gray:
                return Color.gray;
            case ColorMethods.ColorName.Green:
                return Color.green;
            case ColorMethods.ColorName.Orange:
                return new Color(255f / 255f, 142f / 255f, 0f / 255f);//Orange
            case ColorMethods.ColorName.Purple:
                return new Color(167f / 255f, 0f / 255f, 255f / 255f);//Purple
            case ColorMethods.ColorName.Red:
                return Color.red;
            case ColorMethods.ColorName.Unknown:
                return new Color(0, 0, 0, 0);//Transparent
            case ColorMethods.ColorName.White:
                return Color.white;
            case ColorMethods.ColorName.Yellow:
                return Color.yellow;
            default:
                return new Color(0, 0, 0, 0);
        }
    }

    private Texture2D MedianBlur(Texture2D texture, int blurValue)
    {
        Mat imgMat = new Mat(texture.height, texture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(texture, imgMat);
        Imgproc.medianBlur(imgMat, imgMat, blurValue);
        Texture2D resultTexture = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(imgMat, resultTexture);

        return resultTexture;
    }
    //private IEnumerator CaptureScreenshotCoroutine(int width, int height)
    //{
    //    yield return new WaitForEndOfFrame();
    //    Texture2D tex = new Texture2D(width, height);
    //    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    tex.Apply();

    //    yield return tex;
    //    string path = SaveImageToGallery(tex, "Name", "Description");
    //    Debug.Log("Picture has been saved at:\n" + path);
    //}

    //protected const string MEDIA_STORE_IMAGE_MEDIA = "android.provider.MediaStore$Images$Media";
    //protected static AndroidJavaObject m_Activity;

    //protected static string SaveImageToGallery(Texture2D a_Texture, string a_Title, string a_Description)
    //{
    //    using (AndroidJavaClass mediaClass = new AndroidJavaClass(MEDIA_STORE_IMAGE_MEDIA))
    //    {
    //        using (AndroidJavaObject contentResolver = Activity.Call<AndroidJavaObject>("getContentResolver"))
    //        {
    //            AndroidJavaObject image = Texture2DToAndroidBitmap(a_Texture);
    //            return mediaClass.CallStatic<string>("insertImage", contentResolver, image, a_Title, a_Description);
    //        }
    //    }
    //}

    //protected static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D a_Texture)
    //{
    //    byte[] encodedTexture = a_Texture.EncodeToPNG();
    //    using (AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory"))
    //    {
    //        return bitmapFactory.CallStatic<AndroidJavaObject>("decodeByteArray", encodedTexture, 0, encodedTexture.Length);
    //    }
    //}

    //protected static AndroidJavaObject Activity
    //{
    //    get
    //    {
    //        if (m_Activity == null)
    //        {
    //            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //            m_Activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    //        }
    //        return m_Activity;
    //    }
    //}

}
