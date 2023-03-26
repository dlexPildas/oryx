using System;

namespace OryxDomain.Models.Exceptions
{
    public class BlockException : Exception
    {
        public BlockException() { }

        public BlockException(string message)
            : base(message)
        {

        }
        public BlockException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
