using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContosoAirlines.Models
{
    public class RootModel
    {
        [Display(Name = "Use application permissions")]
        public bool UseAppPermissions { get; set; } = false;
    }
}