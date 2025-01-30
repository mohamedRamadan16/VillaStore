using System.ComponentModel.DataAnnotations;

namespace MagicVilla.Models.DTOs
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }

    }
}
