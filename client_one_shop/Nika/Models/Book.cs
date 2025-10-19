using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_one_shop.Nika.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Name { get; set; } = default!;
        public string? Author { get; set; }
        public decimal Price { get; set; }
        public string? ISBN { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
