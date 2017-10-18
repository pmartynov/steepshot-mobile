﻿using System;
using Steepshot.Core.Exceptions;

namespace Steepshot.Core.Models.Requests
{
    public class UserExistsRequests
    {
        public UserExistsRequests(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new UserException(Localization.Errors.EmptyUsernameField);

            Username = username;
        }

        public string Username { get; }
    }

    public class UserProfileRequest : NamedRequest
    {
        public UserProfileRequest(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new UserException(Localization.Errors.EmptyUsernameField);

            Username = username;
        }

        public string Username { get; }
    }

    public enum FriendsType
    {
        Followers,
        Following
    }

    public class UserFriendsRequest : NamedRequestWithOffsetLimitFields
    {
        public UserFriendsRequest(string username, FriendsType type)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new UserException(Localization.Errors.EmptyUsernameField);

            Username = username;
            Type = type;
        }

        public string Username { get; }
        public FriendsType Type { get; }
    }
}