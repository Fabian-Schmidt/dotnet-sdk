﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// ------------------------------------------------------------

namespace Dapr.Actors
{
    using System;
    using System.Globalization;
    using Microsoft.Extensions.Logging;


    internal sealed class ActorTrace
    {
        internal static readonly ActorTrace Instance = new ActorTrace();

        private readonly ILogger logger;
        private readonly ITraceWriter traceWriter;

        /// <summary>
        /// Prevents a default instance of the <see cref="ActorTrace" /> class from being created.
        /// </summary>
        internal ActorTrace(ILogger logger = null)
        {
            // TODO: Replace with actual TraceWriter (or integrate with distributed tracing).
            // Use ConsoleTraceWriter during development & test.
            this.traceWriter = new ConsoleTraceWriter();
            var loggerFactory = new LoggerFactory();
            this.logger = logger ?? loggerFactory.CreateLogger("ActorTrace");
            
        }

        /// <summary>
        /// Interface for traces.
        /// </summary>
        private interface ITraceWriter
        {
            /// <summary>
            /// Writes info trace.
            /// </summary>
            /// <param name="infoText">Text to trace.</param>
            void WriteInfo(string infoText);

            /// <summary>
            /// Writes warning trace.
            /// </summary>
            /// <param name="warningText">Text to trace.</param>
            void WriteWarning(string warningText);

            /// <summary>
            /// Writes Error trace.
            /// </summary>
            /// <param name="errorText">Text to trace.</param>
            void WriteError(string errorText);
        }

        internal void WriteInfo(string type, string format, params object[] args)
        {
            this.WriteInfoWithId(type, string.Empty, format, args);
        }

        internal void WriteInfoWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                this.logger.LogInformation($"{type}: {id} {format}");
                this.traceWriter.WriteInfo($"{type}: {id} {format}");
            }
            else
            {
                this.logger.LogInformation($"{type}: {id} {string.Format(CultureInfo.InvariantCulture, format, args)}");
                this.traceWriter.WriteInfo($"{type}: {id} {string.Format(CultureInfo.InvariantCulture, format, args)}");
            }
        }

        internal void WriteWarning(string type, string format, params object[] args)
        {
            this.logger.LogWarning(type, string.Empty, format, args);
        }

        internal void WriteWarningWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                this.logger.LogWarning($"{type}: {id} {format}");
            }
            else
            {
                this.logger.LogWarning($"{type}: {id} {string.Format(CultureInfo.InvariantCulture, format, args)}");
            }
        }

        internal void WriteError(string type, string format, params object[] args)
        {
            this.WriteErrorWithId(type, string.Empty, format, args);
        }

        internal void WriteErrorWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                this.logger.LogError($"{type}: {id} {format}");
                this.traceWriter.WriteInfo($"{type}: {id} {format}");
            }
            else
            {
                this.logger.LogError($"{type}: {id} {string.Format(CultureInfo.InvariantCulture, format, args)}");
                this.traceWriter.WriteInfo($"{type}: {id} {format}");
            }
        }

        private class ConsoleTraceWriter : ITraceWriter
        {
            public void WriteError(string errorText)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {errorText}");
                Console.ResetColor();
            }

            public void WriteInfo(string infoText)
            {
                Console.WriteLine(infoText);
            }

            public void WriteWarning(string warningText)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: {warningText}");
                Console.ResetColor();
            }
        }
    }
}
