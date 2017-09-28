﻿using System;
using Steepshot.Core.Models;
using UIKit;

namespace Steepshot.iOS.Helpers
{
    public static class PhotoHeight
    {
        private const int minHeight = 100;
        private const int maxHeight = 500;

        public static nfloat Get(Size imageSize)
        {
            var correction = UIScreen.MainScreen.Bounds.Width;
            if (imageSize.Width != 0)
            {
                var height = UIScreen.MainScreen.Bounds.Width * ((float)imageSize.Height / (float)imageSize.Width);
                if (height >= minHeight && height <= maxHeight)
                {
                    correction = height;
                }
            }
            return correction;
        }
    }
}
