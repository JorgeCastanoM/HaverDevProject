using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.Models
{
    public enum NcrPhase
    {
        [Display(Name = "Quality Representative")]
        QualityInspector,
        [Display(Name = "Engineering")]
        Engineer,
        [Display(Name = "Operations")]
        Operations,
        [Display(Name = "Procurement")]
        Procurement,
        [Display(Name = "Re-Inspection")]
        ReInspection,
        [Display(Name = "Closed")]
        Closed,
        [Display(Name = "Archive")]
        Archive,
        [Display(Name = "Void")]
        Void,
        [Display(Name = "Draft")]
        Draft,
    }
}
