using Microsoft.AspNetCore.Mvc;
using HeartRateZoneService.Workers;
using System.Net;
using HeartRateZoneService.Domain;

namespace HeartRateZoneService.Controllers
{
    [ApiController]
    [Route("api/heart-rate")]
    public class HeartRateZoneController : ControllerBase
    {
        private readonly HeartRateZoneWorker _worker;

        public HeartRateZoneController(HeartRateZoneWorker worker)
        {
            _worker = worker;
        }

        [HttpGet("consumed-messages/count")]
        public IActionResult GetConsumedMessagesCount()
        {
            int count = _worker.GetConsumedDataCount();
            return Ok(count);
        }

        [HttpGet("GetLatestMessage")]
        [ProducesResponseType(typeof(Biometrics), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetLatestMessage()
        {
            var latestMessage = _worker.GetLatestMessage();

            if (latestMessage == null)
            {
                return NotFound();
            }

            return Ok(latestMessage);
        }

        [HttpGet("GetMessages")]
        [ProducesResponseType(typeof(Biometrics[]), 200)]
        public IActionResult GetMessages()
        {
            var messages = _worker.GetAllMessages();
            return Ok(messages);
        }
    }
}
