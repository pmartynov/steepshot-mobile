﻿using System.Threading;
using System.Threading.Tasks;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Models.Responses;

namespace Steepshot.Core.Presenters
{
    public class SignInPresenter : BasePresenter
    {
        public async Task<OperationResult<LoginResponse>> TrySignIn(string login, string postingKey)
        {
            return await TryRunTask(SignIn, CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None), login, postingKey);
        }

        private Task<OperationResult<LoginResponse>> SignIn(CancellationTokenSource cts, string login, string postingKey)
        {
            var request = new AuthorizedRequest(login, postingKey);
            return Api.LoginWithPostingKey(request, cts);
        }
    }
}