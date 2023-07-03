using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using WebSocketSharp;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Environment;

namespace Data
{
    public class ImageData
    {
#pragma warning disable IDE1006 // ��� ��Ÿ��
        public int index { get; set; }
        public bool ret { get; set; }
        public string frame { get; set; }
        public string poseframe { get; set; }
#pragma warning restore IDE1006 // ��� ��Ÿ��

        /// <summary>
        /// �����Ͽ��� ���� ���ڿ� �̹��� (frame)�� byte[] �迭�� �ٲ� ��
        /// </summary>
        public byte[] Frame_decoded { get; set; }

        /// <summary>
        /// Frame_decoded byte[] �迭�� Texture2D�� ��ȯ�� ��.
        /// </summary>
        public Texture2D Frame_Texture { get; set; }

        /// <summary>
        /// poseframe�� string ���� float3[] �迭�� ��ȯ�� ��
        /// </summary>
        public Unity.Mathematics.float3[] PoseArray { get; set; }

        private int imgWidth = 1280;
        private int imgHeight = 720;
        private int imgDepth = 3;
        public int posePointLength = GlobalSetting.POSE_RIGGINGPOINTS_COUNT;

        public bool stage1_InitComplete = false; // ������ �����ܰ� �Ϸ�, ������ �ڵ� EnqueueImageData ���� ���°� ����
        public bool stage2_AssignImgDics = false; // ������ �ڵ忡�� ImgDics�� ��ġ �� ���°� ����
        public bool stage3_SetTexture = false; // ������ �ڵ忡�� Unity_SetTexture ���� �� ����


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

        public void ConvertPoseString_to_float3Array()
        {
            PoseArray = new Unity.Mathematics.float3[posePointLength];

            string[] _points = poseframe.Split(',');

            for (int i = 0; i < posePointLength; i++)
            {
                float x = float.Parse(_points[0 + (i * 3)]) / 100;
                float y = float.Parse(_points[1 + (i * 3)]) / 100;
                float z = float.Parse(_points[2 + (i * 3)]) / 300;
                PoseArray[i] = new Unity.Mathematics.float3(x, y, z);
            }
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

        public Texture2D CopyTexture()
        {
            if (Frame_Texture == null)
            {
                throw new Exception($"{index} webcam texture is null");
            }

            Texture2D outputTexture = new Texture2D(Frame_Texture.width, Frame_Texture.height);

            Color[] pixels = Frame_Texture.GetPixels();

            outputTexture.SetPixels(pixels);
            outputTexture.Apply();

            return outputTexture;
        }
    }
}
