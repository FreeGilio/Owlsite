using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class Category
    {
        [Key]
        public int Category_Id { get; private set; }

        public string Name { get; private set; }
    }
}
