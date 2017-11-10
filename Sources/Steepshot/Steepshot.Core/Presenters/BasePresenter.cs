﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cryptography.ECDSA;
using Ditch;
using Steepshot.Core.Authority;
using Steepshot.Core.HttpClient;
using Steepshot.Core.Utils;
using Steepshot.Core.Exceptions;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Services;

namespace Steepshot.Core.Presenters
{
    public abstract class BasePresenter
    {
        private static readonly Dictionary<string, double> CurencyConvertationDic;
        private static readonly CultureInfo CultureInfo;
        protected static readonly ISteepshotApiClient Api;

        private static readonly object ctsSync = new object();
        private static CancellationTokenSource _reconecTokenSource;
        private static IConnectionService _connectionService;

        public static IConnectionService ConnectionService => _connectionService ?? (_connectionService = AppSettings.ConnectionService);
        public static bool ShouldUpdateProfile;
        public static event Action<string> OnAllert;

        protected CancellationTokenSource OnDisposeCts;

        public static string Currency
        {
            get
            {
                if (AppSettings.AppInfo.GetPlatform() == "iOS")
                    return "SBD";
                return Chain == KnownChains.Steem ? "$" : "₽";
            }
        }
        public static User User { get; set; }
        public static KnownChains Chain { get; set; }

        static BasePresenter()
        {
            CultureInfo = CultureInfo.InvariantCulture;
            User = new User();
            User.Load();
            Chain = User.Chain;
            //TODO:KOA: endpoint for CurencyConvertation needed
            CurencyConvertationDic = new Dictionary<string, double> { { "GBG", 2.4645 }, { "SBD", 1 } };

            Api = new DitchApi();

            var ts = GetReconectToken();
            TryRunTask(TryConnect, ts, Chain, AppSettings.IsDev);
            // static constructor initialization.
            Task.Run(() =>
            {
                var init = new Secp256k1Manager();
            });
        }

        protected BasePresenter()
        {
            OnDisposeCts = new CancellationTokenSource();
        }

        private static async Task<List<string>> TryConnect(CancellationToken token, KnownChains chain, bool isDev)
        {
            var isConnectionAvailable = ConnectionService.IsConnectionAvailable();
            var isConnected = await Api.Connect(chain, isDev, isConnectionAvailable, token);

            if (!isConnected)
            {
                OnAllert?.Invoke(isConnectionAvailable
                    ? Localization.Errors.InternetUnavailable
                    : Localization.Errors.EnableConnectToBlockchain);

                await TryRunTask(TryReconect, token);
            }
            return new List<string>();
        }

        private static async Task<List<string>> TryReconect(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                    return null;

                await Task.Delay(5000, token);

                var isConnected = ConnectionService.IsConnectionAvailable();
                if (!isConnected)
                {
                    OnAllert?.Invoke(Localization.Errors.EnableConnectToBlockchain);
                }
                else
                {
                    isConnected = Api.TryReconnectChain(Chain, token);
                    if (!isConnected)
                    {
                        OnAllert?.Invoke(Localization.Errors.EnableConnectToBlockchain);
                    }
                    else
                    {
                        OnAllert?.Invoke(string.Empty);
                        return null;
                    }
                }
            } while (true);
        }

        public static async Task SwitchChain(bool isDev)
        {
            if (AppSettings.IsDev == isDev)
                return;

            AppSettings.IsDev = isDev;

            var ts = GetReconectToken();
            await TryRunTask(TryConnect, ts, Chain, isDev);
        }

        public static async Task SwitchChain(UserInfo userInfo)
        {
            if (Chain == userInfo.Chain)
                return;

            User.SwitchUser(userInfo);

            Chain = userInfo.Chain;

            var ts = GetReconectToken();
            await TryRunTask(TryConnect, ts, userInfo.Chain, AppSettings.IsDev);
        }

        public static async Task SwitchChain(KnownChains chain)
        {
            if (Chain == chain)
                return;

            Chain = chain;


            var ts = GetReconectToken();
            await TryRunTask(TryConnect, ts, chain, AppSettings.IsDev);
        }

        public static string ToFormatedCurrencyString(Money value, string postfix = null)
        {
            var dVal = value.ToDouble();
            if (!string.IsNullOrEmpty(value.Currency) && CurencyConvertationDic.ContainsKey(value.Currency))
                dVal *= CurencyConvertationDic[value.Currency];
            return $"{Currency} {dVal.ToString("F", CultureInfo)}{(string.IsNullOrEmpty(postfix) ? string.Empty : " ")}{postfix}";
        }


        private static CancellationToken GetReconectToken()
        {
            CancellationToken ts;
            lock (ctsSync)
            {
                if (_reconecTokenSource != null && !_reconecTokenSource.IsCancellationRequested)
                {
                    _reconecTokenSource.Cancel();
                }
                _reconecTokenSource = new CancellationTokenSource();
                ts = _reconecTokenSource.Token;
            }

            return ts;
        }

        #region TryRunTask

        protected static async Task<OperationResult<TResult>> TryRunTask<TResult>(Func<CancellationToken, Task<OperationResult<TResult>>> func, CancellationToken ct)
        {
            try
            {
                var available = ConnectionService.IsConnectionAvailable();
                if (!available)
                    return new OperationResult<TResult> { Errors = new List<string> { Localization.Errors.InternetUnavailable } };

                return await func(ct);
            }
            catch (OperationCanceledException)
            {
                // to do nothing
            }
            catch (ApplicationExceptionBase ex)
            {
                var opr = new OperationResult<TResult>();
                opr.Errors.Add(ex.Message);
                return opr;
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            return null;
        }

        protected static async Task<OperationResult<TResult>> TryRunTask<T1, TResult>(Func<CancellationToken, T1, Task<OperationResult<TResult>>> func, CancellationToken ct, T1 param1)
        {
            try
            {
                var available = ConnectionService.IsConnectionAvailable();
                if (!available)
                    return new OperationResult<TResult> { Errors = new List<string> { Localization.Errors.InternetUnavailable } };

                return await func(ct, param1);
            }
            catch (OperationCanceledException)
            {
                // to do nothing
            }
            catch (ApplicationExceptionBase ex)
            {
                var opr = new OperationResult<TResult>();
                opr.Errors.Add(ex.Message);
                return opr;
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            return null;
        }

        protected static async Task<OperationResult<TResult>> TryRunTask<T1, T2, TResult>(Func<CancellationToken, T1, T2, Task<OperationResult<TResult>>> func, CancellationToken ct, T1 param1, T2 param2)
        {
            try
            {
                var available = ConnectionService.IsConnectionAvailable();
                if (!available)
                    return new OperationResult<TResult> { Errors = new List<string> { Localization.Errors.InternetUnavailable } };

                return await func(ct, param1, param2);
            }
            catch (OperationCanceledException)
            {
                // to do nothing
            }
            catch (ApplicationExceptionBase ex)
            {
                var opr = new OperationResult<TResult>();
                opr.Errors.Add(ex.Message);
                return opr;
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            return null;
        }


        protected static async Task<List<string>> TryRunTask(Func<CancellationToken, Task<List<string>>> func, CancellationToken ct)
        {
            try
            {
                var available = ConnectionService.IsConnectionAvailable();
                if (!available)
                    return new List<string> { Localization.Errors.InternetUnavailable };

                return await func(ct);
            }
            catch (OperationCanceledException)
            {
                // to do nothing
            }
            catch (ApplicationExceptionBase ex)
            {
                return new List<string> { ex.Message };
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            return null;
        }

        protected static async Task<List<string>> TryRunTask<T1>(Func<CancellationToken, T1, Task<List<string>>> func, CancellationToken ct, T1 param1)
        {
            try
            {
                var available = ConnectionService.IsConnectionAvailable();
                if (!available)
                    return new List<string> { Localization.Errors.InternetUnavailable };

                return await func(ct, param1);
            }
            catch (OperationCanceledException)
            {
                // to do nothing
            }
            catch (ApplicationExceptionBase ex)
            {
                return new List<string> { ex.Message };
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            return null;
        }

        protected static async Task<List<string>> TryRunTask<T1, T2>(Func<CancellationToken, T1, T2, Task<List<string>>> func, CancellationToken ct, T1 param1, T2 param2)
        {
            try
            {
                var available = ConnectionService.IsConnectionAvailable();
                if (!available)
                    return new List<string> { Localization.Errors.InternetUnavailable };

                return await func(ct, param1, param2);
            }
            catch (OperationCanceledException)
            {
                // to do nothing
            }
            catch (ApplicationExceptionBase ex)
            {
                return new List<string> { ex.Message };
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
            }
            return null;
        }

        #endregion

        public void TasksCancel(bool andDispose = false)
        {
            if (!OnDisposeCts.IsCancellationRequested)
                OnDisposeCts.Cancel();

            OnDisposeCts = new CancellationTokenSource();
        }
    }
}
