﻿using System;
using CoreGraphics;
using Foundation;
using Steepshot.iOS.Helpers;
using UIKit;

namespace Steepshot.iOS.CustomViews
{
    public class SearchTextField : UITextField
    {
        public UIButton ClearButton
        {
            get;
            private set;
        }

        public Action ClearButtonTapped;

        public SearchTextField(Action returnButtonTapped, string placeholder)
        {
            ClearButton = new UIButton();
            ClearButton.Hidden = true;
            ClearButton.SetImage(UIImage.FromBundle("ic_delete_tag"), UIControlState.Normal);
            ClearButton.Frame = new CGRect(0, 0, 16, 16);
            ClearButton.TouchDown += (sender, e) =>
            {
                Text = string.Empty;
                ClearButton.Hidden = true;
                ((TagFieldDelegate)Delegate).ChangeBackground(this);
                ClearButtonTapped?.Invoke();
            };
            RightView = ClearButton;
            RightViewMode = UITextFieldViewMode.Always;

            var _searchPlaceholderAttributes = new UIStringAttributes
            {
                Font = Constants.Regular14,
                ForegroundColor = Constants.R151G155B158,
            };

            var at = new NSMutableAttributedString();
            at.Append(new NSAttributedString(placeholder, _searchPlaceholderAttributes));
            AttributedPlaceholder = at;
            AutocorrectionType = UITextAutocorrectionType.No;
            AutocapitalizationType = UITextAutocapitalizationType.None;
            BackgroundColor = Constants.R245G245B245;
            Font = Constants.Regular14;
            Layer.CornerRadius = 20;

            Delegate = new TagFieldDelegate(returnButtonTapped);
            EditingChanged += DoEditingChanged;
        }

        private void DoEditingChanged(object sender, EventArgs e)
        {
            ClearButton.Hidden = Text.Length == 0;
        }

        public void Clear()
        {
            Text = string.Empty;
            ClearButton.Hidden = true;
        }

        public override CGRect TextRect(CGRect forBounds)
        {
            return base.TextRect(forBounds.Inset(20, 0));
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            return base.EditingRect(forBounds.Inset(20, 0));
        }

        public override CGRect RightViewRect(CGRect forBounds)
        {
            return base.RightViewRect(forBounds.Inset(20, 0));
        }
    }
}
