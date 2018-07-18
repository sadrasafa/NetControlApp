using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetControlApp.Data;
using NetControlApp.Models;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<ApplicationUser> _userManager;
        public ApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<DataJSON> PostAsync([FromBody] DataJSON value)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var run = new RunModel
            {
                RunName = value.runName,
                User = user,
                Time = DateTime.Now,
                NetType = value.network.type,
                Network = null,
                NetNodes = value.network.nodes,
                DoContact = value.do_contact,
                Target = value.targets,
                DrugTarget = value.drug_targets,
                AlgorithmType = value.algorithm.type,
                AlgorithmParams = value.algorithm.param,
                Progress = null,
                BestResult = null,
                IsCompleted = false
            };
            _context.Add(run);
            _context.SaveChanges();
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