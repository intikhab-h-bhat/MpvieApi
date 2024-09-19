using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MpvieApi.Data;
using MpvieApi.Entities;
using MpvieApi.Models;
using System.Diagnostics.Eventing.Reader;

namespace MpvieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly MovieDbContext _context;
        private readonly IMapper _mapper;

        public PersonController(MovieDbContext context,IMapper mapper)
        {
            _context = context; 
            _mapper = mapper;
            
        }

      
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {

            BaseResponseModel res = new BaseResponseModel();

            try
            {
                var actorCount = _context.Person.Count();
                var actorList =_mapper.Map<List<ActorViewModel>>(_context.Person.Skip(pageIndex * pageSize).Take(pageSize).ToList());
                    //.Select(y => new ActorViewModel
                    //{
                    //    Id = y.Id,
                    //    Name = y.Name,
                    //    DateOfBirth = y.DateOfBirth,
                    //}).ToList();
                       


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

        [HttpGet]
        [Route("search/{searchtext}")]
        public IActionResult SearchActor(string searchtext)
        {
            BaseResponseModel res=new BaseResponseModel();

            try
            {
                var searchedActor=_context.Person.Where(x=>x.Name.Contains(searchtext))
                    .Select(x=> new
                    {
                        x.Id,
                        x.Name,
                    }).ToList();
                res.Status = true;
                res.Message = "Success";
                res.Data = searchedActor;

                return Ok(res);

            }
            catch(Exception ex) {
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
                responseModel.Message = "Something Wrong";

                return BadRequest(responseModel);
            }
        }


        [HttpPut]
        public IActionResult UpdatePerson(ActorViewModel model)
        {
            BaseResponseModel res=new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {


                    var putModel = _mapper.Map<Person>(model);
                    if (model.Id <= 0)
                    {
                        res.Status = false;
                        res.Message = "Invalid Id";
                        return BadRequest(res);

                    }

                    var actorDetails = _context.Person.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefault();

                    if (actorDetails == null)
                    {
                        res.Status = false;
                        res.Message = "No Record Found";
                        return BadRequest(res);
                    }

                    _context.Person.Update(putModel);
                    _context.SaveChanges();

                    res.Status = true;
                    res.Message = "Updated Successfully";
                    res.Data = model;

                    return Ok(res);
                }
                else 
                { 
                    res.Status = false;
                    res.Message = "Validation failed";
                    return BadRequest(res);
                }
            }
            catch (Exception)
            {

                res.Status = false;
                res.Message = "something wrong";

                return BadRequest(res);
            }
             
        }



        [HttpDelete("{id}")]
        public IActionResult DeletePerson(int id)
        {
            BaseResponseModel res =new BaseResponseModel();

            if (id <= 0)
            {

                res.Status = false;
                res.Message = "Id acannot be Zero";                 
               
                return BadRequest(res);

            }
            try
            {

                var actorDetails = _context.Person.Where(x => x.Id == id).FirstOrDefault();
                if (actorDetails == null)
                {
                    res.Status = false;
                    res.Message = "Person does not exist";

                    return BadRequest(res);

                }

                _context.Person.Remove(actorDetails);
                _context.SaveChanges();

                res.Status = true;
                res.Message = "Person deleted successfully";
                return Ok(res);
            }
            catch (Exception)
            {
                res.Status = false;
                res.Message = "Wrong";

                return BadRequest(res);

            }

        }

    }
}
