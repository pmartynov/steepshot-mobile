// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Steepshot.iOS.Views
{
	[Register ("PreLoginViewController")]
	partial class PreLoginViewController
	{
		[Outlet]
		UIKit.NSLayoutConstraint aboveConstant { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint belowConstant { get; set; }

		[Outlet]
		UIKit.UISwitch devSwitch { get; set; }

		[Outlet]
		UIKit.UIView golosHidden { get; set; }

		[Outlet]
		UIKit.UIButton loginButton { get; set; }

		[Outlet]
		UIKit.UILabel loginLabel { get; set; }

		[Outlet]
		UIKit.UITextField loginText { get; set; }

		[Outlet]
		UIKit.UIImageView logo { get; set; }

		[Outlet]
		UIKit.UIPickerView picker { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint pickerHeight { get; set; }

		[Outlet]
		UIKit.UILabel signLabel { get; set; }

		[Outlet]
		UIKit.UIButton signUpButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (aboveConstant != null) {
				aboveConstant.Dispose ();
				aboveConstant = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (belowConstant != null) {
				belowConstant.Dispose ();
				belowConstant = null;
			}

			if (devSwitch != null) {
				devSwitch.Dispose ();
				devSwitch = null;
			}

			if (loginButton != null) {
				loginButton.Dispose ();
				loginButton = null;
			}

			if (loginLabel != null) {
				loginLabel.Dispose ();
				loginLabel = null;
			}

			if (loginText != null) {
				loginText.Dispose ();
				loginText = null;
			}

			if (logo != null) {
				logo.Dispose ();
				logo = null;
			}

			if (picker != null) {
				picker.Dispose ();
				picker = null;
			}

			if (pickerHeight != null) {
				pickerHeight.Dispose ();
				pickerHeight = null;
			}

			if (signLabel != null) {
				signLabel.Dispose ();
				signLabel = null;
			}

			if (signUpButton != null) {
				signUpButton.Dispose ();
				signUpButton = null;
			}

			if (golosHidden != null) {
				golosHidden.Dispose ();
				golosHidden = null;
			}
		}
	}
}
