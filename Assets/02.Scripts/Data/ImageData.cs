using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using WebSocketSharp;

namespace Data
{
    public class ImageData
    {
#pragma warning disable IDE1006 // ��� ��Ÿ��
        public int index { get; set; }
        public bool ret { get; set; }
        public string frame { get; set; }
#pragma warning restore IDE1006 // ��� ��Ÿ��

        public byte[] Frame_decoded { get; set; }

        public void ConvertImgString_to_byteArray()
        {
            // Step 1: Decode the Base64 encoded data
            byte[] decodedData = Convert.FromBase64String(frame);

            // Step 2: Decompress the decoded data using GZip
#pragma warning disable IDE0063 // ������ 'using' �� ���
#pragma warning disable IDE0090 // 'new(...)' ���
            using (MemoryStream compressedStream = new MemoryStream(decodedData))
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(decompressedStream);
                    }
                    Frame_decoded = decompressedStream.ToArray();
                    // TODO : ConvertData decompressedData�� �����Ҷ�, camIndex�� Ű�� �ؼ� dict�� �����ؾ� �Ѵ�.

                    //Debug.Log(decompressedStream.Length.ToString());
                }
            }
#pragma warning restore IDE0090 // 'new(...)' ���
#pragma warning restore IDE0063 // ������ 'using' �� ���
        }
    }
}
