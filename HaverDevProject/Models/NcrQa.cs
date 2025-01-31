using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaverDevProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[ModelMetadataType(typeof(NcrQaMetaData))]
[Table("ncrQA")]
public class NcrQa : Auditable
{    
    public int NcrQaId { get; set; }
    public bool NcrQaStatusFlag { get; set; } = false;
    public bool NcrQaItemMarNonConforming { get; set; }    
    public bool NcrQaProcessApplicable { get; set; }    
    public DateTime NcrQacreationDate { get; set; }    
    public string NcrQaOrderNumber { get; set; }    
    public string NcrQaSalesOrder { get; set; }    
    public int NcrQaQuanReceived { get; set; }    
    public int NcrQaQuanDefective { get; set; }    
    public string NcrQaDescriptionOfDefect { get; set; }  
    public string NcrQaUserId { get; set; }  
    public bool NcrQaEngDispositionRequired { get; set; }    
    public int NcrId { get; set; }    
    public Ncr Ncr { get; set; }    
    public int ItemId { get; set; }    
    public Item Item { get; set; }
    public int DefectId { get; set; }
    public Defect Defect { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public string NcrQaDefectVideo { get; set; }
    public ICollection<ItemDefectPhoto> ItemDefectPhotos { get; set; } = new HashSet<ItemDefectPhoto>();   
}
