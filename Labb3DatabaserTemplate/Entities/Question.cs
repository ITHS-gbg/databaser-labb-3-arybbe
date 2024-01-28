using MongoDB.Bson;

namespace DataAccess.Entities;

public class Question
{
    public ObjectId Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<string> Options { get; set; } 
    public int CorrectOptionIndex { get; set; } 
}