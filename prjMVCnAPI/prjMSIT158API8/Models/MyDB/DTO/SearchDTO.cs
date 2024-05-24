namespace prjMSIT158API8.Models.DTO
{
    public class SearchDTO
    {
        public int categoryId { get; set; } = 0;
        public string? keyword { get; set; }
        public string? sortBy { get; set; }
        public string? sortType { get; set; }
        public int page { get; set; } = 1;
        public int pagesSize { get; set; } = 9;
    }
}
