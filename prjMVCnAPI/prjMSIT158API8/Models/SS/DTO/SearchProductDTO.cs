﻿using Microsoft.EntityFrameworkCore.Infrastructure;

namespace prjMSIT158API8.Models.SS.DTO
{
	public class SearchProductDTO
	{
		public string? searchword { get; set; }
		public string? ProductName { get; set; }
		public int Stocks { get; set; }
		public int UnitPrice { get; set; }
		public string? Description { get; set; }
        //跨資料表:
        public int SubCatId{ get; set; }
        public string? SubcatName { get; set; }
        public int LabelId { get; set; }
        public string? LabelName { get; set; }
        public int CatId { get; set; }
        public string? CatName { get; set; }
        public int? ActiveId { get; set; }
        public string? ActiveName { get; set; }

        //labelid
        //subcat
        //Activeid
        //LaunchTime-新上市TBC


    }
}
