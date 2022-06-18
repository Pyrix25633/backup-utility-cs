public class Logger {
    private static string barFull = "█", barEmpty = " ";
    /// <summary>
    /// Function to output a success message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Success(string message) {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(GetHour());
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("(Success) ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to output an info message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Info(string message) {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(GetHour());
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("(Info) ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to output a warning message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Warning(string message) {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(GetHour());
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("(Warning) ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to output an error message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Error(string message) {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(GetHour());
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("(Error) ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to clear the last console line
    ///(<paramref name="line"/>)
    /// </summary>
    /// <param name="line">The line to remove, default 1</param>
    public static void RemoveLine(Int16 line = 1) {
        Int32 currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - line);
        for (Int32 i = 0; i < Console.WindowWidth; i++)
            Console.Write(" ");
        Console.SetCursorPosition(0, currentLineCursor - line);
    }
    /// <summary>
    /// Function to output the hour
    /// </summary>
    public static void WriteHour() {
        Console.Write(GetHour());
    }
    /// <summary>
    /// Function to get the string hour
    /// </summary>
    /// <returns>The string hour</returns>
    public static string GetHour() {
        int hour = DateTime.Now.Hour, minute = DateTime.Now.Minute, 
            second = DateTime.Now.Second, millisecond = DateTime.Now.Millisecond;
        return "[" + (hour < 10 ? "0" : "") + hour.ToString() + ":" + (minute < 10 ? "0" : "") + minute.ToString() + ":" +
               (second < 10 ? "0" : "") + second.ToString() + "." +
               (millisecond < 100 ? (millisecond < 10 ? "00" : "0") : "") + millisecond.ToString() + "] ";
    }
    /// <summary>
    /// Function to print a progress bar string
    /// (<paramref name="current"/>, <paramref name="total"/>)
    /// </summary>
    /// <param name="current">The current stage</param>
    /// <param name="total">The total</param>
    public static void ProgressBar(Int64 current, Int64 total) {
        string bar = "[";
        Int16 percent = (Int16)((float)current / total * 100);
        for(Int16 i = 1; i <= percent; i++) {
            bar += barFull;
        }
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(bar);
        bar = "";
        for(Int16 i = (Int16)(percent + 1); i <= 100; i++) {
            bar += barEmpty;
        }
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.Write(bar);
        bar = "] " + percent.ToString() + "% (" + HumanReadableSize(current) + "/" + HumanReadableSize(total) + ")";
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(bar);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to print a progress bar string
    /// (<paramref name="size"/>)
    /// </summary>
    /// <param name="size">The size in bytes</param>
    /// <returns>A string with size and unit</returns>
    public static string HumanReadableSize(Int64 size) {
        Int16 unit = 1024;
        // Bytes
        if(size < unit) return size.ToString() + "B";
        // KiBytes
        Int64 KiBytes = (Int64)Math.Floor((float)size / unit);
        Int16 Bytes = (Int16)(size % unit);
        if(KiBytes < unit) return KiBytes.ToString() + "KiB&" + Bytes.ToString() + "B";
        // MiBytes
        Int32 MiBytes = (Int32)Math.Floor((float)KiBytes / unit);
        KiBytes %= unit;
        if(MiBytes < unit) return MiBytes.ToString() + "MiB&" + KiBytes.ToString() + "KiB";
        Int16 GiBytes = (Int16)Math.Floor((float)MiBytes / unit);
        MiBytes %= unit;
        return GiBytes.ToString() + "GiB&" + MiBytes.ToString() + "MiB";
    }
}