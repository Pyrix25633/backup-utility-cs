public class Arguments {
    /// <summary>
    /// Initializer
    /// </summary>
    public Arguments() {
        source = "";
        destination = "";
        errors = 0;
        repeat = false;
    }
    public string source, destination;
    public string? removed;
    public Int32 time;
    public bool repeat;
    public Int16 errors; 

    /// <summary>
    /// Function to parse the arguments
    /// </summary>
    public void Parse(string[] args) {
        Int16 length = (Int16)args.Length;
        if(length == 0) {
            errors = 255;
            return;
        }
        for(Int16 i = 0; i < length; i++) {
            if(args[i][0] == '-') {
                switch(args[i]) {
                    case "-s":
                        source = args[i + 1];
                        break;
                    case "-d":
                        destination = args[i + 1];
                        break;
                    case "-r":
                        removed = args[i + 1];
                        break;
                    case "-t":
                        string s = args[i + 1];
                        if(!char.IsNumber(s[s.Length - 1])) {
                            char unit = s[s.Length - 1];
                            s = s.Substring(0, s.Length - 1);
                            switch(unit) {
                                case 's':
                                    errors += (Int16)(Int32.TryParse(s, out time) ? 0 : 1);
                                    break;
                                case 'm':
                                    errors += (Int16)(Int32.TryParse(s, out time) ? 0 : 1);
                                    time *= 60;
                                    break;
                                case 'h':
                                    errors += (Int16)(Int32.TryParse(s, out time) ? 0 : 1);
                                    time *= 3600;
                                    break;
                                default:
                                    errors += 1;
                                    break;
                            }
                        }
                        else {
                            errors += (Int16)(Int32.TryParse(s, out time) ? 0 : 1);
                        }
                        repeat = true;
                        break;
                    case "--":
                        break;
                    default:
                        errors += 1;
                        break;
                }
            }
        }
    }
}