﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AN.Integration.API.Extensions;
using AN.Integration.API.Services;
using AN.Integration.OneC.Messages;
using AN.Integration.OneC.Models;

namespace AN.Integration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly HttpQueueClient _httpQueueClient;
        private readonly ILogger<ContactController> _logger;

        public ContactController(HttpQueueClient httpQueueClient, ILogger<ContactController> logger)
        {
            _httpQueueClient = httpQueueClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Contact contact)
        {
            _logger.LogIsOk<Contact>(contact.Code, nameof(Post));

            var (statusCode, content) = await _httpQueueClient
                .SendMessageAsync(new UpsertMessage<Contact>(contact));
            return StatusCode(statusCode, content);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(Contact contact)
        {
            _logger.LogIsOk<Contact>(contact.Code, nameof(Patch));

            var (statusCode, content) = await _httpQueueClient
                .SendMessageAsync(new UpsertMessage<Contact>(contact));
            return StatusCode(statusCode, content);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string code)
        {
            _logger.LogIsOk<Contact>(code, nameof(Delete));

            var (statusCode, content) = await _httpQueueClient
                .SendMessageAsync(new DeleteMessage<Contact>(code));
            return StatusCode(statusCode, content);
        }
    }
}