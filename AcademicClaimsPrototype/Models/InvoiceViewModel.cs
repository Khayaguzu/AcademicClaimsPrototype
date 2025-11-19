using System;
using System.Collections.Generic;
using System.Linq;

namespace AcademicClaimsPrototype.Models
{
    public class InvoiceViewModel
    {
        public string LecturerEmail { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public decimal TotalAmount => Claims.Sum(c => (decimal)c.Amount);
    }
}