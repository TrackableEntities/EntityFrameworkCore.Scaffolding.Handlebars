using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffoldingSample.Contexts;

namespace ScaffoldingSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly NorthwindSlimContext _context;

        public EmployeeController(NorthwindSlimContext context)
        {
            _context = context;
        }

        // GET api/employee
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var employees = await _context.Employees
                .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.LastName,
                    e.FirstName,
                    BirthDate = e.BirthDate.GetValueOrDefault().ToShortDateString(),
                    HireDate = e.HireDate.GetValueOrDefault().ToShortDateString(),
                    Country = e.Country.ToString()
                })
                .ToListAsync();
            return Ok(employees);
        }
    }
}
