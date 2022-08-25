using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using MySql.Data.MySqlClient;

namespace MozartService
{
    public partial class MozartService : ServiceBase
    {
        private int eventId = 1;
        //private Boolean migratingDump;
        private MigrationHandler migrationHandler;
        private Queue<String> compressedFilesQueue;
        private Boolean iscompressionQueue;
        private Queue<String> decompressedFilesQueue;
        private Boolean ismigrationQueue;



        public MozartService()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
           // dumpsQueue = new Queue<string>();
            decompressedFilesQueue = new Queue<string>();
            compressedFilesQueue = new Queue<string>();

            migrationHandler = new MigrationHandler();
            //migratingDump = false;
            ismigrationQueue = false;
            iscompressionQueue = false;

            if (!System.Diagnostics.EventLog.SourceExists("MozartSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MozartSource", "MozartLog");
            }
            eventLog1.Source = "MozartSource";
            eventLog1.Log = "MozartLog";

        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart.");
            Timer compressionTimer = new Timer();
            Timer migrationTimer = new Timer();

            compressionTimer.Interval = 120000; // 120 seconds
            migrationTimer.Interval = 120000; // 120 seconds
            //timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            compressionTimer.Elapsed += new ElapsedEventHandler(this.OnCompressionTimer);
            migrationTimer.Elapsed += new ElapsedEventHandler(this.OnMigrationTimer);
            compressionTimer.Start();
            migrationTimer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop.");
        }
        public async void OnCompressionTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            
            if (compressedFilesQueue.Count > 0 && !iscompressionQueue)
            {
                iscompressionQueue = true;
                
                do
                {
                    eventLog1.WriteEntry("Started Decompression: ", EventLogEntryType.Information, eventId++);
                    string path = compressedFilesQueue.Dequeue();
                    
                    await migrationHandler.DecompressFileAsync(path); 
                    eventLog1.WriteEntry("Finished Decompression: " + path, EventLogEntryType.Information, eventId++);

                } while (compressedFilesQueue.Count > 0);
                
                iscompressionQueue = false;
            }


        }

        public async void OnMigrationTimer(object sender, ElapsedEventArgs args)
        {
            if (decompressedFilesQueue.Count > 0 && !ismigrationQueue)
            {
                ismigrationQueue = true;
                
                do
                {
                    string path = decompressedFilesQueue.Dequeue();
                    string dbname;
                    eventLog1.WriteEntry("Started Migration: " + path, EventLogEntryType.Information, eventId++);
                    //validations
                    if (migrationHandler.ValidateFile(path))
                    {
                        string nameInFile = migrationHandler.ReadFileExtractName(path);
                        if (nameInFile != null)
                        {
                            dbname = nameInFile;
                        }
                        else
                        {
                            dbname = Path.GetFileNameWithoutExtension(path);
                        }
                            
                        await migrationHandler.RestoreMysqlDump(path, dbname);
                        eventLog1.WriteEntry("Finished to restore DB: " + path, EventLogEntryType.Information, eventId++);
                        
                        eventLog1.WriteEntry("Starting DB migration to SQL Server! ", EventLogEntryType.Information, eventId++);
                        await migrationHandler.MigrateData(dbname, Path.GetFileNameWithoutExtension(path));
                        eventLog1.WriteEntry("Finished DB migration! ", EventLogEntryType.Information, eventId++);
                        
                        await migrationHandler.CleanResources(path, dbname);
                    }
                    
                } while (decompressedFilesQueue.Count > 0);

                ismigrationQueue = false;
            }
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

        private async void fileSystemWatcher1_Created(object sender, System.IO.FileSystemEventArgs e) 
        {
        
            eventLog1.WriteEntry(" Fw1 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
            compressedFilesQueue.Enqueue(e.FullPath);
            
        }

        private void fileSystemWatcher2_Created(object sender, FileSystemEventArgs e)
        {
            eventLog1.WriteEntry(" Fw2 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
            compressedFilesQueue.Enqueue(e.FullPath);
        }

        private void fileSystemWatcher3_Created(object sender, FileSystemEventArgs e)
        {
            eventLog1.WriteEntry(" Fw3 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
            compressedFilesQueue.Enqueue(e.FullPath);
        }

        private void fileSystemWatcher4_Created(object sender, FileSystemEventArgs e)
        {
            eventLog1.WriteEntry(" Fw4 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
            compressedFilesQueue.Enqueue(e.FullPath);
        }

        private void fileSystemWatcher5_Created(object sender, FileSystemEventArgs e)
        {
            eventLog1.WriteEntry(" Fw5 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
            compressedFilesQueue.Enqueue(e.FullPath);
        }

        private void fileSystemWatcher6_Created(object sender, FileSystemEventArgs e)
        {
            eventLog1.WriteEntry(" Fw6 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
            compressedFilesQueue.Enqueue(e.FullPath);
        }

        private void fileSystemWatcher7_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".sql")
            {
                eventLog1.WriteEntry(" Fw7 File Created: " + e.FullPath, EventLogEntryType.Information, eventId++);
                decompressedFilesQueue.Enqueue(e.FullPath);
            }
        }

        private void fileSystemWatcher7_Changed(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".sql" && !decompressedFilesQueue.Contains(e.FullPath))
            {
                eventLog1.WriteEntry(" Fw7 File Changed: " + e.FullPath, EventLogEntryType.Information, eventId++);
                decompressedFilesQueue.Enqueue(e.FullPath);
            }
            
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void fileSystemWatcher3_Changed(object sender, FileSystemEventArgs e)
        {

        }
    }
}
