using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{

    [Serializable]

    public class NameExistsException : Exception
    {
        public string Name { get; }

        public NameExistsException() { }

        public NameExistsException(string message)
            : base(message) { }

        public NameExistsException(string message, string name)
            : this(message)
        {
            Name = name;
        }
    }
}
