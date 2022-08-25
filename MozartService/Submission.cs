using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MozartService
{
    public partial class Submission
    {
        public int Id { get; set; }
        public int? Year { get; set; }
        public string Quarter { get; set; }
        public string Partner { get; set; }
        public string Password { get; set; }
        public string Filename { get; set; }
        public string Logs { get; set; }
        public string Createdby { get; set; }
    }
}
