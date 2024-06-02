using Microsoft.EntityFrameworkCore.Infrastructure;

namespace prjMSIT158API8.Models.SS.DTO
{
	public class SearchProductDTO
	{
		public string? keyword { get; set; }
		public string? ProductName { get; set; }
		public int Stocks { get; set; }
		public TLabel LabelName { get; set; } = new TLabel();
	}
}
