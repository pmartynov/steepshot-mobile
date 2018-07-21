﻿using System;

namespace Steepshot.Core.Services
{
    public interface ILogService
    {
        void Fatal(Exception ex);

        void Error(Exception ex);

        void Warning(Exception ex);

        void Info(Exception ex);
    }
}