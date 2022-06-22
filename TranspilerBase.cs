using System.Diagnostics;

namespace jarvc
{
    internal class TranspilerBase
    {

        protected static string Beautify(string input)
        {
            File.WriteAllText("./~tmp.jarvs", input);
            Process p = Process.Start(Environment.GetEnvironmentVariable("APPDATA") + "\\npm\\js-beautify.cmd", "-rqf ./~tmp.jarvs");
            p.WaitForExit();
            string ret = File.ReadAllText("./~tmp.jarvs");
            File.Delete("./~tmp.jarvs");
            return ret;
        }

        protected static string IterativeAscendReplacer((string, int[]) transput)
        {
            string[] lines = transput.Item1.Split('\n');
            int[] ascend = transput.Item2.Reverse().ToArray();

            foreach (int i in ascend)
            {
                int cl = i;
                while (lines[i].Contains("xd"))
                    lines[i] = lines[i].Replace("xd", "(" + lines[--cl].Replace("xd =", "").Replace(";", "").Trim() + ")");
                for (int l = i - 1; l >= cl; l--)
                    lines[l] = "";
            }

            return lines.Aggregate((a, b) => a + "\n" + b);
        }

        protected static string Minify(string input)
        {
            string uri = "https://www.toptal.com/developers/javascript-minifier/raw";
            HttpClient c = new();
            Dictionary<string, string> values = new() { { "input", input } };
            using FormUrlEncodedContent content = new(values);
            Task<HttpResponseMessage> response = c.PostAsync(uri, content);
            response.Wait();
            Task<string> txt = response.Result.Content.ReadAsStringAsync();
            txt.Wait();
            return txt.Result;
        }

        protected static (string, int[]) PrimitiveTranspile(string input)
        {
            string[] lines = input.Split('\r').Aggregate((a, b) => a + b).Split('\n');
            string result = "";
            List<int> xds = new();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                bool reqxd = lines[i + 1].Contains("xd");
                if (reqxd) xds.Add(i + 1);
                result += TranspileLine(lines[i], reqxd, i) + "\n";
            }
            result += TranspileLine(lines[^1], false, lines.Length - 1);
            return (result, xds.ToArray());
        }

        protected static string TranspileCommand(string cmd, bool xd, int i)
        {
            if (cmd.StartsWith("send the tweet"))
            {
                if (xd) throw new Jarvisception("Can't take input from this line (" + i + ")");
                return "console.log(" + cmd[14..].Trim() + ");";
            }
            else if (cmd.StartsWith("remember"))
            {
                if (xd) throw new Jarvisception("Can't take input from this line (" + i + ")");
                string[] vars = cmd.Trim().Split(",");
                string res = "let ";
                for (int v = 0; v < vars.Length - 1; v++)
                    res += vars[v][9..].Trim().Replace(" to be ", " = ") + ", ";
                res += vars[^1][9..].Trim().Replace(" to be ", " = ");
                res += ";";
                return res;
            }
            else
            {
                string res = "";
                if (xd) res += "xd = ";
                if (cmd.StartsWith("add"))
                {
                    if (!xd) return "";
                    res += cmd[3..].Trim().Replace(" and ", " + ");
                }
                else
                    throw new Jarvisception("Jarvis has a problem with understanding your sentence in line " + i);
                return res + ";";
            }
        }

        protected static string TranspileLine(string line, bool xd, int i)
        {
            if (!line.StartsWith("Jarvis")) throw new Jarvisception("You didn't address Jarvis correctly in line " + (i + 1));
            if (!line.EndsWith(".")) throw new Jarvisception("You didn't put a period at the end of the line you idiot in line " + (i + 1));
            string[] commands = line[6..].Trim().Split(".");
            return commands.Select(a => a.Trim() == "" ? "" : TranspileCommand(a, xd, i)).Aggregate((a, b) => a + b);
        }
    }
}