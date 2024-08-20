using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disploy_ALL_DataBase_Backup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteErrorLogs("EXE Started.........");
            string serverName = ConfigurationManager.AppSettings["ServerName"];
            string DB = ConfigurationManager.AppSettings["DatabaseName"];
            WriteErrorLogs("serverName......... : " + serverName);
            WriteErrorLogs("DB......... : " + DB);
            if (!string.IsNullOrEmpty(DB))
            {
                foreach (string databaseName in DB.Split('|'))
                {
                    WriteErrorLogs("databaseName......... : " + databaseName);
                    string DT = DateTime.Now.ToString().Replace(" ", "").Replace("-", "_").Replace(":", "_");
                    // Configuration
                    string backupFilePath = ConfigurationManager.AppSettings["BackupFilePath"] + databaseName + "_" + DT + ".bak";
                    WriteErrorLogs("backupFilePath......... : " + backupFilePath);
                    try
                    {
                        WriteErrorLogs("try......... : ");
                        // Create a ServerConnection object
                        ServerConnection connection = new ServerConnection(serverName);
                        Server sqlServer = new Server(connection);

                        // Specify the database
                        Database db = sqlServer.Databases[databaseName];

                        // Create a Backup object
                        Backup backup = new Backup
                        {
                            Action = BackupActionType.Database,
                            Database = db.Name,
                            BackupSetDescription = $"Full backup of {db.Name} on {DateTime.Now}",
                            BackupSetName = $"{db.Name} Backup"
                        };

                        // Add a BackupDeviceItem to the Backup object
                        backup.Devices.Add(new BackupDeviceItem(backupFilePath, DeviceType.File));

                        // Execute the backup
                        backup.SqlBackup(sqlServer);

                        Console.WriteLine($"Backup of database '{databaseName}' completed successfully and saved to '{backupFilePath}'.");
                    }
                    catch (Exception ex)
                    {
                        WriteErrorLogs("catch Error......... : " + ex.Message.ToString());
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
        public static void WriteErrorLogs(string Message)
        {
            try
            {
                string Date = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
                StreamWriter sw = File.AppendText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Logs\" + Date + "-Log.txt");
                sw.WriteLine(DateTime.Now + "  " + Message);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
