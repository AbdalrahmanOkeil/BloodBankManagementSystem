using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Application.DTOs
{
    public class RegisterHospitalDTO
    {
        [Required, MaxLength(256)]
        public string Name { get; set; }
        [Required, MaxLength(128)]
        public string Email {  get; set; }
        [Required, MaxLength(256)]
        public string Password { get; set; }
        [Required, MaxLength(30)]
        public string PhoneNumber { get; set; }
    }
}
