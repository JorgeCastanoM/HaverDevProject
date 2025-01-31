using HaverDevProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaverDevProject.ViewModels
{
    public class NcrSupplierReportDTO
    {
        public string NcrNumber { get; set; }
        public bool NcrStatus { get; set; }
        public string SupplierName { get; set; }
        public string NcrQaOrderNumber { get; set; }
        public int ItemSAP { get; set; }
        public string ItemName { get; set; }
        public string NcrQaDefect { get; set; }
        public List<string> DefectNames { get; set; } = new List<string>();
        public int NcrQaQuanReceived { get; set; }
        public int NcrQaQuanDefective { get; set; }
        public string NcrQaDescriptionOfDefect { get; set; }
        public string EngDispositionType { get; set; }
        public string EngDispositionDescription { get; set; }
        public string OpDispositionType { get; set; }
        public string OperationDescription { get; set; }
    }
}
