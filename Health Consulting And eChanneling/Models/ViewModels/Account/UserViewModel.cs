﻿using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.Account
{
    public class UserViewModel
    {
        public UserViewModel()
        {
        }
        public UserViewModel(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Username = row.Username;
            RoleConfirm = row.RoleConfirm;
            EmailAddress = row.EmailAddress;
            ProfileImage = row.ProfileImage;
            Password = row.Password;
        }
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public int RoleConfirm { get; set; }
        public String ProfileImage { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

    }
}