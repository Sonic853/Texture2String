
using System;
using System.Text;
using UdonLab;
using UnityEngine;

namespace Sonic853.Texture2String
{
    public static class Texture2String
    {
        public static string ToText(this Texture2D texture, bool useLength = true) => RGBToText(texture, useLength);
        public static string RGBToText(this Texture2D texture, bool useLength = true) => ColorToText(texture, false, useLength);
        public static string RGBAToText(this Texture2D texture, bool useLength = true) => ColorToText(texture, true, useLength);
        public static string ColorToText(this Texture2D texture, bool useAlpha = false, bool useLength = true)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null!");
                return "";
            }

            // 获取 Texture2D 图片的所有像素颜色（使用 Color32 格式）
            var pixels = texture.GetPixels32();

            var bytes = new byte[0];

            int length = -1;

            foreach (var pixel in pixels)
            {
                var _bytes = useAlpha ? RGBAToBytes(pixel) : RGBToBytes(pixel);
                if (!useLength)
                {
                    UdonArrayPlus.AddRange(ref bytes, _bytes);
                    continue;
                }
                if (length == -1)
                {
                    length = pixel.GetTextLength();
                    if (length <= 0) return "";
                    continue;
                }
                var bytesLength = bytes.Length;
                if (bytesLength >= length) break;
                if (bytesLength + _bytes.Length < length)
                {
                    UdonArrayPlus.AddRange(ref bytes, _bytes);
                    continue;
                }
                foreach (var b in _bytes)
                {
                    if (b == 0x00) continue;
                    if (bytes.Length >= length) break;
                    UdonArrayPlus.Add(ref bytes, b);
                }
                break;
            }
            return Encoding.UTF8.GetString(bytes);
        }
        public static string AlphaToText(this Texture2D texture, bool useLength = true)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null!");
                return "";
            }

            // 获取 Texture2D 图片的所有像素颜色（使用 Color32 格式）
            var pixels = texture.GetPixels32();

            var pixelsLength = pixels.Length;

            var bytes = new byte[0];

            if (!useLength || useLength && pixelsLength <= 3)
            {
                foreach (var pixel in pixels)
                {
                    if (pixel.a == 0x00) continue;
                    UdonArrayPlus.Add(ref bytes, pixel.a);
                }
                return Encoding.UTF8.GetString(bytes);
            }

            var length = new Color32[] { pixels[0], pixels[1], pixels[2] }.GetTextLengthAlpha();

            var remainingPixelsLength = pixels.Length - 3;
            if (length <= 0 || remainingPixelsLength <= 0) return "";

            var remainingPixels = new Color32[remainingPixelsLength];
            Array.Copy(pixels, 3, remainingPixels, 0, remainingPixelsLength);

            foreach (var pixel in remainingPixels)
            {
                if (pixel.a == 0x00) continue;
                if (bytes.Length >= length) break;
                UdonArrayPlus.Add(ref bytes, pixel.a);
            }
            return Encoding.UTF8.GetString(bytes);
        }
        public static byte[] RGBToBytes(Color32 color)
        {
            return new byte[]{
                color.r,
                color.g,
                color.b
            };
        }
        public static byte[] RGBAToBytes(Color32 color)
        {
            return new byte[]{
                color.a,
                color.r,
                color.g,
                color.b
            };
        }
        public static byte AToByte(Color32 color) => color.a;
        // 如果存在 rgb 或 argb，只需使用 rgb 存为长度
        public static int GetTextLength(this Color32 color)
        {
            return (color.r << 16) | (color.g << 8) | color.b;
        }
        public static Color32 LengthToColor(int length)
        {
            return new Color32((byte)((length >> 16) & 0xFF), (byte)((length >> 8) & 0xFF), (byte)(length & 0xFF), 255);
        }
        public static Color32 LengthToColor(Color32 color, int length)
        {
            return new Color32((byte)((length >> 16) & 0xFF), (byte)((length >> 8) & 0xFF), (byte)(length & 0xFF), color.a);
        }
        public static Color32[] LengthToAlpha(Color32[] colors, int length)
        {
            colors[0] = new Color32(colors[0].r, colors[0].g, colors[0].b, (byte)((length >> 16) & 0xFF));
            colors[1] = new Color32(colors[1].r, colors[1].g, colors[1].b, (byte)((length >> 8) & 0xFF));
            colors[2] = new Color32(colors[2].r, colors[2].g, colors[2].b, (byte)(length & 0xFF));

            return colors;
        }
        public static Color32[] LengthToAlpha(ref Color32[] colors, int length)
        {
            return colors = LengthToAlpha(colors, length);
        }
        // 如果存在 alpha 则使用前三个的 alpha 值
        public static int GetTextLengthAlpha(this Color32[] colors)
        {
            return (colors[0].a << 16) | (colors[1].a << 8) | colors[2].a;
        }
        public static string RGBToHex(Color32 color)
        {
            return $"{color.r:x2}{color.g:x2}{color.b:x2}";
        }
        public static string RGBAToHex(Color32 color)
        {
            return $"{color.a:x2}{color.r:x2}{color.g:x2}{color.b:x2}";
        }
        public static string AToHex(Color32 color)
        {
            return $"{color.a:x2}";
        }
        public static string HexToString(string hex)
        {
            // 计算字节数组的长度（每两个字符表示一个字节）
            var byteCount = hex.Length / 2;
            var bytes = new byte[0];

            // 将每两个十六进制字符转换为一个字节
            for (var i = 0; i < byteCount; i++)
            {
                var substring = hex.Substring(i * 2, 2);
                if (substring.Equals("00")) continue;
                UdonArrayPlus.Add(ref bytes, Convert.ToByte(substring, 16));
            }

            // 将字节数组转换为字符串（使用 UTF-8 编码）
            return Encoding.UTF8.GetString(bytes);
        }
        public static string StringToHex(string input)
        {
            // 将字符串转换为字节数组（使用 UTF-8 编码）
            var bytes = Encoding.UTF8.GetBytes(input);
            var hexString = "";
            foreach (var b in bytes)
            {
                hexString += $"{b:x2}";
            }

            return hexString;
        }
    }
}
