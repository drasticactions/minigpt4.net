// <copyright file="EmguCVImageProcessing.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace minigpt4.net;

public class EmguCVImageProcessing : IImageProcessing
{
    public NativeMethods.MiniGPT4Embedding EncodeImage(nint ctx, NativeMethods.MiniGPT4Image image)
    {
        NativeMethods.EncodeImage(ctx, ref image, out var embedding, IntPtr.Zero).ThrowIfError();
        return embedding;
    }

    public NativeMethods.MiniGPT4Image LoadImage(nint ctx, string path)
    {
        int IMAGE_RESIZE = 224;
        Mat m = CvInvoke.Imread(path, ImreadModes.Color);
        CvInvoke.CvtColor(m, m, ColorConversion.Bgr2Rgb);

        m = new Mat(m.Rows, m.Cols, DepthType.Cv8U, m.NumberOfChannels, m.DataPointer, 0);

        Mat m2 = new Mat();
        CvInvoke.Resize(m, m2, new Size(IMAGE_RESIZE, IMAGE_RESIZE), 0, 0, Inter.Linear);
        MCvScalar mean = new MCvScalar(0.48145466, 0.4578275, 0.40821073);
        MCvScalar std = new MCvScalar(0.26862954, 0.26130258, 0.27577711);

        m2.ConvertTo(m2, DepthType.Cv32F, 1.0 / 255.0);
        CvInvoke.MeanStdDev(m2, ref mean, ref std);

        VectorOfMat channels = new VectorOfMat();
        CvInvoke.Split(m2, channels);
        InputOutputArray mix_channel = channels.GetInputOutputArray();

        CvInvoke.HConcat(new Mat[] { mix_channel.GetMat(0).Reshape(1, 1), mix_channel.GetMat(1).Reshape(1, 1), mix_channel.GetMat(2).Reshape(1, 1) }, m2);
        m.Dispose();
        return new NativeMethods.MiniGPT4Image
        {
            data = m2.DataPointer,
            height = m2.Rows,
            width = m2.Cols,
            channels = m2.NumberOfChannels,
            format = NativeMethods.MiniGPT4ImageFormat.MINIGPT4_IMAGE_FORMAT_F32,
        };
    }
}
