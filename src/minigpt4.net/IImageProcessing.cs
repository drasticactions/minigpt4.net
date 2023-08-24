namespace minigpt4.net;

public interface IImageProcessing
{
    NativeMethods.MiniGPT4Image LoadImage(IntPtr ctx, string path);

    NativeMethods.MiniGPT4Embedding EncodeImage(IntPtr ctx, NativeMethods.MiniGPT4Image image);
}