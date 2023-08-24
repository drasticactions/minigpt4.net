namespace minigpt4.net;

public class MiniGpt4ModelOptions
{
    public MiniGpt4ModelOptions(string model, string llmModel, int seed = 1337, int nctx = 2048, int nbatch = 512,
        bool numa = false,
        NativeMethods.MiniGPT4Verbosity verbose = NativeMethods.MiniGPT4Verbosity.MINIGPT4_VERBOSITY_DEBUG)
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

    public int Seed { get; }

    public int NCtx { get; }

    public int NBatchSize { get; }

    public bool Numa { get; }
}
