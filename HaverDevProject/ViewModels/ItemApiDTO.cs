using HaverDevProject.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.ViewModels
{
    public class ItemApiDTO
    {       
        public int ItemId { get; set; }
        public string SummaryItem => $"{ItemNumber} - {ItemName}";        
        public int ItemNumber { get; set; }
        public string ItemName { get; set; }
    }
}
