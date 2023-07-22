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
        //public int index { get; set; }
        //public bool ret { get; set; }
        //public string frame { get; set; }
        //public string poseframe { get; set; }

        public int cam_count { get; set; }
        public string pose_string { get; set; }
        public string img_0 { get; set; }
        public string img_1 { get; set; }
        public string img_2 { get; set; }
        public string img_3 { get; set; }
#pragma warning restore IDE1006 // ��� ��Ÿ��

        public byte[] Img1_decoded { get; set; }
        public byte[] Img2_decoded { get; set; }
        public byte[] Img3_decoded { get; set; }
        public byte[] Img4_decoded { get; set; }

        public Texture2D Img1_Texture { get; set; }
        public Texture2D Img2_Texture { get; set; }
        public Texture2D Img3_Texture { get; set; }
        public Texture2D Img4_Texture { get; set; }

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

        private int imgWidth = 640;
        private int imgHeight = 480;
        private int imgDepth = 3;
        public int posePointLength = GlobalSetting.POSE_RIGGINGPOINTS_COUNT; // �Ǽ��� 3�������̶� 33

        public bool stage1_InitComplete = false; // ������ �����ܰ� �Ϸ�, ������ �ڵ� EnqueueImageData ���� ���°� ����
        public bool stage2_AssignImgDics = false; // ������ �ڵ忡�� ImgDics�� ��ġ �� ���°� ����
        public bool stage3_SetTexture = false; // ������ �ڵ忡�� Unity_SetTexture ���� �� ����

        // =====

        /// <summary>
        /// ���ڿ� �̹����� byte[] �迭�� ��ȯ�Ѵ�.
        /// </summary>
        public void ConvertImgString_to_byteArray()
        {
            if (img_0 != null)
            {
                //Debug.Log("img_0 is not null");
                String_to_byteArray(0);
                //Debug.Log(Img1_decoded.Length);
            }

            if (img_1 != null)
            {
                //Debug.Log("img_1 is not null");
                String_to_byteArray(1);
                //Debug.Log(Img2_decoded.Length);
            }

            if (img_2 != null)
            {
                //Debug.Log("img_2 is not null");
                String_to_byteArray(2);
            }

            if (img_3 != null)
            {
                //Debug.Log("img_3 is not null");
                String_to_byteArray(3);
            }

            //Debug.Log(Img3_decoded); // Null
        }

        void String_to_byteArray(int index)
        {
            string _frame = "";

            if (index == 0) _frame = img_0;
            else if (index == 1) _frame = img_1;
            else if (index == 2) _frame = img_2;
            else if (index == 3) _frame = img_3;

            // Step 1: Decode the Base64 encoded data
            byte[] decodedData = Convert.FromBase64String(_frame);

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

                    if (index == 0) Img1_decoded = decompressedStream.ToArray();
                    else if (index == 1) Img2_decoded = decompressedStream.ToArray();
                    else if (index == 2) Img3_decoded = decompressedStream.ToArray();
                    else if (index == 3) Img4_decoded = decompressedStream.ToArray();
                    //Frame_decoded = decompressedStream.ToArray();
                    // TODO : ConvertData decompressedData�� �����Ҷ�, camIndex�� Ű�� �ؼ� dict�� �����ؾ� �Ѵ�.

                    //Debug.Log(decompressedStream.Length.ToString());
                }
            }
#pragma warning restore IDE0090 // 'new(...)' ���
#pragma warning restore IDE0063 // ������ 'using' �� ���
        }

        public bool isIndexCanProcess(int index)
        {
            //index *= 3; // 3�� �Ǽ��踸ŭ 
            //return true;

            // ���� �Ʒ� ���ǽĿ��� ��� �� ������ �ʿ���
            // �Ǽ� �ϳ��� ���� ���Ǹ� �����µ� �׷� ��Ȳ�� �ƴ�.
            if (index == 0 ||
                index == 11 ||
                index == 12 ||
                index == 13 ||
                index == 14 ||
                index == 15 ||
                index == 16 ||
                index == 23 ||
                index == 24 ||
                index == 25 ||
                index == 26 ||
                index == 27 ||
                index == 28)
                return true;

            return false;
        }

        public void ConvertPoseString_to_float3Array()
        {
            PoseArray = new Unity.Mathematics.float3[posePointLength];

            //string[] _points = poseframe.Split(',');
            string[] _points = pose_string.Split(','); // 99��

            float _x = 0;
            float _y = 0;
            float _z = 0;
            for (int i = 0; i < posePointLength - 7; i++)
            {
                //if (!isIndexCanProcess(i)) continue;
                // ���� ���� �ػ� 1280 * 720���� ���� ������ �ִ�.
                // ���� / 100�� �� ����
                // x�� : 0 ~ 12.8 (�� ���� ������ ���� ���� AI�� ������)
                // y�� : 0 ~ 7.2 (Ư�̻��� ���� ����)

                int cW = GlobalSetting.camWidth;
                int cH = GlobalSetting.camHeight;

                float x = float.Parse(_points[0 + (i * 3)]) * (1.48f / 3.2f);
                //float x = (float.Parse(_points[0 + (i * 3)]) - cW / 2) / 100;
                //float x = (float.Parse(_points[0 + (i * 3)]) - cW / 2) * (2.545f / 6.4f) / 100;

                float y = float.Parse(_points[1 + (i * 3)]) * (1 / 2.4f);
                //float y = (float.Parse(_points[1 + (i * 3)]) - cH / 2) / 100;
                //float y = (float.Parse(_points[1 + (i * 3)]) - cH / 2) * (1.42f / 3.6f) / 100;

                float z = float.Parse(_points[2 + (i * 3)]);
                //float z = float.Parse(_points[2 + (i * 3)]) / 300;
                //float z = 0;

                //Debug.Log($"{x}, {y}, {z}");

                if (i == 0)
                {
                    _x = x;
                }
                else if (i == 1)
                {
                    _y = y;
                }
                else if (i == 2)
                {
                    _z = z;
                }
                
                GlobalSetting.SetVectorMinMax(x, y, z);
                PoseArray[i] = new Unity.Mathematics.float3(x, y, z);
            }

            for (int i = 0; i < posePointLength; i++)
            {
                if (!isIndexCanProcess(i))
                {
                    PoseArray[i] = new Unity.Mathematics.float3(0, 0, 0);
                }
            }

            //PoseArray[33] = PoseArray[23];
            PoseArray[33] = (PoseArray[23] + PoseArray[24]) / 2 + new Unity.Mathematics.float3(0, 0.3f, 0);
            PoseArray[34] = PoseArray[33] + new Unity.Mathematics.float3(0, 0.2f, 0);
            PoseArray[35] = PoseArray[34] + new Unity.Mathematics.float3(0, 0.2f, 0);
            PoseArray[36] = PoseArray[35] + new Unity.Mathematics.float3(0, 0.2f, 0);
            PoseArray[37] = PoseArray[36] + new Unity.Mathematics.float3(-0.1f, 0.2f, 0); // Right shoulder
            PoseArray[38] = PoseArray[36] + new Unity.Mathematics.float3(0.1f, 0.2f, 0); // Left shoulder
            PoseArray[39] = PoseArray[36] + new Unity.Mathematics.float3(0, 0.22f, 0); // Neck
            //PoseArray[34] = (PoseArray[11] + PoseArray[12]) / 2;
            //Debug.Log(PoseArray[33]);

            // [33] = ([23] + [24]) / 2
            Debug.Log($"pos : {_x}, {_y}, {_z}");
        }

        // =====

        public void Unity_SetTexture()
        {
            Img1_Texture = Unity_CreateTexture2D(Img1_decoded, imgWidth, imgHeight, imgDepth);
            Img2_Texture = Unity_CreateTexture2D(Img2_decoded, imgWidth, imgHeight, imgDepth);
            Img3_Texture = Unity_CreateTexture2D(Img3_decoded, imgWidth, imgHeight, imgDepth);
            Img4_Texture = Unity_CreateTexture2D(Img4_decoded, imgWidth, imgHeight, imgDepth);
            //Frame_Texture = Unity_CreateTexture2D(imgWidth, imgHeight, imgDepth);
        }

        public bool IsTextureExisted()
        {
            for (int i = 0; i < cam_count; i++)
            {
                if (i == 0 && Img1_Texture == null)
                {
                    return false;
                }

                if (i == 1 && Img2_Texture == null)
                {
                    return false;
                }

                if (i == 2 && Img3_Texture == null)
                {
                    return false;
                }

                if (i == 3 && Img4_Texture == null)
                {
                    return false;
                }
            }

            return true;
        }

        Texture2D Unity_CreateTexture2D(byte[] frame_decoded, int tWidth, int tHeight, int tDepth)
        {
            if (frame_decoded == null) { return null; }

            //int tWidth = 640;
            //int tHeight = 480;
            //int tDepth = 3;

            // Step 3: Create a new Texture2D and load the decompressed data
            Texture2D recoveredTexture = new Texture2D(tWidth, tHeight, TextureFormat.RGB24, false);
            recoveredTexture.LoadRawTextureData(frame_decoded);
            recoveredTexture.Apply();

            return recoveredTexture;
        }

        public Texture2D CopyTexture(int index)
        {
            Texture2D targetTexture = null;

            if (index == 0) targetTexture = Img1_Texture;
            else if (index == 1) targetTexture = Img2_Texture;
            else if (index == 2) targetTexture = Img3_Texture;
            else if (index == 3) targetTexture = Img4_Texture;

            if (targetTexture == null)
            {
                throw new Exception($"{index} webcam texture is null");
            }

            Texture2D outputTexture = new Texture2D(targetTexture.width, targetTexture.height);

            Color[] pixels = targetTexture.GetPixels();

            outputTexture.SetPixels(pixels);
            outputTexture.Apply();

            return outputTexture;
        }
    }
}
