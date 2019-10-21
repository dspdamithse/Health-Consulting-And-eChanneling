using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.Doctors
{
    public class FullDoctorViewModel
    {
        public FullDoctorViewModel()
        {

        }
        public FullDoctorViewModel(UserDTO urow, DoctorDTO drow)
        {
            Id = urow.Id;
            FirstName = urow.FirstName;
            LastName = urow.LastName;
            EmailAddress = urow.EmailAddress;
            ProfileImage = urow.ProfileImage;
            Password = urow.Password;

            SLMC_Reg_No = drow.SLMC_Reg_No;
            ContactNumber = drow.ContactNumber;
            About = drow.About;
            SpecialistAreaName = drow.SpecialistAreaName;
            SpecialistAreaId = drow.SpecialistAreaId;
            UserId = drow.UserId;

        }

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public String ProfileImage { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        [Display(Name = "SLMC Reg. No. :")]
        public string SLMC_Reg_No { get; set; }
        [Required]
        [Display(Name = "Contact Number :")]
        public string ContactNumber { get; set; }

        [Display(Name = "About Doctor (Optianal) :")]
        [AllowHtml]
        public string About { get; set; }
        [Display(Name = "Specialist :")]
        public int UserId { get; set; }
        public string SpecialistAreaName { get; set; }
        public int SpecialistAreaId { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> SpecialistArea { get; set; }
        public SelectList DoctorSpecialist { get; internal set; }
    }
}