using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_one_shop.Nika.Models
{

    public sealed class BookWithCounts : Book
    {
        public int ImageCount { get; set; }
        public int PdfCount { get; set; }
    }

    public sealed class BookImage
    {
        public int BookImageId { get; set; }
        public int BookId { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long? ByteSize { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public byte[]? ImageData { get; set; } // not filled in listing methods
    }

    public sealed class BookFile
    {
        public int BookFileId { get; set; }
        public int BookId { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long? ByteSize { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public byte[]? PdfData { get; set; } // not filled in listing methods
    }


}
