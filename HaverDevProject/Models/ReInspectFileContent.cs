using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.Models
{
    public class ReInspectFileContent
    {
        [Key, ForeignKey("NcrReInspectPhoto")]
        public int ReInspectFileContentID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        public NcrReInspectPhoto NcrReInspectPhoto { get; set;}

    }
}
