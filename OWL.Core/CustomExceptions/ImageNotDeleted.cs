using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    public class ImageNotDeletedException : Exception
    {
        public string ExistingImage { get; }

        public ImageNotDeletedException() { }

        public ImageNotDeletedException(string message)
            : base(message) { }

        public ImageNotDeletedException(string message, string image)
            : this(message)
        {
            ExistingImage = image;
        }
    }
}
