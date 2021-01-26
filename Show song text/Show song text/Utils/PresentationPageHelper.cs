using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Show_song_text.Utils
{
    public class PresentationPageHelper
    {
        private Label label;

        public Label Label
        {
            get { return label; }
            set { label = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private double fontSize;

        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }



        public PresentationPageHelper(Label label, string text, int fontSize)
        {
            Label = label;
            Text = text;
            FontSize = fontSize;
        }


    }
}
