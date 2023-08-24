namespace minigpt4.net;

public class MiniGpt4Options
{
    public float MirostatEta { get; set; } = 1.00f;

    public float MirostatTau { get; set; } = 5.00f;

    public int Mirostat { get; set; } = 0;

    public float AlphaFrequency { get; set; } = 1.00f;

    public float AlphaPresence { get; set; } = 1.00f;

    public float RepeatPenalty { get; set; } = 1.10f;

    public int RepeatLastN { get; set; } = 64;

    public int PenalizeNl { get; set; } = 1;

    public float TypicalP { get; set; } = 1.00f;

    public float TfsZ { get; set; } = 1.00f;

    public float TopP { get; set; } = 0.90f;

    public int TopK { get; set; } = 40;

    public float Temperature { get; set; } = 0.80f;

    public int Threads { get; set; } = 0;
}