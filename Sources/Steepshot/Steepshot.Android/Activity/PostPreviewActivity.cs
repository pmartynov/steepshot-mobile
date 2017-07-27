using System;
using Android.App;
using Android.OS;
using Com.Lilarcor.Cheeseknife;
using Square.Picasso;
using Steepshot.Base;
using Steepshot.Core.Utils;
using Steepshot.Presenter;

using Steepshot.Utils;
using Steepshot.View;

namespace Steepshot.Activity
{
    [Activity(Label = "PostPreviewActivity", ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class PostPreviewActivity : BaseActivity, IPostPreviewView
    {
        private PostPreviewPresenter _presenter;
		private string _path;

        protected override void CreatePresenter()
        {
            _presenter = new PostPreviewPresenter(this);
        }

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
				Reporter.SendCrash(ex, BasePresenter.User.Login, BasePresenter.AppVersion);
			}
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Cheeseknife.Reset(this);
		}
    }
}