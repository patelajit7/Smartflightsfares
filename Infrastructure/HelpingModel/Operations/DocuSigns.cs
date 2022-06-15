using System;
using System.Collections.Generic;
namespace Infrastructure.HelpingModel.Operations
{
    public class DocuSigns
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int CardId { get; set; }
        public int AgentId { get; set; }
        public int PortalId { get; set; }
        public double BookingAmount { get; set; }
        public double AcceptedAmount { get; set; }
        public string CardNumber { get; set; }
        public string IP { get; set; }
        public bool? IsAccepted { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        public string EmailId { get; set; }
        public string EnvelopeId { get; set; }
        public string SendIP { get; set; }
        public string CCHolderName { get; set; }
    }
    public class DocuSignsDetails
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int DocuSingsId { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }

    }
    public class DocuSignsVM
    {
        public DocuSigns DocuSigns{ get; set; }
        public List<DocuSignsDetails> DocuSignsDetails { get; set; }
}
}
