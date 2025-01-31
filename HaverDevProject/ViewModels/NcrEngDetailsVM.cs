using HaverDevProject.Models;

namespace HaverDevProject.ViewModels
{
    public class NcrEngDetailsVM
    {
        public Ncr Ncr { get; set; }
        public NcrQa NcrQa { get; set; }
        public NcrEng NcrEng { get; set; }
        public NcrOperation NcrOperation { get; set; }
        public NcrProcurement NcrProcurement { get; set; }
        public NcrReInspect NcrReInspect { get; set; }
        public ApplicationUser Creator { get; set; }
        public ApplicationUser Editor { get; set; }

    }
}
