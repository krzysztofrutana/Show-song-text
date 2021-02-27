using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Show_song_text.Droid.CustomRenderer;

[assembly: ExportRenderer(typeof(Xamarin.Forms.RadioButton), typeof(RadioButtonCustomRenderer))]
namespace Show_song_text.Droid.CustomRenderer
{
    public class RadioButtonCustomRenderer : RadioButtonRenderer
    {
        public RadioButtonCustomRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.RadioButton> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.ButtonTintList = ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#FE1A1A"));
            }
        }
    }
}