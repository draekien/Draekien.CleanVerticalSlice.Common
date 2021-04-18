using System;

namespace Draekien.CleanVerticalSlice.Common.Application.Exceptions
{
    /// <summary>
    /// An exception for when something is unable to be located from a set of search parameters
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
