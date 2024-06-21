using OWL.Core.DTO;
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
        public int Id { get;  set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image {  get; set; }

        public string Motion { get; set; }

        public Move(int id, string name, string description, string image, string motion)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
            Motion = motion;

        }
        public Move(MoveDto moveDto)
        {
            Id = moveDto.Id;
            Name = moveDto.Name;
            Description = moveDto.Description;
            Image = moveDto.Image;
            Motion = moveDto.Motion;
        }

        public Move()
        {

        }


        public static List<Move> MapToMoves(List<MoveDto> moveDtos)
        {

            List<Move> moves = new List<Move>();

            try
            {
                foreach (MoveDto moveDto in moveDtos)
                {
                    moves.Add(new Move(moveDto));
                }
            }
            catch (Exception ex)
            {

            }


            return moves;
        }
    }
}
