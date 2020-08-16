// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.XEvent.XELite;
using TSqlCoverage.XEvents;

namespace TSqlCoverage
{
    internal class XEventsController
    {
        private readonly SqlConnection connection;
        private readonly string sessionName;
    
        private readonly ManualResetEventSlim bufferDetectedSync = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim bufferReady = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim stopReady = new ManualResetEventSlim(false);
        private string startChallenge;
        private string stopChallenge;
        private bool bufferDetected;

        private readonly HandleXEvent applicationHandler;
        private HandleXEvent currentHandler;

        public XEventsController(SqlConnection connection, string sessionName, HandleXEvent handler)
        {
            this.connection = connection;
            this.sessionName = sessionName;
            this.applicationHandler = handler;

            this.currentHandler = IgnoreHandler;
        }

        public Task EventHandler(IXEvent e)
            => currentHandler(e);

        public async Task CreateEventSession(string sessionName, XEventPredicate predicate)
        {
            var whereClause = predicate is null
                ? null
                : $@"WHERE ({ predicate })"
                ;

            var createEventCommand = $@"
                CREATE EVENT SESSION [{ sessionName }] ON SERVER
                    ADD EVENT sqlserver.sql_batch_starting,
                    ADD EVENT sqlserver.sp_statement_starting (
                        SET collect_object_name = 0, collect_statement = 0
                        { whereClause }
                    )
                    WITH (
                        MAX_MEMORY = 30 MB,
                        EVENT_RETENTION_MODE = NO_EVENT_LOSS,
                        MAX_DISPATCH_LATENCY = 1 SECONDS,
                        MAX_EVENT_SIZE = 0 KB,
                        MEMORY_PARTITION_MODE = NONE,
                        TRACK_CAUSALITY = OFF,
                        STARTUP_STATE = OFF
                    ) 

                ALTER EVENT SESSION [{ sessionName }] ON SERVER STATE = START
            ";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = createEventCommand;
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DropEventSession(string sessionName)
        {
            using(var command = connection.CreateCommand())
            {
                command.CommandText = $@"DROP EVENT SESSION [{ sessionName }] ON SERVER";
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Start()
        {
            currentHandler = StartBufferChallengeHandler;

            uint i = 0;
            while (!bufferDetected)
            {
                startChallenge = $"--! start challenge: { sessionName } ({ i++ }) !--";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = startChallenge;
                    await command.ExecuteNonQueryAsync();
                }

                await Task.Delay(1);
            }

            bufferDetectedSync.Set();
            bufferReady.Wait();
        }

        public async Task Stop()
        {
            this.stopChallenge = $"--! stop challenge: {sessionName} !--";

            currentHandler = StopBufferChallengeHandler;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = stopChallenge;
                await command.ExecuteNonQueryAsync();
            }

            this.stopReady.Wait();
        }

        private Task StartBufferChallengeHandler(IXEvent e) {
            bufferDetected = true;
            bufferDetectedSync.Wait();

            if (e.Name == "sql_batch_starting" && startChallenge.Equals(e.Fields["batch_text"]))
            {
                currentHandler = applicationHandler;
                bufferReady.Set();
            }

            return Task.CompletedTask;
        }

        private Task StopBufferChallengeHandler(IXEvent e) {
            if (e.Name == "sql_batch_starting" && stopChallenge.Equals(e.Fields["batch_text"]))
            {
                stopReady.Set();
                currentHandler = IgnoreHandler; 
            }
            else
            {
                applicationHandler(e);
            }

            return Task.CompletedTask;
        }

        private Task IgnoreHandler(IXEvent e)
            => Task.CompletedTask;
    }
}
