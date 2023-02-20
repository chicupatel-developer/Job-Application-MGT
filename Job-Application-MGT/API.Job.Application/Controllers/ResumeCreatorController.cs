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
using ResumeService.Job.Application.Models;
using SelectPdf;
using System.Text;


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


        // create pdf resume as byte[] and display @ browser
        [HttpPost]
        [Route("createAndDownloadResume")]
        public IActionResult CreateAndDownloadResume(MyResume myResume)
        {
            _response = new APIResponse();

            try
            {

                // throw new Exception();

                // instantiate a html to pdf converter object
                HtmlToPdf converter = _resumeCreator.GetHtmlToPdfObject();

                // prepare data
                // Personal Info
                PersonalInfo personalInfo = new PersonalInfo();
                personalInfo = myResume.PersonalInfo;
                if (personalInfo == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Personal-Info Null!";
                    return BadRequest(_response);
                }

                // Technical Skills List<string>
                List<string> skills = new List<string>();
                skills = myResume.Skills;
                if (skills == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Technical-Skills Null!";
                    return BadRequest(_response);
                }

                // Work Experience
                List<WorkExperience> workExps = new List<WorkExperience>();
                workExps = myResume.WorkExperience;
                if (workExps == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Work-Experience Null!";
                    return BadRequest(_response);
                }

                // Education              
                List<Education> educations = new List<Education>();
                educations = myResume.Education;
                if (educations == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Education Null!";
                    return BadRequest(_response);
                }

                var content = _resumeCreator.GetPageHeader() +
                                _resumeCreator.GetPersonalInfoString(personalInfo) +
                                _resumeCreator.GetTechnicalSkillsString(skills) +
                                _resumeCreator.GetWorkExperienceString(workExps) +
                                _resumeCreator.GetEducationString(educations) +
                                _resumeCreator.GetPageFooter();

                // create pdf as byte[] and display @ browser
                var pdf = converter.ConvertHtmlString(content);
                var pdfBytes = pdf.Save();


                // UserResumeCreate db-table
                // process to add client's ip address and datetime
                // and PersonalInfo>FirstName and LastName @ db
                var hostName = System.Net.Dns.GetHostName();
                var ips = System.Net.Dns.GetHostAddresses(hostName);
                StringBuilder myIpAddress = new StringBuilder();
                foreach (var ip in ips)
                {
                    myIpAddress.Append(ip.ToString() + ",");
                }
                UserResumeCreate userData = new UserResumeCreate()
                {
                    FirstName = personalInfo.FirstName,
                    LastName = personalInfo.LastName,
                    ResumeCreatedAt = DateTime.Now,
                    UserIPAddress = myIpAddress.ToString().Substring(0, (myIpAddress.ToString().Length))
                };
                if (_resumeCreator.AddUserDataWhenResumeCreated(userData))
                {
                    return File(pdfBytes, "application/pdf");
                    // return File(pdfBytes, "application/pdf", "your_resume.pdf");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Memory File Creation Error!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server Error!");
            }
        }

    }
}
