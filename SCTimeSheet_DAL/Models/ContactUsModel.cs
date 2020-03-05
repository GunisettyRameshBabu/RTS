using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("t_TS_ContactUs")]
    public class ContactUsModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }

        [Phone(ErrorMessage ="Please enter valid email")]
        [Required(ErrorMessage ="Please enter contact")]
        public string Contact { get; set; }

        //[EmailAddress(ErrorMessage ="Invalid Email , Please correct")]
        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }
    }
}
