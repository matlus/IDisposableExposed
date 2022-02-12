using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    internal sealed class ImdbMovie
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public int Year { get; set; }
    }
}
