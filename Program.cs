using System;

public class Program {
    static void Main(string[] args) {
        // Version
        string version = "1.4.0";
        // Lists
        string[] sourceList = new string[0], destinationList = new string[0];
        FileInfo[] sourceInfoList = new FileInfo[0], destinationInfoList = new FileInfo[0];
        EnumerationOptions enumOptions = new EnumerationOptions();
        enumOptions.RecurseSubdirectories = true;
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
        else {
            Logger.Info("Extension list not set, only file size will be used to compare files");
        }
        while(true) {
            // Scan folders
            Logger.Info("Starting source folder scan...");
            try {
                sourceList = Directory.EnumerateFileSystemEntries(arguments.source, "*", enumOptions).ToArray();
                Logger.Success("Source folder scanned: " + sourceList.Length + " items found");
            }
            catch(Exception e) {
                Logger.Error("Error while scanning source folder: " + e);
                continue;
            }
            Logger.Info("Starting destination folder scan...");
            try {
                destinationList = Directory.EnumerateFileSystemEntries(arguments.destination, "*", enumOptions).ToArray();
                Logger.Success("Destination folder scanned: " + destinationList.Length + " items found");
            }
            catch(Exception e) {
                Logger.Error("Error while scanning destination folder: " + e);
                continue;
            }
            // Build file info
            Logger.Info("Building source file info list...");
            try {
                foreach(string item in sourceList) {
                    sourceInfoList.Append(new FileInfo(item));
                }
                Logger.Success("Source file info list built");
            }
            catch(Exception e) {
                Logger.Error("Error while building source file info list: " + e);
                continue;
            }
            sourceList = new string[0];
            Logger.Info("Building source file info list...");
            try {
                foreach(string item in sourceList) {
                    sourceInfoList.Append(new FileInfo(item));
                }
                Logger.Success("Source file info list built");
            }
            catch(Exception e) {
                Logger.Error("Error while building source file info list: " + e);
                continue;
            }
            destinationList = new string[0];
            // Close log stream
            Logger.TerminateLogging();
            if(!arguments.repeat) break;
            Thread.Sleep(arguments.time * 1000);
            // Reopen log stream
            Logger.ReinitializeLogging();
        }
    }
}