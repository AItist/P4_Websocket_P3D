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
        /// 작성 완료된 텍스쳐들을 업데이트한다.
        /// </summary>
        void ApplyTextures()
        {
            // FIX : 이미지 텍스처 생성까지 관리자 Update에서 작성함 다음 단계 진행
            Update_Websocket_texture2D();

            // 작업 후 이미지 리스트 클리어
            TextureInImgList.Clear();
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

            //Debug.Log("OnMessage");
        }

        /// <summary>
        /// 주 관리자 Update에서 텍스처 업데이트시 실행할 이벤트
        /// </summary>
        /// <param name="tex"></param>
        void Update_Websocket_texture2D()
        {
            if (TextureInImgList == null) { return; }

            // TODO : 현재 paintDecals 1번만 대응하도록 설정됨. 추후 여러 대의 웹캠 환경 모사하도록 조정 필요
            paintDecals[0].IsClick = true;
            paintDecals[0].Texture = TextureInImgList[0].CopyTexture();

            // 입력 이벤트 발생 준비
            cwInputManager.IsClick = true;

            IsDecal_updated = true;
        }

        #endregion

        #region Datas

        /// <summary>
        /// 웹소켓에서 받은 이미지 데이터 받는 큐
        /// </summary>
        public Queue<Data.ImageData> ImgQueue { get; private set; }

        /// <summary>
        /// Update를 통해 Texture 이미지를 생성한 인스턴스만 모은 리스트
        /// TODO: 추후 여러 장의 이미지를 어떻게 웹캠 paintable에 분배할지 알고리즘 조정
        /// </summary>
        public List<Data.ImageData> TextureInImgList { get; set; }


        private bool isDecal_updated = false;

        /// <summary>
        /// 데칼이 업데이트 되었는지 확인하는 변수
        /// TODO : 웹캠 업데이트 limit에 따라 이 변수값 toggle 조건 변경하도록 조정
        /// </summary>
        public bool IsDecal_updated
        {
            get { return isDecal_updated; }
            set { isDecal_updated = value; }
        }

        #endregion

        #region Late Update: 주로 업데이트된 데이터 서버 전송

        private void LateUpdate()
        {
            if (!isDecal_updated) { return; }
            IsDecal_updated = false;

            ExportTextures();
        }

        /// <summary>
        /// 텍스처를 paintable 객체에서 추출해낸다.
        /// </summary>
        private void ExportTextures()
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
                        byte[] byteArray = GetPaintableTexture(paintableTextures[j]);
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

        /// <summary>
        /// PaintableTexture 인자에서 데칼 박혀있는 텍스처 추출하기
        /// </summary>
        /// <param name="paintableTexture"></param>
        /// 
        /// <returns></returns>
        private byte[] GetPaintableTexture(P3dPaintableTexture paintableTexture)
        {
            return GetData(paintableTexture, Settings.DefaultTextureFormat);
        }

        /// <summary>
        /// PaintableTexture 인자에서 ExportTextureFormat형으로 텍스처 추출
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
        /// 이미지 데이터 base64 문자열화
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private string encodeString(byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        #endregion

        #region 2: texture 가진 dict -> json 문자열화 후 서버 전송

        /// <summary>
        /// 텍스처 이미지를 가진 dict 인자를 json 문자열 직렬화 후 서버로 전송
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
