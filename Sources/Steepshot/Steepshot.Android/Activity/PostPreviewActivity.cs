using System;
using Android.App;
using Android.OS;
using Com.Lilarcor.Cheeseknife;
using Square.Picasso;
using Steepshot.Base;
using Steepshot.Core.Utils;
using Steepshot.Utils;

namespace Steepshot.Activity
{
    [Activity(Label = "PostPreviewActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PostPreviewActivity : BaseActivity
    {
        private string _path;

#pragma warning disable 0649, 4014
        [InjectView(Resource.Id.photo)] ScaleImageView _photo;
#pragma warning restore 0649


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.lyt_post_preview);
            Cheeseknife.Inject(this);

            _path = Intent.GetStringExtra("PhotoURL");
            try
            {
                Picasso.With(this).Load(_path).NoFade().Resize(Resources.DisplayMetrics.WidthPixels, 0).Into(_photo);
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Cheeseknife.Reset(this);
        }
    }
}