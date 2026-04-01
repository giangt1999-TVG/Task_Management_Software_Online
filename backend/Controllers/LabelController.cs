using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.LabelRequestModel;
using tms_api.ViewModels.LabelViewModel;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILogger<LabelController> _logger;
        private readonly ILabelRepository _labelRepository;

        public LabelController(ILogger<LabelController> logger,
            ILabelRepository labelRepository)
        {
            _logger = logger;
            _labelRepository = labelRepository;
        }

        /// <summary>
        /// Get all labels in project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("label-project")]
        [Authorize]
        public async Task<IActionResult> GetLabelInProject([FromQuery] int projectId)
        {
            try
            {
                var labels = await _labelRepository.GetLabelInProject(projectId);
                var returnData = labels.Select(s => new LabelDto
                {
                    LabelID = s.LabelId,
                    Name = s.Name,
                    Color = s.Color
                }).ToList();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Get label successfully!", Data = returnData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Create a new label
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateLabel([FromBody] NewLabelRequestModel model)
        {
            try
            {
                var isExistedLabel = await _labelRepository.CheckLabelExistInProject(model.Name, model.ProjectId);
                if (isExistedLabel)
                {
                    return BadRequest("Duplicate label in project");
                }
                var newLabel = new Label
                {
                    Name = model.Name,
                    Color = model.Color,
                    CreatedDate = DateTime.Now,
                    ProjectId = model.ProjectId,
                    IsDeleted = false
                };

                await _labelRepository.AddAsync(newLabel);
                await _labelRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new label successfully!", Data = new { newLabel.LabelId, newLabel.Name, newLabel.Color, newLabel.ProjectId } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
