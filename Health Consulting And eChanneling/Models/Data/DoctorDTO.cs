using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.Data
{
    [Table("tblDoctors")]
    public class DoctorDTO
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string SLMC_Reg_No { get; set; }
        public string ContactNumber { get; set; }
        public string Image { get; set; }
        public string About { get; set; }
        public string SpecialistAreaName { get; set; }
        public int SpecialistAreaId { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }

        [ForeignKey("SpecialistAreaId")]
        public virtual SpecialistAreaDTO SpecialistArea { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO User { get; set; }
    }
}