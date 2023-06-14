using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using WebSocketSharp;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace Data
{
    public class ImageData
    {
#pragma warning disable IDE1006 // ��� ��Ÿ��
        public int index { get; set; }
        public bool ret { get; set; }
        public string frame { get; set; }
#pragma warning restore IDE1006 // ��� ��Ÿ��

        /// <summary>
        /// �����Ͽ��� ���� ���ڿ� �̹��� (frame)�� byte[] �迭�� �ٲ� ��
        /// </summary>
        public byte[] Frame_decoded { get; set; }

        public Texture2D Frame_Texture { get; set; }

        private int imgWidth = 640;
        private int imgHeight = 480;
        private int imgDepth = 3;

        /// <summary>
        /// ���ڿ� �̹����� byte[] �迭�� ��ȯ�Ѵ�.
        /// </summary>
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

        public void Unity_SetTexture()
        {
            Frame_Texture = Unity_CreateTexture2D(imgWidth, imgHeight, imgDepth);
        }

        Texture2D Unity_CreateTexture2D(int tWidth, int tHeight, int tDepth)
        {
            if (Frame_decoded == null) { return null; }

            //int tWidth = 640;
            //int tHeight = 480;
            //int tDepth = 3;

            // Step 3: Create a new Texture2D and load the decompressed data
            Texture2D recoveredTexture = new Texture2D(tWidth, tHeight, TextureFormat.RGB24, false);
            recoveredTexture.LoadRawTextureData(Frame_decoded);
            recoveredTexture.Apply();

            return recoveredTexture;
        }

        public ImageData DeepCopy()
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, this);
                memoryStream.Position = 0;

                return (ImageData) formatter.Deserialize(memoryStream);
            }
        }
    }
}
