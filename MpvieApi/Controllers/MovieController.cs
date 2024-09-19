using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MpvieApi.Data;
using MpvieApi.Entities;
using MpvieApi.Models;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.IO;


namespace MpvieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        private readonly MovieDbContext _context;
        private readonly IMapper _mapper;

        public MovieController(MovieDbContext context, IMapper mapper)
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
                var movieCount = _context.Movie.Count();
                var movieList = _mapper.Map<List<MovieDetailsViewModel>>(_context.Movie.Include(x => x.Actors).Skip(pageIndex * pageSize).Take(pageSize).ToList());
                //.Select(x=> new MovieViewListModel
                //{
                //    Id = x.Id,
                //    Title = x.Title,
                //    Actors=x.Actors.Select(y=> new ActorViewModel 
                //    { 
                //        Id = y.Id,
                //        Name = y.Name,
                //        DateOfBirth = y.DateOfBirth,
                //    }).ToList(),
                //    CoverImage = x.CoverImage,
                //    Language = x.Language,
                //    ReleaseDate = x.ReleaseDate
                //}).ToList();


                res.Status = true;
                res.Message = "Success";
                res.Data = new { Movies = movieList, Count = movieCount };

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
        public IActionResult GetMovieById(int id)
        {

            BaseResponseModel res = new BaseResponseModel();

            try
            {

                var movie = _context.Movie.Include(x => x.Actors).Where(x => x.Id == id).FirstOrDefault();
                //.Select(x => new MovieDetailsViewModel
                //{
                //    Id = x.Id,
                //    Title = x.Title,
                //    Actors = x.Actors.Select(y => new ActorViewModel
                //    {
                //        Id = y.Id,
                //        Name = y.Name,
                //        DateOfBirth = y.DateOfBirth,
                //    }).ToList(),
                //    CoverImage = x.CoverImage,
                //    Language = x.Language,
                //    ReleaseDate = x.ReleaseDate,
                //   Description = x.Description
                //})
                // .FirstOrDefault();

                if (movie == null)
                {
                    res.Status = false;
                    res.Message = "There is No record";
                    return BadRequest(res);
                }

                var movieData = _mapper.Map<MovieDetailsViewModel>(movie);

                res.Status = true;
                res.Message = "Success";
                //res.Data =movie;
                res.Data = movieData;

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
        public IActionResult AddMovie(CreateMovieViewMode model)
        {
            BaseResponseModel responseModel = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                    var actors = _context.Person.Where(x => model.Actors.Contains(x.Id)).ToList();

                    if (actors.Count() != model.Actors.Count())
                    {
                        responseModel.Status = false;
                        responseModel.Message = "Inavlid actor Ids";

                        return BadRequest(responseModel);
                    }

                    var postModel = _mapper.Map<Movie>(model);
                    postModel.Actors = actors;
                    //var postModel = new Movie()
                    //{
                    //    Title = model.Title,
                    //    ReleaseDate = model.ReleaseDate,
                    //    Language = model.Language,
                    //    CoverImage= model.CoverImage,
                    //    Description = model.Description,
                    //    Actors = actors,
                    //};

                    _context.Movie.Add(postModel);
                    _context.SaveChanges();

                    var responseData = _mapper.Map<MovieDetailsViewModel>(postModel);
                    //var responseData = new MovieDetailsViewModel
                    //{
                    //    Id = postModel.Id,
                    //    Title = postModel.Title,
                    //    Actors = postModel.Actors.Select(y => new ActorViewModel
                    //    {
                    //        Id = y.Id,
                    //        Name = y.Name,
                    //        DateOfBirth = y.DateOfBirth,
                    //    }).ToList(),
                    //    CoverImage = postModel.CoverImage,
                    //    Language = postModel.Language,
                    //    ReleaseDate = postModel.ReleaseDate,
                    //    Description = postModel.Description
                    //};


                    responseModel.Status = true;
                    responseModel.Message = "Created Successfully";
                    responseModel.Data = responseData;

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


        [HttpPut]
        public IActionResult UpdateMovie(CreateMovieViewMode model)
        {
            BaseResponseModel responseModel = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0)
                    {
                        responseModel.Status = false;
                        responseModel.Message = "Id cannot be less than or equal to 0";

                        return BadRequest(responseModel);


                    }


                    var actors = _context.Person.Where(x => model.Actors.Contains(x.Id)).ToList();

                    if (actors.Count() != model.Actors.Count())
                    {
                        responseModel.Status = false;
                        responseModel.Message = "Inavlid actor Ids";

                        return BadRequest(responseModel);
                    }
                    var movieDetails = _context.Movie.Include(x => x.Actors).Where(x => x.Id == model.Id).FirstOrDefault();

                    if (movieDetails == null)
                    {
                        responseModel.Status = false;
                        responseModel.Message = "Movie Not Found";

                        return NotFound(responseModel);
                    }

                    movieDetails.Title = model.Title;
                    movieDetails.Description = model.Description;
                    movieDetails.Language = model.Language;
                    movieDetails.ReleaseDate = model.ReleaseDate;
                    movieDetails.CoverImage = model.CoverImage;

                    //Find Removed Actors

                    var removedActors = movieDetails.Actors.Where(x => !model.Actors.Contains(x.Id)).ToList();

                    foreach (var actor in removedActors)
                    {
                        movieDetails.Actors.Remove(actor);
                    }

                    //Find Added Actors

                    var addedActors = actors.Except(movieDetails.Actors).ToList();

                    foreach (var actor in addedActors)
                    {
                        movieDetails.Actors.Add(actor);
                    }

                    _context.SaveChanges();

                    var responseData = new MovieDetailsViewModel
                    {
                        Id = movieDetails.Id,
                        Title = movieDetails.Title,
                        Actors = movieDetails.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                        CoverImage = movieDetails.CoverImage,
                        Language = movieDetails.Language,
                        ReleaseDate = movieDetails.ReleaseDate,
                        Description = movieDetails.Description
                    };


                    responseModel.Status = true;
                    responseModel.Message = "Updated Successfully";
                    responseModel.Data = responseData;

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

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            BaseResponseModel res = new BaseResponseModel();

            if (id <= 0)
            {


                res.Status = false;
                res.Message = "Inavalid Id";
                return BadRequest(res);

            }
            try
            {
                var movieDetails = _context.Movie.Where(x => x.Id == id).FirstOrDefault();
                if (movieDetails == null)
                {

                    res.Status = false;
                    res.Message = "Reord does not exist";
                    return BadRequest(res);
                }

                _context.Movie.Remove(movieDetails);
                _context.SaveChanges();

                res.Status = true;
                res.Message = $"{movieDetails.Title} Record Deleted sucessfully";

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
        [Route("upload")]
        public async Task<IActionResult> UploadMoviePoster(IFormFile imageFile)
        {
            try
            {
                var fileName = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName.TrimStart('\"').TrimEnd('\"');

                string newPath = @"D:\movie-Image";

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(Path.GetExtension(fileName)))
                {

                    return BadRequest(new BaseResponseModel
                    {
                        Status = false,
                        Message = "Extension not allowed"
                    });
                }
                string newFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string fullFilePath = Path.Combine(newPath, newFileName);

                using (var stram = new FileStream(fullFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stram);
                }
                //return Ok(new { path = fullFilePath });
                return Ok(new { ProfileImage = $"{HttpContext.Request.Scheme}:// {HttpContext.Request.Host}/Staticfiles/{newFileName}" });

            }
            catch (Exception)
            {

                return BadRequest(new BaseResponseModel
                {
                    Status = false,
                    Message = "Something Wrong"
                }); ;
            }


        }


    }
}
