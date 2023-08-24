using System;
using OpenCvSharp;

namespace minigpt4.net;

public class OpenCVSharpImageProcessing : IImageProcessing
{
    public NativeMethods.MiniGPT4Image LoadImage(nint ctx, string path)
    {
        int IMAGE_RESIZE = 224;
        Mat m = Cv2.ImRead(path, ImreadModes.Color);
        Cv2.CvtColor(m, m, ColorConversionCodes.BGR2RGB);

        m = new Mat(m.Rows, m.Cols, MatType.CV_8UC(m.Channels()), m.Data, 0);

        Mat m2 = new Mat();
        Cv2.Resize(m, m2, new Size(IMAGE_RESIZE, IMAGE_RESIZE), 0, 0, InterpolationFlags.Linear);
        Scalar mean = new Scalar(0.48145466, 0.4578275, 0.40821073);
        Scalar std = new Scalar(0.26862954, 0.26130258, 0.27577711);

        m2.ConvertTo(m2, MatType.CV_32F, 1.0 / 255.0);
        Cv2.Subtract(m2, mean, m2);
        Cv2.Divide(m2, std, m2);

        Mat[] channels = new Mat[3];
        Cv2.Split(m2, out channels);
        Mat mix_channel = new Mat();
        Cv2.HConcat(new[] { channels[0].Reshape(1, 1), channels[1].Reshape(1, 1), channels[2].Reshape(1, 1) }, mix_channel);

        m.Release();
        return new NativeMethods.MiniGPT4Image
        {
            data = mix_channel.Data,
            height = mix_channel.Rows,
            width = mix_channel.Cols,
            channels = mix_channel.Channels(),
            format = NativeMethods.MiniGPT4ImageFormat.MINIGPT4_IMAGE_FORMAT_F32,
        };
    }
}
