using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class ContentBase
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public ContentBase() {
            this.Created = DateTime.UtcNow;
        }
    }
}
