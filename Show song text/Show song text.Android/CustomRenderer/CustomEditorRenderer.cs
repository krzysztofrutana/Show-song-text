﻿using Android.Content;
using Android.Text.Method;
using Android.Views;
using ShowSongText.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

// This is neccesary to fix Editor scrolling bug in android when Editr is in ScrollView

[assembly: ExportRenderer(typeof(Editor), typeof(CustomEditorRenderer))]
namespace ShowSongText.Droid.CustomRenderer
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;

                //While scrolling inside Editor stop scrolling parent view.
                nativeEditText.OverScrollMode = OverScrollMode.Always;
                nativeEditText.ScrollBarStyle = ScrollbarStyles.InsideInset;
                nativeEditText.SetOnTouchListener(new DroidTouchListener());

                //For Scrolling in Editor innner area
                Control.VerticalScrollBarEnabled = true;
                Control.MovementMethod = ScrollingMovementMethod.Instance;
                Control.ScrollBarStyle = Android.Views.ScrollbarStyles.InsideInset;
                //Force scrollbars to be displayed
                Android.Content.Res.TypedArray a = Control.Context.Theme.ObtainStyledAttributes(new int[0]);
                InitializeScrollbars(a);
                a.Recycle();
            }

            if (Control != null)
            {
                //Control.SetBackground(null);
                //Control.SetBackgroundResource(Resource.Drawable.et_underline_selector);
            }
        }

    }
}