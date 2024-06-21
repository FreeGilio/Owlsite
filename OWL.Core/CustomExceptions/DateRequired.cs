using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    public class DateRequiredException : Exception
    {
        public DateRequiredException() : base("Date is required for the article.")
        {
        }

        public DateRequiredException(string message) : base(message)
        {
        }

        public DateRequiredException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
