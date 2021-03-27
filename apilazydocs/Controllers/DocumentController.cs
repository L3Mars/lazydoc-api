using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Entities;
using ApiLazyDoc.Models;
using ApiLazyDoc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly DocumentService _documentService;

        public DocumentController(DocumentService documentService)
        {
            this._documentService = documentService;
        }

        public DocumentController(ILogger<DocumentController> logger, DocumentService documentService)
        {
            _logger = logger;
            this._documentService = documentService;
        }

        [HttpGet("{documentId}")]
        public ActionResult<Document> Get(Guid documentId)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            var document = this._documentService.Get(documentId, userId);
            return Ok(document);
        }

        [HttpPost("GetDocumentsEntetes")]
        public ActionResult<List<DocumentEntete>> GetDocumentsEntetes(List<string> tags)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            var documentsEntetes = this._documentService.GetDocumentsEntete(tags, userId);
            return Ok(documentsEntetes);
        }

        [HttpPost]
        public ActionResult<int> Post(IFormCollection files)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            var idDocument = this._documentService.AddDocument(files, userId);
            return Ok(idDocument);
        }

        [HttpPost("AddTags")]
        public ActionResult AddTags(DocumentTags documentTags)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            this._documentService.TagDocument(documentTags, userId);
            return Ok();
        }

        [HttpGet("Delete/{documentId}")]
        public ActionResult Delete(Guid documentId)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            this._documentService.Delete(documentId, userId);
            return Ok();
        }
    }
}
