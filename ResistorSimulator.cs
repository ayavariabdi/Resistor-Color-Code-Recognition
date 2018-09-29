using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResistorSimulator : MonoBehaviour
{
    //public Image baseField;
    public Image[] _4lines = new Image[4];
    public Image[] _5lines = new Image[5];
    public TMPro.TextMeshProUGUI ohmText;
    public void Set(List<ColorMethods.ColorName> colorData, ResistorOhmFinder.LineCount linesNumber)
    {
        if (colorData == null || colorData.Count == 0 || colorData.Count > 6)
        {
            Debug.Log("Lines not found!");
            return;
        }

        ResistorData resistor = new ResistorData();
        int result = resistor.Set(colorData);
        if (result == -2)
        {
            ohmText.text = "...";
        }
        else if (result == -1)
        {
            ohmText.text = "Flip the resistor";
        }
        else
        {
            ohmText.text = resistor.ohmValue;
        }

        if (linesNumber == ResistorOhmFinder.LineCount.four)
        {
            if (colorData.Count != 4)
                return;

            _4lines[0].transform.parent.gameObject.SetActive(true);
            _5lines[0].transform.parent.gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
            {
                switch (colorData[i])
                {
                    case ColorMethods.ColorName.Black:
                        _4lines[i].color = Color.black;
                        break;
                    case ColorMethods.ColorName.Blue:
                        _4lines[i].color = Color.blue;
                        break;
                    case ColorMethods.ColorName.Brown:
                        _4lines[i].color = new Color(71f / 255f, 34f / 255f, 10f / 255f);//Brown
                        break;
                    case ColorMethods.ColorName.Gray:
                        _4lines[i].color = Color.gray;
                        break;
                    case ColorMethods.ColorName.Green:
                        _4lines[i].color = Color.green;
                        break;
                    case ColorMethods.ColorName.Orange:
                        _4lines[i].color = new Color(255f / 255f, 142f / 255f, 0f / 255f);//Orange
                        break;
                    case ColorMethods.ColorName.Purple:
                        _4lines[i].color = new Color(167f / 255f, 0f / 255f, 255f / 255f);//Purple
                        break;
                    case ColorMethods.ColorName.Red:
                        _4lines[i].color = Color.red;
                        break;
                    case ColorMethods.ColorName.Unknown:
                        _4lines[i].color = Color.cyan;
                        break;
                    case ColorMethods.ColorName.White:
                        _4lines[i].color = Color.white;
                        break;
                    case ColorMethods.ColorName.Yellow:
                        _4lines[i].color = Color.yellow;
                        break;
                    case ColorMethods.ColorName.Gold:
                        _4lines[i].color = new Color(255f / 255f, 255f / 255f, 150f / 255f);//Gold
                        break;
                }
            }
        }
        else if (linesNumber == ResistorOhmFinder.LineCount.five)
        {
            if (colorData.Count != 5)
                return;

            _4lines[0].transform.parent.gameObject.SetActive(false);
            _5lines[0].transform.parent.gameObject.SetActive(true);

            for (int i = 0; i < 5; i++)
            {
                switch (colorData[i])
                {
                    case ColorMethods.ColorName.Black:
                        _5lines[i].color = Color.black;
                        break;
                    case ColorMethods.ColorName.Blue:
                        _5lines[i].color = Color.blue;
                        break;
                    case ColorMethods.ColorName.Brown:
                        _5lines[i].color = new Color(71f / 255f, 34f / 255f, 10f / 255f);//Brown
                        break;
                    case ColorMethods.ColorName.Gray:
                        _5lines[i].color = Color.gray;
                        break;
                    case ColorMethods.ColorName.Green:
                        _5lines[i].color = Color.green;
                        break;
                    case ColorMethods.ColorName.Orange:
                        _5lines[i].color = new Color(255f / 255f, 142f / 255f, 0f / 255f);//Orange
                        break;
                    case ColorMethods.ColorName.Purple:
                        _5lines[i].color = new Color(167f / 255f, 0f / 255f, 255f / 255f);//Purple
                        break;
                    case ColorMethods.ColorName.Red:
                        _5lines[i].color = Color.red;
                        break;
                    case ColorMethods.ColorName.Unknown:
                        _5lines[i].color = Color.cyan;
                        break;
                    case ColorMethods.ColorName.White:
                        _5lines[i].color = Color.white;
                        break;
                    case ColorMethods.ColorName.Yellow:
                        _5lines[i].color = Color.yellow;
                        break;
                    case ColorMethods.ColorName.Gold:
                        _4lines[i].color = new Color(255f / 255f, 255f / 255f, 150f / 255f);//Gold
                        break;
                }
            }
        }


    }



}
