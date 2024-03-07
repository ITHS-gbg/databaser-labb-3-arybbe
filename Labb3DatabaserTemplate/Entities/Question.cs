using MongoDB.Bson;

namespace DataAccess.Entities;

public class Question
{
    public ObjectId Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string[] Options { get; set; } = [];
    public int CorrectOptionIndex { get; set; } 
}