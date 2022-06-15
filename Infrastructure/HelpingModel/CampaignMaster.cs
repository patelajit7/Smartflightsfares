namespace Infrastructure.HelpingModel
{
    public class CampaignMasters
    {
        public int Id { get; set; }

        public int AffiliateId { get; set; }

        /// <summary>
        /// Our Friendlly name
        /// </summary>
        public string CampaignName { get; set; }

        /// google,bing,kayak
        /// </summary>
        public string UtmSource { get; set; }

        /// <summary>
        /// cpc campaign decide TFN
        /// </summary>
        public string UtmMedium { get; set; }

        /// <summary>
        /// Campaign name like spring sales
        /// </summary>
        public string UtmCampaign { get; set; }
        /// <summary>
        /// EX: running shoes
        /// </summary>
        public string UtmTerm { get; set; }
        /// <summary>
        /// EX: logolink,textlink
        /// </summary>
        public string UtmContent { get; set; }

        /// <summary>
        /// EX: logolink,textlink
        /// </summary>
        public string UtmKeyword { get; set; }

        
        /// <summary>
        /// Campaign Phone number
        /// </summary>
        public string TollFreeNumber { get; set; }
        /// <summary>


        public string ClickedId { get; set; }
        public string UtmPublisher  { get; set; }
        public string UtmPublisherId { get; set; }
        public string UtmChannelId  { get; set; }

        public int PortalID { get; set; }
    }
}
