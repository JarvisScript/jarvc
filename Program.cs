using System;
using System.IO;

namespace jarvc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "")
                throw new Jarvisception("No file specified to transpile");

            string input = File.Exists(args[0]) ? File.ReadAllText(args[0]) : File.ReadAllText(args[0] + ".jarv");

            string[] flags = Array.Empty<string>();

            // flags and shit (to be done) (sometime)

            Transpiler t = new(flags);
            string output = t.Transpile(input);

            File.WriteAllText(args[0] + ".js", output);
        }
    }
}