using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    public class InvalidImageExtensionException : Exception
    {
        public string Image { get; }

        public InvalidImageExtensionException() { }

        public InvalidImageExtensionException(string message)
            : base(message) { }

        public InvalidImageExtensionException(string message, string image)
            : this(message)
        {
            Image = image;
        }
    }
}
