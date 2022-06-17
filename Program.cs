using System;

public class Program {
    static void Main(string[] args) {
        Logger.Success("Hello, this is a success");
        Logger.Info("Hello, this is an info");
        Logger.Warning("Hello, this is a warning");
        Logger.Error("Hello, this is an error");
        Console.WriteLine();
        for(Int16 i = 0; i <= 100; i++) {
            Logger.RemoveLine();
            Console.WriteLine(Logger.ProgressBarString(i, 100));
            Thread.Sleep(200);
        }
    }
}