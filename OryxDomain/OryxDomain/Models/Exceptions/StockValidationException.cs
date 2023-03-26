using System;

namespace OryxDomain.Models.Exceptions
{
    public class StockValidationException : Exception
    {
        public StockValidationException() { }

        public StockValidationException(string message)
            : base(message)
        {

        }
        public StockValidationException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
