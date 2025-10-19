using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_one_shop.Nika.Models
{
    internal class BookItem
    {
        public int BookId { get; set; }
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public decimal Price { get; set; }
    }
}
