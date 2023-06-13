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
#pragma warning disable IDE1006 // 명명 스타일
        public int index { get; set; }
        public bool ret { get; set; }
        public string frame { get; set; }
#pragma warning restore IDE1006 // 명명 스타일

        public byte[] Frame_decoded { get; set; }

        public void ConvertImgString_to_byteArray()
        {
            // Step 1: Decode the Base64 encoded data
            byte[] decodedData = Convert.FromBase64String(frame);

            // Step 2: Decompress the decoded data using GZip
#pragma warning disable IDE0063 // 간단한 'using' 문 사용
#pragma warning disable IDE0090 // 'new(...)' 사용
            using (MemoryStream compressedStream = new MemoryStream(decodedData))
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(decompressedStream);
                    }
                    Frame_decoded = decompressedStream.ToArray();
                    // TODO : ConvertData decompressedData는 저장할때, camIndex를 키로 해서 dict에 저장해야 한다.

                    //Debug.Log(decompressedStream.Length.ToString());
                }
            }
#pragma warning restore IDE0090 // 'new(...)' 사용
#pragma warning restore IDE0063 // 간단한 'using' 문 사용
        }
    }
}
