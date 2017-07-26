﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;
using Sweetshot.Library.Models.Requests;
using UIKit;

namespace Steepshot.iOS
{
    public partial class DescriptionViewController : BaseViewController
    {
        protected DescriptionViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public DescriptionViewController()
        {
        }

        public UIImage ImageAsset;

        private TagsCollectionViewSource _collectionviewSource;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            photoView.Image = ImageAsset;
            postPhotoButton.TouchDown += (sender, e) => PostPhoto();
            Activeview = descriptionTextField;
            //Collection view initialization
            tagsCollectionView.RegisterClassForCell(typeof(TagCollectionViewCell), nameof(TagCollectionViewCell));
            tagsCollectionView.RegisterNibForCell(UINib.FromName(nameof(TagCollectionViewCell), NSBundle.MainBundle), nameof(TagCollectionViewCell));
            // research flow layout
            /*tagsCollectionView.SetCollectionViewLayout(new UICollectionViewFlowLayout()
            {
                EstimatedItemSize = new CGSize(100, 50),
                
            }, false);*/
            _collectionviewSource = new TagsCollectionViewSource((sender, e) =>
            {
                var myViewController = new PostTagsViewController();
                this.NavigationController.PushViewController(myViewController, true);
            });
            _collectionviewSource.TagsCollection = new List<string>() { "" }; //BaseViewController.User.TagsList;
            _collectionviewSource.RowSelectedEvent += CollectionTagSelected;
            tagsCollectionView.Source = _collectionviewSource;

            UITapGestureRecognizer tap = new UITapGestureRecognizer(() =>
                {
                    descriptionTextField.ResignFirstResponder();
                });
            View.AddGestureRecognizer(tap);
            descriptionTextField.Layer.BorderWidth = 1;
            descriptionTextField.Layer.BorderColor = UIColor.Black.CGColor;
        }

        public override void ViewWillAppear(bool animated)
        {
            NavigationController.NavigationBarHidden = false;
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            _collectionviewSource.TagsCollection.Clear();
            _collectionviewSource.TagsCollection.Add("");
            _collectionviewSource.TagsCollection.AddRange(TagsList);
            tagsCollectionView.ReloadData();
            tagsCollectionView.LayoutIfNeeded();
            collectionHeight.Constant = tagsCollectionView.ContentSize.Height;
        }

        public override void ViewDidDisappear(bool animated)
        {
            if (IsMovingFromParentViewController)
                TagsList.Clear();
            base.ViewDidDisappear(animated);
        }

        private async void PostPhoto()
        {
            loadingView.Hidden = false;
            postPhotoButton.Enabled = false;

            try
            {
                byte[] photoByteArray;
                using (NSData imageData = photoView.Image.AsJPEG(0.4f))
                {
                    photoByteArray = new Byte[imageData.Length];
                    Marshal.Copy(imageData.Bytes, photoByteArray, 0, Convert.ToInt32(imageData.Length));
                }

                var request = new UploadImageRequest(User.SessionId, descriptionTextField.Text, photoByteArray, TagsList.ToArray());
                var imageUploadResponse = await Api.Upload(request);

                if (imageUploadResponse.Success)
                {
                    TagsList.Clear();
                    ShouldProfileUpdate = true;
                    NavigationController.PopViewController(true);
                }
                else
                {
                    Reporter.SendCrash("Photo upload error: " + imageUploadResponse.Errors[0], User.Login, AppVersion);
                    ShowAlert(imageUploadResponse.Errors[0]);
                }
            }
            catch (Exception ex)
            {
                Reporter.SendCrash(ex, User.Login, AppVersion);
            }
            finally
            {
                loadingView.Hidden = true;
                postPhotoButton.Enabled = true;
            }
        }

        void CollectionTagSelected(int row)
        {
            _collectionviewSource.TagsCollection.RemoveAt(row);
            TagsList.RemoveAt(row - 1);
            tagsCollectionView.ReloadData();
        }

        protected override void CalculateBottom()
        {
            Bottom = (Activeview.Frame.Y + scrollView.Frame.Y - scrollView.ContentOffset.Y + Activeview.Frame.Height + Offset);
        }
    }
}
