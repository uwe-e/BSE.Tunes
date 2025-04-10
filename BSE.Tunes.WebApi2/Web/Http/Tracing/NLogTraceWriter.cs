﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Tracing;

namespace BSE.Tunes.WebApi.Web.Http.Tracing
{
	public class NLogTraceWriter : ITraceWriter
	{
		private static readonly NLog.Logger classLogger = NLog.LogManager.GetCurrentClassLogger();
		private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> loggingMap =
			new Lazy<Dictionary<TraceLevel, Action<string>>>(() => new Dictionary<TraceLevel, Action<string>>
                {
                    {TraceLevel.Info, classLogger.Info},
                    {TraceLevel.Debug, classLogger.Debug},
                    {TraceLevel.Error, classLogger.Error},
                    {TraceLevel.Fatal, classLogger.Fatal},
                    {TraceLevel.Warn, classLogger.Warn}
                });
		private Dictionary<TraceLevel, Action<string>> Logger
		{
			get
			{
				return loggingMap.Value;
			}
		}

		public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
		{
			if (level != TraceLevel.Off)
			{
				var record = new TraceRecord(request, category, level);
				traceAction(record);
				Log(record);
			}
		}
		
		private void Log(TraceRecord record)
		{
			var message = new System.Text.StringBuilder();

			if (record.Request != null)
			{
				if (record.Request.Method != null)
				{
					message.Append(record.Request.Method);
				}

				if (record.Request.RequestUri != null)
				{
					message.Append(" ").Append(record.Request.RequestUri);
				}
			}

			if (!string.IsNullOrWhiteSpace(record.Category))
			{
				message.Append(" ").Append(record.Category);
			}

			if (!string.IsNullOrWhiteSpace(record.Operator))
			{
				message.Append(" ").Append(record.Operator).Append(" ").Append(record.Operation);
			}

			if (!string.IsNullOrWhiteSpace(record.Message))
			{
				message.Append(" ").Append(record.Message);
			}

			if (record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
			{
				message.Append(" ").Append(record.Exception.GetBaseException().Message);
			}

			Logger[record.Level](message.ToString());
		}
	}
}