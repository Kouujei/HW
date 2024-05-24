using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using prjAjaxDemo.Models;
using prjMVCDemo.Models.DTO;
using System.Security.Cryptography;

namespace prjAjaxDemo.Controllers
{
    public class ApiController : Controller
    {

        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; //依賴注入Dependency Injection
        public ApiController(MyDBContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return Content("Hi", "text/html", System.Text.Encoding.UTF8);
        }
        //Ajax 測試
        public IActionResult reContent()
        {
            System.Threading.Thread.Sleep(3000);
            return Content("Hi");
            //return Content("Hi", "text/html");
            //return Content("Hi", "text/plain");
        }
        //回傳不重複城市名稱
        public IActionResult reJSON()
        {
            var ct = _context.Addresses.Select(x => x.City).Distinct();
            return Json(ct);

        }
        //下拉式選單_1city名稱
        public IActionResult City()
        {
            var ct = _context.Addresses.Select(x => x.City).Distinct();
            return Json(ct);
        }
        //下拉式選單_2district名稱
        public IActionResult District(string city)
        {
            var ct = _context.Addresses.Where(x => x.City== city).Select(x => x.SiteId).Distinct();
            return Json(ct);
        }
        //下拉式選單_3Road名稱
        public IActionResult Road(string district)
        {
            var ct = _context.Addresses.Where(x => x.SiteId == district).Select(x => x.Road);;
            return Json(ct);
        }
        //public IActionResult Register(MemberDTO member)
        //{
        //    if (string.IsNullOrEmpty(member.userName))
        //    {
        //        member.userName = "guest";
        //    }
        //    return Content($"Hello {member.userName}，{member.Age} 歲了，電子郵件是 {member.Email}", "text/html", System.Text.Encoding.UTF8);
        //    //if (string.IsNullOrEmpty(name))
        //    //{
        //    //    name = "路人甲";
        //    //}
        //    //return Content($"{name}您好,{age}歲!!");
        //}
        //public IActionResult Avatar(int id = 1)
        //{
        //    Member member = _context.Members.Find(id);
        //    if (member != null)
        //    {
        //        byte[] img = member.FileData;
        //        if (img != null)
        //        {
        //            return File(img, "image/jpeg");
        //        }
        //    }
        //    return NotFound();
        //    //https://localhost:7298/api/Avatar?id=3 只有id才能用/
        //    //如果是
        //}
        public IActionResult Avatar(int id = 1)
        {

                Member? member = _context.Members.Find(id);
                if (member != null)
                {
                    byte[] img = member.FileData;
                    if (img != null)
                    {
                        return File(img, "image/jpeg");
                    }

                }
                return NotFound();
            
        }

        public IActionResult Register(Member member, IFormFile avatar)
        {
            if (string.IsNullOrEmpty(member.Name))
            {
                member.Name = "guest";
            }

            //取得上傳檔案的資訊
            //string info = $"{avatar.FileName} - {avatar.Length} - {avatar.ContentType}";
            //string info = _hostEnvironment.ContentRootPath;

            //檔案上傳寫進資料夾
            //todo1 判斷檔案是否存在
            //todo2 限制上傳檔案的大小跟類型 

            //實際路徑
            //string uploadPath = @"C:\Users\User\Documents\workspace\MSIT158Site\wwwroot\uploads\a.png";
            //WebRootPath：C: \Users\User\Documents\workspace\MSIT158Site\wwwroot
            //ContentRootPath：C:\Users\User\Documents\workspace\MSIT158Site
            string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", avatar.FileName);
            string info = uploadPath;
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                avatar.CopyTo(fileStream);
            }

            //檔案上傳轉成二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                avatar.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }

            //寫進資料庫
            member.FileName = avatar.FileName;
            member.FileData = imgByte;
            _context.Members.Add(member);
            _context.SaveChanges();

            return Content(info, "text/plain", System.Text.Encoding.UTF8);
            // return Content($"Hello {member.Name}，{member.Age} 歲了，電子郵件是 {member.Email}", "text/html", System.Text.Encoding.UTF8);
        }
        //景點
        public IActionResult Spots([FromBody] SearchDTO search)
        {
            //三元運算子的語法是 condition ? value_if_true : value_if_false
            //如果id=0 回傳所有 : 如果id不是0 回傳搜尋的id
            var spots = search.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == search.categoryId);

            if (!string.IsNullOrEmpty(search.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(search.keyword) || s.SpotDescription.Contains(search.keyword));
            }

            switch (search.sortBy)
            {
                case "spotTitle":
                    spots = search.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = search.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有多少筆資料
            int totalCount = spots.Count();
            //每頁要顯示幾筆資料
            int pageSize = search.pagesSize;   //searchDTO.pageSize ?? 9;
            //目前第幾頁
            int page = search.page;
            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //分頁
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);

            //包裝要傳給client端的資料
            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalCount = totalCount;
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = spots.ToList();

            return Json(spotsPaging);

        }

    }
}
