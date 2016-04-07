using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamA.Models
{
    public class TeacherVM
    {
       [Required(ErrorMessage = "Name is Required")]
       public string Username { get; set; }


       [Required(ErrorMessage = "Email is Required")]
       [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                           ErrorMessage = "Email is not valid")]
       public string Email { get; set; }
       public int IsConfirmed { get; set; }
    }
}