
    using System.ComponentModel.DataAnnotations;

    namespace HaverDevProject.ViewModels
    {
        public class UserVM
        {
            public string ID { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Role")]
            public string SelectedRole { get; set; }
            public bool Status { get; set; } = true;
        }
    }

