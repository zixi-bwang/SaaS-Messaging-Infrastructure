using System;
namespace web_starter.Utilities.Exceptions
{
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message) : base(message)
        {

        }
    }
}
