using DataAccess.Entities;
using MongoDB.Driver;

namespace DataAccess.Services;

public class QuizRepository
{
    private readonly IMongoCollection<Question> _questionsCollection;
    private readonly IMongoCollection<Quiz> _quizzesCollection;

    public QuizRepository()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _questionsCollection =
            database.GetCollection<Question>("Questions", new MongoCollectionSettings() { AssignIdOnInsert = true });
        _quizzesCollection =
            database.GetCollection<Quiz>("Quizzes", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }
}