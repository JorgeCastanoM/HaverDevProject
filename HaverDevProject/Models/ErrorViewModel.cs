namespace HaverDevProject.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } //string?

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
