using Microsoft.AspNetCore.Mvc;
using Web.Repository;
using Microsoft.EntityFrameworkCore;


namespace Web.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] //Đảm bảo đây là một API controller
    public class SuggestionsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public SuggestionsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("query không được để trống.");
            }
            var suggestions = await _dataContext.Products
                .Where(p => EF.Functions.Like(p.Name, $"{query}%"))
                .Select(p => new { Id = p.Id, Name = p.Name })
                .Take(10)
                .ToListAsync();
            return Ok(suggestions);

        }
    }
}
