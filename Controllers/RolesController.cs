using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController: ControllerBase
    {
        private DataContext _context;

        public RolesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> List(
            [FromQuery] bool Inactivos)
        {
            if(Inactivos)
                return await _context.Roles.ToListAsync();
            return await _context.Roles.Where(r => r.Activo).ToListAsync();
        }

    }
}