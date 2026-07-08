namespace ibc2025;

public class Question
{
    public string QuestionText { get; }
    public string Answer { get; }
    public string Reference { get; }
    public string[]? Options { get; }
    public bool IsMultiChoice { get; }
    public int Time { get; set; }
    public bool Used { get; set; }

    public Question(string questionText, string answer, string[] options, string reference, int time)
    {
        QuestionText = questionText;
        Answer = answer;
        Options = options;
        Reference = reference;
        IsMultiChoice = true;
        Time = time;
        Used = false;
    }

    public Question(string questionText, string answer, string reference, int time)
    {
        QuestionText = questionText;
        Answer = answer;
        Reference = reference;
        IsMultiChoice = false;
        Time = time;
        Used = false;
    }
}