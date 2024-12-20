﻿namespace Interpreter
{
    internal struct TextSpan
    {
        public int Start { get; }
        public int Length { get; }
        public int End { get => Start + Length; }

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Creates a span from the start and end indexes
        /// </summary>
        /// <param name="start">The index of the first character</param>
        /// <param name="end">The index after the last character</param>
        public static TextSpan FromBounds(int start, int end)
        {
            return new TextSpan(start, end - start);
        }
    }
}
