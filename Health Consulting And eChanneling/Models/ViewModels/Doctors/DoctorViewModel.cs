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
            SLMC_Reg_No = row.SLMC_Reg_No;
            ContactNumber = row.ContactNumber;
            Image = row.Image;
            About = row.About;
            SpecialistAreaName = row.SpecialistAreaName;
            SpecialistAreaId = row.SpecialistAreaId;
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name :")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name :")]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [Display(Name = "SLMC Reg. No. :")]
        public string SLMC_Reg_No { get; set; }
        [Required]
        [Display(Name = "Contact Number :")]
        public string ContactNumber { get; set; }
        [Display(Name = "Profile Image (Optianal) :")]
        public string Image { get; set; }
        [Display(Name = "About Doctor (Optianal) :")]
        public string About { get; set; }
        [Display(Name = "Specialist :")]
        public string SpecialistAreaName { get; set; }
        public int SpecialistAreaId { get; set; }

        public IEnumerable<SelectListItem> SpecialistArea { get; set; }
    }
}
