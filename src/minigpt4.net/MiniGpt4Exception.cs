namespace minigpt4.net;

public class MiniGpt4Exception : Exception
{
    public MiniGpt4Exception(NativeMethods.MiniGPT4Error error)
        : base(NativeMethods.ErrorCodeToString(error))
    {
    }
}