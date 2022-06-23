using System;

public class Program {
    static void Main(string[] args) {
        // Version
        string version = "1.4.1";
        // Lists
        string[] sourceList = new string[0], destinationList = new string[0], extensionList = new string[0];
        DirectoryEntry[] sourceInfoList = new DirectoryEntry[0], destinationInfoList = new DirectoryEntry[0],
            toCopyList = new DirectoryEntry[0], toRemoveFileList = new DirectoryEntry[0], toRemoveFolderList = new DirectoryEntry[0];
        EnumerationOptions enumOptions = new EnumerationOptions();
        enumOptions.RecurseSubdirectories = true;
        // Other variables
        Int32 length, filesToCopy, filesCopied, foldersToCopy, foldersCopied,
            filesToRemove, filesRemoved, foldersToRemove, foldersRemoved, sleepTime;
        UInt64 sizeToCopy, sizeCopied, sizeToRemove, sizeRemoved;
        Int64 timestamp;
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
            // Getting full path
            if(Directory.Exists(arguments.removed)) arguments.removed = new FileInfo(arguments.removed).FullName;
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
            // Timestamp
            timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + arguments.time;
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
            for(Int32 i = 0; i < length; i++) {
                if(sourceInfoList[i].ToCopy(destinationInfoList, arguments.allExtensions, extensionList)) {
                    toCopyList = toCopyList.Append(sourceInfoList[i]).ToArray();
                    if((sourceInfoList[i].fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
                        foldersToCopy++;
                    }
                    else {
                        filesToCopy++;
                        sizeToCopy += (UInt64)sourceInfoList[i].fileInfo.Length;
                    }
                }
            }
            Logger.Success(foldersToCopy.ToString() + " folder" + (foldersToCopy == 1 ? "" : "s") + " and " +
                filesToCopy.ToString() + " file" + (filesToCopy == 1 ? "" : "s") + " to copy (" +
                Logger.HumanReadableSize(sizeToCopy) + ")");
            // Items to remove
            Logger.Info("Determining items to remove...");
            length = destinationInfoList.Length;
            filesToRemove = 0; foldersToRemove = 0; sizeToRemove = 0;
            for(Int32 i = 0; i < length; i++) {
                if(destinationInfoList[i].ToRemove(sourceInfoList)) {
                    if((destinationInfoList[i].fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
                        toRemoveFolderList = toRemoveFolderList.Append(destinationInfoList[i]).ToArray();
                        foldersToRemove++;
                    }
                    else {
                        toRemoveFileList = toRemoveFileList.Append(destinationInfoList[i]).ToArray();
                        filesToRemove++;
                        sizeToRemove += (UInt64)destinationInfoList[i].fileInfo.Length;
                    }
                }
            }
            Logger.Success(foldersToRemove.ToString() + " folder" + (foldersToRemove == 1 ? "" : "s") + " and " +
                filesToRemove.ToString() + " file" + (filesToRemove == 1 ? "" : "s") + " to remove (" +
                Logger.HumanReadableSize(sizeToRemove) + ")");
            // Clear info lists
            sourceInfoList = new DirectoryEntry[0]; destinationInfoList = new DirectoryEntry[0];
            // Copy files
            length = toCopyList.Length;
            filesCopied = 0; foldersCopied = 0; sizeCopied = 0;
            for(Int32 i = 0; i < length; i++) {
                DirectoryEntry e = toCopyList[i];
                Logger.InfoReason(e.reason, e.relativePath);
                Logger.ProgressBar(sizeCopied, sizeToCopy);
                string destinationPath = arguments.destination + Path.DirectorySeparatorChar + e.relativePath;
                if((e.fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) { // Copy folder
                    try {
                        Directory.CreateDirectory(destinationPath);
                        foldersCopied++;
                        Logger.RemoveLine(); Logger.RemoveLine();
                        Logger.SuccessReason(e.reason, e.relativePath);
                    }
                    catch(Exception exc) {
                        Logger.RemoveLine(); Logger.RemoveLine();
                        Logger.Error("Could not copy: " + e.relativePath + ", error: " + exc);
                    }
                }
                else { // Copy file
                    try {
                        File.Copy(e.fileInfo.FullName, destinationPath, true);
                        filesCopied++;
                        sizeCopied += (UInt64)e.fileInfo.Length;
                        Logger.RemoveLine(); Logger.RemoveLine();
                        Logger.SuccessReason(e.reason, e.relativePath);
                    }
                    catch(Exception exc) {
                        Logger.RemoveLine(); Logger.RemoveLine();
                        Logger.Error("Could not copy: " + e.relativePath + ", error: " + exc);
                    }
                }
            }
            toCopyList = new DirectoryEntry[0];
            // Remove files
            length = toRemoveFileList.Length;
            filesRemoved = 0; sizeRemoved = 0;
            for(Int32 i = 0; i < length; i++) {
                DirectoryEntry e = toRemoveFileList[i];
                UInt64 fileSize = (UInt64)e.fileInfo.Length;
                bool err;
                Logger.InfoReason(e.reason, e.relativePath);
                Logger.ProgressBar(sizeRemoved, sizeToRemove);
                if(arguments.removed != null) { // Move
                    string newPath = arguments.removed + Path.DirectorySeparatorChar + e.relativePath;
                    try {
                        File.Move(e.fileInfo.FullName, newPath, true);
                        err = false;
                    }
                    catch(Exception exc1) {
                        try {
                            Directory.CreateDirectory(newPath.Substring(0, newPath.Length - e.fileInfo.Name.Length));
                            File.Move(e.fileInfo.FullName, newPath);
                            err = false;
                        }
                        catch(Exception exc2) {
                            Logger.RemoveLine(); Logger.RemoveLine();
                            Logger.Error("Could not remove: " + e.relativePath + ", error 1: " + exc1 + ", error 2: " + exc2);
                            err = true;
                        }
                    }
                }
                else { // Completely remove
                    try {
                        File.Delete(e.fileInfo.FullName);
                        err = false;
                    }
                    catch(Exception exc) {
                        Logger.RemoveLine(); Logger.RemoveLine();
                        Logger.Error("Could not remove: " + e.relativePath + ", error: " + exc);
                        err = true;
                    }
                }
                if(!err) {
                    sizeRemoved += fileSize;
                    filesRemoved++;
                    Logger.RemoveLine(); Logger.RemoveLine();
                    Logger.SuccessReason(e.reason, e.relativePath);
                }
            }
            toRemoveFileList = new DirectoryEntry[0];
            // Remove folders
            length = toRemoveFolderList.Length;
            foldersRemoved = 0;
            for(Int32 i = 0; i < length; i++) {
                DirectoryEntry e = toRemoveFolderList[i];
                bool err;
                Logger.InfoReason(e.reason, e.relativePath);
                if(arguments.removed != null) { // Move
                    string newPath = arguments.removed + Path.DirectorySeparatorChar + e.relativePath;
                    try {
                        Directory.CreateDirectory(newPath);
                        Directory.Delete(e.fileInfo.FullName);
                        err = false;
                    }
                    catch(Exception exc) {
                        Logger.RemoveLine();
                        Logger.Error("Could not remove: " + e.relativePath + ", error: " + exc);
                        err = true;
                    }
                }
                else { // Completely remove
                    try {
                        Directory.Delete(e.fileInfo.FullName);
                        err = false;
                    }
                    catch(Exception exc) {
                        Logger.RemoveLine();
                        Logger.Error("Could not remove: " + e.relativePath + ", error: " + exc);
                        err = true;
                    }
                }
                if(!err) {
                    foldersRemoved++;
                    Logger.RemoveLine();
                    Logger.SuccessReason(e.reason, e.relativePath);
                }
            }
            toRemoveFolderList = new DirectoryEntry[0];
            // Log copied and removed items
            Logger.Success(foldersCopied.ToString() + " folder" + (foldersRemoved == 1 ? "" : "s") + " and " +
                filesCopied.ToString() + " file" + (filesCopied == 1 ? "" : "s") + " copied (" + Logger.HumanReadableSize(sizeCopied) +
                "), " + foldersRemoved.ToString() + " folder" + (foldersRemoved == 1 ? "" : "s") + " and " +
                filesRemoved.ToString() + " file" + (filesRemoved == 1 ? "" : "s") + " removed (" + Logger.HumanReadableSize(sizeRemoved) +
                "), delta: " + (sizeCopied >= sizeRemoved ? "+" : "-") + Logger.HumanReadableSize((UInt64)Math.Abs((float)(sizeCopied - sizeRemoved))));
            // Close log stream
            Logger.TerminateLogging();
            if(!arguments.repeat) break;
            sleepTime = (Int32)(timestamp - new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
            if(sleepTime > 0) {
                Logger.Info("Waiting " + sleepTime + " seconds from now, process can be terminated with 'Ctrl + C' before the next scan");
                Thread.Sleep(sleepTime * 1000);
            }
            // Reopen log stream
            Logger.ReinitializeLogging();
        }
    }
}