namespace Common.DTOs;

public record QuestionRecord(string Id, string Text, string[] Options, string CorrectOptionIndex);