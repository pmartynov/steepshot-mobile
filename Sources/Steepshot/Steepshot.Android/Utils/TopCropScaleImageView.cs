﻿using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace Steepshot.Utils
{
	public class TopCropScaleImageView : ImageView
	{
		public TopCropScaleImageView(Context c) : base(c)
		{
			setup();
		}

		public TopCropScaleImageView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			setup();
		}

		public TopCropScaleImageView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			setup();
		}

		private void setup()
		{
			SetScaleType(ScaleType.Matrix);
		}

		protected override bool SetFrame(int l, int t, int r, int b)
		{
			if (Drawable == null)
				return false;
			
			float frameWidth = r - l;
			float frameHeight = b - t;
			float originalImageWidth = (float)Drawable.IntrinsicWidth;
			float originalImageHeight = (float)Drawable.IntrinsicHeight;

			float usedScaleFactor = 1;

			if ((frameWidth > originalImageWidth) || (frameHeight > originalImageHeight))
			{
				float fitHorizontallyScaleFactor = frameWidth / originalImageWidth;
				float fitVerticallyScaleFactor = frameHeight / originalImageHeight;

				usedScaleFactor = Math.Max(fitHorizontallyScaleFactor, fitVerticallyScaleFactor);
			}

			float newImageWidth = originalImageWidth * usedScaleFactor;
			float newImageHeight = originalImageHeight * usedScaleFactor;

			Matrix matrix = ImageMatrix;
			matrix.SetScale(usedScaleFactor, usedScaleFactor, 0, 0); // Replaces the old matrix completly

			matrix.PostTranslate((frameWidth - newImageWidth) / 2, frameHeight - newImageHeight);
			ImageMatrix.Set(matrix);
			return base.SetFrame(l, t, r, b);
		}
	}
}
