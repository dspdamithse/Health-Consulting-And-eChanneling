using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.Doctors
{
    public class DoctorSpecialistViewModel
    {
        public DoctorSpecialistViewModel()
        {
        }
        public DoctorSpecialistViewModel(DoctorSpecialistDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}