using HaverDevProject.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.ViewModels
{
    public class DefectApiDTO
    {        
        public int DefectId { get; set; }        
        public string DefectName { get; set; }
    }
}
