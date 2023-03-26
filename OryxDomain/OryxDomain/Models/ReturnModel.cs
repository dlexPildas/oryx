using System;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class ReturnBase
    {
        public bool IsError { get; set; }
        public string MessageError { get; set; }

        public void SetError(Exception exception)
        {
            IsError = true;
            MessageError = exception.Message;
        }

        public void ThrowException()
        {
            throw new Exception(MessageError);
        }
    }

    public class ReturnModel<T> : ReturnBase
    {
        public ReturnModel()
        {
        }

        public T ObjectModel { get; set; }
    }

    public class ReturnListModel<T> : ReturnBase
    {
        public ReturnListModel()
        {
        }

        public IList<T> ObjectModel { get; set; }
    }
}
