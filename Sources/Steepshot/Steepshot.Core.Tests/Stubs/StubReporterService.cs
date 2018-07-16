﻿using Newtonsoft.Json;
using Steepshot.Core.Services;
using System;


namespace Steepshot.Core.Tests.Stubs
{
    public class StubReporterService : IReporterService
    {
        public string SendMessage(string message)
        {
            Console.WriteLine(message);
            return string.Empty;
        }

        public string SendCrash(Exception ex)
        {
            Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return String.Empty;
        }

        public void SendCrash(Exception ex, object param1)
        {
            Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}{JsonConvert.SerializeObject(param1)}");
        }

        public void SendCrash(Exception ex, object param1, object param2)
        {
            Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}{JsonConvert.SerializeObject(param1)}{Environment.NewLine}{JsonConvert.SerializeObject(param2)}");
        }
    }
}
