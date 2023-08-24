using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine;
using CommandLine.Text;
using minigpt4.net;

class Options
{
    [Option('v', "verbose", Default = 0, HelpText = "Increase output verbosity")]
    public int Verbose { get; set; }

    [Option('m', "model", Required = true, HelpText = "Path to the model file")]
    public string Model { get; set; }

    [Option('l', "llm_model", Required = true, HelpText = "Path to language model")]
    public string LlmModel { get; set; }

    [Option('t', "threads", Default = 0, HelpText = "Number of threads to use")]
    public int Threads { get; set; }

    [Option("image", Required = true, HelpText = "Image to encode")]
    public string Image { get; set; }

    [Option("texts", Required = true, HelpText = "Texts to encode")]
    public IEnumerable<string> Texts { get; set; }

    [Option("temp", Default = 0.80f, HelpText = "Temperature")]
    public float Temperature { get; set; }

    [Option("top_k", Default = 40, HelpText = "top_k")]
    public int TopK { get; set; }

    [Option("top_p", Default = 0.90f, HelpText = "top_p")]
    public float TopP { get; set; }

    [Option("tfs_z", Default = 1.00f, HelpText = "tfs_z")]
    public float TfsZ { get; set; }

    [Option("typical_p", Default = 1.00f, HelpText = "typical_p")]
    public float TypicalP { get; set; }

    [Option("repeat_last_n", Default = 64, HelpText = "repeat_last_n")]
    public int RepeatLastN { get; set; }

    [Option("repeat_penalty", Default = 1.10f, HelpText = "repeat_penalty")]
    public float RepeatPenalty { get; set; }

    [Option("alpha_presence", Default = 1.00f, HelpText = "alpha_presence")]
    public float AlphaPresence { get; set; }

    [Option("alpha_frequency", Default = 1.00f, HelpText = "alpha_frequency")]
    public float AlphaFrequency { get; set; }

    [Option("mirostat", Default = 0, HelpText = "mirostat")]
    public int Mirostat { get; set; }

    [Option("mirostat_tau", Default = 5.00f, HelpText = "mirostat_tau")]
    public float MirostatTau { get; set; }

    [Option("mirostat_eta", Default = 1.00f, HelpText = "mirostat_eta")]
    public float MirostatEta { get; set; }

    [Option("penalize_nl", Default = 1, HelpText = "penalize_nl")]
    public int PenalizeNl { get; set; }

    [Option("n_ctx", Default = 2048, HelpText = "n_ctx")]
    public int NCtx { get; set; }

    [Option("n_batch_size", Default = 512, HelpText = "n_batch_size")]
    public int NBatchSize { get; set; }

    [Option("seed", Default = 1337, HelpText = "seed")]
    public int Seed { get; set; }

    [Option("numa", Default = false, HelpText = "numa")]
    public bool Numa { get; set; }

    public MiniGpt4ModelOptions GenerateModelOptions()
    {
        return new MiniGpt4ModelOptions(this.Model, this.LlmModel, this.Seed, this.NCtx, this.NBatchSize, this.Numa);
    }

    public MiniGpt4Options GenerateOptions()
    {
        return new MiniGpt4Options()
        {
            AlphaFrequency = this.AlphaFrequency,
            AlphaPresence = this.AlphaPresence,
            MirostatEta = this.MirostatEta,
            Mirostat = this.Mirostat,
            MirostatTau = this.MirostatTau,
            PenalizeNl = this.PenalizeNl,
            RepeatLastN = this.RepeatLastN,
            RepeatPenalty = this.RepeatPenalty,
            Temperature = this.Temperature,
            TfsZ = this.TfsZ,
            TopK = this.TopK,
            TopP = this.TopP,
            TypicalP = this.TypicalP,
        };
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var parser = new Parser(settings => { settings.CaseSensitive = false; });

        var optionsResult = parser.ParseArguments<Options>(args);

        optionsResult.WithNotParsed(errors =>
        {
            var text = HelpText.AutoBuild(optionsResult);
            Console.WriteLine(text);
            Environment.Exit(1);
        });

        await optionsResult.WithParsedAsync(RunWithOptions);
    }

    static async Task RunWithOptions(Options options)
    {
        var miniGpt4Options = options.GenerateModelOptions();
        var miniGpt4 = new MiniGpt4(miniGpt4Options, new EmguCVImageProcessing());
        var result = miniGpt4.ChatImageAsync(options.Image, options.Texts.First());
        await foreach (var response in result)
        {
            Console.Write(response);
        }
    }
}