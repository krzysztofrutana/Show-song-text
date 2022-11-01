using Android.Content;
using Android.Content.Res;
using ShowSongText.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.RadioButton), typeof(RadioButtonCustomRenderer))]
namespace ShowSongText.Droid.CustomRenderer
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