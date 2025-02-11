﻿using System.ComponentModel.DataAnnotations;

namespace Magic_Villa_MVC.Models.DTOs
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }
        public VillaDTO Villa {  get; set; } 
    }
}
