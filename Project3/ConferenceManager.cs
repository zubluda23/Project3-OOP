namespace Project3;


public class ConferenceManager
{
    private List<Conference> conferences;
    private readonly JsonConferenceService jsonService;

    public ConferenceManager()
    {
        conferences = new List<Conference>();
        jsonService = new JsonConferenceService();
    }

    public void LoadData(string filePath)
    {
        conferences = jsonService.LoadFromFile(filePath);
    }

    public void SaveData(string filePath)
    {
        jsonService.SaveToFile(filePath, conferences);
    }

    public List<Conference> GetAll()
    {
        return new List<Conference>(conferences);
    }

    public void Add(Conference item)
    {
        if (item != null)
        {
            conferences.Add(item);
        }
    }

    public void Update(int index, Conference item)
    {
        if (index >= 0 && index < conferences.Count && item != null)
        {
            conferences[index] = item;
        }
    }

    public void Delete(int index)
    {
        if (index >= 0 && index < conferences.Count)
        {
            conferences.RemoveAt(index);
        }
    }


    public List<Conference> FilterByField(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) return GetAll();

        return (from c in conferences
                where c.Field.Contains(field, StringComparison.OrdinalIgnoreCase)
                select c).ToList();
    }


    public List<Conference> FilterByDeadline(DateTime deadline)
    {
        return (from c in conferences
                where c.Deadline.Date <= deadline.Date
                select c).ToList();
    }


    public List<Conference> FilterByMaxFee(decimal maxFee)
    {
        return (from c in conferences
                where c.Fee <= maxFee
                select c).ToList();
    }
}