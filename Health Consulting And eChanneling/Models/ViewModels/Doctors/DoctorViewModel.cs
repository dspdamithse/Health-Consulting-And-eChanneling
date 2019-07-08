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
            RegNumber = row.RegNumber;
            ContactNumber = row.ContactNumber;
            Image = row.Image;
            About = row.About;
            SpecialistAreaName = row.SpecialistAreaName;
            SpecialistAreaId = row.SpecialistAreaId;
        }
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string RegNumber { get; set; }
        public string ContactNumber { get; set; }
        public string Image { get; set; }
        public string About { get; set; }
        public string SpecialistAreaName { get; set; }
        public int SpecialistAreaId { get; set; }

        public IEnumerable<SelectListItem> SpecialistArea { get; set; }
    }
}
