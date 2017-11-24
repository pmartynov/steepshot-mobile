﻿using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Steepshot.Base;
using Steepshot.Core;
using Steepshot.Core.Presenters;
using Steepshot.Core.Utils;
using Steepshot.Utils;
using Android.Content;
using Android.Runtime;
using Square.Picasso;

namespace Steepshot.Activity
{
    [Activity(Label = Constants.Steepshot, MainLauncher = true, Icon = "@mipmap/launch_icon", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, Icon = "@drawable/logo_login", DataMimeType = "image/*")]
    public sealed class SplashActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppDomain.CurrentDomain.UnhandledException -= OnCurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnTaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser -= OnUnhandledExceptionRaiser;

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledExceptionRaiser;

            var d = new Picasso.Builder(this);
            Cache = new LruCache(this);
            d.MemoryCache(Cache);
            Picasso.SetSingletonInstance(d.Build());


            if (Intent.ActionSend.Equals(Intent.Action) && Intent.Type != null)
            {
                Intent intent;
                if (BasePresenter.User.IsAuthenticated)
                {
                    intent = new Intent(Application.Context, typeof(PostDescriptionActivity));
                    var uri = (Android.Net.Uri)Intent.GetParcelableExtra(Intent.ExtraStream);
                    intent.PutExtra(PostDescriptionActivity.PhotoExtraPath, uri.ToString());
                }
                else
                {
                    intent = new Intent(this, typeof(PreSignInActivity));
                }
                StartActivity(intent);
            }
            else
            {
                StartActivity(BasePresenter.User.IsAuthenticated ? typeof(RootActivity) : typeof(GuestActivity));
            }
        }

        private void OnTaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            AppSettings.Reporter.SendCrash(e.Exception);
            this.ShowAlert(Localization.Errors.UnexpectedError, Android.Widget.ToastLength.Short);
        }

        private void OnCurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
                ex = new Exception(e.ExceptionObject.ToString());
            AppSettings.Reporter.SendCrash(ex);
            this.ShowAlert(Localization.Errors.UnexpectedError, Android.Widget.ToastLength.Short);
        }

        private void OnUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            AppSettings.Reporter.SendCrash(e.Exception);
            this.ShowAlert(Localization.Errors.UnexpectedError, Android.Widget.ToastLength.Short);
        }
    }
}
