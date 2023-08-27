using Data;
using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static PaintIn3D.P3dWindow; 

namespace Management
{
    public class MainManager : MonoBehaviour
    {
        #region Instance
        private static MainManager instance;

        public static MainManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<MainManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("MainManager");
                        instance = obj.AddComponent<MainManager>();
                    }
                }
                return instance;
            }
        }
        #endregion

        /// <summary>
        /// P3���� ������ �̹����� �޾ƿ���, \n
        /// �ϼ��� �ؽ�ó�� �����ϱ� ���� ������ �ν��Ͻ�
        /// </summary>
        public WebSocket_.P4_Websocket websocket;


        [Header("Decal reference structure")]
        /// <summary>
        /// ī�޶� ��Į ó�� ���� ����ü
        /// </summary>
        public List<DecalContainer> decalContainer;


        [Header("P3d resources")]
        /// <summary>
        /// �Է� �̺�Ʈ�� �߻���Ű�� ��Ʈ Ŭ����
        /// </summary>
        public CW.Common.CwInputManager cwInputManager;
        public P3dPaintable _paintable;
        public PoseDirector poseDirector;
        private Material[] _mats;
        private P3dPaintableTexture[] _texture;

        #region Start

        private void Initialize()
        {
            Debug.Log("Manager initialized.");
        }

        private void Start()
        {
            if (instance == null)
            {
                Instance.Initialize();
            }

            if (ImgQueue == null)
            {
                ImgQueue = new Queue<Data.ImageData>();
            }

            if (TextureInImgList == null)
            {
                TextureInImgList = new List<Data.ImageData>();
            }

            // ������ ���� ����
            if (websocket == null)
            {
                GameObject obj = new GameObject("Websocket");
                obj.transform.parent = this.transform;
                websocket = obj.AddComponent<WebSocket_.P4_Websocket>();
            }

        }

        #endregion Start

        [Header("Test Parameters")]
        public int tstX;
        public int tstY;
        public int tstWidth;
        public int tstHeight;
        public Vector3 tstImgScale1;

        public Vector2 customClickVector;
        
        #region Update

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("1");
                Transform cam = decalContainer[1].originCamera.transform;
                Vector3 cur = cam.localPosition;
                cam.localPosition = new Vector3(cur.x, cur.y + 0.1f, cur.z);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Transform cam = decalContainer[1].originCamera.transform;
                Vector3 cur = cam.localPosition;
                cam.localPosition = new Vector3(cur.x, cur.y - 0.1f, cur.z);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Transform cam = decalContainer[1].originCamera.transform;
                Vector3 cur = cam.localPosition;
                cam.localPosition = new Vector3(cur.x - 0.1f, cur.y, cur.z);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Transform cam = decalContainer[1].originCamera.transform;
                Vector3 cur = cam.localPosition;
                cam.localPosition = new Vector3(cur.x + 0.1f, cur.y, cur.z);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Clear");
                //decalContainer[1].paintDecal.Swit = true;
                foreach(var pTexture in decalContainer[1].paintable.PaintableTextures)
                {
                    pTexture.Clear();
                }

            }

            if (ImgQueue.Count == 0) { return; }

            Data.ImageData iData = ImgQueue.Dequeue();

            iData.Unity_SetTexture(tstX, tstY, tstWidth, tstHeight);
            // -----

            //decalContainer[0].paintDecal.Scale = tstImgScale1;

            // -----
            iData.stage3_SetTexture = true;

            if (iData.IsTextureExisted())
            {
                TextureInImgList.Add(iData);
            }

            if (TextureInImgList.Count > 0)
            {
                ApplyTextures();
            }

            ImgQueue.Clear();

            
            //return;
        }

        /// <summary>
        /// �ۼ� �Ϸ�� �ؽ��ĵ��� ������Ʈ�Ѵ�.
        /// </summary>
        void ApplyTextures()
        {
            // FIX : �̹��� �ؽ�ó �������� ������ Update���� �ۼ��� ���� �ܰ� ����
            Update_Websocket_pose_texture2D();

            //// �۾� �� �̹��� ����Ʈ Ŭ����
            //TextureInImgList.Clear();
            UnloadAllImgData();
        }

        public void UnloadAllImgData()
        {
            foreach(ImageData data in TextureInImgList)
            {
                if (data == null) { continue; }

                if (data.Frame_Texture != null)
                {
                    //Destroy(data.Frame_Texture);
                    data.Frame_Texture = null;
                }

                if (data.Img1_Texture != null)
                {
                    //Destroy(data.Img1_Texture);
                    data.Img1_Texture = null;
                }

                if (data.Img2_Texture != null)
                {
                    //Destroy(data.Img2_Texture);
                    data.Img2_Texture = null;
                }

                if (data.Img3_Texture != null)
                {
                    //Destroy(data.Img3_Texture);
                    data.Img3_Texture = null;
                }

                if (data.Img4_Texture != null)
                {
                    //Destroy(data.Img4_Texture);
                    data.Img4_Texture = null;
                }
            }

            // �۾� �� �̹��� ����Ʈ Ŭ����
            TextureInImgList.Clear();
        }

        #endregion Update

        #region Events

        /// <summary>
        /// �����Ͽ��� ���� �̹��� ������ �ν��Ͻ��� ������ �ν��Ͻ��� �����´�.
        /// </summary>
        /// <param name="imageData"> �̹��� ������ �ν��Ͻ� </param>
        public void EnqueueImageData(Data.ImageData imageData)
        {
            if (imageData == null) { return; }

            ImgQueue.Enqueue(imageData);

            imageData.stage2_AssignImgDics = true;
        }

        

        /// <summary>
        /// �� ������ Update���� �ؽ�ó ������Ʈ�� ������ �̺�Ʈ
        /// </summary>
        /// <param name="tex"></param>
        void Update_Websocket_pose_texture2D()
        {
            if (TextureInImgList == null) { return; }

            //// TODO : ���� paintDecals 1���� �����ϵ��� ������. ���� ���� ���� ��ķ ȯ�� ����ϵ��� ���� �ʿ�
            //paintDecals[0].IsClick = true;
            //paintDecals[0].Texture = TextureInImgList[0].CopyTexture();
            if (TextureInImgList.Count == 0) { return; }

            Data.ImageData iData = TextureInImgList[0];

            // ���� ���ñ⿡ ���� ���� ����
            poseDirector.ApplyPose(iData);
            
            if (iData.Img1_Texture != null)
            {
                Texture old = decalContainer[0].paintDecal.Texture;
                decalContainer[0].paintDecal.IsClick = true;
                decalContainer[0].paintDecal.Texture = iData.CopyTexture(0);
                //decalContainer[0].paintDecal.Texture = iData.Img1_Texture;
                Destroy(old);
            }

            if (iData.Img2_Texture != null)
            {
                Texture old = decalContainer[1].paintDecal.Texture;
                decalContainer[1].paintDecal.IsClick = true;
                decalContainer[1].paintDecal.Texture = iData.CopyTexture(1);
                //decalContainer[1].paintDecal.Texture = iData.Img2_Texture;
                Destroy(old);
            }

            if (iData.Img3_Texture != null)
            {
                Texture old = decalContainer[2].paintDecal.Texture;
                decalContainer[2].paintDecal.IsClick = true;
                decalContainer[2].paintDecal.Texture = iData.CopyTexture(2);
                //decalContainer[2].paintDecal.Texture = iData.Img3_Texture;
                Destroy(old);
            }

            if (iData.Img4_Texture != null)
            {
                Texture old = decalContainer[3].paintDecal.Texture;
                decalContainer[3].paintDecal.IsClick = true;
                decalContainer[3].paintDecal.Texture = iData.CopyTexture(3);
                //decalContainer[3].paintDecal.Texture = iData.Img4_Texture;
                Destroy(old);
            }

            // �Է� �̺�Ʈ �߻� �غ�
            cwInputManager.IsClick = true;
            cwInputManager.customClickVector = customClickVector;

            // TODO: 0803 Ŭ�� ��ġ Ŀ���� ����

            IsDecal_updated = true;
        }

        #endregion

        #region Datas

        /// <summary>
        /// �����Ͽ��� ���� �̹��� ������ �޴� ť
        /// </summary>
        public Queue<Data.ImageData> ImgQueue { get; private set; }

        /// <summary>
        /// Update�� ���� Texture �̹����� ������ �ν��Ͻ��� ���� ����Ʈ
        /// TODO: ���� ���� ���� �̹����� ��� ��ķ paintable�� �й����� �˰��� ����
        /// </summary>
        public List<Data.ImageData> TextureInImgList { get; set; }


        private bool isDecal_updated = false;

        /// <summary>
        /// ��Į�� ������Ʈ �Ǿ����� Ȯ���ϴ� ����
        /// TODO : ��ķ ������Ʈ limit�� ���� �� ������ toggle ���� �����ϵ��� ����
        /// </summary>
        public bool IsDecal_updated
        {
            get { return isDecal_updated; }
            set { isDecal_updated = value; }
        }

        #endregion

        #region Late Update: �ַ� ������Ʈ�� ������ ���� ����

        private void LateUpdate()
        {
            //return;
            if (!isDecal_updated) { return; }
            IsDecal_updated = false;

            ExportTextures();
        }

        public SkinnedMeshRenderer testRenderer;

        /// <summary>
        /// �ؽ�ó�� paintable ��ü���� �����س���.
        /// </summary>
        private async void ExportTextures()
        {
            //// p3dPaintDecal���� �����´�.
            //Texture2D[] textures = GetTextures();

            //Texture2D texResult = MergeTextures(textures);

            //testRenderer.material.SetTexture("_MainTex", texResult);

            ////-----
            //byte[] byteArray = texResult.EncodeToPNG();

            //string encodeStr = EncodeToString(byteArray);

            //SerializeAndSendServer(encodeStr);
            ////-----


            //-----
            //P3dPaintableTexture[] p3dTextures = _paintable.GetComponents<P3dPaintableTexture>();

            //// ������ p3d �ؽ�ó���� byte[] �̹��� �迭�� �̾Ƴ���.
            //List<byte[]> textureData = GetTextureByteDatas(p3dTextures);

            //// ���� �ؽ�ó���� �ϳ��� �ؽ�ó�� &���� �Ѵ�.
            //byte[] textureResult = IntersectByteDatas(textureData);

            //string encodeStr = EncodeToString(textureResult);

            ////Debug.Log("1");
            //SerializeAndSendServer(encodeStr);
            //-----



            //_mats = _paintable.Materials;
            // ���⼭ ���� paintable �������� �ؽ�ó�� �����´�.
            //_texture = _paintable.GetComponents<P3dPaintableTexture>();
            P3dPaintableTexture[] texture1 = decalContainer[0].paintable.GetComponents<P3dPaintableTexture>();
            P3dPaintableTexture[] texture2 = decalContainer[1].paintable.GetComponents<P3dPaintableTexture>();
            P3dPaintableTexture[] texture3 = decalContainer[2].paintable.GetComponents<P3dPaintableTexture>();
            P3dPaintableTexture[] texture4 = decalContainer[3].paintable.GetComponents<P3dPaintableTexture>();
            //_texture = new P3dPaintableTexture[1];
            //_texture[0] = decalContainer[0].texture;

            // 1: material���� texture ����ͼ�, �ȿ� �ִ� texture ���� �� dict �Ҵ�
            var task1 = GetEncodedTextureAsync(texture1);
            var task2 = GetEncodedTextureAsync(texture2);
            var task3 = GetEncodedTextureAsync(texture3);
            var task4 = GetEncodedTextureAsync(texture4);

            await Task.WhenAll(task1, task2, task3, task4);

            string result1 = task1.Result;
            string result2 = task2.Result;
            string result3 = task3.Result;
            string result4 = task4.Result;
            //Dictionary<string, string> result = GetEncodedTexture(_texture);
            //return;

            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("width", "640"); result.Add("height", "480");
            result.Add("0", result1); result.Add("1", result2) ; result.Add("2", result3); result.Add("3", result4);

            // 2: string ������ ����ȭ �� ���� ����
            //SerializeAndSendServer(result1);    // test

            SerializeAndSendServer(result);
        }

        private Texture2D[] GetTextures()
        {
            List<Texture2D> _p3dTextures = new List<Texture2D>();
            foreach(DecalContainer cont in decalContainer)
            {
                //Texture2D tex = cont.texture.Texture as Texture2D; // TODO: �ϴ� Texture ��ü�� ����..

                Texture2D tex = cont.texture.GetReadableCopy(false);

                _p3dTextures.Add(tex);
            }
            return _p3dTextures.ToArray();
        }

        private Texture2D MergeTextures(Texture2D[] textures)
        {
            int width = textures[0].width;
            int height = textures[0].height;

            Texture2D result = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color mergedColor = new Color(1, 1, 1, 1);
                    foreach (Texture2D texture in textures)
                    {
                        Color pixelColor = texture.GetPixel(x, y);
                        mergedColor.r *= pixelColor.r;
                        mergedColor.g *= pixelColor.g;
                        mergedColor.b *= pixelColor.b;
                        mergedColor.a *= pixelColor.a;
                    }
                    result.SetPixel(x, y, mergedColor);
                }
            }

            result.Apply();
            return result;
        }

        #region Attempt 1
        private List<byte[]> GetTextureByteDatas(P3dPaintableTexture[] textures)
        {
            List<byte[]> data = new List<byte[]>();
            foreach(P3dPaintableTexture tex in textures)
            {
                byte[] ba = GetPaintableTexture(tex);
                data.Add(ba);
            }
            return data;
        }

        private byte[] IntersectByteDatas(List<byte[]> data)
        {
            if (data == null || data.Count < 4)
            {
                throw new ArgumentException("Data list should contain at least 4 byte arrays.");
            }

            int count = data[0].Length;
            byte[] result = new byte[count];
            //Debug.Log(data[0].Length);
            //Debug.Log(data[1].Length);
            //Debug.Log(data[2].Length);
            //Debug.Log(data[3].Length);
            //Debug.Log(result.Length);

            for (int i = 0; i < count; i++)
            {
                int b1 = (i < data[0].Length) ? data[0][i] : 255;
                int b2 = (i < data[1].Length) ? data[1][i] : 255;
                int b3 = (i < data[2].Length) ? data[2][i] : 255;
                int b4 = (i < data[3].Length) ? data[3][i] : 255;
                result[i] = (byte)(b1 & b2 & b3 & b4);
            }

            return result;
        }

        private string EncodeToString(byte[] byteArray)
        {
            string encodeStr = encodeString(byteArray);

            return encodeStr;
        }
        #endregion

        #region 1: Paintable ���ο� �ִ� ��� textures ���ڿ� ���ڵ� �� dict�� ��ȯ

        /// <summary>
        /// P3dPaintableTextures �ȿ� �ִ� �ؽ��� 1�� ��������
        /// </summary>
        /// <param name="paintableTextures"></param>
        /// <returns> ���ڵ� �Ϸ�� �̹��� ���� </returns>
        private string GetEncodedTexture(P3dPaintableTexture[] paintableTextures)
        {
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            string result = "";

            int i = 0; // _paintable.materials[0]
            int j = 0; // _paintable.GetComponents<P3ddPaintableTexture>()[0];

            // materials i �� ��Ī�Ǵ� j�� ���� ����
            // ���� materials ���� ����Դµ� ���� ���� ������ mat 1 tex 1�� ������ ����.
            // i == 0 : �Ʒ� ���ǽ��� 0�� ���� ���� ����� materials �迭�� �ε��� 0(1��°)��.
            // j == 0 : ù ��° P3dTexture
            if (paintableTextures[j].Slot.Index == i)
            {
                byte[] byteArray = GetPaintableTexture(paintableTextures[j]);
                string encodeStr = encodeString(byteArray);

                result = encodeStr;
            }

            return result;
        }

        private async Task<string> GetEncodedTextureAsync(P3dPaintableTexture[] paintableTextures)
        {
            int i = 0; // _paintable.materials[0]
            int j = 0; // _paintable.GetComponents<P3ddPaintableTexture>()[0];

            byte[] byteArray = GetPaintableTexture(paintableTextures[0]);

            //Destroy(paintableTextures[0]);
            //paintableTextures[0] = null;

            return await Task.Run(() => {
                //Dictionary<string, string> dict = new Dictionary<string, string>();
                string result = "";

                // materials i �� ��Ī�Ǵ� j�� ���� ����
                // ���� materials ���� ����Դµ� ���� ���� ������ mat 1 tex 1�� ������ ����.
                // i == 0 : �Ʒ� ���ǽ��� 0�� ���� ���� ����� materials �迭�� �ε��� 0(1��°)��.
                // j == 0 : ù ��° P3dTexture
                string encodeStr = encodeString(byteArray);

                result = encodeStr;
                //if (paintableTextures[j].Slot.Index == i)
                //{
                //    //byte[] byteArray = GetPaintableTexture(paintableTextures[j]);
                //    string encodeStr = encodeString(byteArray);

                //    result = encodeStr;
                //}

                return result;
            });
        }

        /// <summary>
        /// PaintableTexture ���ڿ��� ��Į �����ִ� �ؽ�ó �����ϱ�
        /// </summary>
        /// <param name="paintableTexture"></param>
        /// 
        /// <returns></returns>
        private byte[] GetPaintableTexture(P3dPaintableTexture paintableTexture)
        {
            return GetData(paintableTexture, Settings.DefaultTextureFormat);
        }

        /// <summary>
        /// PaintableTexture ���ڿ��� ExportTextureFormat������ �ؽ�ó ����
        /// </summary>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private byte[] GetData(P3dPaintableTexture t, ExportTextureFormat f)
        {
            switch (Settings.DefaultTextureFormat)
            {
                case ExportTextureFormat.PNG: return t.GetPngData();
                case ExportTextureFormat.TGA: return t.GetTgaData();
            }

            return null;
        }

        /// <summary>
        /// �̹��� ������ base64 ���ڿ�ȭ
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private string encodeString(byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        #endregion

        #region 2: texture ���� dict -> json ���ڿ�ȭ �� ���� ����

        /// <summary>
        /// �ؽ�ó �̹����� ���� dict ���ڸ� json ���ڿ� ����ȭ �� ������ ����
        /// </summary>
        /// <param name="dict"></param>
        private void SerializeAndSendServer(Dictionary<string, string> dict)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

            websocket.Send_Message(json);
        }

        private void SerializeAndSendServer(string str)
        {
            //Debug.Log(str);
            //Debug.Log(json);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(str);
            websocket.Send_Message(json);

            //websocket.Send_Message(str);
        }

        #endregion

        #endregion
    }
}
