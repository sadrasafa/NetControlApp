using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetControlApp.Data;
using NetControlApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

public class Network
{
    public string type { get; set; }
    public string nodes { get; set; }
}

public class Algorithm
{
    public string type { get; set; }
    public string param { get; set; }
}

public class DataJSON
{
    public string runName { get; set; }
    public Network network { get; set; }
    public string targets { get; set; }
    public string drug_targets { get; set; }
    public string userID { get; set; }
    public bool do_contact { get; set; }
    public Algorithm algorithm { get; set; }
}
namespace NetControlApp.Controllers
{
    [Route("/[controller]")]
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/
        [HttpPost]
        public DataJSON Post([FromBody] DataJSON value)
        {

            return value;
        }

        // PUT api/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
