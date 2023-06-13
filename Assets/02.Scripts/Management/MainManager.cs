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
        /// P3에서 가공된 이미지를 받아오고, \n
        /// 완성된 텍스처를 전달하기 위한 웹소켓 인스턴스
        /// </summary>
        public WebSocket_.P4_Websocket websocket;

        /// <summary>
        /// 입력 이벤트를 발생시키는 루트 클래스
        /// </summary>
        [Header("P3d resources")]
        public CW.Common.CwInputManager cwInputManager;

        /// <summary>
        /// 웹소켓으로 받은 텍스처를 업데이트하는 인스턴스
        /// TODO : 이 PaintDecal은 나중에 리스트 또는 사전형 변수로 변경한다.
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

            // 웹소켓 생성 지시
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
        /// 웹소켓에서 받은 이미지 데이터 인스턴스를 관리자 인스턴스로 가져온다.
        /// </summary>
        /// <param name="imageData"> 이미지 데이터 인스턴스 </param>
        public void EnqueueImageData(Data.ImageData imageData)
        {
            if (imageData == null) { return; }

            ImgQueue.Enqueue(imageData);

            Debug.Log("OnMessage");
        }

        /// <summary>
        /// 웹소켓에서 텍스처 업데이트시 실행할 이벤트
        /// </summary>
        /// <param name="tex"></param>
        void Update_Websocket_texture2D()
        {
            pDecal.IsClick = true;
            // 입력 이벤트 발생 준비
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

                // 텍스쳐 업데이트를 알림
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

            // 1: material마다 texture 갖고와서, 안에 있는 texture 추출 및 dict 할당
            Dictionary<string, string> result = GetEncodedTextures(_paintable, _mats, _textures);

            // 2: dict json 직렬화 및 서버 전달
            SerializeAndSendServer(result);
        }

        #region 1: Paintable 내부에 있는 모든 textures 문자열 인코딩 후 dict로 반환

        /// <summary>
        /// P3dPaintableTextures 안에 있는 텍스쳐 다 가져오기
        /// </summary>
        /// <param name="paintable"></param>
        /// <param name="materials"></param>
        /// <param name="paintableTextures"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetEncodedTextures(P3dPaintable paintable, Material[] materials, P3dPaintableTexture[] paintableTextures)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // 1: material마다 texture 갖고와서, 안에 있는 texture 추출 및 dict 할당
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

                        // 에디터 오류생김 주석걸자.
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
