using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public PaintIn3D.P3dPaintDecal pDecal;

        [Header("paintable resource")]
        public P3dPaintable _paintable;
        public Material[] _mats;
        public P3dPaintableTexture[] _textures;

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
            ImgQueue.Clear();
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

            Debug.Log("OnMessage");
        }

        /// <summary>
        /// �����Ͽ��� �ؽ�ó ������Ʈ�� ������ �̺�Ʈ
        /// </summary>
        /// <param name="tex"></param>
        void Update_Websocket_texture2D()
        {
            pDecal.IsClick = true;
            // �Է� �̺�Ʈ �߻� �غ�
            cwInputManager.IsClick = true;

            IsDecal_updated = true;
        }

        #endregion

        #region Datas

        public Queue<Data.ImageData> ImgQueue { get; private set; }

        private Texture2D websocket_texture2D;

        public Texture2D Websocket_texture2D
        {
            set
            {
                websocket_texture2D = value;
                pDecal.Texture = websocket_texture2D;

                // �ؽ��� ������Ʈ�� �˸�
                Update_Websocket_texture2D();

                //Debug.Log("Websocket texture2D updated");
            }
        }

        private bool isDecal_updated = false;

        public bool IsDecal_updated
        {
            get { return isDecal_updated; }
            set { isDecal_updated = value; }
        }

        #endregion

        #region Late Update

        private void LateUpdate()
        {
            if (!isDecal_updated) { return; }
            IsDecal_updated = false;

            ExportTextures();
        }

        public void ExportTextures()
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
                        byte[] byteArray = GetPaintableTexture(paintableTextures[j], material);
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

        private byte[] GetPaintableTexture(P3dPaintableTexture paintableTexture, Material material)
        {
            return GetData(paintableTexture, Settings.DefaultTextureFormat);
        }

        private byte[] GetData(P3dPaintableTexture t, ExportTextureFormat f)
        {
            switch (Settings.DefaultTextureFormat)
            {
                case ExportTextureFormat.PNG: return t.GetPngData();
                case ExportTextureFormat.TGA: return t.GetTgaData();
            }

            return null;
        }

        private string encodeString(byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        #endregion

        private void SerializeAndSendServer(Dictionary<string, string> dict)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

            websocket.SendDataToServer(json);
        }

        #endregion
    }
}
