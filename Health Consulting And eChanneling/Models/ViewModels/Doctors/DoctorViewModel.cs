using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.Doctors
{
    public class DoctorViewModel
    {
        public DoctorViewModel()
        {
        }
        public DoctorViewModel(DoctorDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Username = row.Username;
            EmailAddress = row.EmailAddress;
            SLMC_Reg_No = row.SLMC_Reg_No;
            ContactNumber = row.ContactNumber;
            Image = row.Image;
            About = row.About;
            SpecialistAreaName = row.SpecialistAreaName;
            SpecialistAreaId = row.SpecialistAreaId;
            Password = row.Password;
            UserId = row.UserId;
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "SLMC Reg. No. :")]
        public string SLMC_Reg_No { get; set; }
        [Required]
        [Display(Name = "Contact Number :")]
        public string ContactNumber { get; set; }
        [Display(Name = "Profile Image (Optianal) :")]
        public string Image { get; set; }
        [Display(Name = "About Doctor (Optianal) :")]
        [AllowHtml]
        public string About { get; set; }
        [Display(Name = "Specialist :")]
        public string SpecialistAreaName { get; set; }
        public int SpecialistAreaId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public int UserId { get; set; }

        public IEnumerable<SelectListItem> SpecialistArea { get; set; }
        public SelectList DoctorSpecialist { get; internal set; }
    }
}
