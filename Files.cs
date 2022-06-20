public class DirectoryEntry {
    /// <summary>
    /// Initializer
    /// </summary>
    public DirectoryEntry(string path, string from) {
        fileInfo = new FileInfo(path);
        relativePath = path.Substring(from.Length);
    }
    public string relativePath;
    public FileInfo fileInfo;
    /// <summary>
    /// Function to compare two files
    /// (<paramref name="file1"/>, <paramref name="file2"/>, <paramref name="allExtensions"/>, <paramref name="extensions"/>)
    /// </summary>
    /// <param name="file1">The first file</param>
    /// <param name="file2">The second file</param>
    /// <param name="allExtensions">If all extensions have to be checked for content differencies</param>
    /// <param name="extensions">The list of the extensions to check for content differencies</param>
    public static bool Compare(FileInfo file1, FileInfo file2, bool allExtensions, string[] extensions) {
        // Check size
        if(file1.Length != file2.Length) return false;
        if(!allExtensions) {
            if(!extensions.Contains(file1.Extension)) return true;
        }
        // Content differences
        FileStream stream1 = new FileStream(file1.FullName, FileMode.Open, FileAccess.Read),
                   stream2 = new FileStream(file2.FullName, FileMode.Open, FileAccess.Read);
        Int32 byte1, byte2;
        do {
            byte1 = stream1.ReadByte();
            byte2 = stream2.ReadByte();
        } while((byte1 == byte2) && (byte1 != -1));
        stream1.Close();
        stream2.Close();
        return (byte1 == byte2);
    }
}