﻿using System.Collections.Generic;
using System.Linq;
using Sweetshot.Library.Models.Responses;

namespace Steepshot.iOS
{
	public static class Extensions
	{
		public static void FilterNSFW(this List<Post> list)
		{
			if (!UserContext.Instanse.NSFW)
				list.RemoveAll(p => p.Category.Contains("nsfw") || p.Tags.Any(t => t.Contains("nsfw")));
		}
	}
}
