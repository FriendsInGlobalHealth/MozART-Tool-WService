using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace MozartService
{
    public class MigrationHandler
    {
        private readonly string MySQLPath = ConfigurationManager.AppSettings["MySQLPath"];
        private readonly string MySQLUser = ConfigurationManager.AppSettings["MySQLUser"];
        private readonly string MySQLPass = ConfigurationManager.AppSettings["MySQLPass"]; 
        private readonly string MySQLLogsPath = ConfigurationManager.AppSettings["MySQLLogsPath"];
        private readonly string SSMAPath = ConfigurationManager.AppSettings["SSMAPath"];
        private readonly string DUPMSPath = ConfigurationManager.AppSettings["DUPMSPath"];
        private readonly string WinRarPath = ConfigurationManager.AppSettings["WinRarPath"];
        private readonly string SSMAConversionAndDataMigration = ConfigurationManager.AppSettings["SSMAConversionAndDataMigration"];
        private readonly string SSMAServersConnectionFile = ConfigurationManager.AppSettings["SSMAServersConnectionFile"];
        private readonly string SSMAVariableValueFile = ConfigurationManager.AppSettings["SSMAVariableValueFile"]; 
        private readonly string SSMALogs = ConfigurationManager.AppSettings["SSMALogs"];
        private readonly string BackUpDUMPSPath = ConfigurationManager.AppSettings["BackUpDUMPSPath"]; 
        private readonly string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

        public MigrationHandler() {
        }

        public String ReadFileExtractName(string dumpPath)
        {
            using (StreamReader sr = new StreamReader(dumpPath))
            {
                Regex regex = new Regex("`(.*?)`");
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null )
                {
                    if (line.Contains("USE "))
                    {
                        var matches = regex.Matches(line);
                        if (matches.Count > 0)
                        {
                            return matches[0].Groups[1].ToString();
                        }
                    
                    }
                    else if(line.Contains("CREATE TABLE"))
                    {
                        return null;
                    }

                }
            }
            return null;
        }

        public async Task<int> RestoreMysqlDump(string dumpPath, string dbName)
        {
            Process processInstance = new Process();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "cmd.exe";
            processInfo.RedirectStandardInput = true;
            processInfo.UseShellExecute = false;
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
            processInfo.RedirectStandardError = true;
            processInstance.StartInfo = processInfo;
            processInstance.Start();


            using (StreamWriter streamWriter = processInstance.StandardInput)
            {
                if (streamWriter.BaseStream.CanWrite)
                {

                    File.AppendAllText(MySQLLogsPath, $"\n[INFO] {DateTime.Now} : Creating database {dbName} \n");
                    streamWriter.WriteLine($" \"{MySQLPath}\" --user={MySQLUser} --password={MySQLPass} -e \"CREATE DATABASE {dbName}\" ");
                    File.AppendAllText(MySQLLogsPath, $"[INFO] {DateTime.Now} : Database successfully created! \n");
                    File.AppendAllText(MySQLLogsPath, $"\n[INFO] {DateTime.Now} : Restoring database {dbName} from dump! {dumpPath} \n");
                    
                    //streamWriter.WriteLine($" \"{MySQLPath}\" --default-character-set=utf8 --user={MySQLUser} --password={MySQLPass} {dbName} < \""+ @""+ dumpPath + "\" ");
                    streamWriter.WriteLine($" \"{MySQLPath}\" --default-character-set=utf8 " +
                                                            $" --init-command=\" SET SESSION FOREIGN_KEY_CHECKS=0; SET UNIQUE_CHECKS=0; SET AUTOCOMMIT=0; \" " +
                                                            $"--user={MySQLUser} " +
                                                            $"--password={MySQLPass} " +
                                                            $"{dbName} < \"" + @"" + dumpPath + "\" ");

                    streamWriter.WriteLine($" \"{MySQLPath}\" --default-character-set=utf8 " +
                                                            $"--user={MySQLUser} " +
                                                            $"--password={MySQLPass} " +
                                                             $" -e \"COMMIT;\" ");

                    // fix issues: drop unique key nid in table t_paciente
                    streamWriter.WriteLine($" \"{MySQLPath}\" --user={MySQLUser} --password={MySQLPass} -e \"ALTER TABLE {dbName}.t_paciente DROP INDEX nid;\" ");
                    streamWriter.WriteLine($" \"{MySQLPath}\" --user={MySQLUser} --password={MySQLPass} -e \"ALTER TABLE {dbName}.t_paciente ADD INDEX (nid);\" ");

                    // fix issues: update invalid date from column data in table t_tratamentotb
                    streamWriter.WriteLine($" \"{MySQLPath}\" --user={MySQLUser} --password={MySQLPass} -e \"UPDATE {dbName}.t_tratamentotb SET data = '0001-01-01 00:00:00' WHERE data = '0000-00-00 00:00:00';\" ");
                   
                }
            }

            /*
            mysql - u[UserName] - p[Password][DB_Name] - sNe
            'show tables' | while read table; do mysql - u[UserName] - p
            [PassWord] - sNe "RENAME TABLE [DB_Name].$table TO 
            [New_DB_Name].$table"; done
            */

            processInstance.WaitForExit();

            File.AppendAllText(MySQLLogsPath, processInstance.StandardError.ReadToEnd() + "\n");
            File.AppendAllText(MySQLLogsPath, $"[INFO] {DateTime.Now} : Finished! \n");

            return 1;
        }

        public async Task<int> MigrateData(string sourcedb, string targetdb)
        {
            Process processInstance = new Process();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "cmd.exe";
            processInfo.RedirectStandardInput = true;
            processInfo.UseShellExecute = false;
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
            processInfo.RedirectStandardError = true;
            processInfo.Verb = "runas";
            processInstance.StartInfo = processInfo;
            processInstance.Start();

            using (StreamWriter streamWriter = processInstance.StandardInput)
            {
                if (streamWriter.BaseStream.CanWrite)
                {
                    File.AppendAllText(MySQLLogsPath, $"\n[INFO] {DateTime.Now} : Migrating {sourcedb} from MySql to Sql Server ... \n");
                    streamWriter.WriteLine($" \"{SSMAPath}\" " +
                        $" -s \"{SSMAConversionAndDataMigration}\"" +
                        $" -c \"{SSMAServersConnectionFile}\"" +
                        $" -v \"{SSMAVariableValueFile}\"" +
                        $" -v $SourceDatabase$ {sourcedb}" +
                        $" -v $TargetDB$ {targetdb}" +
                        $" -l \"{SSMALogs}\" ");

                }
            }

            processInstance.WaitForExit();

            File.AppendAllText(MySQLLogsPath, processInstance.StandardError.ReadToEnd() + "\n");
            File.AppendAllText(MySQLLogsPath, $"[INFO] {DateTime.Now} : Process Completed! \n");

            return 1;
        }

        public async Task<string> DecompressFileAsync(string filepath)
        {
            String filePwd = GetFilePassword(Path.GetFileName(filepath));
            if (filePwd == null)
            {
                File.AppendAllText(MySQLLogsPath, $"\n[ERROR] {DateTime.Now} : Password Not Provided for file {Path.GetFileName(filepath)}. \n");
                return null;
            };

            File.AppendAllText(MySQLLogsPath, $"\n[INFO] {DateTime.Now} : Started Decompressing file {Path.GetFileName(filepath)}. \n");


            await Decompress(filepath, filePwd);


            /*
            
            bool checkPassword = ZipFile.CheckZipPassword(filepath, filePwd);
            if (checkPassword)
            {
                    using (ZipFile zip = ZipFile.Read(filepath))
                    {
                        zip.Password = filePwd;
                        zip.FlattenFoldersOnExtract = true;
                        zip.ExtractAll(DUPMSPath, ExtractExistingFileAction.OverwriteSilently);
                    }
            }else {
                    File.AppendAllText(MySQLLogsPath, $"\n[ERROR] {DateTime.Now} : Wrong Password for file {Path.GetFileName(filepath)}. \n");
                    return null;
            }

            */
            File.AppendAllText(MySQLLogsPath, $"\n[INFO] {DateTime.Now} : Finished Decompressing file {Path.GetFileName(filepath)}. \n");
            return "Finished!";
        }

        public async Task<int> CleanResources(string dumpPath, string dbName)
        {
            Process processInstance = new Process();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "cmd.exe";
            processInfo.RedirectStandardInput = true;
            processInfo.UseShellExecute = false;
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
            processInfo.RedirectStandardError = true;
            processInstance.StartInfo = processInfo;
            processInstance.Start();


            using (StreamWriter streamWriter = processInstance.StandardInput)
            {
                if (streamWriter.BaseStream.CanWrite)
                {
                    File.AppendAllText(MySQLLogsPath, $"\n[INFO] {DateTime.Now} : Cleaning Resources... \n");

                    streamWriter.WriteLine($" \"{MySQLPath}\" --user={MySQLUser} --password={MySQLPass} -e \"DROP DATABASE {dbName}\" ");

                }
            }

            processInstance.WaitForExit();

            File.AppendAllText(MySQLLogsPath, processInstance.StandardError.ReadToEnd() + "\n");

            // Move file to disc E:
            File.Move(dumpPath, BackUpDUMPSPath + Path.GetFileName(dumpPath));
            File.AppendAllText(MySQLLogsPath, $"[INFO] {DateTime.Now} : Finished! \n"); 

            return 1;
        }

        private String GetFilePassword(string filename)
        { 
            var statement = $"SELECT password FROM mozart_q_submissions.submission where filename = '{filename}' ";
                
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(statement, connection);

                try
                {
                    var pwd = command.ExecuteScalar();
                    connection.Close();
                    if (pwd != null)
                    {
                        return pwd.ToString();
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                catch (Exception ex)
                {
                    return null;
                }
                
            }
        }

        public bool ValidateFile(string filepath)
        {
            // check file extension
            if (Path.GetExtension(filepath) != ".sql")
            {
                File.AppendAllText(MySQLLogsPath, $"\n[ERROR] {DateTime.Now} : Wrong File Type {Path.GetFileName(filepath)}. \n");
                return false;
            }

            // check if file name contain spaces
            if (Path.GetFileNameWithoutExtension(filepath).Split(' ').Length > 1)
            {
                File.AppendAllText(MySQLLogsPath, $"\n[ERROR] {DateTime.Now} : File name contains spaces {Path.GetFileName(filepath)}. \n");
                return false;
            }

            return true;
        }

        private async Task<int> Decompress(string filepath, String password)
        {
            Process processInstance = new Process();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "cmd.exe";
            processInfo.RedirectStandardInput = true;
            processInfo.UseShellExecute = false;
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
            processInfo.RedirectStandardError = true;
            processInstance.StartInfo = processInfo;
            processInstance.Start();


            using (StreamWriter streamWriter = processInstance.StandardInput)
            {
                if (streamWriter.BaseStream.CanWrite)
                {
                    if (password != null)
                    {
                        streamWriter.WriteLine($" \"{WinRarPath}\" e {filepath} *.* -p{password} {DUPMSPath} ");
                    }
                    else
                    {
                        // if not password protected
                        streamWriter.WriteLine($" \"{WinRarPath}\" e {filepath} *.* {DUPMSPath} ");
                    }
                    

                }
            }

            processInstance.WaitForExit();

            File.AppendAllText(MySQLLogsPath, processInstance.StandardError.ReadToEnd() + "\n");
            return 1;
        }

    }
}
