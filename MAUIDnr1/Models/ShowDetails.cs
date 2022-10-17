namespace MAUIDnr1.Models
{
    public class ShowDetails
    {
        public ShowDetails()
        {
            Guests = new List<Guest>();
            Links = new List<Link>();
            Sponsors = new List<Sponsor>();
            Tags = new List<Tag>();
            File = new File();
        }

        public List<Guest> Guests { get; set; }
        public List<Sponsor> Sponsors { get; set; }
        public List<Link> Links { get; set; }
        public List<Tag> Tags { get; set; }
        public File File { get; set; }
    }
}