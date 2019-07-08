using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.Doctors
{
    public class SpecialistAreaViewModel
    {
        public SpecialistAreaViewModel()
        {
        }
        public SpecialistAreaViewModel(SpecialistAreaDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }
        public int Id { get; set; }
        [Display(Name="Specialist Category")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}