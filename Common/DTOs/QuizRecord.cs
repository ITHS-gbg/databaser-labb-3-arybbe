namespace Common.DTOs;

public record QuizRecord(string Id, string Name, string Description, List<QuestionRecord> Questions);