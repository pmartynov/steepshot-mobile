using System;
using Steepshot.Core.Authority;

namespace Steepshot.Core.Models.Requests
{
    public enum FollowType
    {
        Follow,
        UnFollow
    }

    public class FollowRequest : AuthorizedRequest
    {
        public FollowRequest(UserInfo user, FollowType type, string username) : base(user)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

            Type = type;
            Username = username;
        }

        public FollowType Type { get; }
        public string Username { get; }
    }
}