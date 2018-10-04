﻿using Foundation;
using Steepshot.Core.Models;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Presenters;
using Steepshot.iOS.Cells;
using Steepshot.iOS.ViewControllers;
using Steepshot.iOS.ViewSources;
using Steepshot.Core.Extensions;
using UIKit;
using Steepshot.Core.Models.Enums;
using Steepshot.Core.Models.Responses;
using Steepshot.Core.Localization;
using Steepshot.Core.Utils;

namespace Steepshot.iOS.Views
{
    public partial class FollowViewController : BaseViewControllerWithPresenter<UserFriendPresenter>
    {
        private readonly FriendsType _friendsType;
        private readonly UserProfileResponse _user;

        public FollowViewController(FriendsType friendsType, UserProfileResponse user)
        {
            _friendsType = friendsType;
            _user = user;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Presenter.FollowType = _friendsType;
            Presenter.SourceChanged += SourceChanged;

            var tableSource = new FollowTableViewSource(Presenter, followTableView);
            followTableView.Source = tableSource;
            followTableView.LayoutMargins = UIEdgeInsets.Zero;
            followTableView.RegisterClassForCellReuse(typeof(FollowViewCell), nameof(FollowViewCell));
            followTableView.RegisterNibForCellReuse(UINib.FromName(nameof(FollowViewCell), NSBundle.MainBundle), nameof(FollowViewCell));
            followTableView.RegisterClassForCellReuse(typeof(LoaderCell), nameof(LoaderCell));

            tableSource.ScrolledToBottom += GetItems;
            tableSource.CellAction += CellAction;

            SetBackButton();
            progressBar.StartAnimating();
            GetItems();
        }

        private void SetBackButton()
        {
            var count = _friendsType == FriendsType.Followers ? _user.FollowersCount : _user.FollowingCount;
            var peopleLabel = new UILabel()
            {
                Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PeopleText, count),
                Font = Helpers.Constants.Regular14,
                TextColor = Helpers.Constants.R15G24B30,
            };
            peopleLabel.SizeToFit();

            var leftBarButton = new UIBarButtonItem(UIImage.FromBundle("ic_back_arrow"), UIBarButtonItemStyle.Plain, GoBack);
            var rightBarButton = new UIBarButtonItem(peopleLabel);
            NavigationItem.LeftBarButtonItem = leftBarButton;
            NavigationItem.RightBarButtonItem = rightBarButton;
            NavigationController.NavigationBar.TintColor = Helpers.Constants.R15G24B30;
            NavigationItem.Title = Presenter.FollowType.GetDescription();
        }

        private void CellAction(ActionType type, UserFriend user)
        {
            switch (type)
            {
                case ActionType.Profile:
                    if (user.Author == AppSettings.User.Login)
                        return;
                    var myViewController = new ProfileViewController();
                    myViewController.Username = user.Author;
                    NavigationController.PushViewController(myViewController, true);
                    break;
                case ActionType.Follow:
                    Follow(user);
                    break;
                default:
                    break;
            }
        }

        private void SourceChanged(Status status)
        {
            InvokeOnMainThread(followTableView.ReloadData);
        }

        public async void GetItems()
        {
            var result = await Presenter.TryLoadNextUserFriendsAsync(_user.Username);
            ShowAlert(result);
            progressBar.StopAnimating();
        }

        private async void Follow(UserFriend user)
        {
            if (user != null)
            {
                var result = await Presenter.TryFollowAsync(user);
                ShowAlert(result);
            }
        }

        public override void ViewDidUnload()
        {
            Presenter.LoadCancel();
            base.ViewDidUnload();
        }
    }
}
