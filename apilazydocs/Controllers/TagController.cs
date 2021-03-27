using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Models;
using ApiLazyDoc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiLazyDoc.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly TagService _tagService;

        public TagController(ILogger<DocumentController> logger, TagService tagService)
        {
            _logger = logger;
            this._tagService = tagService;
        }

        [HttpGet("GetTags")]
        public ActionResult<List<string>> GetTags()
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            var userTags = this._tagService.GetUserTags(userId);
            return Ok(userTags);
        }

        [HttpDelete("{documentId}/{tagId}")]
        public ActionResult Delete(Guid documentId, int tagId)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            this._tagService.DeleteTag(documentId, tagId, userId);
            return Ok();
        }

        [HttpPost("{documentId}")]
        public ActionResult<Tag> Delete(TagRequest tagRequest, Guid documentId)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            var tag = this._tagService.Add(documentId, tagRequest, userId);
            return Ok(tag);
        }
    }
}
