using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIMiri.Models
{
    public class MDirectoriosCompartidos
    {
        public int IdDirectorio { get; set; }
        public string? NameDirectorio { get; set; }
    }
}
