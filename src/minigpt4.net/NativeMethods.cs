using System.Runtime.InteropServices;

namespace minigpt4.net;

public static class NativeMethods
{
    private const string LibraryName = "libminigpt4";
    
    // Enums
    public enum MiniGPT4DataType
    {
        F16,
        F32,
        I32,
        L64,
        Q4_0,
        Q4_1,
        Q5_0,
        Q5_1,
        Q8_0,
        Q8_1,
        Q2_K,
        Q3_K,
        Q4_K,
        Q5_K,
        Q6_K,
        Q8_K
    }
    
    public enum MiniGPT4Error
    {
        None,
        LoadModelFileHeader,
        LoadModelFileVersion,
        LoadModelMiniGPT4DataType,
        LoadLanguageModel,
        OpenImage,
        ImageSize,
        MmapSupport,
        FailedToAddString,
        LLamaProjectionEmbeddingInvalidSize,
        FailedToAddEmbedding,
        EosToken,
        Eos,
        ImageNot224_244_3,
        ImageNotF32,
        ImageChannelsExpectedRGB,
        ImageFormatExpectedU8,
        PathDoesNotExist,
        DumpModelFileOpen,
        OpenCVNotLinked
    }

    public enum MiniGPT4Verbosity
    {
        MINIGPT4_VERBOSITY_NONE = 0,
        MINIGPT4_VERBOSITY_ERROR = 1,
        MINIGPT4_VERBOSITY_INFO = 2,
        MINIGPT4_VERBOSITY_DEBUG = 3
    }

    public enum MiniGPT4ImageFormat
    {
        MINIGPT4_IMAGE_FORMAT_UNKNOWN = 0,
        MINIGPT4_IMAGE_FORMAT_F32 = 1,
        MINIGPT4_IMAGE_FORMAT_U8 = 2
    }

    public enum MiniGPT4ImageLoadFlags
    {
        MINIGPT4_IMAGE_LOAD_FLAG_NONE
    }

    // Structs
    [StructLayout(LayoutKind.Sequential)]
    public struct MiniGPT4Image
    {
        public IntPtr data;
        public int width;
        public int height;
        public int channels;
        public MiniGPT4ImageFormat format;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MiniGPT4Embedding
    {
        public IntPtr data;
        public UIntPtr elements;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MiniGPT4Embeddings
    {
        public IntPtr embeddings;
        public UIntPtr n_embeddings;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MiniGPT4Images
    {
        public IntPtr images;
        public UIntPtr n_images;
    }
    
    // Function Signatures
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr minigpt4_model_load(string path, string llm_model, int verbosity, int seed, int n_ctx, int n_batch, bool numa);
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_image_load_from_file(IntPtr ctx, string path, out MiniGPT4Image image, int flags);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_preprocess_image(IntPtr ctx, ref MiniGPT4Image image, out MiniGPT4Image preprocessed_image, int flags);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_encode_image(IntPtr ctx, ref MiniGPT4Image image, out MiniGPT4Embedding embedding, IntPtr n_threads);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_begin_chat_image(IntPtr ctx, ref MiniGPT4Embedding image_embedding, string s, IntPtr n_threads);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_end_chat_image(IntPtr ctx, out IntPtr token, IntPtr n_threads, float temp, int top_k, float top_p, float tfs_z, float typical_p, int repeat_last_n, float repeat_penalty, float alpha_presence, float alpha_frequency, int mirostat, float mirostat_tau, float mirostat_eta, int penalize_nl);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_system_prompt(IntPtr ctx, IntPtr n_threads);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_begin_chat(IntPtr ctx, string s, IntPtr n_threads);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_end_chat(IntPtr ctx, out IntPtr token, IntPtr n_threads, float temp, int top_k, float top_p, float tfs_z, float typical_p, int repeat_last_n, float repeat_penalty, float alpha_presence, float alpha_frequency, int mirostat, float mirostat_tau, float mirostat_eta, int penalize_nl);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_reset_chat(IntPtr ctx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_contains_eos_token(string s);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_is_eos(string s);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_free(IntPtr ctx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_free_image(ref MiniGPT4Image image);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_free_embedding(ref MiniGPT4Embedding embedding);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr minigpt4_error_code_to_string(int error_code);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int minigpt4_quantize_model(string in_path, string out_path, int data_type);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void minigpt4_set_verbosity(int verbosity);
    
    public static IntPtr ModelLoad(string path, string llm_model, int verbosity, int seed, int n_ctx, int n_batch, bool numa)
    {
        return minigpt4_model_load(path, llm_model, verbosity, seed, n_ctx, n_batch, numa);
    }

    public static MiniGPT4Error ImageLoadFromFile(IntPtr ctx, string path, out MiniGPT4Image image, int flags)
    {
        return (MiniGPT4Error)minigpt4_image_load_from_file(ctx, path, out image, flags);
    }

    public static MiniGPT4Error PreprocessImage(IntPtr ctx, ref MiniGPT4Image image, out MiniGPT4Image preprocessedImage, int flags)
    {
        return (MiniGPT4Error)minigpt4_preprocess_image(ctx, ref image, out preprocessedImage, flags);
    }

    public static MiniGPT4Error EncodeImage(IntPtr ctx, ref MiniGPT4Image image, out MiniGPT4Embedding embedding, IntPtr n_threads)
    {
        return (MiniGPT4Error)minigpt4_encode_image(ctx, ref image, out embedding, n_threads);
    }
    
    public static MiniGPT4Error BeginChatImage(IntPtr ctx, ref MiniGPT4Embedding imageEmbedding, string s, IntPtr n_threads)
    {
        return (MiniGPT4Error)minigpt4_begin_chat_image(ctx, ref imageEmbedding, s, n_threads);
    }

    public static MiniGPT4Error EndChatImage(IntPtr ctx, out IntPtr token, IntPtr n_threads, float temp, int top_k, float top_p, float tfs_z, float typical_p, int repeat_last_n, float repeat_penalty, float alpha_presence, float alpha_frequency, int mirostat, float mirostat_tau, float mirostat_eta, int penalize_nl)
    {
        return (MiniGPT4Error)minigpt4_end_chat_image(ctx, out token, n_threads, temp, top_k, top_p, tfs_z, typical_p, repeat_last_n, repeat_penalty, alpha_presence, alpha_frequency, mirostat, mirostat_tau, mirostat_eta, penalize_nl);
    }

    public static MiniGPT4Error SystemPrompt(IntPtr ctx, IntPtr n_threads)
    {
        return (MiniGPT4Error)minigpt4_system_prompt(ctx, n_threads);
    }

    public static MiniGPT4Error BeginChat(IntPtr ctx, string s, IntPtr n_threads)
    {
        return (MiniGPT4Error)minigpt4_begin_chat(ctx, s, n_threads);
    }

    public static MiniGPT4Error EndChat(IntPtr ctx, out IntPtr token, IntPtr n_threads, float temp, int top_k, float top_p, float tfs_z, float typical_p, int repeat_last_n, float repeat_penalty, float alpha_presence, float alpha_frequency, int mirostat, float mirostat_tau, float mirostat_eta, int penalize_nl)
    {
        return (MiniGPT4Error)minigpt4_end_chat(ctx, out token, n_threads, temp, top_k, top_p, tfs_z, typical_p, repeat_last_n, repeat_penalty, alpha_presence, alpha_frequency, mirostat, mirostat_tau, mirostat_eta, penalize_nl);
    }

    public static MiniGPT4Error ResetChat(IntPtr ctx)
    {
        return (MiniGPT4Error)minigpt4_reset_chat(ctx);
    }

    public static MiniGPT4Error ContainsEOSToken(string s)
    {
        return (MiniGPT4Error)minigpt4_contains_eos_token(s);
    }

    public static MiniGPT4Error IsEOS(string s)
    {
        return (MiniGPT4Error)minigpt4_is_eos(s);
    }
    
    public static MiniGPT4Error Free(IntPtr ctx)
    {
        return (MiniGPT4Error)minigpt4_free(ctx);
    }

    public static MiniGPT4Error FreeImage(ref MiniGPT4Image image)
    {
        return (MiniGPT4Error)minigpt4_free_image(ref image);
    }

    public static MiniGPT4Error FreeEmbedding(ref MiniGPT4Embedding embedding)
    {
        return (MiniGPT4Error)minigpt4_free_embedding(ref embedding);
    }
    
    public static MiniGPT4Error QuantizeModel(string in_path, string out_path, int data_type)
    {
        return (NativeMethods.MiniGPT4Error)NativeMethods.minigpt4_quantize_model(in_path, out_path, data_type);
    }

    public static void SetVerbosity(NativeMethods.MiniGPT4Verbosity verbosity)
    {
        NativeMethods.minigpt4_set_verbosity((int)verbosity);
    }
    
    public static string ErrorCodeToString(NativeMethods.MiniGPT4Error error_code)
    {
        return Marshal.PtrToStringAnsi(minigpt4_error_code_to_string((int)error_code));
    }
}