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

    
    public event Action UpdateQuestionList;
    public event Action UpdateQuestionListForQuiz;

    public void AddQuiz(QuizRecord quizRecord)
    {
        var newQuiz = new Quiz()
        {
            Name = quizRecord.Name,
            Description = quizRecord.Description,
            Questions = new List<Question>()
        };

        _quizzesCollection.InsertOne(newQuiz);
    }

    public void AddQuestion(QuestionRecord questionRecord)
    {
        if (!int.TryParse(questionRecord.CorrectOption, out var correctOptionIndex))
        {
            throw new FormatException("CorrectOptionIndex is not a valid number");
        }

        var newQuestion = new Question()
        {
            Text = questionRecord.Text,
            Options = questionRecord.Options.ToArray(),
            CorrectOptionIndex = correctOptionIndex
        };

        _questionsCollection.InsertOne(newQuestion);
    }

    public List<QuizRecord> GetAllQuizzes()
    {
        var filter = Builders<Quiz>.Filter.Empty;
        var allQuizzes = _quizzesCollection.Find(filter).ToList()
            .Select(q =>
                new QuizRecord(q.Id.ToString(), q.Name, q.Description, new List<QuestionRecord>()));

        return allQuizzes.ToList();
    }

    public IEnumerable<QuestionRecord> GetAllQuestions()
    {
        var filter = Builders<Question>.Filter.Empty;
        var allQuestions = _questionsCollection.Find(filter).ToList()
            .Select(q =>
                new QuestionRecord(q.Id.ToString(), q.Text, q.Options.ToList(), q.CorrectOptionIndex.ToString()));

        return allQuestions;
    }

    public void AddQuestionToQuiz(string quizId, string questionId)
    {
        if (!ObjectId.TryParse(quizId, out ObjectId quizObjectId) ||
            !ObjectId.TryParse(questionId, out ObjectId questionObjectId))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        var quizFilter = Builders<Quiz>.Filter.Eq("_id", quizObjectId);
        var questionFilter = Builders<Question>.Filter.Eq("_id", questionObjectId);

        var quiz = _quizzesCollection.Find(quizFilter).FirstOrDefault();
        var question = _questionsCollection.Find(questionFilter).FirstOrDefault();

        if (quiz == null || question == null)
        {
            throw new Exception("Quiz or Question not found.");
        }

        var update = Builders<Quiz>.Update.AddToSet(q => q.Questions, question);
        _quizzesCollection.UpdateOne(quizFilter, update);

        UpdateQuestionListForQuiz?.Invoke();
    }

    public void RemoveQuestionFromQuiz(string quizId, string questionId)
    {
        if (!ObjectId.TryParse(quizId, out ObjectId quizObjectId) ||
            !ObjectId.TryParse(questionId, out ObjectId questionObjectId))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        var quizFilter = Builders<Quiz>.Filter.Eq("_id", quizObjectId);
        var update = Builders<Quiz>.Update.PullFilter(q => q.Questions, q => q.Id == questionObjectId);
        _quizzesCollection.UpdateOne(quizFilter, update);

        UpdateQuestionListForQuiz?.Invoke();
    }

    public void DeleteQuiz(string quizId)
    {
        if (!ObjectId.TryParse(quizId, out ObjectId quizObjectId))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        var filter = Builders<Quiz>.Filter.Eq("_id", quizObjectId);
        _quizzesCollection.DeleteOne(filter);
    }

    public void DeleteQuestion(string questionId)
    {
        if (!ObjectId.TryParse(questionId, out ObjectId questionObjectId))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        DeleteQuestionFromQuizzes(questionId);

        var filter = Builders<Question>.Filter.Eq("_id", questionObjectId);
        _questionsCollection.DeleteOne(filter);
        UpdateQuestionList?.Invoke();
    }

    public void DeleteQuestionFromQuizzes(string questionId)
    {
        var filter = Builders<Quiz>.Filter.ElemMatch(q => q.Questions, q => q.Id == ObjectId.Parse(questionId));
        var update = Builders<Quiz>.Update.PullFilter(q => q.Questions, q => q.Id == ObjectId.Parse(questionId));
        _quizzesCollection.UpdateMany(filter, update);
    }
    public bool QuizNameExists(string name, string excludeId = null)
    {
        var filter = Builders<Quiz>.Filter.Eq(q => q.Name, name);
        if (!string.IsNullOrEmpty(excludeId) && ObjectId.TryParse(excludeId, out var objectId))
        {
            var idFilter = Builders<Quiz>.Filter.Ne(q => q.Id, objectId);
            filter = Builders<Quiz>.Filter.And(filter, idFilter);
        }
        return _quizzesCollection.Find(filter).Any();
    }

    public bool QuestionTextExists(string text, string excludeId = null)
    {
        var filter = Builders<Question>.Filter.Eq(q => q.Text, text);
        if (!string.IsNullOrEmpty(excludeId) && ObjectId.TryParse(excludeId, out var objectId))
        {
            var idFilter = Builders<Question>.Filter.Ne(q => q.Id, objectId);
            filter = Builders<Question>.Filter.And(filter, idFilter);
        }
        return _questionsCollection.Find(filter).Any();
    }

    public void UpdateQuiz(QuizRecord quizRecord)
    {
        if (!ObjectId.TryParse(quizRecord.Id, out ObjectId quizId))
        {
            throw new ArgumentException("Invalid quiz ID format");
        }

        var existingQuiz = _quizzesCollection.Find(q => q.Id == quizId).FirstOrDefault();
        if (existingQuiz == null)
        {
            throw new Exception("Quiz not found");
        }

        
        if (existingQuiz.Name == quizRecord.Name && existingQuiz.Description == quizRecord.Description)
        {
            return;
        }

        var filter = Builders<Quiz>.Filter.Eq("_id", quizId);
        var update = Builders<Quiz>.Update
            .Set(q => q.Name, quizRecord.Name)
            .Set(q => q.Description, quizRecord.Description);

        _quizzesCollection.UpdateOne(filter, update);
    }

    public void UpdateQuestion(QuestionRecord questionRecord)
    {
        if (!ObjectId.TryParse(questionRecord.Id, out ObjectId questionId))
        {
            throw new ArgumentException("Invalid question ID format");
        }

        var existingQuestion = _questionsCollection.Find(q => q.Id == questionId).FirstOrDefault();
        if (existingQuestion == null)
        {
            throw new Exception("Question not found");
        }

        if (existingQuestion.Text == questionRecord.Text &&
            existingQuestion.Options.SequenceEqual(questionRecord.Options.ToArray()) &&
            existingQuestion.CorrectOptionIndex == int.Parse(questionRecord.CorrectOption))
        {
            return;
        }

        var filter = Builders<Question>.Filter.Eq("_id", questionId);
        var update = Builders<Question>.Update
            .Set(q => q.Text, questionRecord.Text)
            .Set(q => q.Options, questionRecord.Options.ToArray())
            .Set(q => q.CorrectOptionIndex, int.Parse(questionRecord.CorrectOption));

        _questionsCollection.UpdateOne(filter, update);
    }

    public IEnumerable<QuestionRecord> GetQuestionsForQuiz(string quizId)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quizzesCollection.Find(filter).FirstOrDefault();

        if (quiz == null || quiz.Questions == null)
        {
            return [];
        }

        return quiz.Questions.Select(q =>
            new QuestionRecord(q.Id.ToString(), q.Text, q.Options.ToList(), q.CorrectOptionIndex.ToString()));
    }
}