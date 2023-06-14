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

        /// <summary>
        /// �Է� �̺�Ʈ�� �߻���Ű�� ��Ʈ Ŭ����
        /// </summary>
        [Header("P3d resources")]
        public CW.Common.CwInputManager cwInputManager;

        /// <summary>
        /// ���������� ���� �ؽ�ó�� ������Ʈ�ϴ� �ν��Ͻ�
        /// TODO : �� PaintDecal�� ���߿� ����Ʈ �Ǵ� ������ ������ �����Ѵ�.
        /// </summary>
        public List<PaintIn3D.P3dPaintDecal> paintDecals;

        [Header("paintable resource")]
        public P3dPaintable _paintable;
        private Material[] _mats;
        private P3dPaintableTexture[] _textures;

        #region Start

        private void Initialize()
        {
            Debug.Log("Manager initialized.");
        }

        private void Start()
        {
            if (ImgQueue == null)
            {
                ImgQueue = new Queue<Data.ImageData>();
            }

            if (TextureInImgList == null)
            {
                TextureInImgList = new List<Data.ImageData>();
            }

            if (instance == null)
            {
                Instance.Initialize();
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

        private void Update()
        {
            if (ImgQueue.Count == 0) { return; }

            int cnt = ImgQueue.Count <= 4 ? ImgQueue.Count : 4;
            for (int i = 0; i < cnt; i++)
            {
                Data.ImageData imgData = ImgQueue.Dequeue();

                imgData.Unity_SetTexture();

                if (imgData.Frame_Texture != null)
                {
                    TextureInImgList.Add(imgData);
                }
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
            Update_Websocket_texture2D();

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

            //Debug.Log("OnMessage");
        }

        /// <summary>
        /// �� ������ Update���� �ؽ�ó ������Ʈ�� ������ �̺�Ʈ
        /// </summary>
        /// <param name="tex"></param>
        void Update_Websocket_texture2D()
        {
            if (TextureInImgList == null) { return; }

            // TODO : ���� paintDecals 1���� �����ϵ��� ������. ���� ���� ���� ��ķ ȯ�� ����ϵ��� ���� �ʿ�
            paintDecals[0].IsClick = true;
            paintDecals[0].Texture = TextureInImgList[0].CopyTexture();

            // �Է� �̺�Ʈ �߻� �غ�
            cwInputManager.IsClick = true;

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
            _mats = _paintable.Materials;
            _textures = _paintable.GetComponents<P3dPaintableTexture>();

            // 1: material���� texture ����ͼ�, �ȿ� �ִ� texture ���� �� dict �Ҵ�
            Dictionary<string, string> result = GetEncodedTextures(_paintable, _mats, _textures);

            // 2: dict json ����ȭ �� ���� ����
            SerializeAndSendServer(result);
        }

        #region 1: Paintable ���ο� �ִ� ��� textures ���ڿ� ���ڵ� �� dict�� ��ȯ

        /// <summary>
        /// P3dPaintableTextures �ȿ� �ִ� �ؽ��� �� ��������
        /// </summary>
        /// <param name="paintable"></param>
        /// <param name="materials"></param>
        /// <param name="paintableTextures"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetEncodedTextures(P3dPaintable paintable, Material[] materials, P3dPaintableTexture[] paintableTextures)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // 1: material���� texture ����ͼ�, �ȿ� �ִ� texture ���� �� dict �Ҵ�
            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];

                for (int j = 0; j < paintableTextures.Length; j++)
                {
                    if (paintableTextures[j].Slot.Index == i)
                    {
                        byte[] byteArray = GetPaintableTexture(paintableTextures[j]);
                        string encodeStr = encodeString(byteArray);

                        string key = $"{i},{j}";
                        dict.Add(key, encodeStr);

                        // ������ �������� �ּ�����.
                        //Debug.Log($"key : {key} \ndata : {encodeStr}");
                    }
                }
            }
            return dict;
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

            websocket.SendDataToServer(json);
        }

        #endregion

        #endregion
    }
}
