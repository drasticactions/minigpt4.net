using System.Runtime.InteropServices;
using System.Text;

namespace minigpt4.net;

public class MiniGpt4 : IDisposable
{
    private IntPtr ctx;
    private IImageProcessing processing;

    public MiniGpt4(MiniGpt4ModelOptions options)
        : this(options, new EmbeddedOpenCVImageProcessing())
    {
    }

    public MiniGpt4(MiniGpt4ModelOptions options, IImageProcessing processing)
    {
        this.ModelOptions = options;
        this.processing = processing;
        this.ctx = NativeMethods.minigpt4_model_load(options.Model, options.LlmModel, (int)options.Verbose,
            options.Seed, options.NCtx,
            options.NBatchSize, options.Numa);
    }

    /// <summary>
    /// Gets the MiniGpt4Options.
    /// </summary>
    public MiniGpt4ModelOptions ModelOptions { get; internal set; }

    public void Dispose()
    {
        NativeMethods.Free(this.ctx);
    }

    public IAsyncEnumerable<string> ChatImageAsync(string imagePath, string text, MiniGpt4Options? options = default, CancellationToken? cancellationToken = default)
    {
        options = options ?? new MiniGpt4Options();
        NativeMethods.SystemPrompt(ctx, options.Threads).ThrowIfError();
        var imageObj = this.processing.LoadImage(this.ctx, imagePath);
        var embedding = this.processing.EncodeImage(this.ctx, imageObj);
        return this.ChatImageAsync(embedding, text, options, cancellationToken);
    }

    public async IAsyncEnumerable<string> ChatImageAsync(NativeMethods.MiniGPT4Embedding imageEmbedding, string text, MiniGpt4Options? options = default, CancellationToken? cancellationToken = default)
    {
        options = options ?? new MiniGpt4Options();
        NativeMethods.SystemPrompt(ctx, options.Threads).ThrowIfError();
        NativeMethods.BeginChatImage(ctx, ref imageEmbedding, text, options.Threads).ThrowIfError();

        IntPtr token = IntPtr.Zero;
        StringBuilder response = new StringBuilder();
        do
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (token != IntPtr.Zero && NativeMethods.ContainsEOSToken(Marshal.PtrToStringAnsi(token)) != 0)
            {
                yield return Marshal.PtrToStringAnsi(token);
            }

            NativeMethods.EndChatImage(ctx, out token, options.Threads,
                options.Temperature, options.TopK, options.TopP, options.TfsZ,
                options.TypicalP, options.RepeatLastN, options.RepeatPenalty, options.AlphaPresence,
                options.AlphaFrequency, options.Mirostat, options.MirostatTau, options.MirostatEta,
                options.PenalizeNl).ThrowIfError();

            response.Append(Marshal.PtrToStringAnsi(token));
            yield return Marshal.PtrToStringAnsi(token);
        } while (NativeMethods.IsEOS(response.ToString()) == 0);
    }

    public async IAsyncEnumerable<string> ChatAsync(string text, MiniGpt4Options? options = default, CancellationToken? cancellationToken = default)
    {
        options = options ?? new MiniGpt4Options();
        NativeMethods.BeginChat(ctx, text, options.Threads).ThrowIfError();
        IntPtr token = IntPtr.Zero;
        StringBuilder response = new StringBuilder();
        do
        {
            cancellationToken?.ThrowIfCancellationRequested();

            if (token != IntPtr.Zero && NativeMethods.ContainsEOSToken(Marshal.PtrToStringAnsi(token)) != 0)
            {
                yield return Marshal.PtrToStringAnsi(token);
            }

            NativeMethods.EndChat(ctx, out token, options.Threads,
                options.Temperature, options.TopK, options.TopP, options.TfsZ,
                options.TypicalP, options.RepeatLastN, options.RepeatPenalty, options.AlphaPresence,
                options.AlphaFrequency, options.Mirostat, options.MirostatTau, options.MirostatEta,
                options.PenalizeNl).ThrowIfError();

            response.Append(Marshal.PtrToStringAnsi(token));
            yield return Marshal.PtrToStringAnsi(token);
        } while (NativeMethods.IsEOS(response.ToString()) == 0);
    }
}