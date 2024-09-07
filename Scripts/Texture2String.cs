
using System;
using System.Text;
using UnityEngine;

namespace Sonic853.Texture2String
{
    public static class Texture2String
    {
        public static string ToText(this Texture2D texture, bool useLength = true, bool evenStorage = false) => RGBToText(texture, useLength, evenStorage);
        public static string RGBToText(this Texture2D texture, bool useLength = true, bool evenStorage = false) => ColorToText(texture, false, useLength, evenStorage);
        public static string RGBAToText(this Texture2D texture, bool useLength = true, bool evenStorage = false) => ColorToText(texture, true, useLength, evenStorage);
        public static string ColorToText(this Texture2D texture, bool useAlpha = false, bool useLength = true, bool evenStorage = false)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null!");
                return "";
            }

            // 获取 Texture2D 图片的所有像素颜色（使用 Color32 格式）
            var pixels = texture.GetPixels32();
            var pixelsLength = pixels.Length;

            var length = pixels[0].GetTextLength();
            if (useLength && length <= 0) return "";

            // 烦，调整Array长度的效率不如一开始就定好的长度
            var bytes = new byte[useLength ? length : useAlpha ? pixelsLength * 4 : pixelsLength * 3];

            var byteIndex = 0;

            // 计算间隔
            var interval = evenStorage ? (pixelsLength - 1) / (length / (useAlpha ? 4 : 3)) : 1;

            for (var i = useLength ? 1 : 0; i < pixelsLength; i += interval)
            {
                var pixel = pixels[i];
                var _bytes = useAlpha ? RGBAToBytes(pixel) : RGBToBytes(pixel);
                var _bytesLength = _bytes.Length;
                if (!useLength)
                {
                    Array.Copy(_bytes, 0, bytes, byteIndex, _bytesLength);
                    byteIndex += _bytesLength;
                    continue;
                }
                if (byteIndex > length) break;
                if (byteIndex + _bytesLength < length)
                {
                    Array.Copy(_bytes, 0, bytes, byteIndex, _bytesLength);
                    byteIndex += _bytesLength;
                    continue;
                }
                foreach (var b in _bytes)
                {
                    // 如果长度超过目标长度，停止复制
                    if (byteIndex >= length) break;

                    // 将字节添加到 bytes 数组中
                    bytes[byteIndex++] = b;
                }
                break;
            }
            return Encoding.UTF8.GetString(bytes);
        }
        public static string AlphaToText(this Texture2D texture, bool useLength = true, bool evenStorage = false)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null!");
                return "";
            }

            // 获取 Texture2D 图片的所有像素颜色（使用 Color32 格式）
            var pixels = texture.GetPixels32();

            var pixelsLength = pixels.Length;

            if (!useLength || useLength && pixelsLength <= 3)
            {
                var _bytes = new byte[pixelsLength];
                for (var i = 0; i < pixelsLength; i++)
                {
                    _bytes[i] = pixels[i].a;
                }
                return Encoding.UTF8.GetString(_bytes);
            }

            if (pixelsLength - 3 <= 0) return "";

            // 读取数据长度
            var length = pixels.GetTextLengthAlpha();
            if (length <= 0) return "";

            var bytes = new byte[length];
            var byteIndex = 0;
            var lengthMinusOne = length - 1;

            // 计算间隔
            var interval = evenStorage ? (pixelsLength - 3) / length : 1;

            for (var i = 3; i < pixelsLength; i += interval)
            {
                bytes[byteIndex] = pixels[i].a;
                if (byteIndex++ >= lengthMinusOne) break;
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
            return new Color32((byte)((length >> 16) & 0xFF), (byte)((length >> 8) & 0xFF), (byte)(length & 0xFF), 0x00);
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
        // public static string RGBToHex(Color32 color)
        // {
        //     return $"{color.r:x2}{color.g:x2}{color.b:x2}";
        // }
        // public static string RGBAToHex(Color32 color)
        // {
        //     return $"{color.a:x2}{color.r:x2}{color.g:x2}{color.b:x2}";
        // }
        // public static string AToHex(Color32 color)
        // {
        //     return $"{color.a:x2}";
        // }
        // public static string HexToString(string hex)
        // {
        //     // 计算字节数组的长度（每两个字符表示一个字节）
        //     var byteCount = hex.Length / 2;
        //     var bytes = new byte[0];

        //     // 将每两个十六进制字符转换为一个字节
        //     for (var i = 0; i < byteCount; i++)
        //     {
        //         var substring = hex.Substring(i * 2, 2);
        //         if (substring.Equals("00")) continue;
        //         UdonArrayPlus.Add(ref bytes, Convert.ToByte(substring, 16));
        //     }

        //     // 将字节数组转换为字符串（使用 UTF-8 编码）
        //     return Encoding.UTF8.GetString(bytes);
        // }
        // public static string StringToHex(string input)
        // {
        //     // 将字符串转换为字节数组（使用 UTF-8 编码）
        //     var bytes = Encoding.UTF8.GetBytes(input);
        //     var hexString = "";
        //     foreach (var b in bytes)
        //     {
        //         hexString += $"{b:x2}";
        //     }

        //     return hexString;
        // }
    }
}
