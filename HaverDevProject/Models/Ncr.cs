using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[ModelMetadataType(typeof(NcrMetaData))]
[Table("ncr")]
public class Ncr
{    
    public int NcrId { get; set; }      
    public string NcrNumber { get; set; }      
    public DateTime NcrLastUpdated { get; set; }
    public string NcrVoidReason { get; set; }
    public bool NcrStatus { get; set; }
    public NcrPhase NcrPhase { get; set; }
    public virtual NcrEng NcrEng { get; set; }    
    public virtual NcrOperation NcrOperation { get; set; }    
    public virtual NcrQa NcrQa { get; set; }
    public virtual NcrProcurement NcrProcurement { get; set; }
    public virtual NcrReInspect NcrReInspect { get; set; }
    public int? ParentId { get; set; } 
    public virtual Ncr ParentNcr { get; set; }
    public virtual ICollection<Ncr> ChildNcrs { get; set; } = new List<Ncr>();
}
