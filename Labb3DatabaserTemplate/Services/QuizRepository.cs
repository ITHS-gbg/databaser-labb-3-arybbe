using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
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

    public void AddQuiz(QuizRecord quizRecord)
    {
        var newQuiz = new Quiz()
        {
            Name = quizRecord.Name,
            Description = quizRecord.Description,
        };

        _quizzesCollection.InsertOne(newQuiz);
    }

    public void AddQuestion(QuestionRecord questionRecord)
    {
        var newQuestion = new Question()
        {
            Text = questionRecord.Text,
            Options = questionRecord.Options,
            CorrectOptionIndex = int.Parse(questionRecord.CorrectOptionIndex)
        };

        _questionsCollection.InsertOne(newQuestion);
    }

    public IEnumerable<QuizRecord> GetAllQuizzes()
    {
        var filter = Builders<Quiz>.Filter.Empty;
        var allQuizzes = _quizzesCollection.Find(filter).ToList()
            .Select(q =>
                new QuizRecord(q.Id.ToString(), q.Name, q.Description, new List<QuestionRecord>()));

        return allQuizzes;
    }

    public IEnumerable<QuestionRecord> GetAllQuestions()
    {
        var filter = Builders<Question>.Filter.Empty;
        var allQuestions = _questionsCollection.Find(filter).ToList()
            .Select(q =>
                new QuestionRecord(q.Id.ToString(), q.Text, q.Options, q.CorrectOptionIndex.ToString()));

        return allQuestions;
    }

    public void DeleteQuiz(string id)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(id));

        _quizzesCollection.DeleteOne(filter);
    }

    public void DeleteQuestion(string id)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(id));

        _questionsCollection.DeleteOne(filter);
    }
}