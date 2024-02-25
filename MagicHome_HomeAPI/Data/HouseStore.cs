using MagicHome_HomeAPI.Models.ModelDTO;

namespace MagicHome_HomeAPI.Data
{
    public static class HouseStore
    {
       public static List<HouseDTO> houses = new List<HouseDTO> { 
         new HouseDTO{ Id =1, Name ="Riverview", Sqft = 120, Occupancy= 4},
         new HouseDTO{ Id =2,Name ="Hillside", Sqft = 100, Occupancy = 3}
        };
    }
}
