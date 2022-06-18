using System;

public class Program {
    static void Main(string[] args) {
        string version = "1.4.0";
        // Parsing arguments
        Arguments arguments = new Arguments();
        try {
            arguments.Parse(args);
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
        //Logging Info
        Logger.Info("Backup utility " + version);
        Logger.Info("Source file: " + arguments.source);
        Logger.Info("Destination folder: " + arguments.destination);
        if(arguments.removed != null)
            Logger.Info("Folder for removed files: " + arguments.removed);
        else
            Logger.Info("Folder for removed files is not set, they will be permanently removed");
        if(arguments.repeat)
            Logger.Info("Delay time: " + arguments.time.ToString() + "s");
        else
            Logger.Info("Delay time not set, program will exit when backup will is finished");
        //Checking for source, destination and removed existance
        if(!Directory.Exists(arguments.source)) {
            Logger.Error("Source folder does not exist!");
            return;
        }
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
        if(arguments.removed != null) {
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
    }
}