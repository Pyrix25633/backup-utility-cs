using System;

public class Program {
    static void Main(string[] args) {
        string version = "1.4.0";
        // Parsing arguments
        Arguments arguments = new Arguments();
        try {
            arguments.Parse(args);
            if(arguments.help) return;
            if(arguments.errors != 0) {
                if(arguments.errors == 255)
                    Logger.Error("Missing arguments!");
                else
                    Logger.Error("Wrong arguments!");
                return;
            }
        }
        catch(Exception e) {
            Logger.Error("Exception parsing arguments: " + e);
            return;
        }
        // Initialize logger
        Logger.InitializeLogging(arguments.log);
        // Logging Info
        Logger.Info("Backup utility " + version);
        // Source
        Logger.Info("Source folder: " + arguments.source);
        if(!Directory.Exists(arguments.source)) {
            Logger.Error("Source folder does not exist!");
            return;
        }
        // Destination
        Logger.Info("Destination folder: " + arguments.destination);
        if(!Directory.Exists(arguments.destination)) {
            Logger.Warning("Destination folder does not exist, attempting creation");
            try {
                Directory.CreateDirectory(arguments.destination);
                Logger.Success("Destination directory creation succeeded");
            }
            catch(Exception e) {
                Logger.Error("Destination directory creation failed, error: " + e);
                return;
            }
        }
        // Removed
        if(arguments.removed != null) {
            Logger.Info("Folder for removed files: " + arguments.removed);
            if(!Directory.Exists(arguments.removed)) {
                Logger.Warning("Folder for removed files does not exist, attempting creation");
                try {
                    Directory.CreateDirectory(arguments.removed);
                    Logger.Success("Directory for removed files creation succeeded");
                }
                catch(Exception e) {
                    Logger.Error("Directory for removed files creation failed, error: " + e);
                    return;
                }
            }
        }
        else Logger.Info("Folder for removed files is not set, they will be permanently removed");
        // Delay time
        if(arguments.repeat) Logger.Info("Delay time: " + arguments.time.ToString() + "s");
        else Logger.Info("Delay time not set, program will exit when backup will is finished");
        // Log
        if(arguments.log) Logger.Info("Logging to file");
        // Extensions
        if(arguments.extensions != null) {
            Logger.Info("File with the list of extensions to check for sha256: " + arguments.extensions);
            if(!File.Exists(arguments.extensions)) {
                Logger.Error("File with the list of extensions to check for sha256 does not exist!");
                return;
            }
        }
        //Close log stream
        Logger.TerminateLogging();
    }
}