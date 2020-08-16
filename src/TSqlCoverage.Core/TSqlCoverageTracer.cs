// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.XEvent.XELite;
using TSqlCoverage.XEvents;

namespace TSqlCoverage
{
    public sealed class TSqlCoverageTracer : IDisposable
    {
        private SqlConnection connection;
        private readonly Action<XEventStatement> handler;
        private readonly XEventsController eventsController;
        private SemaphoreSlim stopSemaphore = new SemaphoreSlim(1, 1);
        private bool disposed = false;
        private bool stopped = false;

        private TSqlCoverageTracer(SqlConnection connection, string sessionName, Action<XEventStatement> handler)
        {
            this.connection = connection;
            this.SessionName = sessionName;
            this.handler = handler;
            this.eventsController = new XEventsController(connection, sessionName, this.XEventHandler);
        }

        public string SessionName { get; }

        public delegate void TracerExceptionHandler(TSqlCoverageTracer tracer, Exception ex);
        public TracerExceptionHandler OnTracerException;

        public SqlCommand CreateCommand()
        {
            return connection.CreateCommand();
        }

        private static string NewSessionName()
            => $@"TSqlCoverage-{Guid.NewGuid().ToString("n")}";

        public static async Task<TSqlCoverageTracer> Start(
            string connectionString,
            Action<XEventStatement> handler,
            XEventPredicate predicate)
        {
            var tracerConnectionString = new SqlConnectionStringBuilder(connectionString) {
                InitialCatalog = "master"
            }
                .ToString();

            var sessionName = NewSessionName();
            var connection = new SqlConnection(connectionString);
            var tracer = new TSqlCoverageTracer(connection, sessionName, handler);
            var eventsController = tracer.eventsController;

            connection.Open();

            int spid;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT @@SPID";
                spid = (short)await command.ExecuteScalarAsync();
            }

            var spidPredicate = XEventField.SqlServer.SessionId.Equal((ulong)spid);
            predicate = predicate is null
                ? spidPredicate
                : predicate & spidPredicate;

            try
            {
                await eventsController.CreateEventSession(sessionName, predicate);
            }
            catch
            {
                connection.Dispose();
                throw;
            }

            // The xevent session is ready to act.

            var tracerThread = new Thread(async () =>
            {
                try
                {
                    var streamer = new XELiveEventStreamer(tracerConnectionString, sessionName);
                    await streamer.ReadEventStream(tracer.eventsController.EventHandler, CancellationToken.None);
                }
                catch (SqlException ex)
                    when (ex.Number == SqlServerErrorNumbers.XEventsSessionStoppedOrDropped && tracer.stopped)
                {
                    /* tracer stop */
                }
                catch (Exception ex)
                {
                    await tracer.Stop();

                    if (!tracer.HandleTracerException(ex))
                        throw;
                }
            });

            tracerThread.Name = sessionName;
            tracerThread.Priority = ThreadPriority.AboveNormal;

            try
            {
                tracerThread.Start();

                await tracer.eventsController.Start();
            }
            catch
            {
                tracer.Dispose();
                throw;
            }

            return tracer;
        }

        private bool HandleTracerException(Exception ex)
        {
            var handler = OnTracerException;

            if (handler == null)
                return false;

            handler(this, ex);
            return true;
        }

        public async Task Stop()
        {
            var semaphore = stopSemaphore;
            semaphore.Wait();

            if (!this.stopped)
            {
                this.stopped = true;
                
                var connection = this.connection;
                var eventsController = this.eventsController;
                this.connection = null;

                try
                {
                    await eventsController.Stop();
                    await eventsController.DropEventSession(this.SessionName);
                }
                finally
                {
                    connection.Dispose();
                }
            }

            semaphore.Release();
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.Stop()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                this.disposed = true;
            
                GC.SuppressFinalize(this);
            }
        }

        private Task XEventHandler(IXEvent xEvent)
        {
            if (xEvent.Name != "sp_statement_starting")
                return Task.CompletedTask;


            var statement = new XEventStatement()
            {
                DatabaseId = (int)(uint)xEvent.Fields["source_database_id"],
                ObjectId = (int)xEvent.Fields["object_id"],
                ObjectType = (string)xEvent.Fields["object_type"],
                Offset = ((int)xEvent.Fields["offset"]) / 2,
                OffsetEnd = ((int)xEvent.Fields["offset_end"]) / 2,
                Statement = (string)xEvent.Fields["statement"]
            };

            this.handler(statement);

            return Task.CompletedTask;
        }
    }
}
