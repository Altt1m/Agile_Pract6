namespace Agile_Pract6
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; } // "важливе", "неважливе" тощо
        public string LabelColor { get; set; }
        public string TaskColor { get; set; }
    }
}