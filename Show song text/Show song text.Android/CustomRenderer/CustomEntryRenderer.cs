using Android.Content;
using ShowSongText.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace ShowSongText.Droid.CustomRenderer
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                //Control.SetBackgroundResource(Resource.Drawable.et_underline_selector);
            }
        }
    }

}