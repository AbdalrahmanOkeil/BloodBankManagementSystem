using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Application.DTOs
{
    public class BloodStockResponseDTO
    {
        public int Id { get; set; }
        public string BloodType { get; set; }
        public int UnitsAvailable { get; set; }
    }
}
