using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HaverDevProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaverDevProject.ViewModels
{    
    public class NcrQaDTOMetaData
    {
        [Display(Name = "NCR No.")]
        [Required(ErrorMessage = "You must provide the NCR Number.")]
        [StringLength(10, ErrorMessage = "The NCR Number cannot be more than 10 characters long.")]
        public string NcrNumber { get; set; }

        [Display(Name = "Status")]
        public bool NcrStatus { get; set; } = true;

        [Display(Name = "Item marked Nonconforming")]
        public bool NcrQaItemMarNonConforming { get; set; }

        [Display(Name = "Identify Process Applicable")]
        public bool NcrQaProcessApplicable { get; set; }

        [Display(Name = "Creation Date")]
        [Required(ErrorMessage = "You must provide the date the NCR was created.")]        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime NcrQacreationDate { get; set; }

        [Display(Name = "PO or Prod. No.")]
        [Required(ErrorMessage = "You must provide the PO or Prod. No.")]
        [StringLength(10, ErrorMessage = "This field must be between 7 and 10 characters.", MinimumLength = 7)]
        public string NcrQaOrderNumber { get; set; }

        [Display(Name = "Sales Order No.")]
        [Required(ErrorMessage = "You must provide the Sales Order No.")]
        [StringLength(45, ErrorMessage = "The Sales Order No. cannot be more than 45 characters.")]
        public string NcrQaSalesOrder { get; set; }

        [Display(Name = "Quantity Received")]
        [Required(ErrorMessage = "You must provide the Quantity Received.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Received must be greater than 0.")]
        public int NcrQaQuanReceived { get; set; }

        [Display(Name = "Quantity Defective")]
        [Required(ErrorMessage = "You must provide the Quantity Defective.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Defective must be greater than 0.")]
        public int NcrQaQuanDefective { get; set; }

        [Display(Name = "Description of Defect")]
        [Required(ErrorMessage = "You must provide the defect description.")]
        [StringLength(300, ErrorMessage = "Only 300 characters for description of defect.")]
        [DataType(DataType.MultilineText)]
        public string NcrQaDescriptionOfDefect { get; set; }

        [Display(Name = "Supplier")]
        [Required(ErrorMessage = "You must select a Supplier.")]
        public int SupplierId { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "You must select an Item.")]
        public int ItemId { get; set; }

        [Display(Name = "Defect")]
        [Required(ErrorMessage = "You must select a Defect.")]
        public int DefectId { get; set; }

        [Display(Name = "Engineer Disposition Required?")]
        public bool NcrQaEngDispositionRequired { get; set; }

        [Display(Name = "Video Link")]
        [StringLength(100, ErrorMessage = "Video link cannot be more than 100 characters.")]
        public string NcrQaDefectVideo { get; set; }

        [Display(Name = "Defect Photos")]
        public ICollection<ItemDefectPhoto> ItemDefectPhotos { get; set; } = new HashSet<ItemDefectPhoto>();
    }
}
