using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal struct TextSpan
    {
        public int Start {  get; private set; }
        public int Length { get; private set; }
        public int End { get => Start + Length; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public TextSpan(int start, int length, int line, int column)
        {
            Start = start;
            Length = length;
            Line = line;
            Column = column;
        }
    }
}
