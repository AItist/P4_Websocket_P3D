using Data;
using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        #region Update
        public int tstX;
        public int tstY;
        public int tstWidth;
        public int tstHeight;
        public Vector3 tstImgScale1;

        private void Update()
        {
            if (ImgQueue.Count == 0) { return; }

            Data.ImageData iData = ImgQueue.Dequeue();

            iData.Unity_SetTexture(tstX, tstY, tstWidth, tstHeight);
            // -----

            decalContainer[0].paintDecal.Scale = tstImgScale1;

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
        }

        /// <summary>
        /// �ۼ� �Ϸ�� �ؽ��ĵ��� ������Ʈ�Ѵ�.
        /// </summary>
        void ApplyTextures()
        {
            // FIX : �̹��� �ؽ�ó �������� ������ Update���� �ۼ��� ���� �ܰ� ����
            Update_Websocket_pose_texture2D();

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

        public Vector2 customClickVector;

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
                decalContainer[0].paintDecal.IsClick = true;
                decalContainer[0].paintDecal.Texture = iData.CopyTexture(0);
            }

            if (iData.Img2_Texture != null)
            {
                decalContainer[1].paintDecal.IsClick = true;
                decalContainer[1].paintDecal.Texture = iData.CopyTexture(1);
            }

            if (iData.Img3_Texture != null)
            {
                decalContainer[2].paintDecal.IsClick = true;
                decalContainer[2].paintDecal.Texture = iData.CopyTexture(2);
            }

            if (iData.Img4_Texture != null)
            {
                decalContainer[3].paintDecal.IsClick = true;
                decalContainer[3].paintDecal.Texture = iData.CopyTexture(3);
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
            if (!isDecal_updated) { return; }
            IsDecal_updated = false;

            ExportTextures();
        }

        /// <summary>
        /// �ؽ�ó�� paintable ��ü���� �����س���.
        /// </summary>
        private void ExportTextures()
        {
            //_mats = _paintable.Materials;
            // ���⼭ ���� paintable �������� �ؽ�ó�� �����´�.
            _texture = _paintable.GetComponents<P3dPaintableTexture>();

            // 1: material���� texture ����ͼ�, �ȿ� �ִ� texture ���� �� dict �Ҵ�
            string result = GetEncodedTexture(_texture);
            //Dictionary<string, string> result = GetEncodedTexture(_texture);

            // 2: string ������ ����ȭ �� ���� ����
            SerializeAndSendServer(result);
        }

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
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(str);
            //Debug.Log(json);

            websocket.Send_Message(json);
        }

        #endregion

        #endregion
    }
}
