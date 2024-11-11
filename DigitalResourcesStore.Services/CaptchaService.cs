using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public interface ICaptchaService
{
    string GenerateCaptchaCode(int length);
    byte[] GenerateCaptchaImage(string captchaCode);

    bool ValidateCaptcha(string userInput);
}
public class CaptchaService : ICaptchaService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CaptchaSessionKey = "CaptchaCode";

    public CaptchaService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Generate a random captcha code and store it in the session
    public string GenerateCaptchaCode(int length = 6)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var captchaCode = new string(Enumerable.Repeat(chars, length)
                                               .Select(s => s[random.Next(s.Length)]).ToArray());

        _httpContextAccessor.HttpContext.Session.SetString(CaptchaSessionKey, captchaCode);
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

    // Validate the captcha code
    public bool ValidateCaptcha(string userInput)
    {
        var storedCaptchaCode = _httpContextAccessor.HttpContext.Session.GetString(CaptchaSessionKey);
        return storedCaptchaCode != null && storedCaptchaCode.Equals(userInput, StringComparison.OrdinalIgnoreCase);
    }
}