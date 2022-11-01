using Android.Content;
using ShowSongText.CustomControls;
using ShowSongText.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace ShowSongText.Droid.CustomRenderer
{
    public class CustomLabelRenderer : LabelRenderer
    {
        public CustomLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                //TODO: Color every 2 lines with chords in other color. 
            }
        }
    }
}