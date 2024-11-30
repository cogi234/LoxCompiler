using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal struct TextSpan
    {
        public int Start {  get; }
        public int Length { get; }
        public int End { get => Start + Length; }

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}
