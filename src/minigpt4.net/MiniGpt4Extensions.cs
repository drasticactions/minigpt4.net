namespace minigpt4.net;

public static class MiniGpt4Extensions
{
    public static void ThrowIfError(this NativeMethods.MiniGPT4Error error)
    {
        if (error != NativeMethods.MiniGPT4Error.None)
        {
            throw new MiniGpt4Exception(error);
        }
    }
}