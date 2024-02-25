using MagicHome_HomeAPI.Data;
using MagicHome_HomeAPI.Logging;
using MagicHome_HomeAPI.Models;
using MagicHome_HomeAPI.Models.ModelDTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicHome_HomeAPI.Controllers
{
    [ApiController]
    [Route("api/HouseAPI")]
    public class HouseAPIController : Controller
    {
        //private readonly ILogger<HouseAPIController> _logger;  
        //public HouseAPIController(ILogger<HouseAPIController> logger)
        //{
        //    _logger = logger;  
        //}
        private readonly ILogging _logger;  
        public HouseAPIController(ILogging logger)
        {
            _logger = logger; 
        }

        [HttpGet]
        public ActionResult<IEnumerable<HouseDTO>> GetAllHouses()
        {
            _logger.Log("Getting all houses","");
            return Ok(HouseStore.houses);
        }
        [HttpGet("{id:int}", Name ="GetHouse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<HouseDTO> GetHouse(int id)
        {
            if(id==0)
            {
                _logger.Log("Get House Error with Id"+ id, "error");
                return BadRequest();
            }
           HouseDTO house = HouseStore.houses.FirstOrDefault(u=>u.Id ==id);
            if(house==null)
            {
                return NotFound();  
            }
           return house;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<HouseDTO> CreateHouse(HouseDTO house)
        {   
            if(HouseStore.houses.FirstOrDefault(u=>u.Name.ToLower()==house.Name.ToLower()) !=null)
            {
                ModelState.AddModelError("CustomError", "House Already Exists");
                return BadRequest();    
            }
            if(house == null) 
            {
                return BadRequest(house);    
            }
            if(house.Id>0) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);    
            }
            house.Id = HouseStore.houses.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;  
            HouseStore.houses.Add(house);
            return CreatedAtRoute("GetHouse",new { id = house.Id},house);


        }
        [HttpDelete("{id:int}", Name = "DeleteHouse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteHouse(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var houseToDelete = HouseStore.houses.FirstOrDefault(u => u.Id == id);
            if (houseToDelete == null)
            {
                return NotFound();
            }


            HouseStore.houses.Remove(houseToDelete);
            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateHouse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateHouse(int id, [FromBody]HouseDTO houseDTO)
        {
        
            if (houseDTO == null|| id!=houseDTO.Id) 
            { 
                return BadRequest();    
            }
            
            var houseToUpdate = HouseStore.houses.FirstOrDefault(u => u.Id == id);
            houseToUpdate.Name = houseDTO.Name;
            houseToUpdate.Occupancy = houseDTO.Occupancy;
            houseToUpdate.Sqft = houseDTO.Sqft;
            return NoContent();

        }
        [HttpPatch("{id:int}", Name = "UpdatePartialHouse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialHouse(int id, JsonPatchDocument<HouseDTO> patchDTO)
        {

            if (patchDTO == null || id==null)
            {
                return BadRequest();
            }

            var houseToUpdate = HouseStore.houses.FirstOrDefault(u => u.Id == id);
           if(houseToUpdate == null)
           {
               return BadRequest();  
           }
           patchDTO.ApplyTo(houseToUpdate,ModelState); 
           if(!ModelState.IsValid)
           {
            return BadRequest(ModelState);    
           }
           return NoContent();  
            

        }


    }
}
