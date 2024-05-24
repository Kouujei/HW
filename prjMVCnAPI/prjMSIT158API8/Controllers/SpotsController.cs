using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjMSIT158API8.Models;
using prjMSIT158API8.Models.DTO;



namespace prjMSIT158API8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly MyDBContext _context;

        public SpotsController(MyDBContext context)
        {
            _context = context;
        }

        // GET: api/Spots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Spot>>> GetSpots()
        {
            return await _context.Spots.ToListAsync();
        }

        // GET: api/Spots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Spot>> GetSpot(int id)
        {
            var spot = await _context.Spots.FindAsync(id);

            if (spot == null)
            {
                return NotFound();
            }

            return spot;
        }

        // PUT: api/Spots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpot(int id, Spot spot)
        {
            if (id != spot.SpotId)
            {
                return BadRequest();
            }

            _context.Entry(spot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpotExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Spots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SpotsPagingDTO>> GetSpotsBySearch(SearchDTO searchDTO)
        {
            //根據分類編號搜尋景點資料
            var spots = searchDTO.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == searchDTO.categoryId);

            //根據關鍵字搜尋景點資料(title、desc)
            if (!string.IsNullOrEmpty(searchDTO.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(searchDTO.keyword) || s.SpotDescription.Contains(searchDTO.keyword));
            }

            //排序
            switch (searchDTO.sortBy)
            {
                case "spotTitle":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有多少筆資料
            int totalCount = spots.Count();
            //每頁要顯示幾筆資料
            int pageSize = searchDTO.pagesSize;   //searchDTO.pageSize ?? 9;
            //目前第幾頁
            int page = searchDTO.page;

            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            //分頁
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);


            //包裝要傳給client端的資料
            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalCount = totalCount;
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = await spots.ToListAsync();


            return spotsPaging;
        }

        // DELETE: api/Spots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpot(int id)
        {
            var spot = await _context.Spots.FindAsync(id);
            if (spot == null)
            {
                return NotFound();
            }

            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpotExists(int id)
        {
            return _context.Spots.Any(e => e.SpotId == id);
        }
    }
}
