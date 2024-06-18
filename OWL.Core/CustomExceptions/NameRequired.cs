using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    public class NameRequiredException : Exception
    {
        public string Name { get; }
        public NameRequiredException() { }
       
        public NameRequiredException(string message)
            : base(message) { }

        public NameRequiredException(string message, string name)
            : this(message)
        {
            Name = name;
        }
    }
}
