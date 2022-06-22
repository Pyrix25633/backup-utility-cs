using System;

public class Program {
    static void Main(string[] args) {
        // Version
        string version = "1.4.0";
        // Lists
        string[] sourceList = new string[0], destinationList = new string[0], extensionList = new string[0];
        DirectoryEntry[] sourceInfoList = new DirectoryEntry[0], destinationInfoList = new DirectoryEntry[0],
            toCopyList = new DirectoryEntry[0], toRemoveFileList = new DirectoryEntry[0], toRemoveFolderList = new DirectoryEntry[0];
        EnumerationOptions enumOptions = new EnumerationOptions();
        enumOptions.RecurseSubdirectories = true;
        // Other variables
        Int64 length, filesToCopy, foldersToCopy, sizeToCopy, filesToRemove, foldersToRemove, sizeToRemove;
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
        // Getting full path
        arguments.source = new FileInfo(arguments.source).FullName;
        arguments.destination = new FileInfo(arguments.destination).FullName;
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
            if(arguments.allExtensions) {
                Logger.Info("All extensions will be checked for content changes");
            }
            else {
                Logger.Info("File with the list of extensions to check for content changes: " + arguments.extensions);
                if(!File.Exists(arguments.extensions)) {
                    Logger.Error("File with the list of extensions to check for content changes does not exist!");
                    return;
                }
                try {
                    extensionList = File.ReadAllLines(arguments.extensions);
                    Logger.Success("Extension list succesfully retrieved from file");
                }
                catch(Exception e) {
                    Logger.Error("Could not retrieve extension list from file, error: " + e);
                    return;
                }
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
                    sourceInfoList = sourceInfoList.Append(new DirectoryEntry(item, arguments.source)).ToArray();
                }
                Logger.Success("Source file info list built");
            }
            catch(Exception e) {
                Logger.Error("Error while building source file info list: " + e);
                continue;
            }
            sourceList = new string[0];
            Logger.Info("Building destination file info list...");
            try {
                foreach(string item in destinationList) {
                    destinationInfoList = destinationInfoList.Append(new DirectoryEntry(item, arguments.destination)).ToArray();
                }
                Logger.Success("Destination file info list built");
            }
            catch(Exception e) {
                Logger.Error("Error while building destination file info list: " + e);
                continue;
            }
            destinationList = new string[0];
            // Items to copy
            Logger.Info("Determining items to copy...");
            length = sourceInfoList.Length;
            filesToCopy = 0; foldersToCopy = 0; sizeToCopy = 0;
            for(Int64 i = 0; i < length; i++) {
                if(sourceInfoList[i].ToCopy(destinationInfoList, arguments.allExtensions, extensionList)) {
                    toCopyList = toCopyList.Append(sourceInfoList[i]).ToArray();
                    if((sourceInfoList[i].fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
                        foldersToCopy++;
                    }
                    else {
                        filesToCopy++;
                        sizeToCopy += sourceInfoList[i].fileInfo.Length;
                    }
                }
            }
            Logger.Success(foldersToCopy.ToString() + " folders and " + filesToCopy.ToString() + " files to copy (" +
                Logger.HumanReadableSize(sizeToCopy) + ")");
            // Items to remove
            Logger.Info("Determining items to remove...");
            length = destinationInfoList.Length;
            filesToRemove = 0; foldersToRemove = 0; sizeToRemove = 0;
            for(Int64 i = 0; i < length; i++) {
                if(destinationInfoList[i].ToRemove(sourceInfoList)) {
                    if((destinationInfoList[i].fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
                        toRemoveFolderList = toRemoveFolderList.Append(destinationInfoList[i]).ToArray();
                        foldersToRemove++;
                    }
                    else {
                        toRemoveFileList = toRemoveFileList.Append(destinationInfoList[i]).ToArray();
                        filesToRemove++;
                        sizeToRemove += destinationInfoList[i].fileInfo.Length;
                    }
                }
            }
            Logger.Success(foldersToRemove.ToString() + " folders and " + filesToRemove.ToString() + " files to remove (" +
                Logger.HumanReadableSize(sizeToRemove) + ")");
            // Close log stream
            Logger.TerminateLogging();
            if(!arguments.repeat) break;
            Thread.Sleep(arguments.time * 1000);
            // Reopen log stream
            Logger.ReinitializeLogging();
        }
    }
}