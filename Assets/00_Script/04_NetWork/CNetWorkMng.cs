using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.Events;
using System.Timers;
using System.Runtime.InteropServices;
using LitJson;
using UnityEngine.SceneManagement;
using System.IO;

public class CNetWorkMng : MonoBehaviour
{
	private CUDPNetWork m_UdpNetwork;
	private Queue<string> m_MsgQuee;
	
	public bool _IsDebuging = false;
	private CMovieSyncPlayer _VideoPlayer;
	private bool m_IsServer; public bool _IsServer { get { return m_IsServer; } set { m_IsServer = value; } }


	void Awake()
	{
		m_MsgQuee = new Queue<string>();
		m_IsServer=false;
		m_UdpNetwork = gameObject.AddComponent<CUDPNetWork>();
		
	}


	public void ResetNetWork()
    {
       m_UdpNetwork.ReleaseNetWorkUdp();

       Destroy(m_UdpNetwork);
	   m_UdpNetwork = null;
	   m_MsgQuee.Clear();
	}

	public void ReConnetNetWork()
    {
		m_UdpNetwork = gameObject.AddComponent<CUDPNetWork>();
		m_UdpNetwork.Initialize((x) => ReceiveData(x));
	}

    void Start()
	{
		_VideoPlayer = transform.GetComponent<CMovieSyncPlayer>();
		 m_IsServer = m_UdpNetwork.Initialize((x) => ReceiveData(x));
		StartCoroutine("PacketGenerator");
	}


	public void Send(PROTOCOL protocol,int nFrameValue=0)
	{ 
		if (m_IsServer == false)
			return;
		
		string SendString = "";
		int nValue = (int)protocol;

		switch (protocol){
			case PROTOCOL.SND_START_MOVIE:			
				SendString = "{ \"ID\":0 }";
				break;	
		}

		for (int i=0; i<CConfigMng.Instance._listIP.Count; i++){
			if (m_UdpNetwork.GetReceiveIp()!= CConfigMng.Instance._listIP[i].strIp){
				m_UdpNetwork.Send(StringToByte(SendString), CConfigMng.Instance._listIP[i].strIp, CConfigMng.Instance._listIP[i].nPort);
			}
		}
	}


	public void PacketParser(string strPacket)
	{
		JsonData jData = JsonMapper.ToObject(strPacket);
	
		Debug.Log("Receive :   " + strPacket + " -> " + (PROTOCOL)int.Parse(jData["ID"].ToString()));

		switch ((PROTOCOL)int.Parse(jData["ID"].ToString()))
		{
			case PROTOCOL.RCV_START_MOVIE:
				if(CConfigMng.Instance._bIsMediaServer==false)
					_VideoPlayer.PlayerSeekToFrame(0);

				break;
			default:break;
		}
	}
		
    private IEnumerator PacketGenerator()
	{
		WaitForSeconds waitSec = new WaitForSeconds(0.001f);
		while (true)
		{
			if (m_MsgQuee.Count > 0){ 
				PacketParser(m_MsgQuee.Dequeue());
			}
			yield return waitSec;
		}
	}
	void ReceiveData(byte[] bytes)
	{
		m_MsgQuee.Enqueue(ByteToString(bytes));
	}

	private string ByteToString(byte[] strByte)
	{
		string str = System.Text.Encoding.Default.GetString(strByte);
		return str;
	}

	private byte[] StringToByte(string str)
	{
		byte[] StrByte = System.Text.Encoding.UTF8.GetBytes(str);
		return StrByte;
	}

	public string GetSendString(string a, int b, string c, float d)
	{
		string strTemp;
		strTemp = "{" +
			"\"" + a + "\"" + ":" + b + "," +
			"\"" + c + "\"" + ":" + d +
			"}";
		return strTemp;
	}

	public string GetSendString(string a, int b, string c, int d)
    {
		string strTemp;
		strTemp = "{"+
			"\"" + a + "\"" + ":" + b +","+
			"\"" + c + "\"" + ":" + d +
			"}";
		return strTemp;
	}
	public string GetSendString(string a, int b)
    {
        string strTemp;
        strTemp = "{" +
                        "\"" + a + "\"" + ":" + b +
						"}";
		return strTemp;

	}
	public string GetSendString(string a, string b)
    {
		string strTemp;
		strTemp = "{" +
						"\"" + a + "\"" + "," + b +
					"}";
		return strTemp;
    }
}



/*
 	public void PacketParser(string strPacket)
	{
		CPanelManager.Instance.PacketParser(strPacket);
		return;

		JsonData jData = JsonMapper.ToObject(strPacket);
		//Debug.Log(strPacket);


		Debug.Log("Receive :   " +strPacket + " -> "+ (PROTOCOL)int.Parse(jData["ID"].ToString()));
		 //{"ID":40 , "VALUE":40.4} }
		switch ((PROTOCOL)int.Parse(jData["ID"].ToString()))
		{

			case PROTOCOL.CLIMATEDRIVER:
				float fValue = float.Parse(jData["VALUE"].ToString());
				CSyncValueManager.Instance._ClimateDriver = fValue;

				break;

			case PROTOCOL.TASK:
                //CPanelManager.Instance.NetWork((int)PROTOCOL.TASK);
                IVIMainScript.Instance.InsertSubWindow("[06]Task");
                IVIMainScript.Instance.InsertDashboardWindow("[05]Dashboard");//이전
                Debug.Log("TASK이다.");
				break;

			case PROTOCOL.PAYLOAD:
				//IVIMainScript.Instance.InsertSubWindow("[08]Payload");	
				CPanelManager.Instance.InsertSubWindow("[08]Payload");	//이전		
				Debug.Log("PAYLOAD이다.");//이전
				break;

			case PROTOCOL.CLIMATE:
				//IVIMainScript.Instance.InsertRouteWindow("[10]Climate");
				//
				CPanelManager.Instance.InsertRouteWindow("[10]Climate");//이전
				Debug.Log("CLIMATE이다.");
				break;

			case PROTOCOL.MUSIC:
				//모바일 뮤직 클릭 시 반응 X

				//0224 모바일 뮤직 클릭 시 IVI 뮤직 패널 열기

				CPanelManager.Instance.InsertMainBGWindow("[03]Music");//이전

				//IVIMainScript.Instance.InsertRouteWindow("[03]Music");
				Debug.Log("MUSIC이다.");
				break;

			//ENTER 씬 삭제됨
			//case PROTOCOL.ENTER:
			//	Debug.Log("ENTER이다.");
			//	StartCoroutine(VideoPanel.Instance.ShowVideo());
			//	Debug.Log("코루틴 클릭");
			//	break;

			case PROTOCOL.IVITASKROUTE:
				//IVI 버튼 눌러 모바일 Task 열기
				MainScript.Instance.InsertSubWindow("01_SubPage_Task");//이전
				break;

			case PROTOCOL.IVIPAYLOAD:
				//5
				//IVI 버튼 눌러 모바일 Payload 열기
				MainScript.Instance.InsertSubWindow("02_SubPage_Payload");//이전
				break;

			case PROTOCOL.IVIMUSIC:
				//IVI 버튼 눌러 모바일 Music 열기
				MainScript.Instance.InsertSubWindow("04_SubPage_Music");//이전
				break;

			case PROTOCOL.PLAYMUSIC:

				//모바일 재생버튼 눌러 IVI 대쉬보드 뮤직패널 열기

				//0224 대시보드 가사패널내려옴 삭제
				//IVIMainScript.Instance.InsertDashboardMusicWindow("[11]Dashboard_Music");
				//IVIMainScript.Instance.PlayDashboardMusic();

				break;

			case PROTOCOL.IVIDRIVER:
				//IVI 왼쪽 온도 버튼 눌러 모바일 Driver 열기
				MainScript.Instance.InsertSubWindow("03_SubPage_Climate");
				break;

			case PROTOCOL.IVIPASSENGER:
				//IVI 오른쪽 온도 버튼 눌러 모바일 Passenger 열기
				MainScript.Instance.InsertSubWindow("03_SubPage_Climate_Passenger");
				break;

			case PROTOCOL.TEMPSLIDER:
				//10
				// 모바일 슬라이더

				Debug.Log("IVIAIRMODE");
				//Debug.Log($"찾기 : {GameObject.Find("01_SubLayer").transform.GetChild(0).Find("ClimateControl").Find("Airmode").gameObject.name}");

				//CPanelManager.Instance.NetWork((int)PROTOCOL.IVIAIRMODE);

				// 버튼 클릭하기,,,?
				//MainScript.Instance.InsertSubWindow("03_SubPage_Climate_Passenger");


				CPanelManager.Instance.NetWork((int)PROTOCOL.TEMPSLIDER);//이전
				break;
			case PROTOCOL.IVIAIRMODEAUTO:
				// IVI 에어모드 버튼
				
				Debug.Log("IVIAIRMODEAUTO");
				

				break;

			case PROTOCOL.IVIAIRMODEAC:
				//IVI 에어모드 버튼
				Debug.Log("IVIAIRMODEAC");
				
				break;
			case PROTOCOL.IVIAIRMODERECIRC:
				//IVI 에어모드 버튼
				Debug.Log("IVIAIRMODERECIRC");
				
				break;
			case PROTOCOL.IVIAIRMODEFRONT:
				//IVI 에어모드 버튼
				Debug.Log("IVIAIRMODEFRONT");
				
				break;


			//모바일
			case PROTOCOL.AIRMODE:
				// 스와이프해서 에어모드 창 열기
				// 15번을 보냄
				// IVI 에어모드 패널 열기

				Debug.Log("AIRMODE");

				CPanelManager.Instance.NetWork((int)PROTOCOL.AIRMODE);


				break;
			case PROTOCOL.AIRMODEAUTO:
				// 에어모드 버튼
				// 모바일의 에어모드 오토 버튼 찾기
				Debug.Log("AIRMODEAUTO");
				//모바일 에어모드 오토 눌렀을 때
				CPanelManager.Instance.NetWork((int)PROTOCOL.AIRMODEAUTO);

				break;

			case PROTOCOL.AIRMODEAC:
				//에어모드 
				Debug.Log("AIRMODEAC");
				CPanelManager.Instance.NetWork((int)PROTOCOL.AIRMODEAC);
				break;
			case PROTOCOL.AIRMODERECIRC:
				//에어모드 버튼
				Debug.Log("AIRMODERECIRC");
				CPanelManager.Instance.NetWork((int)PROTOCOL.AIRMODERECIRC);
				break;
			case PROTOCOL.AIRMODEFRONT:
				//에어모드 버튼
				Debug.Log("AIRMODEFRONT");
				CPanelManager.Instance.NetWork((int)PROTOCOL.AIRMODEFRONT);
				break;
			case PROTOCOL.IVIAIRMODEOFF:
				//20
				//IVI에어모드 OFF 버튼
				Debug.Log("IVIAIRMODEOFF");
				CPanelManager.Instance.NetWork((int)PROTOCOL.IVIAIRMODEOFF);
				break;
			case PROTOCOL.AIRMODEOFF:
				//에어모드 OFF 버튼
				Debug.Log("AIRMODEOFF");
				CPanelManager.Instance.NetWork((int)PROTOCOL.AIRMODEOFF);

				break;
			case PROTOCOL.IVICONTAINER:
				//IVI 컨테이너 클릭
				Debug.Log("IVICONTAINER");

				break;
			case PROTOCOL.IVICONTAINERHUMIDITY:
				//IVI 컨테이너 HUMIDITY 클릭
				Debug.Log("IVICONTAINERHUMIDITY");

				break;
			case PROTOCOL.CONTAINER:
				//컨테이너 스와이프
				CPanelManager.Instance.NetWork((int)PROTOCOL.CONTAINER);
				Debug.Log("CONTAINER");

				break;
			case PROTOCOL.CONTAINERHUMIDITY:
				//25
				//컨테이너 HUMIDITY 클릭
				Debug.Log("CONTAINERHUMIDITY");

				break;

			case PROTOCOL.IVIROUTE1:
				
				//IVIROUTE1 클릭
				Debug.Log("IVIROUTE1");

				break;
			case PROTOCOL.IVIROUTE2:

				//IVIROUTE2 클릭
				Debug.Log("IVIROUTE2");

				break;
			case PROTOCOL.IVIROUTE3:

				//IVIROUTE3 클릭
				Debug.Log("IVIROUTE3");

				break;
			case PROTOCOL.IVIROUTE4:

				//IVIROUTE4 클릭
				Debug.Log("IVIROUTE4");

				break;
			case PROTOCOL.ROUTE1:
				//30
				//ROUTE1 클릭
				Debug.Log("ROUTE1");
				CPanelManager.Instance.NetWork((int)PROTOCOL.ROUTE1);
				break;
			case PROTOCOL.ROUTE2:

				//ROUTE2 클릭
				Debug.Log("ROUTE2");
				CPanelManager.Instance.NetWork((int)PROTOCOL.ROUTE2);

				break;
			case PROTOCOL.ROUTE3:

				//ROUTE3 클릭
				Debug.Log("ROUTE3");
				CPanelManager.Instance.NetWork((int)PROTOCOL.ROUTE3);
				break;
			case PROTOCOL.ROUTE4:

				//ROUTE4 클릭
				Debug.Log("ROUTE4");
				CPanelManager.Instance.NetWork((int)PROTOCOL.ROUTE4);
				break;




			case PROTOCOL.RESTART:

				//리스타트
				//CPanelManager.Instance.NetWork((int)PROTOCOL.RESTART);
				//VideoPanel.Instance.InitialVideoPlayer();

				CPanelManager.Instance.NetWork((int)PROTOCOL.RESTART);

				Debug.Log("RESTART 하는 중....");

				break;
			//case PROTOCOL.IVIPAYLOAD2:

			//	IVIROUTE2 클릭
			//	Debug.Log("IVIPAYLOAD2");

			//	break;
			//case PROTOCOL.IVIPAYLOAD3:

			//	IVIROUTE3 클릭
			//	Debug.Log("IVIPAYLOAD3");

			//	break;
			case PROTOCOL.PAYLOADX:

				//PAYLOADX 클릭
				CPanelManager.Instance.NetWork((int)PROTOCOL.PAYLOADX);
				Debug.Log("PAYLOADX");

				break;
			case PROTOCOL.PAYLOAD1:
				//30
				//ROUTE1 클릭
				Debug.Log("PAYLOAD1");
				CPanelManager.Instance.NetWork((int)PROTOCOL.PAYLOAD1);
				break;
			case PROTOCOL.PAYLOAD2:

				//ROUTE2 클릭
				Debug.Log("PAYLOAD2");
				CPanelManager.Instance.NetWork((int)PROTOCOL.PAYLOAD2);

				break;
			case PROTOCOL.PAYLOAD3:

				//ROUTE3 클릭
				Debug.Log("PAYLOAD3");
				CPanelManager.Instance.NetWork((int)PROTOCOL.PAYLOAD3);
				break;
			case PROTOCOL.PAYLOAD4:

				//ROUTE4 클릭
				Debug.Log("PAYLOAD4");
				CPanelManager.Instance.NetWork((int)PROTOCOL.PAYLOAD4);
				break;



			case PROTOCOL.MUSIC00:

				CPanelManager.Instance.NetWork((int)PROTOCOL.MUSIC00);
				Debug.Log("MUSIC00");
				break;
			case PROTOCOL.MUSIC01:
				CPanelManager.Instance.NetWork((int)PROTOCOL.MUSIC01);
				Debug.Log("MUSIC01");
				break;
			case PROTOCOL.MUSIC02:
				CPanelManager.Instance.NetWork((int)PROTOCOL.MUSIC02);
				Debug.Log("MUSIC02");
				break;
			case PROTOCOL.MUSIC03:
				CPanelManager.Instance.NetWork((int)PROTOCOL.MUSIC03);
				Debug.Log("MUSIC03");
				break;
			case PROTOCOL.MUSIC04:
				CPanelManager.Instance.NetWork((int)PROTOCOL.MUSIC04);
				Debug.Log("MUSIC04");
				break;



			case PROTOCOL.MusicSwipeLeft:
				CPanelManager.Instance.NetWork(47);

				Debug.Log("MusicSwipeLeft");
				break;

			case PROTOCOL.MusicSwipeRight:
				CPanelManager.Instance.NetWork(48);
				Debug.Log("MusicSwipeRight");
				break;

		}

		//Send(PROTOCOL.MSG_OK);
	}

 */
/*
 
	public void Send(PROTOCOL protocol,bool BdEBUG)
	{
		string SendString = "";
		switch (protocol)
		{
			case PROTOCOL.TASK:
				SendString = GetSendString("ID", 0);

				//m_UdpNetwork.Send(StringToByte(SendString), "10.100.0.2", 2222);

				break;
			case PROTOCOL.PAYLOAD:

				//SendString = "{ \"ID\": 2 }";

				//SendString = GetSendString("ID", 2, "PLAY_LIST", 4);
				SendString = GetSendString("ID", 1, "PLAY_LIST", 4.123f);
				Debug.Log(SendString);

				//m_UdpNetwork.Send(StringToByte(SendString), "10.100.0.2", 2222);

				break;

			case PROTOCOL.CLIMATE:
				SendString = GetSendString("ID", 2);

				break;

			case PROTOCOL.MUSIC:
				SendString = GetSendString("ID", 3);

				Debug.Log(SendString);
				break;

			//case PROTOCOL.ENTER:
			//	SendString = GetSendString("ID", 4);
			//	break;

			case PROTOCOL.IVITASKROUTE:
				SendString = GetSendString("ID", 4);
				break;

			case PROTOCOL.IVIPAYLOAD:
				SendString = GetSendString("ID", 5);
				break;

			case PROTOCOL.IVIMUSIC:
				SendString = GetSendString("ID", 6);
				break;

			case PROTOCOL.PLAYMUSIC:
				SendString = GetSendString("ID", 7);
				break;

			case PROTOCOL.IVIDRIVER:
				SendString = GetSendString("ID", 8);
				break;

			case PROTOCOL.IVIPASSENGER:
				SendString = GetSendString("ID", 9);
				break;
			case PROTOCOL.TEMPSLIDER:
				SendString = GetSendString("ID", 10);
				break;
			case PROTOCOL.IVIAIRMODEAUTO:
				SendString = GetSendString("ID", 11);
				break;
			case PROTOCOL.IVIAIRMODEAC:
				SendString = GetSendString("ID", 12);
				break;
			case PROTOCOL.IVIAIRMODERECIRC:
				SendString = GetSendString("ID", 13);
				break;
			case PROTOCOL.IVIAIRMODEFRONT:
				SendString = GetSendString("ID", 14);
				break;
			case PROTOCOL.AIRMODE:
				SendString = GetSendString("ID", 15);
				break;
			case PROTOCOL.AIRMODEAUTO:
				SendString = GetSendString("ID", 16);
				break;
			case PROTOCOL.AIRMODEAC:
				SendString = GetSendString("ID", 17);
				break;
			case PROTOCOL.AIRMODERECIRC:
				SendString = GetSendString("ID", 18);
				break;
			case PROTOCOL.AIRMODEFRONT:
				SendString = GetSendString("ID", 19);
				break;
			case PROTOCOL.IVIAIRMODEOFF:
				SendString = GetSendString("ID", 20);
				break;
			case PROTOCOL.AIRMODEOFF:
				SendString = GetSendString("ID", 21);
				break;
			case PROTOCOL.IVICONTAINER:
				SendString = GetSendString("ID", 22);
				break;
			case PROTOCOL.IVICONTAINERHUMIDITY:
				SendString = GetSendString("ID", 23);
				break;
			case PROTOCOL.CONTAINER:
				SendString = GetSendString("ID", 24);
				break;
			case PROTOCOL.CONTAINERHUMIDITY:
				SendString = GetSendString("ID", 25);
				break;
			case PROTOCOL.IVIROUTE1:
				SendString = GetSendString("ID", 26);
				break;
			case PROTOCOL.IVIROUTE2:
				SendString = GetSendString("ID", 27);
				break;
			case PROTOCOL.IVIROUTE3:
				SendString = GetSendString("ID", 28);
				break;
			case PROTOCOL.IVIROUTE4:
				SendString = GetSendString("ID", 29);
				break;
			case PROTOCOL.ROUTE1:
				SendString = GetSendString("ID", 30);
				break;
			case PROTOCOL.ROUTE2:
				SendString = GetSendString("ID", 31);
				break;
			case PROTOCOL.ROUTE3:
				SendString = GetSendString("ID", 32);
				break;
			case PROTOCOL.ROUTE4:
				SendString = GetSendString("ID", 33);
				break;
			//case PROTOCOL.IVIPAYLOAD1:
			//	SendString = GetSendString("ID", 34);
			//	break;
			//case PROTOCOL.IVIPAYLOAD2:
			//	SendString = GetSendString("ID", 35);
			//	break;
			//case PROTOCOL.IVIPAYLOAD3:
			//	SendString = GetSendString("ID", 36);
			//	break;
			case PROTOCOL.PAYLOADX:
				SendString = GetSendString("ID", 37);
				break;
			case PROTOCOL.PAYLOAD1:
				SendString = GetSendString("ID", 38);
				break;
			case PROTOCOL.PAYLOAD2:
				SendString = GetSendString("ID", 39);
				break;
			case PROTOCOL.PAYLOAD3:
				SendString = GetSendString("ID", 40);
				break;
			case PROTOCOL.PAYLOAD4:
				SendString = GetSendString("ID", 41);
				break;


			case PROTOCOL.MUSIC00:
				SendString = GetSendString("ID", 42);
				break;
			case PROTOCOL.MUSIC01:
				SendString = GetSendString("ID", 43);
				break;
			case PROTOCOL.MUSIC02:
				SendString = GetSendString("ID", 44);
				break;
			case PROTOCOL.MUSIC03:
				SendString = GetSendString("ID", 45);
				break;
			case PROTOCOL.MUSIC04:
				SendString = GetSendString("ID", 46);
				break;


			case PROTOCOL.MusicSwipeLeft:
				SendString = GetSendString("ID", 47);
				break;
			case PROTOCOL.MusicSwipeRight:
				SendString = GetSendString("ID", 48);
				break;


				//CLIMATEDRIVER = 49, //"ID" : 49, "VALUE" : 0~40
    //CLIMATEPASSENGER = 50, //"ID" : 50, "VALUE" : 0~40
    //CLIMATESYNC = 51, //"ID" : 51, "VALUE" : 0~40

    //CLIMATEFANLEFT = 52, //"ID" : 52, "VALUE" : 0~10
    //CLIMATEFANRIGHT = 53, //"ID" : 53, "VALUE" : 0~10

    //CLIMATESEATLEFT = 54, //"ID" : 54, "VALUE" : 0~3
    //CLIMATESEATRIGHT = 55, //"ID" : 55, "VALUE" : 0~3

    //CONTAINERTEMP = 56, //"ID" : 56, "VALUE" : -10~3
    //CONTAINERHUMID = 57 ////"ID" : 57, "VALUE" : 0~100


			case PROTOCOL.CLIMATEDRIVER:
				SendString = GetSendString("ID", 49);
				break;
				//case PROTOCOL.CLIMATEPASSENGER:
				//	SendString = GetSendString("ID", 47);
				//	break;
				//case PROTOCOL.CLIMATESYNC:
				//	SendString = GetSendString("ID", 47);
				//	break;

				//case PROTOCOL.CLIMATEFANLEFT:
				//	SendString = GetSendString("ID", 47);
				//	break;
				//case PROTOCOL.CLIMATEFANRIGHT:
				//	SendString = GetSendString("ID", 47);
				//	break;
				//case PROTOCOL.CLIMATESEATLEFT:
				//	SendString = GetSendString("ID", 47);
				//	break;

				//case PROTOCOL.CLIMATESEATRIGHT:
				//	SendString = GetSendString("ID", 47);
				//	break;
				//case PROTOCOL.CONTAINERTEMP:
				//	SendString = GetSendString("ID", 47);
				//	break;
				//case PROTOCOL.CONTAINERHUMID:
				//	SendString = GetSendString("ID", 47);
				//	break;












		}

		m_UdpNetwork.Send(StringToByte(SendString), SendIP, SendPort);
	}


*/


