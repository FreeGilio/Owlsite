using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    [Serializable]
    public class IdNotFoundException : Exception
    {
        public int Id { get; }

        public IdNotFoundException() { }

        public IdNotFoundException(string message)
            : base(message) { }

        public IdNotFoundException(int id)
            : this($"Entity with ID '{id}' not found.")
        {
            Id = id;
        }

        public IdNotFoundException(int id, Exception innerException)
            : this($"Entity with ID '{id}' not found.", innerException)
        {
            Id = id;
        }

        public IdNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
