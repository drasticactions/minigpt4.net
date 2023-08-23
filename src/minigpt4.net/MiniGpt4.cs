using System.Runtime.InteropServices;
using System.Text;

namespace minigpt4.net;

public class MiniGpt4 : IDisposable
{
    private IntPtr ctx;
    private IImageProcessing processing;
    
    public MiniGpt4(MiniGpt4Options options) : this(options, new EmbeddedOpenCVImageProcessing())
    {
    }
    
    public MiniGpt4(MiniGpt4Options options, IImageProcessing processing)
    {
        this.Options = options;
        this.processing = processing;
        this.ctx = NativeMethods.minigpt4_model_load(options.Model, options.LlmModel, (int)options.Verbose,
            options.Seed, options.NCtx,
            options.NBatchSize, options.Numa);
    }

    /// <summary>
    /// Gets the MiniGpt4Options.
    /// </summary>
    public MiniGpt4Options Options { get; internal set; }

    public void Dispose()
    {
        NativeMethods.Free(this.ctx);
    }

    public IAsyncEnumerable<string> ChatImageAsync(string imagePath, string text, CancellationToken? cancellationToken = default)
    {
        NativeMethods.SystemPrompt(ctx, this.Options.Threads).ThrowIfError();
        var imageObj = this.processing.LoadImage(this.ctx, imagePath);
        var preprocessedImage = this.processing.PreprocessImage(this.ctx, imageObj);
        var embedding = this.processing.EncodeImage(this.ctx, preprocessedImage);
        return this.ChatImageAsync(embedding, text, cancellationToken);
    }

    public async IAsyncEnumerable<string> ChatImageAsync(NativeMethods.MiniGPT4Embedding imageEmbedding, string text, CancellationToken? cancellationToken = default)
    {
        NativeMethods.SystemPrompt(ctx, this.Options.Threads).ThrowIfError();
        NativeMethods.BeginChatImage(ctx, ref imageEmbedding, text, this.Options.Threads).ThrowIfError();

        IntPtr token = IntPtr.Zero;
        StringBuilder response = new StringBuilder();
        do
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (token != IntPtr.Zero && NativeMethods.ContainsEOSToken(Marshal.PtrToStringAnsi(token)) != 0)
            {
                yield return Marshal.PtrToStringAnsi(token);
            }

            NativeMethods.EndChatImage(ctx, out token, this.Options.Threads,
                this.Options.Temperature, this.Options.TopK, this.Options.TopP, this.Options.TfsZ,
                this.Options.TypicalP, this.Options.RepeatLastN, this.Options.RepeatPenalty, this.Options.AlphaPresence,
                this.Options.AlphaFrequency, this.Options.Mirostat, this.Options.MirostatTau, this.Options.MirostatEta,
                this.Options.PenalizeNl).ThrowIfError();

            response.Append(Marshal.PtrToStringAnsi(token));
            yield return Marshal.PtrToStringAnsi(token);
        } while (NativeMethods.IsEOS(response.ToString()) == 0);
    }

    public async IAsyncEnumerable<string> ChatAsync(string text, CancellationToken? cancellationToken = default)
    {
        NativeMethods.BeginChat(ctx, text, this.Options.Threads).ThrowIfError();
        IntPtr token = IntPtr.Zero;
        StringBuilder response = new StringBuilder();
        do
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (token != IntPtr.Zero && NativeMethods.ContainsEOSToken(Marshal.PtrToStringAnsi(token)) != 0)
            {
                yield return Marshal.PtrToStringAnsi(token);
            }

            NativeMethods.EndChat(ctx, out token, this.Options.Threads,
                this.Options.Temperature, this.Options.TopK, this.Options.TopP, this.Options.TfsZ,
                this.Options.TypicalP, this.Options.RepeatLastN, this.Options.RepeatPenalty, this.Options.AlphaPresence,
                this.Options.AlphaFrequency, this.Options.Mirostat, this.Options.MirostatTau, this.Options.MirostatEta,
                this.Options.PenalizeNl).ThrowIfError();

            response.Append(Marshal.PtrToStringAnsi(token));
            yield return Marshal.PtrToStringAnsi(token);
        } while (NativeMethods.IsEOS(response.ToString()) == 0);
    }
}