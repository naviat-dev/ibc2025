namespace ibc2025;

public class Question
{
    public string QuestionText { get; }
    public string Answer { get; }
    public string Reference { get; }
    public string[]? Options { get; }
    public bool IsMultiChoice { get; }
    public bool Used { get; set; }

    public Question(string questionText, string answer, string[] options, string reference)
    {
        QuestionText = questionText;
        Answer = answer;
        Options = options;
        Reference = reference;
        IsMultiChoice = true;
        Used = false;
    }

    public Question(string questionText, string answer, string reference)
    {
        QuestionText = questionText;
        Answer = answer;
        Reference = reference;
        IsMultiChoice = false;
        Used = false;
    }
}