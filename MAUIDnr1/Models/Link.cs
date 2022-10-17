namespace MAUIDnr1.Models
{
    public class Link
    {
        public int Id { get; set; }
        public int ShowId { get; set; }
        public string Text { get; set; } = "";
        public string Url { get; set; } = "";
        public string LastError { get; set; } = "";
    }
}