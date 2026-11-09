namespace Project3;

public class Conference
{
    public string Title { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public DateTime Deadline { get; set; } = DateTime.Now;
    public decimal Fee { get; set; } = 0.0m;

    public Conference() { }

    public Conference(string title, string field, string organizer, string location, string startDate, DateTime deadline, decimal fee)
    {
        Title = title;
        Field = field;
        Organizer = organizer;
        Location = location;
        StartDate = startDate;
        Deadline = deadline;
        Fee = fee;
    }

    public string GetInfo()
    {
        return $"{Title} ({Field}) ; {Organizer} ; Дедлайн: {Deadline:dd.MM.yyyy} ; Вартість: {Fee} грн";
    }
}