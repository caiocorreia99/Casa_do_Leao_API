using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.DataBase
{
    public class Event
    {
        public int IdEvent { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate{ get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public bool Published { get; set; } = false;
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
