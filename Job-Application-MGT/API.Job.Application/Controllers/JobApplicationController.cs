using EF.Core.Job.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLib.Job.Application.DTO;
using ServiceLib.Job.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
namespace API.Job.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        // file upload location settings from appsettings.json
        private readonly IConfiguration _configuration;

        private APIResponse _response;
        private readonly IJobApplicationRepository _jobAppRepo;

        public JobApplicationController(IConfiguration configuration, IJobApplicationRepository jobAppRepo)
        {
            _jobAppRepo = jobAppRepo;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getAllJobApps")]
        public IActionResult GetAllJobApps()
        {
            try
            {
                var allJobApps = _jobAppRepo.GetAllJobApps();
                return Ok(allJobApps);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }         
        }

        [HttpGet]
        [Route("getAppStatusTypes")]
        public IActionResult GetAppStatusTypes()
        {
            try
            {
                var appStatusTypes = _jobAppRepo.GetAppStatusTypes();
                return Ok(appStatusTypes);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("addJobApplication")]
        public IActionResult AddJobApplication(JobApplication jobAppData)
        {
            _response = new APIResponse();
            try
            {
                // check for null
                // jobAppData = null;
                if (jobAppData == null)
                {
                    return BadRequest();
                }

                // check for exception
                // throw new Exception();

                // check for ModelState
                // ModelState.AddModelError("contactPersonName", "contact person name is required!");
             
                if (ModelState.IsValid)
                {
                    _jobAppRepo.AddJobApp(jobAppData);
                    _response.ResponseCode = 0;
                    _response.ResponseMessage = "Job Applied Successfully !";
                    return Ok(_response);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error !");
            }
        }

    }
}
