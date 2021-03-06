﻿using System;
using Xamarin.Forms;

namespace XamarinLabelFontSizer
{
    public struct FontCalc
    {
        public double FontSize { private set; get; }
        public double TextHeight { private set; get; }

        public FontCalc(Label label, double fontSize, double containerWidth, double containerHeight)
            : this()
        {
            // Save the font size.
            FontSize = fontSize;

            // Recalculate the Label height.
            label.FontSize = fontSize;
            SizeRequest sizeRequest =
                label.Measure(containerWidth, containerHeight);

            // Save that height.
            TextHeight = sizeRequest.Request.Height;
        }
    }
}
