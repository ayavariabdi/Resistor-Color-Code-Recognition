using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResistorOhmFinder : MonoBehaviour
{
    public enum LineCount
    {
        four, five
    }
    public static List<ColorData> GetLines(Color[,] data, out Color baseColorInfo)
    {
        //Convert all data to ColorData.
        ColorData[,] colorInfo = GetColorData(data);

        //Find the center of the vertical axis of pixel group.
        int j = colorInfo.GetLength(0) / 2;

        //Set base color info to most left and center pixel.
        baseColorInfo = colorInfo[j, 0].color;

        //Set most left and center pixel as base color of resistor.
        ColorMethods.HSV baseColor = ColorMethods.GetHSV(colorInfo[j, 0].color);

        //Create an empty list to save lines.
        List<ColorData> lines = new List<ColorData>();

        //Set tolerance level to check if a color is same as the base color since base color has no known range.
        float[] tolerance = { 5f, 0.1f, 0.3f };

        //Create a flag to know if last checked pixel was a part of a line.
        ColorData lastPixel = null;

        //Iterate through all pixel from left to right to resolve their colors and find line colors.
        for (int i = 1; i < colorInfo.GetLength(1); i++)
        {
            //Get current pixel.
            ColorData currentPixel = colorInfo[j, i];

            //If current pixel is white, check if it is just a shine effect.
            if (currentPixel.colorName == ColorMethods.ColorName.White)
            {
                currentPixel = CheckShine(i, colorInfo);
            }

            //If color of current pixel couldn't be resolved, continue to next pixel.
            if (currentPixel.colorName == ColorMethods.ColorName.Unknown)
            {
                Debug.LogWarning("Unknown pixel color found: " + currentPixel.color.ToString());
                continue;
            }

            //If current pixel is NOT same as the base color ...
            if (CompareColors(baseColor, ColorMethods.GetHSV(currentPixel.color), tolerance) == false)
            {
                /*If last checked pixel was NOT a line color, that means we found a new line on resistor.
                 We should add this pixel to lines list and turn on the flag to let next pixel know last
                 pixel was a line pixel.*/
                if (lastPixel == null)
                {
                    if (lines.Count < 6)
                    {
                        lines.Add(currentPixel);
                        lastPixel = currentPixel;
                    }
                    /*If there are already 6 lines but found a seventh one, that means this object is not a resistor.*/
                    else
                    {
                        return null;
                    }
                }
                /*If last checked pixel was a line color, but not same as this pixel's color; that means
                 there are two different lines next to each other without a base color seperating them.
                 That is not possible on a resistor.*/
                else
                {
                    if (lastPixel.colorName != currentPixel.colorName)
                    {
                        Debug.LogWarning("Two different adjoining lines found: 1)" + currentPixel.colorName.ToString() + " 2)" + lastPixel.colorName.ToString());
                    }
                }
            }
            //If current pixel is same as the base color, turn off the flag of last pixel was a line.
            else
            {
                lastPixel = null;
            }
        }
        return lines;
    }
    public static List<ColorMethods.ColorName> GetLines(Color[,] data, LineCount lineCount)
    {
        List<ColorMethods.ColorName> result = new List<ColorMethods.ColorName>();

        if (lineCount == LineCount.four)
        {
            //Add most left line
            ColorMethods.ColorName color = GetColor(data, 1);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add second line
            int linePosition = data.GetLength(1) / 3;
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add third line
            linePosition = data.GetLength(1) / 3 * 2;
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add fourth line
            linePosition = data.GetLength(1) - 2;
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;
        }
        else
        {
            //Add most left line
            ColorMethods.ColorName color = GetColor(data, 1);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add second line
            int linePosition = (int)(data.GetLength(1) * 0.27f);
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add third line
            linePosition = (int)(data.GetLength(1) * 0.5f);
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add fourth line
            linePosition = (int)(data.GetLength(1) * 0.73f);
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;

            //Add fourth line
            linePosition = data.GetLength(1) - 2;
            color = GetColor(data, linePosition);
            //if (color != ColorMethods.ColorName.Unknown)
            result.Add(color);
            //else
            //    return null;
        }
        //Debug.Log("LINES FOUND!");
        return result;


    }

    private static ColorData[,] GetColorData(Color[,] data)
    {
        ColorData[,] colorData = new ColorData[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                colorData[i, j] = new ColorData() { color = data[i, j], colorName = ColorMethods.GetResistorColorCode(data[i, j]) };
            }
        }
        return colorData;
    }

    private static bool CompareColors(ColorMethods.HSV baseColor, ColorMethods.HSV color, float[] tolerance)
    {
        return color.h >= baseColor.h - tolerance[0] && color.h <= baseColor.h + tolerance[0] &&
                color.s >= baseColor.s - tolerance[1] && color.s <= baseColor.s + tolerance[1] &&
                color.v >= baseColor.v - tolerance[2] && color.v <= baseColor.v + tolerance[2];
    }

    private static ColorData CheckShine(int i, ColorData[,] colorInfo)
    {
        int midPixel = colorInfo.GetLength(0) / 2;
        for (int j = 0; j < colorInfo.GetLength(0); j++)
        {
            if (colorInfo[j, i].colorName == ColorMethods.ColorName.Unknown || colorInfo[j, i].colorName == ColorMethods.ColorName.White)
            {
                continue;
            }
            else
            {
                return colorInfo[j, i];
            }
        }
        return colorInfo[midPixel, i];
    }
    private static ColorMethods.ColorName GetColor(Color[,] data, int i)
    {
        float r = 0f, g = 0f, b = 0f;
        for (int j = 0; j < data.GetLength(0); j++)
        {
            r += data[j, i].r;
            g += data[j, i].g;
            b += data[j, i].b;
        }
        r /= data.GetLength(0);
        g /= data.GetLength(0);
        b /= data.GetLength(0);

        return ColorMethods.GetResistorColorCode(new Color(r, g, b));
    }

    private static ColorMethods.ColorName GetColor2(Color[,] data, int i)
    {


        int j = (data.GetLength(0) - 1) / 2;

        ColorMethods.ColorName color = ColorMethods.GetResistorColorCode(data[j, i]);
        if (color != ColorMethods.ColorName.Unknown)
            return color;

        //If color did not recognized, look left..
        if (i > 0 && (color = ColorMethods.GetResistorColorCode(data[j, i - 1])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look rigth
        if (i < data.GetLength(1) - 1 && (color = ColorMethods.GetResistorColorCode(data[j, i + 1])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look top   
        if (j > 0 && (color = ColorMethods.GetResistorColorCode(data[j - 1, i])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look bottom   
        if (data.GetLength(0) > 1 && (color = ColorMethods.GetResistorColorCode(data[j + 1, i])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look top left   
        if (j > 0 && i > 0 && (color = ColorMethods.GetResistorColorCode(data[j - 1, i - 1])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look top rigth   
        if (j > 0 && i < data.GetLength(1) - 1 && (color = ColorMethods.GetResistorColorCode(data[j - 1, i + 1])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look bottom left
        if (data.GetLength(0) > 1 && i > 0 && (color = ColorMethods.GetResistorColorCode(data[j + 1, i - 1])) != ColorMethods.ColorName.Unknown)
            return color;

        //If not recognized, look bottom right
        if (data.GetLength(0) > 1 && i < data.GetLength(1) - 1 && (color = ColorMethods.GetResistorColorCode(data[j + 1, i + 1])) != ColorMethods.ColorName.Unknown)
            return color;

        //If never recognized, return null
        return ColorMethods.ColorName.Unknown;
    }

}
public class ColorData
{
    public Color color;
    public ColorMethods.ColorName colorName;

}
public class ResistorData
{
    public string ohmValue;
    public int Set(List<ColorMethods.ColorName> lineData)
    {
        ohmValue = string.Empty;
        if (lineData.Count == 4)
        {
            float digit1 = GetDigitOfLine(lineData[0]);
            float digit2 = GetDigitOfLine(lineData[1]);
            float multiplier = GetMultipliertOfLine(lineData[2]);
            float tolerance = GetToleranceOfLine(lineData[3]);

            if (digit1 == -2 || digit2 == -2 || multiplier == -2)
                return -2;
            else if (digit1 == -1 || digit2 == -1 || multiplier == -1)
                return -1;

            ohmValue = StringifyValue(digit1, digit2, multiplier, tolerance);
            return 0;
        }
        else if (lineData.Count == 5)
        {
            float digit1 = GetDigitOfLine(lineData[0]);
            float digit2 = GetDigitOfLine(lineData[1]);
            float digit3 = GetDigitOfLine(lineData[2]);
            float multiplier = GetMultipliertOfLine(lineData[3]);
            float tolerance = GetToleranceOfLine(lineData[4]);

            if (digit1 == -2 || digit2 == -2 || digit3 == -2 || multiplier == -2)
                return -2;
            else if (digit1 == -1 || digit2 == -1 || digit3 == -1 || multiplier == -1)
                return -1;

            ohmValue = StringifyValue(digit1, digit2, digit3, multiplier, tolerance);
            return 0;
        }
        return -3;
    }

    private float GetDigitOfLine(ColorMethods.ColorName colorName)
    {
        switch (colorName)
        {
            case ColorMethods.ColorName.Black:
                return 0;
            case ColorMethods.ColorName.Blue:
                return 6;
            case ColorMethods.ColorName.Brown:
                return 1;
            case ColorMethods.ColorName.Gold:
                return -1;
            case ColorMethods.ColorName.Gray:
                return 8;
            case ColorMethods.ColorName.Green:
                return 5;
            case ColorMethods.ColorName.Orange:
                return 3;
            case ColorMethods.ColorName.Purple:
                return 7;
            case ColorMethods.ColorName.Red:
                return 2;
            case ColorMethods.ColorName.Unknown:
                return -2;
            case ColorMethods.ColorName.White:
                return 9;
            case ColorMethods.ColorName.Yellow:
                return 4;
            default:
                return -2;
        }
    }
    private float GetToleranceOfLine(ColorMethods.ColorName colorName)
    {
        switch (colorName)
        {
            case ColorMethods.ColorName.Black:
                return -1;
            case ColorMethods.ColorName.Blue:
                return 0.25f;
            case ColorMethods.ColorName.Brown:
                return 1;
            case ColorMethods.ColorName.Gold:
                return 5;
            case ColorMethods.ColorName.Gray:
                return 0.05f;
            case ColorMethods.ColorName.Green:
                return 0.5f;
            case ColorMethods.ColorName.Orange:
                return -1;
            case ColorMethods.ColorName.Purple:
                return 0.1f;
            case ColorMethods.ColorName.Red:
                return 2f;
            case ColorMethods.ColorName.Unknown:
                return -2;
            case ColorMethods.ColorName.White:
                return -1;
            case ColorMethods.ColorName.Yellow:
                return 4;
            default:
                return -2;
        }
    }
    private float GetMultipliertOfLine(ColorMethods.ColorName colorName)
    {
        switch (colorName)
        {
            case ColorMethods.ColorName.Black:
                return 1;
            case ColorMethods.ColorName.Blue:
                return 1000000;
            case ColorMethods.ColorName.Brown:
                return 10;
            case ColorMethods.ColorName.Gold:
                return 0.1f;
            case ColorMethods.ColorName.Gray:
                return -1f;
            case ColorMethods.ColorName.Green:
                return 100000;
            case ColorMethods.ColorName.Orange:
                return 1000;
            case ColorMethods.ColorName.Purple:
                return 10000000;
            case ColorMethods.ColorName.Red:
                return 100;
            case ColorMethods.ColorName.Unknown:
                return -2f;
            case ColorMethods.ColorName.White:
                return -1f;
            case ColorMethods.ColorName.Yellow:
                return 10000;
            default:
                return -2f;
        }
    }
    private string StringifyValue(float digit1, float digit2, float multiplier, float tolerance)
    {
        string multStr = string.Empty;

        float value = (digit1 * 10 + digit2) * multiplier;
        if (value > 1000000000)
        {
            value /= 1000000000;
            multStr = "B";
        }
        else if (value > 10000000)
        {
            value /= 1000000;
            multStr = "M";
        }
        else if (value > 1000)
        {
            value /= 1000;
            multStr = "K";
        }
        return string.Format("{0}{1}ohms ± {2}%", value, multStr, tolerance == -1 ||tolerance == -2 ? "?" : tolerance.ToString());
    }
    private string StringifyValue(float digit1, float digit2, float digit3, float multiplier, float tolerance)
    {
        string multStr = string.Empty;

        float value = (digit1 * 100 + digit2 * 10 + digit3) * multiplier;
        if (value > 1000000000)
        {
            value /= 1000000000;
            multStr = "B";
        }
        else if (value > 10000000)
        {
            value /= 1000000;
            multStr = "M";
        }
        else if (value > 1000)
        {
            value /= 1000;
            multStr = "K";
        }
        return string.Format("{0}{1}ohms ± {2}%", value, multStr, tolerance);
    }

}
