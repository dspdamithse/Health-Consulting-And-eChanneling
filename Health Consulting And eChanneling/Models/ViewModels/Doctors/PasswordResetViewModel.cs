using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.Doctors
{
    public class PasswordResetViewModel
    {
        public PasswordResetViewModel()
        {

        }
        public PasswordResetViewModel(DoctorDTO row)
        {
            Id = row.Id;
            Password = row.Password;
            UserId = row.UserId;
        }
        public int Id { get; set; }
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public int UserId { get; set; }
    }
}