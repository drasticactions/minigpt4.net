namespace minigpt4.net;

public class MiniGpt4Options
{
    public MiniGpt4Options(string model, string llmModel, int seed = 1337, int nctx = 2048, int nbatch = 512,
        bool numa = false,
        NativeMethods.MiniGPT4Verbosity verbose = NativeMethods.MiniGPT4Verbosity.MINIGPT4_VERBOSITY_NONE)
    {
        this.Model = model;
        this.LlmModel = llmModel;
        this.Seed = seed;
        this.NCtx = nctx;
        this.Numa = numa;
        this.NBatchSize = nbatch;
        this.Verbose = verbose;
    }

    public string Model { get; }

    public string LlmModel { get; }

    public NativeMethods.MiniGPT4Verbosity Verbose { get; }

    public int Threads { get; set; }

    public int Seed { get; }

    public int NCtx { get; }

    public int NBatchSize { get; }

    public bool Numa { get; }

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
}