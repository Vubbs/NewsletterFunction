using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsletterFunction.Models
{
    public class Article
    {
        public int Id { get; set; }
        public DateTime DateStamp { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string ContentSummary { get; set; } = string.Empty;
    }
}
