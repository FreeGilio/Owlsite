using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    public class InvalidImageTypeException : Exception
    {
        
            public string Image { get; }

            public InvalidImageTypeException() { }

            public InvalidImageTypeException(string message)
                : base(message) { }

            public InvalidImageTypeException(string message, string image)
                : this(message)
            {
                Image = image;
            }
        
    }
}
