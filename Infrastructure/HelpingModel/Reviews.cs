using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{

    public class GoogleReviews
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Rating { get; set; }
        public int Reviews { get; set; }
        public DateTime Created { get; set; }
        public List<GoogleSubReviews> GoogleSubReviews { get; set; }
    }
    public class GoogleSubReviews
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string UserName { get; set; }
        public decimal UserRating { get; set; }
        public string UserImage { get; set; }
        public string UserLink { get; set; }
        public string Date { get; set; }
        public string ReviewText { get; set; }
        public string ReviewSummary { get; set; }
        public DateTime Created { get; set; }
    }
}
