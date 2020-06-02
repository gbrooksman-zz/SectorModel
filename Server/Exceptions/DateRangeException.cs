using System;
using System.Collections.Generic;
using System.Text;
using SectorModel.Shared.Entities;

namespace SectorModel.Server.Exceptions
{
    public class DateRangeException : Exception
    {
        public DateRangeException()
        {
        }

        public DateRangeException(string message)
            : base(message)
        {
        }

        public DateRangeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
