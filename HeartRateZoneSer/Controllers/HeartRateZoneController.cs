using Microsoft.AspNetCore.Mvc;
using HeartRateZoneSer.Workers;
using HeartRateZoneSer.Domain;
using System.Collections.Generic;

namespace HeartRateZoneSer.Controllers
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
            Dictionary<Guid, int> deviceQuantities = _worker.GetConsumedDataCount();
            return Ok(deviceQuantities);
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


        [HttpGet("GetLatestdeviceIDquantity")]
        [ProducesResponseType(typeof(Biometrics), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetLatestdeviceIDquantity()
        {
            var latestMessage = _worker.GetLatestdeviceIDquantity();

            if (latestMessage == null)
            {
                return NotFound();
            }

            return Ok(latestMessage);
        }

        [HttpGet("GetalldeviceIDsquantities")]
        [ProducesResponseType(typeof(Biometrics[]), 200)]
        public IActionResult GetalldeviceIDsquantities()
        {
            var messages = _worker.GetalldeviceIDsquantities();
            return Ok(messages);
        }
    }
}
