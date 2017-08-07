﻿using System.Collections.Generic;
using System.Globalization;
using Steepshot.Core;
using Steepshot.Core.Authority;
using Steepshot.Core.HttpClient;
using Steepshot.Core.Models;
using Steepshot.Core.Utils;

namespace Steepshot.Base
{
    public class BasePresenter
    {
        private static ISteepshotApiClient _apiClient;
        public static string AppVersion { get; set; }
        public static string Currency => Chain == KnownChains.Steem ? "$" : "₽";
        private static readonly Dictionary<string, double> CurencyConvertationDic;
        private static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

        public static bool ShouldUpdateProfile;
        public static User User { get; set; }
        public static KnownChains Chain { get; set; }

        protected IBaseView View;

        protected static ISteepshotApiClient Api
        {
            get
            {
                if (_apiClient == null)
                    SwitchChain(Chain);
                return _apiClient;
            }
        }

        static BasePresenter()
        {
            User = new User();
            User.Load();
            Chain = User.Chain;
            //TODO:KOA: endpoint for CurencyConvertation needed
            CurencyConvertationDic = new Dictionary<string, double> { { "GBG", 2.4645 }, { "SBD", 1 } };
        }

        public BasePresenter(IBaseView view)
        {
            View = view;
        }

        public static void SwitchChain(bool isDev)
        {
            if (AppSettings.IsDev == isDev && _apiClient != null)
                return;

            AppSettings.IsDev = isDev;

            InitApiClient(Chain, isDev);
        }

        public static void SwitchChain(UserInfo userInfo)
        {
            if (Chain == userInfo.Chain && _apiClient != null)
                return;

            User.SwitchUser(userInfo);

            Chain = userInfo.Chain;
            InitApiClient(userInfo.Chain, AppSettings.IsDev);
        }

        public static void SwitchChain(KnownChains chain)
        {
            if (Chain == chain && _apiClient != null)
                return;

            Chain = chain;
            InitApiClient(chain, AppSettings.IsDev);
        }

        private static void InitApiClient(KnownChains chain, bool isDev)
        {
            if (isDev)
            {
                _apiClient = new DitchApi(chain == KnownChains.Steem ? Constants.SteemUrlQa : Constants.GolosUrlQa, chain);
            }
            else
            {
                _apiClient = new DitchApi(chain == KnownChains.Steem ? Constants.SteemUrl : Constants.GolosUrl, chain);
            }
        }

        public static string ToFormatedCurrencyString(Money value)
        {
            var dVal = value.ToDouble();
            if (!string.IsNullOrEmpty(value.Currency) && CurencyConvertationDic.ContainsKey(value.Currency))
                dVal *= CurencyConvertationDic[value.Currency];
            return $"{Currency} {dVal.ToString("F",CultureInfo)}";
        }
    }
}