public class DirectoryEntry {
    /// <summary>
    /// Initializer
    /// </summary>
    public DirectoryEntry(string path, string from) {
        fileInfo = new FileInfo(path);
        relativePath = path.Substring(from.Length + 1);
    }
    public string relativePath;
    public FileInfo fileInfo;
    /// <summary>
    /// Function to search in the destination folder for the same file and compare them
    /// (<paramref name="destinationList"/>, <paramref name="allExtensions"/>, <paramref name="extensions"/>)
    /// </summary>
    /// <param name="destinationList">The list of all files in the destination folder</param>
    /// <param name="allExtensions">If all extensions have to be checked for content differencies</param>
    /// <param name="extensions">The list of the extensions to check for content differencies</param>
    /// <returns>Returns true if the file has to be copied</returns>
    public bool ToCopy(DirectoryEntry[] destinationList, bool allExtensions, string[] extensions) {
        Int64 pos;
        bool found = IsInList(destinationList, out pos);
        if(!found) {
            Logger.Info("To copy because not there: " + relativePath);
            return true;
        }
        DirectoryEntry e = destinationList[pos];
        if((e.fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) return false;
        // Check size
        if(fileInfo.Length != e.fileInfo.Length) {
            Logger.Info("To copy because different size: " + relativePath);
            return true;
        }
        if(!allExtensions) {
            if(!extensions.Contains(fileInfo.Extension)) return false;
        }
        // Content differences
        try {
            FileStream stream1 = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read),
                    stream2 = new FileStream(e.fileInfo.FullName, FileMode.Open, FileAccess.Read);
            Int32 byte1, byte2;
            do {
                byte1 = stream1.ReadByte();
                byte2 = stream2.ReadByte();
            } while((byte1 == byte2) && (byte1 != -1));
            stream1.Close();
            stream2.Close();
            bool toCopy = (byte1 != byte2);
            if(toCopy) Logger.Info("To copy because different content: " + relativePath);
            return toCopy;
        }
        catch(Exception exc) {
            Logger.Error("Could not check the file " + relativePath + " for content differences, error: " + exc);
            return false;
        }
    }
    public bool ToRemove(DirectoryEntry[] sourceList) {
        bool toRemove = !IsInList(sourceList, out _);
        if(toRemove) Logger.Info("To remove: " + relativePath);
        return toRemove;
    }
    /// <summary>
    /// Function to search a file in a list of files
    /// (<paramref name="list"/>, <paramref name="pos"/>)
    /// </summary>
    /// <param name="list">The list of files</param>
    /// <param name="pos">The returned position</param>
    /// <returns>Returns true if the file is in the list</returns>
    private bool IsInList(DirectoryEntry[] list, out Int64 pos) {
        Int64 length = list.Length;
        for(pos = 0; pos < length; pos++) {
            if(relativePath == list[pos].relativePath) return true;
        }
        return false;
    }
}