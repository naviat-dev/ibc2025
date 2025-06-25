namespace ibc2025;

public class Question
{
    public string QuestionText { get; }
    public string Answer { get; }
    public string Reference { get; }
    public string[]? Options { get; }
    public Boolean IsMultiChoice { get; }

    public Question(string questionText, string answer, string[] options, string reference)
    {
        this.QuestionText = questionText;
        this.Answer = answer;
        this.Options = options;
        this.Reference = reference;
        this.IsMultiChoice = true;
    }

    public Question(string questionText, string answer, string reference)
    {
        this.QuestionText = questionText;
        this.Answer = answer;
        this.Reference = reference;
        this.IsMultiChoice = false;
    }
}