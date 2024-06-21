using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    [Serializable]
    public class NewsNotFoundException : Exception
    {
        public NewsNotFoundException() { }

        public NewsNotFoundException(string title)
        : base(String.Format("News article cannot be found: {0}", title))
        {

        }

    }
}
