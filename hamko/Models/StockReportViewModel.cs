using hamko.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace hamko.ViewModels
{
    public class StockReportViewModel
    {
        public int? SelectedProductId { get; set; }
        public int? SelectedBranchId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }



        public List<SelectListItem> Products { get; set; }
        public List<SelectListItem> Branches { get; set; }

        public List<StockReportRow> PurchaseResults { get; set; } = new List<StockReportRow>();
        public List<StockReportRow> SalesResults { get; set; } = new List<StockReportRow>();
    }
}
