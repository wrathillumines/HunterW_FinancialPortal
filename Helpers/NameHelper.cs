using HunterW_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.Helpers
{
    public class NameHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public string DisplayName(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var dname = user.DisplayName;

            return (dname);
        }
    }
}