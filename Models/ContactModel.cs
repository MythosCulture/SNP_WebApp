using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Webapp_project.Models
{
    public class ContactModel
    {
        [Required]
        [StringLength(maximumLength: 78, MinimumLength = 2)]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 78, MinimumLength = 5)]
        public string Subject { get; set; }
        [Required]
        [StringLength(maximumLength:5000, MinimumLength = 10)]
        public string Message { get; set; }

    }
}
