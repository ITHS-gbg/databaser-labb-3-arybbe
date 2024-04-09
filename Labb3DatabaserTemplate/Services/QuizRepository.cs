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
        if (!int.TryParse(questionRecord.CorrectOptionIndex, out var correctOptionIndex))
        {
            throw new FormatException("CorrectOptionIndex is not a valid number");
        }

        var newQuestion = new Question()
        {
            Text = questionRecord.Text,
            Options = questionRecord.Options,
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
                new QuestionRecord(q.Id.ToString(), q.Text, q.Options, q.CorrectOptionIndex.ToString()));

        return allQuestions;
    }

    public void AddQuestionToQuiz(string quizId, string questionId)
    {
        var quizFilter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quizzesCollection.Find(quizFilter).FirstOrDefault();

        var questionFilter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionId));
        var question = _questionsCollection.Find(questionFilter).FirstOrDefault();

        quiz.Questions.Add(question);

        var update = Builders<Quiz>.Update.Set("Questions", quiz.Questions);
        _quizzesCollection.UpdateOne(quizFilter, update);

    }

    public void RemoveQuestionFromQuiz(string quizId, string questionId)
    {
        var quizFilter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quizzesCollection.Find(quizFilter).FirstOrDefault();

        var questionFilter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionId));
        var question = _questionsCollection.Find(questionFilter).FirstOrDefault();

        quiz.Questions.Remove(question);

        var update = Builders<Quiz>.Update.Set("Questions", quiz.Questions);
        _quizzesCollection.UpdateOne(quizFilter, update);
    }

    public void DeleteQuiz(string quizId)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));

        _quizzesCollection.DeleteOne(filter);
    }

    public void DeleteQuestion(string questionId)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionId));

        _questionsCollection.DeleteOne(filter);
    }

    public void DeleteQuestionFromQuizzes(string questionId)
    {
        var filter = Builders<Quiz>.Filter.ElemMatch(q => q.Questions, q => q.Id == ObjectId.Parse(questionId));
        var update = Builders<Quiz>.Update.PullFilter(q => q.Questions, q => q.Id == ObjectId.Parse(questionId));
        _quizzesCollection.UpdateMany(filter, update);
    }


    public void UpdateQuiz(QuizRecord quizRecord)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizRecord.Id));
        var update = Builders<Quiz>.Update.Set("Name", quizRecord.Name).Set("Description", quizRecord.Description);

        _quizzesCollection.UpdateOne(filter, update);
    }

    public void UpdateQuestion(QuestionRecord questionRecord)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionRecord.Id));
        var update = Builders<Question>.Update.Set("Text", questionRecord.Text).Set("Options", questionRecord.Options)
            .Set("CorrectOptionIndex", int.Parse(questionRecord.CorrectOptionIndex));

        _questionsCollection.UpdateOne(filter, update);
    }

    public IEnumerable<QuestionRecord> GetQuestionsForQuiz(string quizId)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quizzesCollection.Find(filter).FirstOrDefault();

        return quiz.Questions.Select(q =>
                       new QuestionRecord(q.Id.ToString(), q.Text, q.Options, q.CorrectOptionIndex.ToString()));
    }

    public QuizRecord GetQuizById(string quizId)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quizzesCollection.Find(filter).FirstOrDefault();

        return new QuizRecord(quiz.Id.ToString(), quiz.Name, quiz.Description, new List<QuestionRecord>());
    }

    public QuestionRecord GetQuestionById(string questionId)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionId));
        var question = _questionsCollection.Find(filter).FirstOrDefault();

        return new QuestionRecord(question.Id.ToString(), question.Text, question.Options, question.CorrectOptionIndex.ToString());
    }

    public QuizRecord GetQuizByName(string name)
    {
        var filter = Builders<Quiz>.Filter.Eq("Name", name);
        var quiz = _quizzesCollection.Find(filter).FirstOrDefault();
        if (quiz is null)
        {
            return null;
        }

        return new QuizRecord(quiz.Id.ToString(), quiz.Name, quiz.Description, new List<QuestionRecord>());
    }

    public QuestionRecord GetQuestionByText(string text)
    {
        var filter = Builders<Question>.Filter.Eq("Text", text);
        var question = _questionsCollection.Find(filter).FirstOrDefault();
        if (question is null)
        {
            return null;
        }

        return new QuestionRecord(question.Id.ToString(), question.Text, question.Options, question.CorrectOptionIndex.ToString());
    }

    public IEnumerable<QuizRecord> GetQuizzesForQuestion(string questionId)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionId));
        var question = _questionsCollection.Find(filter).FirstOrDefault();

        if (question is null)
        {
            return Enumerable.Empty<QuizRecord>();
        }

        var quizFilter = Builders<Quiz>.Filter.Empty;
        var allQuizzes = _quizzesCollection.Find(quizFilter).ToList();

        var quizzesWithQuestion = allQuizzes
            .Where(quiz => quiz.Questions.Any(q => q.Id == question.Id))
            .Select(quiz => new QuizRecord(
                quiz.Id.ToString(),
                quiz.Name,
                quiz.Description,
                quiz.Questions.Select(q => new QuestionRecord(
                    q.Id.ToString(),
                    q.Text,
                    q.Options,
                    q.CorrectOptionIndex.ToString()
                )).ToList() 
            ));

        return quizzesWithQuestion;
    }
}