using MagicVilla.Models;
using MagicVilla.Models.DTOs;

namespace MagicVilla
{
    public static class VillaStore
    {
        static List<VillaDTO> Villas = new List<VillaDTO>()
            {
                new VillaDTO() {Id = 1, Name = "RRVS19", Sqft = 300, Occupancy = 50},
                new VillaDTO() {Id = 2, Name = "RRVS18", Sqft = 250, Occupancy = 35},
                new VillaDTO() {Id = 3, Name = "RRVS17", Sqft = 200, Occupancy = 20}
            };

        public static List<VillaDTO> VillaList = Villas;
        public static void AddVilla(VillaDTO villa) => Villas.Add(villa);
        
        public static void DeleteVilla(VillaDTO villa)
        {
            if (villa == null || villa.Id == 0)
                return;
            Villas.Remove(villa);
        }

        public static void UpdateVilla(VillaDTO villa)
        {
            if (villa == null || villa.Id == 0)
                return;
            
            VillaDTO villaFromList = Villas.FirstOrDefault(v => v.Id == villa.Id);

            if (villaFromList == null)
                return;

            villaFromList.Id = villa.Id;
            villaFromList.Name = villa.Name;
            villaFromList.Sqft = villa.Sqft;
            villaFromList.Occupancy = villa.Occupancy;
        }

    }
}
