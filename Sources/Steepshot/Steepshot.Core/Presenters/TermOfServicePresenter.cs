﻿using System.Threading;
using System.Threading.Tasks;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Responses;

namespace Steepshot.Core.Presenters
{
    public class TermOfServicePresenter : BasePresenter
    {
        public async Task<OperationResult<TermOfServiceResponse>> TryGetTermsOfService()
        {
            return await TryRunTask(TermsOfService, CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None));
        }

        private async Task<OperationResult<TermOfServiceResponse>> TermsOfService(CancellationTokenSource cts)
        {
            return await Api.TermsOfService(cts);
        }
    }
}
