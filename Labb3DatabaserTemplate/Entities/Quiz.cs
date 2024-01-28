using MongoDB.Bson;

namespace DataAccess.Entities;

public class Quiz
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<Question> Questions { get; set; }
}