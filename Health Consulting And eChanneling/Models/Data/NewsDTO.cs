using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.Data
{
    [Table("News")]
    public class NewsDTO
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
    }
}