﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Steepshot.Core.Models.Responses;
using Steepshot.Core.Presenters;
using Steepshot.Core.Utils;
using Steepshot.iOS.Cells;
using Steepshot.iOS.ViewControllers;
using Steepshot.iOS.ViewSources;
using UIKit;

namespace Steepshot.iOS.Views
{
    public partial class CommentsViewController : BaseViewControllerWithPresenter<CommentsPresenter>
    {
        protected override void CreatePresenter()
        {
            _presenter = new CommentsPresenter();
        }

        private readonly CommentsTableViewSource _tableSource = new CommentsTableViewSource();
        public string PostUrl;
        private bool _navigationBarHidden;

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

            _tableSource.Flaged += (vote, url, action) =>
            {
                Flag(vote, url, action);
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

        public override void ViewWillAppear(bool animated)
        {
            _navigationBarHidden = NavigationController.NavigationBarHidden;
            NavigationController.SetNavigationBarHidden(false, true);
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (IsMovingFromParentViewController)
                NavigationController.SetNavigationBarHidden(_navigationBarHidden, true);
            base.ViewWillDisappear(animated);
        }

        public async Task GetComments()
        {
            progressBar.StartAnimating();
            try
            {
                var result = await _presenter.GetComments(PostUrl);
                _tableSource.TableItems.Clear();
                _tableSource.TableItems.AddRange(result);
                commentsTable.ReloadData();
                //kostil?
                commentsTable.SetContentOffset(new CGPoint(0, commentsTable.ContentSize.Height - commentsTable.Frame.Height), false);
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                commentsTable.SetContentOffset(new CGPoint(0, commentsTable.ContentSize.Height - commentsTable.Frame.Height), false);
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            finally
            {
                progressBar.StopAnimating();
            }
        }

        public async Task Vote(bool vote, string postUrl, Action<string, VoteResponse> action)
        {
            if (!BasePresenter.User.IsAuthenticated)
            {
                LoginTapped();
                return;
            }
            try
            {
                var response = await _presenter.Vote(_presenter.Posts.First(p => p.Url == postUrl));
                if (response.Success)
                {
                    _tableSource.TableItems.First(p => p.Url == postUrl).Vote = vote;
                    action.Invoke(postUrl, response.Result);
                    _tableSource.TableItems.First(p => p.Url == postUrl).Flag = false;
                }
                else
                    ShowAlert(response.Errors[0]);
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
        }



        public async Task Flag(bool vote, string postUrl, Action<string, VoteResponse> action)
        {
            if (!BasePresenter.User.IsAuthenticated)
            {
                LoginTapped();
                return;
            }
            try
            {
                var post = _presenter.Posts.First(p => p.Url == postUrl);
                var flagResponse = await _presenter.Flag(post);
                if (flagResponse.Success)
                {
                    post.Flag = flagResponse.Result.IsSucces;
                    if (flagResponse.Result.IsSucces)
                    {
                        if (post.Vote)
                            if (post.NetVotes == 1)
                                post.NetVotes = -1;
                            else
                                post.NetVotes--;
                        post.Vote = false;
                    }
                    action.Invoke(postUrl, flagResponse.Result);
                }
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
        }

        public async Task CreateComment()
        {
            try
            {
                if (!BasePresenter.User.IsAuthenticated)
                {
                    LoginTapped();
                    return;
                }
                var response = await _presenter.CreateComment(commentTextView.Text, PostUrl);
                if (response.Success)
                {
                    commentTextView.Text = string.Empty;
                    await GetComments();
                }
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
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
