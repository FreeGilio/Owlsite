using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.DTO
{
    public class MoveDto
    {
        public int Id { get;  set; }

        public string Name { get;  set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Motion { get;  set; }

        public MoveDto() { }

        public MoveDto(Move move)
        {
            Id = move.Id;
            Name = move.Name;
            Description = move.Description;
            Image = move.Image;
            Motion = move.Motion;
        }
    }
}
