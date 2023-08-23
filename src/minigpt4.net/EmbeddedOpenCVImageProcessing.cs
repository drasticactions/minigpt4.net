namespace minigpt4.net;

public class EmbeddedOpenCVImageProcessing : IImageProcessing
{
    public NativeMethods.MiniGPT4Image LoadImage(IntPtr ctx, string path)
    {
        NativeMethods.ImageLoadFromFile(ctx, path, out var img,0).ThrowIfError();
        return img;
    }

    public NativeMethods.MiniGPT4Image PreprocessImage(IntPtr ctx, NativeMethods.MiniGPT4Image image)
    {
        NativeMethods.PreprocessImage(ctx, ref image, out var preprocessedImage, 0).ThrowIfError();
        return preprocessedImage;
    }

    public NativeMethods.MiniGPT4Embedding EncodeImage(IntPtr ctx, NativeMethods.MiniGPT4Image image)
    {
        NativeMethods.EncodeImage(ctx, ref image, out var embedding, IntPtr.Zero).ThrowIfError();
        return embedding;
    }
}