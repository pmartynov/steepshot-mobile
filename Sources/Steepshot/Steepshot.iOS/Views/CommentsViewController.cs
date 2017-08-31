﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Models.Responses;
using Steepshot.Core.Utils;
using Steepshot.iOS.Cells;
using Steepshot.iOS.ViewControllers;
using Steepshot.iOS.ViewSources;
using UIKit;

namespace Steepshot.iOS.Views
{
    public partial class CommentsViewController : BaseViewController
    {
        protected CommentsViewController(IntPtr handle) : base(handle) { }

        public CommentsViewController() { }

        private readonly CommentsTableViewSource _tableSource = new CommentsTableViewSource();
        public string PostUrl;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.SetNavigationBarHidden(false, false);

            commentsTable.Source = _tableSource;
            commentsTable.LayoutMargins = UIEdgeInsets.Zero;
            commentsTable.RegisterClassForCellReuse(typeof(CommentTableViewCell), nameof(CommentTableViewCell));
            commentsTable.RegisterNibForCellReuse(UINib.FromName(nameof(CommentTableViewCell), NSBundle.MainBundle), nameof(CommentTableViewCell));
            Activeview = commentTextView;
            _tableSource.Voted += (vote, url, action) =>
            {
                Vote(vote, url, action);
            };

            _tableSource.GoToProfile += (username) =>
            {
                var myViewController = new ProfileViewController();
                myViewController.Username = username;
                NavigationController.PushViewController(myViewController, true);
            };

            commentsTable.RowHeight = UITableView.AutomaticDimension;
            commentsTable.EstimatedRowHeight = 150f;
            commentTextView.Delegate = new TextViewDelegate();

            sendButton.TouchDown += (sender, e) =>
            {
                CreateComment();
            };

            GetComments();
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (IsMovingFromParentViewController)
                NavigationController.SetNavigationBarHidden(true, true);
            base.ViewWillDisappear(animated);
        }

        public async Task GetComments()
        {
            progressBar.StartAnimating();
            try
            {
                var request = new NamedInfoRequest(PostUrl);
                var result = await Api.GetComments(request);
                _tableSource.TableItems.Clear();
                _tableSource.TableItems.AddRange(result.Result.Results);
                commentsTable.ReloadData();
                //kostil?
                commentsTable.SetContentOffset(new CGPoint(0, commentsTable.ContentSize.Height - commentsTable.Frame.Height), false);
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                commentsTable.SetContentOffset(new CGPoint(0, commentsTable.ContentSize.Height - commentsTable.Frame.Height), false);
            }
            catch (Exception ex)
            {
                Reporter.SendCrash(ex, User.Login, AppVersion);
            }
            finally
            {
                progressBar.StopAnimating();
            }
        }

        public async Task Vote(bool vote, string postUrl, Action<string, VoteResponse> action)
        {
            if (!User.IsAuthenticated)
            {
                LoginTapped();
                return;
            }
            try
            {
                int diezid = postUrl.IndexOf('#');
                string posturl = postUrl.Substring(diezid + 1);

                var voteRequest = new VoteRequest(User.UserInfo, vote ? VoteType.Up : VoteType.Down, posturl);
                var response = await Api.Vote(voteRequest);
                if (response.Success)
                {
                    _tableSource.TableItems.First(p => p.Url == postUrl).Vote = vote;
                    action.Invoke(postUrl, response.Result);
                }
            }
            catch (Exception ex)
            {
                Reporter.SendCrash(ex, User.Login, AppVersion);
            }
        }

        public async Task CreateComment()
        {
            try
            {
                if (!User.IsAuthenticated)
                {
                    LoginTapped();
                    return;
                }
                var reqv = new CreateCommentRequest(User.UserInfo, PostUrl, commentTextView.Text, commentTextView.Text);
                var response = await Api.CreateComment(reqv);
                if (response.Success)
                {
                    commentTextView.Text = string.Empty;
                    await GetComments();
                }
            }
            catch (Exception ex)
            {
                Reporter.SendCrash(ex, User.Login, AppVersion);
            }
        }

        void LoginTapped()
        {
            NavigationController.PushViewController(new PreLoginViewController(), true);
        }

        protected override void CalculateBottom()
        {
            Bottom = (Activeview.Frame.Y + bottomView.Frame.Y + Activeview.Frame.Height + Offset);
        }

        class TextViewDelegate : UITextViewDelegate
        {
            public override bool ShouldChangeText(UITextView textView, NSRange range, string text)
            {
                if (text == "\n")
                {
                    textView.ResignFirstResponder();
                    return false;
                }
                return true;
            }
        }
    }
}

