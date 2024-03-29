﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.Data
{
    public class Db: DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<SidebarDTO> Sidebar { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public DbSet<ProductDTO> Products { get; set; }
        public DbSet<SpecialistAreaDTO> SpecialistArea { get; set; }
        public DbSet<DoctorSpecialistDTO> DoctorSpecialist { get; set; }
        public DbSet<DoctorDTO> Doctors { get; set; }
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<RoleDTO> Roles { get; set; }
        public DbSet<UserRoleDTO> UserRoles { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }
        public DbSet<NewsDTO> News { get; set; }
        public DbSet<MediServiceDTO> MediService { get; set; }

    }
}
