using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMethods
{
    public enum ColorName
    {
        Black, Brown, Red, Orange, Yellow, Green, Blue, Purple, Gray, White, Unknown, Gold
    }
    private enum RGBColorName
    {
        red,
        green,
        blue
    }
    private class RGBColor
    {
        public RGBColorName name;
        public float value;
    }
    public class HSV
    {
        public float h, s, v;
        public HSV(float h, float s, float v)
        {
            this.h = h; this.s = s; this.v = v;
        }
        public HSV(HSV hsv)
        {
            h = hsv.h; s = hsv.s; v = hsv.v;
        }

        public override string ToString()
        {
            return "H: " + h.ToString("0.00") + " S: " + s.ToString("0.00") + " V: " + v.ToString("0.00");
            //return "H: " + h + " S: " + s + " V: " + v;

        }
    }
    public static float GetHue(Color color)
    {
        RGBColor maxColor = GetMax(color);
        RGBColor minColor = GetMin(color);
        float delta = maxColor.value - minColor.value;
        float r = color.r, g = color.g, b = color.b;
        float h = 0;
        switch (maxColor.name)
        {
            case RGBColorName.red:
                h = (g - b) / delta % 6;
                break;
            case RGBColorName.green:
                h = (b - r) / delta + 2;
                break;
            case RGBColorName.blue:
                h = (r - g) / delta + 4;
                break;
            default:
                h = 0f;
                break;
        }
        h *= 60;
        if (Mathf.Sign(h) == -1f)
        {
            h += 360f;
        }
        if (float.IsNaN(h))
        {
            h = 0f;
        }
        return h;
    }

    public static float GetLightness(Color color)
    {
        RGBColor maxColor = GetMax(color);
        RGBColor minColor = GetMin(color);
        return (maxColor.value + minColor.value) / 2f;
    }

    public static float GetSaturation(Color color)
    {
        RGBColor maxColor = GetMax(color);
        RGBColor minColor = GetMin(color);
        float delta = maxColor.value - minColor.value;
        if (delta == 0)
        {
            return 0;
        }

        return delta / (1 - Mathf.Abs(2 * GetLightness(color) - 1));
    }
    public static float GetValue(Color color)
    {
        return GetMax(color).value;
    }

    public static HSV GetHSV(Color color)
    {
        return new HSV(GetHue(color), GetSaturation(color), GetValue(color));
    }

    public static HSV GetHSL(Color color)
    {
        return new HSV(GetHue(color), GetSaturation(color), GetLightness(color));
    }


    public static ColorName GetResistorColorCode(Color color)
    {
        HSV hsv = GetHSV(color);
        ////hsv.h *= 255;
        //hsv.s *= 100;
        //hsv.v *= 100;
        Dictionary<ColorName, List<HSV[]>> colors = new Dictionary<ColorName, List<HSV[]>>();



        HSV[] brown = { new HSV(-1f, 0.15f, 0.05f), new HSV(50f, .7f, 0.5f) };//s:0.6->0.3(mismatching with unknown)
        HSV[] brown2 = { new HSV(300f, 0.15f, 0.05f), new HSV(360f, .7f, 0.25f) };//s:0.6->0.3(mismatching with unknown)
        HSV[] red1 = { new HSV(-1f, .3f, .25f), new HSV(11f, 2f, 2f) };
        HSV[] red2 = { new HSV(320f, .3f, .25f), new HSV(360f, 2f, 2f) };//340->320(mismatching with purple)
        HSV[] gold = { new HSV(35f, .1f, .25f), new HSV(70f, .6f, .7f) };
        HSV[] orange = { new HSV(11f, .3f, .4f), new HSV(45f, 2f, 2f) };
        HSV[] yellow = { new HSV(55f, .29f, .39f), new HSV(85f, 2f, 2f) };
        HSV[] green = { new HSV(85f, .3f, .25f), new HSV(179f, 2f, 2f) };
        HSV[] blue = { new HSV(179f, .3f, .25f), new HSV(240f, 2f, 2f) };
        HSV[] purple = { new HSV(240f, .15f, .25f), new HSV(320f, 2f, 2f) };//340->320(mismatching with red)
        HSV[] gray = { new HSV(-1f, -1f, .05f), new HSV(360f, .2f, .75f) };
        HSV[] white = { new HSV(-1f, -1f, .75f), new HSV(360f, .1f, 2f) };
        HSV[] black = { new HSV(-1f, -1f, -1f), new HSV(360f, 2f, 0.2f) };//v:0.05->0.1(mismatching with unknown)


        colors.Add(ColorName.Brown, new List<HSV[]>() { brown });
        colors[ColorName.Brown].Add(brown2);
        colors.Add(ColorName.Red, new List<HSV[]>() { red1 });
        colors[ColorName.Red].Add(red2);
        colors.Add(ColorName.Gold, new List<HSV[]>() { gold });
        colors.Add(ColorName.Orange, new List<HSV[]>() { orange });
        colors.Add(ColorName.Yellow, new List<HSV[]>() { yellow });
        colors.Add(ColorName.Green, new List<HSV[]>() { green });
        colors.Add(ColorName.Blue, new List<HSV[]>() { blue });
        colors.Add(ColorName.Purple, new List<HSV[]>() { purple });
        colors.Add(ColorName.Gray, new List<HSV[]>() { gray });
        colors.Add(ColorName.White, new List<HSV[]>() { white });
        colors.Add(ColorName.Black, new List<HSV[]>() { black });

        foreach (var item in colors)
        {
            foreach (var subItem in item.Value)
            {
                if (IsHsvBetween(hsv, subItem[0], subItem[1]))
                    return item.Key;
            }
        }

        return ColorName.Unknown;
    }
    //public static ColorName GetResistorColorCode(Color color)
    //{
    //    HSV hsv = GetHSL(color);
    //    //hsv.h *= 255;
    //    hsv.s *= 255;
    //    hsv.v *= 255;
    //    Dictionary<ColorName, List<HSV[]>> colors = new Dictionary<ColorName, List<HSV[]>>();

    //    HSV[] black = { new HSV(0, 0, 0), new HSV(360, 255, 50) };
    //    colors.Add(ColorName.Black, new List<HSV[]>() { black });
    //    HSV[] brown = { new HSV(0, 90, 10), new HSV(30, 255, 100) };
    //    colors.Add(ColorName.Brown, new List<HSV[]>() { brown });
    //    HSV[] red1 = { new HSV(0, 255f*0.5f, 255f*0.5f), new HSV(13, 255, 150) };
    //    colors.Add(ColorName.Red, new List<HSV[]>() { red1 });
    //    HSV[] red2 = { new HSV(342, 65, 50), new HSV(360, 255, 50) };
    //    colors[ColorName.Red].Add(red2);
    //    HSV[] orange = { new HSV(8, 100, 100), new HSV(18, 255, 150) };
    //    colors.Add(ColorName.Orange, new List<HSV[]>() { orange });
    //    HSV[] yellow = { new HSV(40, 130, 100), new HSV(60, 255, 160) };
    //    colors.Add(ColorName.Yellow, new List<HSV[]>() { yellow });
    //    HSV[] green = { new HSV(90, 50, 60), new HSV(144, 255, 150) };
    //    colors.Add(ColorName.Green, new List<HSV[]>() { green });
    //    HSV[] blue = { new HSV(160, 50, 50), new HSV(212, 255, 150) };
    //    colors.Add(ColorName.Blue, new List<HSV[]>() { blue });
    //    HSV[] purple = { new HSV(260, 40, 50), new HSV(310, 255, 150) };
    //    colors.Add(ColorName.Purple, new List<HSV[]>() { purple });
    //    HSV[] gray = { new HSV(0, 0, 50), new HSV(360, 50, 80) };
    //    colors.Add(ColorName.Gray, new List<HSV[]>() { gray });
    //    HSV[] white = { new HSV(0, 0, 90), new HSV(360, 15, 140) };
    //    colors.Add(ColorName.White, new List<HSV[]>() { white });

    //    foreach (var item in colors)
    //    {
    //        foreach (var subItem in item.Value)
    //        {
    //            if (IsHsvBetween(hsv, subItem[0], subItem[1]))
    //                return item.Key;
    //        }
    //    }

    //    return ColorName.Unknown;
    //}

    private static RGBColor GetMax(Color color)
    {
        RGBColor maxColor = new RGBColor();
        maxColor.name = RGBColorName.red;
        maxColor.value = color.r;
        if (color.g > maxColor.value)
        {
            maxColor.value = color.g;
            maxColor.name = RGBColorName.green;
        }
        if (color.b > maxColor.value)
        {
            maxColor.value = color.b;
            maxColor.name = RGBColorName.blue;
        }
        return maxColor;
    }
    private static RGBColor GetMin(Color color)
    {
        RGBColor minColor = new RGBColor();
        minColor.name = RGBColorName.red;
        minColor.value = color.r;
        if (color.g < minColor.value)
        {
            minColor.value = color.g;
            minColor.name = RGBColorName.green;
        }
        if (color.b < minColor.value)
        {
            minColor.value = color.b;
            minColor.name = RGBColorName.blue;
        }
        return minColor;
    }
    private static bool IsHsvBetween(HSV value, HSV border1, HSV border2)
    {
        HSV lowerBorder = new HSV(border1);
        if (border2.h < lowerBorder.h)
            lowerBorder.h = border2.h;
        if (border2.s < lowerBorder.s)
            lowerBorder.s = border2.s;
        if (border2.v < lowerBorder.v)
            lowerBorder.v = border2.v;

        HSV upperBorder = new HSV(border2);
        if (border1.h > upperBorder.h)
            upperBorder.h = border1.h;
        if (border1.s > upperBorder.s)
            upperBorder.s = border1.s;
        if (border1.v > upperBorder.v)
            upperBorder.v = border1.v;

        //if (value.h >= lowerBorder.h && value.h <= upperBorder.h &&
        //    value.s >= lowerBorder.s && value.s <= upperBorder.s &&
        //    value.v >= lowerBorder.v && value.v <= upperBorder.v)
        //    return true;
        if (value.h >= lowerBorder.h)
            if (value.s >= lowerBorder.s)
                if (value.v >= lowerBorder.v)
                    if (value.h <= upperBorder.h)
                        if (value.s <= upperBorder.s)
                            if (value.v <= upperBorder.v)
                                return true;

        return false;
    }

}


