// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Search.ashx.cs" company="Sitecore">
//   Copyright (c) Sitecore. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Sitecore.LogWatcher.Services
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.SessionState;
    using Newtonsoft.Json;

    using Sitecore.Diagnostics;

    /// <summary>
    /// Search End Point
    /// </summary>
    public class LogWatcher : IHttpHandler
    {
        public class MessageDetails
        {
            public LogNotificationLevel Level { get; set; }

            public string Message { get; set; }

            public Exception Exception { get; set; }

            public DateTime Time { get; set; }
        }

        private static List<MessageDetails> messages = new List<MessageDetails>();

        private static object syncObject = new object();

        static LogWatcher()
        {
            LogNotification.Notification += (level, message, exception) =>
                {
                    string msg = message;
                    if ((string.IsNullOrEmpty(msg) || msg.ToLowerInvariant() == "Application error.".ToLowerInvariant()) && exception != null)
                    {
                        msg = exception.Message;
                    }

                    if (level == LogNotificationLevel.Error || level == LogNotificationLevel.Fatal)
                    {
                        lock (syncObject)
                        {
                            messages.Add(new MessageDetails{Level = level,
                            Message = msg,
                            Exception = exception,
                            Time = DateTime.UtcNow
                            });
                        }
                    }
                };
        }

        public static void Clear()
        {
            lock (syncObject)
            {
                messages.Clear();
            }
        }

        public string GetErrorCount()
        {
            return messages.Count.ToString();
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["clear"] == "1")
            {
                lock (syncObject)
                {
                    messages.Clear();
                }
            }

            context.Response.Write(this.GetErrorCount());
        }

        public bool IsReusable { get; private set; }

        public static List<MessageDetails> GetMessages()
        {
            lock (syncObject)
            {
                return new List<MessageDetails>(messages);
            }
        }
    }
}