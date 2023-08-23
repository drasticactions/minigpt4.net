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

    public MiniGpt4Options Generate()
    {
        return new MiniGpt4Options(this.Model, this.LlmModel, this.Seed, this.NCtx, this.NBatchSize, this.Numa)
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
    static void Main(string[] args)
    {
        var parser = new Parser(settings => { settings.CaseSensitive = false; });

        var optionsResult = parser.ParseArguments<Options>(args);

        optionsResult.WithNotParsed(errors =>
        {
            var text = HelpText.AutoBuild(optionsResult);
            Console.WriteLine(text);
            Environment.Exit(1);
        });

        optionsResult.WithParsedAsync(RunWithOptions);
    }

    static async Task RunWithOptions(Options options)
    {
        var miniGpt4Options = options.Generate();
        var miniGpt4 = new MiniGpt4(miniGpt4Options);
        var result = miniGpt4.ChatImageAsync(options.Image, options.Texts.First());
        await foreach (var response in result)
        {
            Console.Write(response);
        }
    }

    // static void RunWithOptions(Options options)
    // {
    //     int verbose = options.Verbose;
    //     string model = options.Model;
    //     string llmModel = options.LlmModel;
    //     int threads = options.Threads;
    //     string image = options.Image;
    //     IEnumerable<string> texts = options.Texts;
    //     float temp = options.Temperature;
    //     int topK = options.TopK;
    //     float topP = options.TopP;
    //     float tfsZ = options.TfsZ;
    //     float typicalP = options.TypicalP;
    //     int repeatLastN = options.RepeatLastN;
    //     float repeatPenalty = options.RepeatPenalty;
    //     float alphaPresence = options.AlphaPresence;
    //     float alphaFrequency = options.AlphaFrequency;
    //     int mirostat = options.Mirostat;
    //     float mirostatTau = options.MirostatTau;
    //     float mirostatEta = options.MirostatEta;
    //     int penalizeNl = options.PenalizeNl;
    //     int nCtx = options.NCtx;
    //     int nBatchSize = options.NBatchSize;
    //     int seed = options.Seed;
    //     bool numa = options.Numa;
    //
    //     if (threads <= 0)
    //     {
    //         threads = Environment.ProcessorCount;
    //     }
    //
    //     Console.WriteLine("=== Args ===");
    //     Console.WriteLine($"Model: {model}");
    //     Console.WriteLine($"LLM Model: {llmModel}");
    //     Console.WriteLine($"Verbose: {verbose}");
    //     Console.WriteLine($"Threads: {threads}");
    //     Console.WriteLine($"Texts: {string.Join(", ", texts)}");
    //     Console.WriteLine($"Images: {image}");
    //     Console.WriteLine("============");
    //     Console.WriteLine($"Running from {Environment.CurrentDirectory}");
    //
    //     if (verbose > 0)
    //     {
    //         minigpt4.net.NativeMethods.minigpt4_set_verbosity(verbose);
    //     }
    //
    //     if (!File.Exists(model))
    //     {
    //         Console.WriteLine($"Model file {model} does not exist");
    //         Environment.Exit(1);
    //     }
    //
    //     if (!File.Exists(llmModel))
    //     {
    //         Console.WriteLine($"LLM Model file {llmModel} does not exist");
    //         Environment.Exit(1);
    //     }
    //
    //     var ctx = minigpt4.net.NativeMethods.minigpt4_model_load(model, llmModel, verbose, seed, nCtx, nBatchSize,
    //         numa);
    //
    //     var error = minigpt4.net.NativeMethods.minigpt4_image_load_from_file(ctx, image, out var imageObj, 0);
    //
    //     error = minigpt4.net.NativeMethods.minigpt4_preprocess_image(ctx, ref imageObj, out var preprocessedImage, 0);
    //     
    //     error = minigpt4.net.NativeMethods.minigpt4_encode_image(ctx, ref preprocessedImage, out var embedding, (IntPtr) threads);
    //
    //    // var embeddings = new NativeMethods.MiniGPT4Embeddings() { embeddings = embedding, n_embeddings = 1 };
    //
    //     error = NativeMethods.minigpt4_system_prompt(ctx, (IntPtr)threads);
    //     var textList = texts.ToList();
    //     error = NativeMethods.minigpt4_begin_chat_image(ctx, ref embedding, textList.First(), threads);
    //     if (error != 0)
    //     {
    //         Console.WriteLine($"Failed to chat image {textList.First()}");
    //         // Handle the error as needed
    //     }
    //
    //     IntPtr token = IntPtr.Zero;
    //     StringBuilder response = new StringBuilder();
    //     response.Capacity = 2048;
    //     var testVal = 0;
    //     do
    //     {
    //         if (token != IntPtr.Zero && NativeMethods.minigpt4_contains_eos_token(Marshal.PtrToStringAnsi(token)) != 0)
    //         {
    //             Console.Write(Marshal.PtrToStringAnsi(token));
    //             Console.Out.Flush();
    //         }
    //
    //         error = NativeMethods.minigpt4_end_chat_image(ctx, out token, threads, temp, topK, topP, tfsZ, typicalP, repeatLastN, repeatPenalty, alphaPresence, alphaFrequency, mirostat, mirostatTau, mirostatEta, penalizeNl);
    //         if (error != 0)
    //         {
    //             Console.WriteLine("Failed to generate chat image");
    //             // Handle the error as needed
    //         }
    //
    //         response.Append(Marshal.PtrToStringAnsi(token));
    //         testVal = NativeMethods.minigpt4_is_eos(response.ToString());
    //     } while (testVal == 0);
    //     Console.WriteLine(response.ToString());
    //
    //     for (var i = 1; i < textList.Count(); i++)
    //     {
    //         var text = textList[i];
    //         NativeMethods.minigpt4_begin_chat(ctx, text, threads);
    //         IntPtr token2 = IntPtr.Zero;
    //         StringBuilder response2 = new StringBuilder();
    //         response2.Capacity = 2048;
    //         var testVal2 = 0;
    //         do
    //         {
    //             if (token2 != IntPtr.Zero && NativeMethods.minigpt4_contains_eos_token(Marshal.PtrToStringAnsi(token2)) != 0)
    //             {
    //                 Console.Write(Marshal.PtrToStringAnsi(token2));
    //                 Console.Out.Flush();
    //             }
    //
    //             error = NativeMethods.minigpt4_end_chat(ctx, out token2, threads, temp, topK, topP, tfsZ, typicalP, repeatLastN, repeatPenalty, alphaPresence, alphaFrequency, mirostat, mirostatTau, mirostatEta, penalizeNl);
    //             if (error != 0)
    //             {
    //                 Console.WriteLine("Failed to generate chat image");
    //                 // Handle the error as needed
    //             }
    //
    //             response2.Append(Marshal.PtrToStringAnsi(token2));
    //             testVal2 = NativeMethods.minigpt4_is_eos(response2.ToString());
    //         } while (testVal2 == 0);
    //         Console.WriteLine(response2.ToString());
    //     }
    // }
}

public static class ObjectHandleExtensions
{
    public static IntPtr ToIntPtr(this object target)
    {
        return GCHandle.Alloc(target).ToIntPtr();
    }

    public static GCHandle ToGcHandle(this object target)
    {
        return GCHandle.Alloc(target);
    }

    public static IntPtr ToIntPtr(this GCHandle target)
    {
        return GCHandle.ToIntPtr(target);
    }
}

// minigpt4.net.NativeMethods.minigpt4_model_load("/Users/drasticactions/Documents/minigpt4/minigpt4-13B-f16.bin", "/Users/drasticactions/Documents/minigpt4/ggml-vicuna-13b-v0-q4_1.bin", 0, 1337, 2048, 512, false);
//
// Console.WriteLine("Hello, World!");