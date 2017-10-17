﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Models.Responses;

namespace Steepshot.Core.Presenters
{
    public class PostDescriptionPresenter : ListPresenter<SearchResult>
    {
        private const int ItemsLimit = 40;

        public async Task<OperationResult<ImageUploadResponse>> TryUpload(UploadImageRequest request)
        {
            return await TryRunTask<UploadImageRequest, ImageUploadResponse>(Upload, CancellationToken.None, request);
        }

        private async Task<OperationResult<ImageUploadResponse>> Upload(CancellationToken ct, UploadImageRequest request)
        {
            return await Api.Upload(request, ct);
        }

        public async Task<List<string>> TryLoadNext(string s)
        {
            return await RunAsSingleTask(LoadNext, s);
        }

        private async Task<List<string>> LoadNext(CancellationToken ct, string s)
        {
            var request = new SearchWithQueryRequest(s)
            {
                Offset = OffsetUrl,
                Limit = ItemsLimit
            };

            var response = await Api.SearchCategories(request, ct);

            if (response.Success)
            {
                var tags = response.Result.Results;
                if (tags.Count > 0)
                {
                    lock (Items)
                        Items.AddRange(string.IsNullOrEmpty(OffsetUrl) ? tags : tags.Skip(1));

                    OffsetUrl = tags.Last().Name;
                }

                if (tags.Count < Math.Min(ServerMaxCount, ItemsLimit))
                    IsLastReaded = true;
            }
            return response.Errors;
        }

        public async Task<List<string>> TryGetTopTags()
        {
            return await RunAsSingleTask(GetTopTags);
        }

        private async Task<List<string>> GetTopTags(CancellationToken ct)
        {
            var request = new OffsetLimitFields()
            {
                Offset = OffsetUrl,
                Limit = ItemsLimit
            };

            var response = await Api.GetCategories(request, ct);

            if (response.Success)
            {
                var tags = response.Result.Results;
                if (tags.Count > 0)
                {
                    lock (Items)
                        Items.AddRange(string.IsNullOrEmpty(OffsetUrl) ? tags : tags.Skip(1));

                    OffsetUrl = tags.Last().Name;
                }

                if (tags.Count < Math.Min(ServerMaxCount, ItemsLimit))
                    IsLastReaded = true;
            }
            return response.Errors;
        }
    }
}
