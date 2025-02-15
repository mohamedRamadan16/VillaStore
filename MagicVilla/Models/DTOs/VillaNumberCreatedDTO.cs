﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla.Models.DTOs
{
    public class VillaNumberCreatedDTO
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }
    }
}
