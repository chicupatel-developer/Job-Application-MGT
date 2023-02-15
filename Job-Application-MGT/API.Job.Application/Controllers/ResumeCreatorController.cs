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
using ResumeService.Job.Application.Interfaces;


namespace API.Job.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeCreatorController : ControllerBase
    {
        private APIResponse _response;

        private readonly IResumeCreator _resumeCreator;

        public ResumeCreatorController(IResumeCreator resumeCreator)
        {
            _resumeCreator = resumeCreator;
        }


    }
}
