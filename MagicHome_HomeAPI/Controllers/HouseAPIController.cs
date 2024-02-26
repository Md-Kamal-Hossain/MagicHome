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
    public class HouseAPIController : ControllerBase
    {
        //private readonly ILogger<HouseAPIController> _logger;  
        //public HouseAPIController(ILogger<HouseAPIController> logger)
        //{
        //    _logger = logger;  
        //}
        //private readonly ILogging _logger;  
        //public HouseAPIController(ILogging logger)
        //{
        //    _logger = logger; 
        //}

        private readonly ApplicationDbContext _db;
        public HouseAPIController(ApplicationDbContext db) 
        { 
             _db =db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<HouseDTO>> GetAllHouses()
        {
            //_logger.Log("Getting all houses","");
            return Ok(_db.Houses.ToList());
        }
        [HttpGet("{id:int}", Name ="GetHouse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<HouseDTO> GetHouse(int id)
        {
            if(id==0)
            {
                //_logger.Log("Get House Error with Id"+ id, "error");
                return BadRequest();
            }
           var house = _db.Houses.FirstOrDefault(u=>u.Id ==id);
            if(house==null)
            {
                return NotFound();  
            }
           return Ok(house);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<HouseDTO> CreateHouse(HouseDTO houseDTO)
        {   
            if(_db.Houses.FirstOrDefault(u=>u.Name.ToLower()==houseDTO.Name.ToLower()) !=null)
            {
                ModelState.AddModelError("CustomError", "House Already Exists");
                return BadRequest();    
            }
            if(houseDTO == null) 
            {
                return BadRequest(houseDTO);    
            }
            if(houseDTO.Id>0) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);    
            }
            House house = new()
            {
                Amenity = houseDTO.Amenity,
                Details = houseDTO.Details,
                Id= houseDTO.Id,
                ImageUrl= houseDTO.ImageUrl,
                Name= houseDTO.Name,    
                Occupancy= houseDTO.Occupancy,
                Rate= houseDTO.Rate,
                Sqft = houseDTO.Sqft

            };
          
            _db.Houses.Add(house);
            _db.SaveChanges();
            return CreatedAtRoute("GetHouse", new {id =houseDTO.Id}, houseDTO);
           


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
            var houseToDelete = _db.Houses.FirstOrDefault(u => u.Id == id);
            if (houseToDelete == null)
            {
                return NotFound();
            }


            _db.Houses.Remove(houseToDelete);
            _db.SaveChanges();
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
            
            //var houseToUpdate = _db.Houses.FirstOrDefault(u => u.Id == id);
            //houseToUpdate.Name = houseDTO.Name;
            //houseToUpdate.Occupancy = houseDTO.Occupancy;
            //houseToUpdate.Sqft = houseDTO.Sqft;
            House house = new House()
            {
                Amenity = houseDTO.Amenity,
                Details = houseDTO.Details,
                Id = houseDTO.Id,
                ImageUrl = houseDTO.ImageUrl,
                Name = houseDTO.Name,
                Occupancy = houseDTO.Occupancy,
                Rate = houseDTO.Rate,
                Sqft = houseDTO.Sqft

            };
            _db.Houses.Update(house);
            _db.SaveChanges();
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

            var house = _db.Houses.FirstOrDefault(u => u.Id == id);
            HouseDTO houseDTO = new ()
            {
                Amenity = house.Amenity,
                Details = house.Details,
                Id = house.Id,
                ImageUrl = house.ImageUrl,
                Name = house.Name,
                Occupancy = house.Occupancy,
                Rate = house.Rate,
                Sqft = house.Sqft

            };

            if (houseDTO == null)
           {
               return BadRequest();  
           }
           patchDTO.ApplyTo(houseDTO,ModelState);
            House houseModel = new()
            {
                Amenity = houseDTO.Amenity,
                Details = houseDTO.Details,
                Id = houseDTO.Id,
                ImageUrl = houseDTO.ImageUrl,
                Name = houseDTO.Name,
                Occupancy = houseDTO.Occupancy,
                Rate = houseDTO.Rate,
                Sqft = houseDTO.Sqft
            };
            _db.Houses.Update(houseModel);
            _db.SaveChanges();
           return NoContent();  
            

        }


    }
}
