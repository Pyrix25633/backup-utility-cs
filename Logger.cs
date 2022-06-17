public class Logger {
    private static string barFull = "█", barEmpty = "░";
    /// <summary>
    /// Function to output a success message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Success(string message) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(GetHour() + "(Success) " + message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to output an info message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Info(string message) {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(GetHour() + "(Info) " + message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to output a warning message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Warning(string message) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(GetHour() + "(Warning) " + message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to output an error message
    /// (<paramref name="message"/>)
    /// </summary>
    /// <param name="message">The message to output</param>
    public static void Error(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(GetHour() + "(Error) " + message);
        Console.ResetColor();
    }
    /// <summary>
    /// Function to clear the last console line
    /// </summary>
    public static void RemoveLine() {
        Int32 currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - 1);
        for (Int32 i = 0; i < Console.WindowWidth; i++)
            Console.Write(" ");
        Console.SetCursorPosition(0, currentLineCursor - 1);
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
    /// Function to get a progress bar string
    /// (<paramref name="current"/>, <paramref name="total"/>)
    /// </summary>
    /// <param name="current">The current stage</param>
    /// <param name="total">The total</param>
    /// <returns>The progress bar string</returns>
    public static string ProgressBarString(Int64 current, Int64 total) {
        string bar = "[";
        Int16 percent = (Int16)((float)current / total * 100);
        for(Int16 i = 1; i <= percent; i++) {
            bar += barFull;
        }
        for(Int16 i = (Int16)(percent + 1); i <= 100; i++) {
            bar += barEmpty;
        }
        return bar + "] " + percent.ToString() + "%";
    }
}