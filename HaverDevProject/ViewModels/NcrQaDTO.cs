using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HaverDevProject.Models;
using Microsoft.AspNetCore.Mvc;
using Humanizer;

namespace HaverDevProject.ViewModels
{
    [ModelMetadataType(typeof(NcrQaDTOMetaData))]
    public class NcrQaDTO : IValidatableObject
    {                
        public int NcrQaId { get; set; }
        public int? ParentId { get; set; }
        public string NcrQaUserId { get; set; }
        public string NcrNumber { get; set; }
        public bool NcrStatus { get; set; } = true;
        public bool NcrQaStatusFlag { get; set; }
        public NcrPhase NcrPhase { get; set; }
        public bool NcrQaItemMarNonConforming { get; set; }        
        public bool NcrQaProcessApplicable { get; set; }        
        public DateTime NcrQacreationDate { get; set; }       
        public string NcrQaOrderNumber { get; set; }        
        public string NcrQaSalesOrder { get; set; }        
        public int NcrQaQuanReceived { get; set; }        
        public int NcrQaQuanDefective { get; set; }       
        public string NcrQaDescriptionOfDefect { get; set; }
        public int SupplierId { get; set; }
        public int NcrId { get; set; }
        public Ncr Ncr { get; set; }
        public int ItemId { get; set; }
        public int DefectId { get; set; } 
        public bool NcrQaEngDispositionRequired { get; set; }
        public string NcrQaDefectVideo { get; set; }
        public ICollection<ItemDefectPhoto> ItemDefectPhotos { get; set; } = new HashSet<ItemDefectPhoto>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NcrQaQuanDefective > NcrQaQuanReceived)
            {
                yield return new ValidationResult("Quantity Defective must be equal or less than Quantity Received.", new[] { "NcrQaQuanDefective" });
            }
        }
    }
}
