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
#pragma warning disable IDE1006 // 명명 스타일

        public int cam_count { get; set; }
        public string pose_string { get; set; }
        public string pose0 { get; set; }
        public string pose1 { get; set; }
        public string pose2 { get; set; }
        public string pose3 { get; set; }
        public string seg0 { get; set; }
        public string seg1 { get; set; }
        public string seg2 { get; set; }
        public string seg3 { get; set; }
        public List<float> pCenter0 { get; set; }
        public List<float> pCenter1 { get; set; }
        public List<float> pCenter2 { get; set; }
        public List<float> pCenter3 { get; set; }
#pragma warning restore IDE1006 // 명명 스타일

        public byte[] Img1_decoded { get; set; }
        public byte[] Img2_decoded { get; set; }
        public byte[] Img3_decoded { get; set; }
        public byte[] Img4_decoded { get; set; }

        public byte[,,] Img1_3darray { get; set; }
        public byte[,,] Img2_3darray { get; set; }
        public byte[,,] Img3_3darray { get; set; }
        public byte[,,] Img4_3darray { get; set; }

        public Texture2D Img1_Texture { get; set; }
        public Texture2D Img2_Texture { get; set; }
        public Texture2D Img3_Texture { get; set; }
        public Texture2D Img4_Texture { get; set; }

        public int[] pose_Center0 { get; set; }
        public int[] pose_Center1 { get; set; }
        public int[] pose_Center2 { get; set; }
        public int[] pose_Center3 { get; set; }

        /// <summary>
        /// 웹소켓에서 받은 문자열 이미지 (frame)를 byte[] 배열로 바꾼 것
        /// </summary>
        public byte[] Frame_decoded { get; set; }

        /// <summary>
        /// Frame_decoded byte[] 배열을 Texture2D로 변환한 것.
        /// </summary>
        public Texture2D Frame_Texture { get; set; }

        /// <summary>
        /// poseframe의 string 값을 float3[] 배열로 변환한 것
        /// </summary>
        public Unity.Mathematics.float3[] PoseArray { get; set; }
        public Unity.Mathematics.float3[] PoseArray_0 { get; set; }
        public Unity.Mathematics.float3[] PoseArray_1 { get; set; }
        public Unity.Mathematics.float3[] PoseArray_2 { get; set; }
        public Unity.Mathematics.float3[] PoseArray_3 { get; set; }


        private int imgWidth = 640;
        private int imgHeight = 480;
        private int imgDepth = 3;
        public int posePointLength = GlobalSetting.POSE_RIGGINGPOINTS_COUNT; // 실수값 3차원축이라 33

        public bool stage1_InitComplete = false; // 웹소켓 생성단계 완료, 관리자 코드 EnqueueImageData 이전 상태값 변경
        public bool stage2_AssignImgDics = false; // 관리자 코드에서 ImgDics에 배치 후 상태값 변경
        public bool stage3_SetTexture = false; // 관리자 코드에서 Unity_SetTexture 실행 후 변경

        // =====

        /// <summary>
        /// 문자열 이미지를 byte[] 배열로 변환한다.
        /// </summary>
        public void ConvertImgString_to_byteArray()
        {
            if (seg0 != null)
            {
                //Debug.Log("img_0 is not null");
                String_to_byteArray(0);
                //Debug.Log(Img1_decoded.Length);
            }

            if (seg1 != null)
            {
                //Debug.Log("img_1 is not null");
                String_to_byteArray(1);
                //Debug.Log(Img2_decoded.Length);
            }

            if (seg2 != null)
            {
                //Debug.Log("img_2 is not null");
                String_to_byteArray(2);
            }

            if (seg3 != null)
            {
                //Debug.Log("img_3 is not null");
                String_to_byteArray(3);
            }

            //Debug.Log(Img3_decoded); // Null
        }

        void String_to_byteArray(int index)
        {
            string _frame = "";

            if (index == 0) _frame = seg0;
            else if (index == 1) _frame = seg1;
            else if (index == 2) _frame = seg2;
            else if (index == 3) _frame = seg3;

            // Step 1: Decode the Base64 encoded data
            byte[] decodedData = Convert.FromBase64String(_frame);

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

                    if (index == 0)
                    {
                        Img1_decoded = decompressedStream.ToArray();
                        //Img1_3darray = ConvertTo3D(Img1_decoded, imgWidth, imgHeight, imgDepth);
                    }
                    else if (index == 1)
                    {
                        Img2_decoded = decompressedStream.ToArray();
                        //Img2_3darray = ConvertTo3D(Img2_decoded, imgWidth, imgHeight, imgDepth);
                    }
                    else if (index == 2)
                    {
                        Img3_decoded = decompressedStream.ToArray();
                        //Img3_3darray = ConvertTo3D(Img3_decoded, imgWidth, imgHeight, imgDepth);
                    }
                    else if (index == 3)
                    {
                        Img4_decoded = decompressedStream.ToArray();
                        //Img4_3darray = ConvertTo3D(Img4_decoded, imgWidth, imgHeight, imgDepth);
                    }
                    //Frame_decoded = decompressedStream.ToArray();
                    // TODO : ConvertData decompressedData는 저장할때, camIndex를 키로 해서 dict에 저장해야 한다.

                    //Debug.Log(decompressedStream.Length.ToString());
                }
            }
#pragma warning restore IDE0090 // 'new(...)' 사용
#pragma warning restore IDE0063 // 간단한 'using' 문 사용
        }

        /// <summary>
        /// 이 포즈 인덱스가 연산 대상인가?
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool isIndexCanProcess(int index)
        {
            // 지금 아래 조건식에서 얘는 좀 보정이 필요함
            // 실수 하나에 대한 조건만 따지는데 그런 상황이 아님.
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

        public void ConvertPtf3(int index)
        {
            Unity.Mathematics.float3[] array = new Unity.Mathematics.float3[posePointLength];
            string[] _points = null;

            if (index == 0)
            {
                _points = pose_string.Split(',');
            }
            else if  (index == 1)
            {
                if (pose0 == null) { return; }
                _points = pose0.Split(',');
            }
            else if (index == 2)
            {
                if (pose1 == null) { return; }
                _points = pose1.Split(',');
            }
            else if (index == 3)
            {
                if (pose2 == null) { return; }
                _points = pose2.Split(',');
            }
            else if (index == 4)
            {
                if (pose3 == null) { return; }
                _points = pose3.Split(',');
            }

            if (_points == null) { return; }

            int cW = GlobalSetting.camWidth;
            int cH = GlobalSetting.camHeight;
            for (int i = 0; i < posePointLength - 7; i++)
            {
                float x = float.Parse(_points[0 + (i * 3)]) * (1.48f / 3.2f);
                float y = float.Parse(_points[1 + (i * 3)]) * (1 / 2.4f);
                float z = float.Parse(_points[2 + (i * 3)]);

                if (index == 0)
                {
                    GlobalSetting.SetVectorMinMax(x, y, z);
                }

                if (isIndexCanProcess(i))
                {
                    array[i] = new Unity.Mathematics.float3(x, y, z);
                }
                else
                {
                    array[i] = new Unity.Mathematics.float3(0, 0, 0);
                }
            }

            array[33] = (array[23] + array[24]) / 2 + new Unity.Mathematics.float3(0, 0.3f, 0);
            array[34] = array[33] + new Unity.Mathematics.float3(0, 0.2f, 0);
            array[35] = array[34] + new Unity.Mathematics.float3(0, 0.2f, 0);
            array[36] = array[35] + new Unity.Mathematics.float3(0, 0.2f, 0);
            array[37] = array[36] + new Unity.Mathematics.float3(-0.1f, 0.2f, 0); // Right shoulder
            array[38] = array[36] + new Unity.Mathematics.float3(0.1f, 0.2f, 0); // Left shoulder
            array[39] = array[36] + new Unity.Mathematics.float3(0, 0.22f, 0); // Neck
            //if (index == 0)
            //{
            //}

            if (index == 0)
            {
                PoseArray = array;
            }
            else if (index == 1)
            {
                PoseArray_0 = array;
            }
            else if (index == 2)
            {
                PoseArray_1 = array;
            }
            else if (index == 3)
            {
                PoseArray_2 = array;
            }
            else if (index == 4)
            {
                PoseArray_3 = array;
            }
        }

        public void ConvertPoseString_to_float3Array()
        {
            ConvertPtf3(0); // PoseArray
            ConvertPtf3(1); // Pose_0
            ConvertPtf3(2); // Pose_1
            ConvertPtf3(3); // Pose_2
            ConvertPtf3(4); // Pose_3
        }

        public void ConvertPoseCenter_to_intArray()
        {
            pose_Center0 = new int[2];
            pose_Center1 = new int[2];
            pose_Center2 = new int[2];
            pose_Center3 = new int[2];

            int width = 1920;
            int height = 1080;

            if (pCenter0 != null)
            {
                pose_Center0[0] = (int)(pCenter0[0] * width);
                pose_Center0[1] = (int)(pCenter0[1] * height);
            }

            if (pCenter1 != null)
            {
                pose_Center1[0] = (int)(pCenter1[0] * width);
                pose_Center1[1] = (int)(pCenter1[1] * height);
            }

            if (pCenter2 != null)
            {
                pose_Center2[0] = (int)(pCenter2[0] * width);
                pose_Center2[1] = (int)(pCenter2[1] * height);
            }

            if (pCenter3 != null)
            {
                pose_Center3[0] = (int)(pCenter3[0] * width);
                pose_Center3[1] = (int)(pCenter3[1] * height);
            }
        }

        public byte[,,] ConvertTo3D(byte[] flatArray, int width, int height, int depth)
        {
            byte[,,] result = new byte[width, height, depth];
            int i = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        result[x, y, z] = flatArray[i];
                        i++;
                    }
                }
            }

            return result;
        }


        // =====

        public void Unity_SetTexture(int tstX, int tstY, int tstWidth, int tstHeight)
        {
            Img1_Texture = Unity_CreateTexture2D(Img1_decoded, imgWidth, imgHeight, imgDepth);
            Img2_Texture = Unity_CreateTexture2D(Img2_decoded, imgWidth, imgHeight, imgDepth);
            Img3_Texture = Unity_CreateTexture2D(Img3_decoded, imgWidth, imgHeight, imgDepth);
            Img4_Texture = Unity_CreateTexture2D(Img4_decoded, imgWidth, imgHeight, imgDepth);
            //Frame_Texture = Unity_CreateTexture2D(imgWidth, imgHeight, imgDepth);

            if (Img1_decoded != null)
            {
                int offsetX = (int)(pCenter0[0] * imgWidth);
                int offsetY = (int)(pCenter0[1] * imgHeight);
                Img1_Texture = TranslateTexture(Img1_Texture, offsetX + tstX, offsetY + tstY);
                //Img1_Texture = ResizeTexture(Img1_Texture, tstWidth, tstHeight);
            }

            if (Img2_decoded != null)
            {
                int offsetX = (int)(pCenter1[0] * imgWidth);
                int offsetY = (int)(pCenter1[1] * imgHeight);
                Img2_Texture = TranslateTexture(Img2_Texture, offsetX + tstX, offsetY + tstY);
            }

            if (Img3_decoded != null)
            {
                int offsetX = (int)(pCenter2[0] * imgWidth);
                int offsetY = (int)(pCenter2[1] * imgHeight);
                Img3_Texture = TranslateTexture(Img3_Texture, offsetX + tstX, offsetY + tstY);
            } 

            if (Img4_decoded != null)
            {
                int offsetX = (int)(pCenter3[0] * imgWidth);
                int offsetY = (int)(pCenter3[1] * imgHeight);
                Img4_Texture = TranslateTexture(Img4_Texture, offsetX + tstX, offsetY + tstY);
            }
        }

        public Texture2D TranslateTexture(Texture2D source, int offsetX, int offsetY)
        {
            int width = source.width;
            int height = source.height;

            Texture2D result = new Texture2D(width, height);

            // Get the pixels from the source texture
            Color[] pixels = source.GetPixels();

            // Copy the pixels to the result texture
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int newX = x + offsetX;
                    int newY = y + offsetY;

                    // Check if the new position is inside the texture
                    if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                    {
                        result.SetPixel(newX, newY, pixels[y * width + x]);
                    }
                }
            }

            // Apply the changes to the result texture
            result.Apply();

            return result;
        }

        public Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
        {
            // Create a new texture
            Texture2D result = new Texture2D(newWidth, newHeight);

            // Copy the pixels from the source texture to the new texture
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    // Calculate the normalized coordinates
                    float u = (float)x / (newWidth - 1);
                    float v = (float)y / (newHeight - 1);

                    // Get the pixel color from the source texture
                    Color color = source.GetPixelBilinear(u, v);

                    // Set the pixel color in the new texture
                    result.SetPixel(x, y, color);
                }
            }

            // Apply the changes to the texture
            result.Apply();

            return result;
        }

        //public Texture2D CompressTexture(Texture2D source)
        //{
        //    int width = source.width;
        //    int height = source.height;

        //    Texture2D result = new Texture2D(width, height);

        //    float centerX = 0.5f;
        //    float centerY = 0.5f;

        //    // Copy the pixels to the result texture
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            // Calculate the normalized coordinates
        //            float u = (float)x / (width - 1);
        //            float v = (float)y / (height - 1);

        //            // Calculate the new coordinates
        //            float newU = centerX + (u - centerX) / Mathf.Abs(u - centerX);
        //            float newV = centerY + (v - centerY) / Mathf.Abs(v - centerY);

        //            // Get the pixel value at the new coordinates
        //            Color color = source.GetPixelBilinear(newU, newV);

        //            // Set the pixel value
        //            result.SetPixel(x, y, color);
        //        }
        //    }

        //    // Apply the changes to the result texture
        //    result.Apply();

        //    return result;
        //}

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

        //Texture2D Unity_CreateTexture2D(byte[,,] frame_decoded, int tWidth, int tHeight, int tDepth)
        //{
        //    if (frame_decoded == null) { return null; }

        //    //int tWidth = 640;
        //    //int tHeight = 480;
        //    //int tDepth = 3;

        //    // Step 3: Create a new Texture2D and load the decompressed data
        //    Texture2D recoveredTexture = new Texture2D(tWidth, tHeight, TextureFormat.RGB24, false);
        //    recoveredTexture.LoadRawTextureData(frame_decoded);
        //    recoveredTexture.Apply();

        //    return recoveredTexture;
        //}

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
