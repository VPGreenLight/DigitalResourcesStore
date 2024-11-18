using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public interface ICaptchaService
{
    string GenerateCaptchaCode(int length);
    byte[] GenerateCaptchaImage(string captchaCode);

}
public class CaptchaService : ICaptchaService
{
    private const string CaptchaSessionKey = "CaptchaCode";

    // Generate a random captcha code and store it in the session
    public string GenerateCaptchaCode(int length = 6)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var captchaCode = new string(Enumerable.Repeat(chars, length)
                                               .Select(s => s[random.Next(s.Length)]).ToArray());
        return captchaCode;
    }

    // Generate a captcha image based on the code
    public byte[] GenerateCaptchaImage(string captchaCode)
    {
        using var bitmap = new Bitmap(120, 50);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        var font = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Pixel);
        graphics.DrawString(captchaCode, font, Brushes.Black, new PointF(10, 10));

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}