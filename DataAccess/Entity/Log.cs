namespace DataAccess.Entity
{
    public class Log
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string DateAndTime { get; set; }
    }
}
