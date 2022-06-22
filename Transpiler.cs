using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace jarvc
{
    internal class Transpiler : TranspilerBase
    {
        private readonly bool expandXD;

        public Transpiler(string[] flags)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                switch (flags[i].ToLower())
                {
                    case "x":
                    case "expandxd":
                        expandXD = true;
                        break;
                }
            }
        }

        public string Transpile(string input)
        {
            return Optimize(PrimitiveTranspile(input));
        }

        private string Optimize((string, int[]) transput)
        {
            if (!expandXD)
                transput.Item1 = IterativeAscendReplacer(transput);

            transput.Item1 = Minify(transput.Item1);

            transput.Item1 = Beautify(transput.Item1);

            return transput.Item1;
        }
    }

    internal class Jarvisception : Exception
    {
        public Jarvisception() { }

        public Jarvisception(string? message) : base(message)
        {
        }

        public Jarvisception(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected Jarvisception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
