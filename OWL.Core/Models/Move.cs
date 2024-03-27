using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class Move
    {
        [Key]
        public int Move_Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Image {  get; private set; }

        public string Motion { get; private set; }
    }
}
