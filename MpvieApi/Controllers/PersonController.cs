using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MpvieApi.Data;
using MpvieApi.Entities;
using MpvieApi.Models;

namespace MpvieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly MovieDbContext _context;
        public PersonController(MovieDbContext context)
        {
            _context = context; 
            
        }

      
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {

            BaseResponseModel res = new BaseResponseModel();

            try
            {
                var actorCount = _context.Person.Count();
                var actorList = _context.Person.Skip(pageIndex * pageSize).Take(pageSize)
                    .Select(y => new ActorViewModel
                    {
                        Id = y.Id,
                        Name = y.Name,
                        DateOfBirth = y.DateOfBirth,
                    }).ToList();
                       


                res.Status = true;
                res.Message = "Success";
                res.Data = new { Person = actorList, Count = actorCount };

                return Ok(res);
            }
            catch (Exception)
            {
                res.Status = false;
                res.Message = "Something went wrong";

                return BadRequest(res);

            }
        }

        [HttpGet("{id}")]
        public IActionResult GetPersonById(int id)
        {

            BaseResponseModel res = new BaseResponseModel();

            try
            {

                var actor = _context.Person.Where(x => x.Id == id).FirstOrDefault();

                if (actor == null)
                {
                    res.Status = false;
                    res.Message = "There is No record";
                    return BadRequest(res);
                }

                var actorDetails = new ActorDetailsViewModel
                {
                    Id=actor.Id,
                    DateOfBirth=actor.DateOfBirth,
                    Name = actor.Name,
                    Movies=_context.Movie.Where(x=> x.Actors.Contains(actor)).Select(x => x.Title).ToArray()
                    
                };

                res.Status = true;
                res.Message = "Success";
                res.Data = actorDetails;

                return Ok(res);
            }
            catch (Exception)
            {
                res.Status = false;
                res.Message = "Something went wrong";

                return BadRequest(res);

            }
        }


        [HttpPost]
        public IActionResult AddMovie(ActorViewModel model)
        {
            BaseResponseModel responseModel = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                   
                    var postModel = new Person()
                    {
                       
                        Name = model.Name,
                        DateOfBirth = model.DateOfBirth,
                        
                    };

                    _context.Person.Add(postModel);
                    _context.SaveChanges();

                    model.Id = postModel.Id;

                    responseModel.Status = true;
                    responseModel.Message = "Created Successfully";
                    responseModel.Data = model;

                    return Ok(responseModel);

                }
                else
                {

                    responseModel.Status = false;
                    responseModel.Message = "Validation failed";
                    responseModel.Data = ModelState;

                    return BadRequest(responseModel);
                }

            }
            catch (Exception)
            {
                responseModel.Status = false;
                responseModel.Message = "Validation failed";

                return BadRequest(responseModel);
            }
        }

    }
}
