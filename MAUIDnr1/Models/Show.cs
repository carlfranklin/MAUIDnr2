namespace MAUIDnr1.Models
{
    public class Show
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public int ShowNumber { get; set; }
        public string ShowTitle { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DateRecorded { get; set; } = DateTime.Now;
        public DateTime DatePublished { get; set; } = DateTime.Now;
        public string ShowDateString { get; set; } = "";
        public string DayOfWeek { get; set; } = "";
        public string Notes { get; set; } = "";
        public ShowDetails ShowDetails { get; set; } = new ShowDetails();
        public string ListDisplayString
        {
            get
            {
                return ShowNumber.ToString() + " - " + ((DateTime)DatePublished).ToShortDateString() + " - " + ShowTitle;
            }
        }
    }
}