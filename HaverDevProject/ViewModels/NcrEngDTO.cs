using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaverDevProject.Models;
using HaverDevProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.ViewModels;

[ModelMetadataType(typeof(NcrEngMetaData))]
public class NcrEngDTO : Auditable
{
    public int DaysSinceCreated

    {

        get

        {

            return (DateTime.Now - NcrEngCreationDate).Days;

        }

    }
    public int NcrEngId { get; set; }
    public string NcrNumber { get; set; }
    public bool NcrStatus { get; set; } = true;
    public bool NcrEngCustomerNotification { get; set; } = false;
    public string NcrEngDispositionDescription { get; set; }
    public bool NcrEngStatusFlag { get; set; }
    
    public DateTime NcrEngCompleteDate { get; set; }
    public DateTime NcrEngCreationDate { get; set; }
    public NcrPhase NcrPhase { get; set; }
    public string NcrEngUserId { get; set; }
    public int EngDispositionTypeId { get; set; }
    public EngDispositionType EngDispositionType { get; set; }
    public int NcrId { get; set; }
    public int DrawingId { get; set; }
    public bool DrawingRequireUpdating { get; set; } = false;
    public int DrawingOriginalRevNumber { get; set; }
    public int DrawingUpdatedRevNumber { get; set; }
    public DateTime DrawingRevDate { get; set; }
    public string DrawingUserId { get; set; }
    public string NcrEngDefectVideo { get; set; }
    public ICollection<EngDefectPhoto> EngDefectPhotos { get; set; } = new HashSet<EngDefectPhoto>();
}
