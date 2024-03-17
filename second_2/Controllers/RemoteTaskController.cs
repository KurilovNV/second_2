using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace second_2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RemoteTaskController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public RemoteTaskController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksFromServer()
        {
            var response = await _httpClient.GetAsync("http://host.docker.internal:32772/Tasks");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error while fetching tasks from the server");
            }
        }
    }
}
