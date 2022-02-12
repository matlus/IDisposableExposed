using DomainLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisposableExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly DomainFacade _domainFacade;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(DomainFacade domainFacade, ILogger<MoviesController> logger)
        {
            _domainFacade = domainFacade;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Movie>> Get()
        {
            _logger.LogInformation("Called Get in Movies Controller");
            return await _domainFacade.GetAllMovies();
        }
    }
}
