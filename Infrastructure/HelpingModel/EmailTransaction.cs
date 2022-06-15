using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class EmailTransaction
    {
        public int TransactionId { get; set; }
        public int PortalId { get; set; }
        public string MailRecipient { get; set; }
        public EmailType EmailType { get; set; }
        public string MailBody { get; set; }
        public string Attachment { get; set; }

    }
}
