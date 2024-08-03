using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Engine;
using WebSocketSharp;

//using Newtonsoft.Json;

namespace Game
{

    //public class websocket
    //{
    //    public static void connect()
    //    {
    //        //if (DLL.L_WebsocketSharp.assembly == null) throw new Exception("DLL Missing: WebsocketSharp");
    //        //var sharp = DLL.L_WebsocketSharp.assembly;
    //        //var ws = sharp.GetType("WebSocketSharp.WebSocket");
    //        ////var a = sharp.CreateInstance("WebSocket",);
    //        //var a = Activator.CreateInstance(ws, "ws://81.68.88.42:9931");
    //        ////var a = new WebSocketSharp.WebSocket("ws://81.68.88.42:9931");
    //        //MethodInfo m = a.GetType().GetMethod("Connect");
    //        //m.Invoke(a, new object[0]);
    //        //ScreenLog.Info("2023-12-29");
    //        //ScreenLog.Info(ws);
    //        //ScreenLog.Info(a);



    //        //ScreenLog.Info("2023-12-30 00");

    //        //var ws = new WebSocketSharp.WebSocket("ws://81.68.88.42:9931");
    //        //ws.Connect();
    //        //ws.OnError += (object sender, ErrorEventArgs e) =>
    //        //{
    //        //    ScreenLog.Info(e.Message);
    //        //};
    //        //ScreenLog.Info(ws);
    //        //ScreenLog.Info("2023-12-30 11");
    //    }
    //}
    public class GameWebsocket
    {
        public static websocket Opened;
    }
    public class websocket : WebSocketSharp.WebSocket
    {

        //源码修改: Logger.cs 
        //         [+] websocket.m_log
        public delegate void cmdCallback(XmlDocument data, Exception e);
        public Dictionary<string, cmdCallback> cmdQueue = new();
        public string Address;
        public enum MessageType
        {
            Message,
            Connect,
            None,
            PlayerMessage
        }
        public static string uuidv4() { return System.Guid.NewGuid().ToString(); }

        public static void m_log(object o)
        {
            //Console.WriteLine(o);
            ScreenLog.Info(o);
        }
        public static XmlDocument getStandardFormat(string type, string data, string UUID = null)
        {
            if (UUID == null) UUID = uuidv4();
            var xml = new XmlDocument();
            XmlDeclaration declare = xml.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xml.AppendChild(declare);
            var el_root = xml.CreateElement("SCWS");
            xml.AppendChild(el_root);
            var el_type = xml.CreateElement("Type");
            el_type.InnerText = type;
            el_root.AppendChild(el_type);
            var el_UUID = xml.CreateElement("UUID");
            el_UUID.InnerText = UUID;
            el_root.AppendChild(el_UUID);
            var el_Data = xml.CreateElement("Data");
            el_Data.InnerText = data;
            //el_Data.
            el_root.AppendChild(el_Data);
            //xml.CreateAttribute("id", "7777");
            //el_root.SetAttribute("idd", "888");
            return xml;
        }

        public websocket(string url = "ws://81.68.88.42:9931") : base(url)
        {
            //new WebSocketSharp.WebSocket()
            //socket = new WebSocket("ws://81.68.88.42:9931");
            Address = url;
            this.Connect();
            this.EmitOnPing = true;
            this.OnMessage += onMessage;
            this.OnError += onError;
            this.OnClose += onClose;
            //socket.OnError
            //raw_send("666");
            if (this.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                ScreenLog.Info($"已建立服务器连接: {this.Address}");
                send("Connection established. Hello, Server!", MessageType.Connect);
            }
        }
        //public override void Logger()
        //{

        //}
        public void onClose(object sender, CloseEventArgs e)
        {
            GameWebsocket.Opened = null;
            ScreenLog.Info($"Websocket {this.Address} Closed [Code {e.Code}] [R: {e.Reason}]");
        }
        public void onMessage(object sender, MessageEventArgs e)
        {
            //m_log(sender);
            //m_log(e);
            //m_log($"[{Address}] {e.Data}");
            if (e.IsPing)
            {
                m_log("Ping");
                return;
            }
            //var error = false;
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(e.Data);
                string uuid = doc.GetElementsByTagName("UUID")[0].InnerText;
                string type = doc.GetElementsByTagName("Type")[0].InnerText;
                string data = doc.GetElementsByTagName("Data")[0].InnerText;
                bool isret = type == "Return";
                m_log($"[{Address}] Type: {type}, UUID: {uuid}");
                switch (type)
                {
                    case "CommandRequest":
                        CommandInput.Exec(data, true);
                        break;
                }

                if (isret)
                {
                    cmdCallback func = cmdQueue[uuid];
                    func.Invoke(doc, null);
                    m_log($"Remove [{uuid}]: {cmdQueue.Remove(uuid)}");
                    m_log($"Queue Length: " + cmdQueue.Count);
                }
            }
            catch (Exception ex)
            {
                m_log(ex);
                return;
            }
        }
        public void onError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            m_log("<Error Event> Error!");
            m_log(e.Message);
        }
        public void raw_send(string data)
        {
            try
            {
                this.Send(data);
            }
            catch (Exception e)
            {
                m_log(e);
            }
        }
        public void send(string data, MessageType type = MessageType.None)
        {
            //m_log($"<send1> {type}, {data}");
            raw_send(xml.xmlToString(getStandardFormat(type.ToString(), data)));
        }
        public void send(string data, cmdCallback callback, MessageType type = MessageType.None)
        {
            var uuid = uuidv4();
            //m_log($"<send2> {type}, {data}, {uuid}");
            raw_send(xml.xmlToString(getStandardFormat(type.ToString(), data, uuid)));
            cmdQueue.Add(uuid, callback);
        }
    }

    //public class websocket
    //{
    //    public static void test()
    //    {
    //        //try
    //        //{
    //        //    Assembly Websocket4Net = Assembly.LoadFrom("C:\\Users\\KJZ\\Desktop\\Websocket4Net.dll");
    //        //    //Assembly.LoadFrom()
    //        //    DLL.Websocket4Net = Websocket4Net;
    //        //    //WebSocket4Net.webso
    //        //    Websocket = Websocket4Net.GetType("Websocket");
    //        //    MessageReceivedEventArgs = Websocket4Net.GetType("MessageReceivedEventArgs");
    //        //    XLog.Information("-----load ok");
    //        //    XLog.Information(Websocket4Net);

    //        //}
    //        //catch (Exception e)
    //        //{
    //        //    XLog.Information(e);
    //        //}


    //        XLog.Information("---- List of all dependency files ----");
    //        var dependency = AppDomain.CurrentDomain.GetAssemblies();

    //        //dependency[0].
    //        foreach (var dll in dependency)
    //        {
    //            XLog.Information(dll.GetName());
    //            //if (dll.GetName().Name == "WebSocket4Net")
    //            //{
    //            //    try
    //            //    {
    //            //        DLL.Websocket4Net.dll = dll;
    //            //        //new WebSocket4Net.WebSocket("",);
    //            //        //DLL.Websocket4Net.WebSocket = dll.GetType("WebSocket", true, true);
    //            //        //XLog.Information(DLL.Websocket4Net.WebSocket == null);
    //            //        //XLog.Information(dll.GetType("WebSocket4Net") == null);
    //            //        var ts = dll.GetTypes();
    //            //        foreach (var i in ts)
    //            //        {
    //            //            if (i.Name == "WebSocket")
    //            //            {
    //            //                DLL.Websocket4Net.WebSocket = i;
    //            //                XLog.Information("77777");
    //            //            }
    //            //            else if (i.Name == "MessageReceivedEventArgs")
    //            //            {
    //            //                DLL.Websocket4Net.MessageReceivedEventArgs = i;
    //            //                XLog.Information("8888888");
    //            //            }
    //            //            else if (i.Name == "WebSocketVersion")
    //            //            {
    //            //                DLL.Websocket4Net.WebSocketVersion = i;

    //            //            }
    //            //            XLog.Information(i.Name);
    //            //            XLog.Information(i.Namespace);
    //            //        }

    //            //        XLog.Information("上面是gettype");
    //            //        DLL.Websocket4Net.IsLoaded = true;
    //            //        XLog.Information("--WebSocket4Net Loaded");
    //            //        XLog.Information(dll);
    //            //    }
    //            //    catch (Exception e)
    //            //    {
    //            //        DLL.Websocket4Net.IsLoaded = false;
    //            //        XLog.Error(e);
    //            //        ScreenLog.Info(e);
    //            //    }
    //            //}
    //        }
    //        XLog.Information("---- List of all dependency files ----");
    //        //XLog.Information(new Fleck.WebSocketException(0));
    //        XLog.Information(new Newtonsoft.Json.JsonException());
    //        //new WebSocketService();
    //        //try
    //        //{
    //        //    XLog.Information(new WebSocket4Net.WebSocketVersion());
    //        //}
    //        //catch (Exception e)
    //        //{
    //        //    Log.Information(e);
    //        //}
    //    }
    //}

    //public partial class WebSocketService
    //{
    //    public delegate void AlarmEventHandler(string sender); //声明关于事件的委托,参数为要回传的数据类型
    //                                                           //public event AlarmEventHandler Alarm; //声明事件
    //    private WebSocket4Net.WebSocket webSocket4Net;
    //    public WebSocketService(string IP = "ws://81.68.88.42:9931")
    //    {
    //        if (webSocket4Net == null)
    //        {
    //            //try
    //            //{
    //            //    //new WebSocket4Net.WebSocket(IP).Open();//
    //            //    //typeof(a);
    //            //    XLog.Information(DLL.Websocket4Net.WebSocketVersion.GetEnumName(-1));
    //            //    webSocket4Net = Activator.CreateInstance(DLL.Websocket4Net.WebSocket, IP, "", null);
    //            //    MethodInfo m = webSocket4Net.GetType().GetMethod("Open");
    //            //    XLog.Information(m);
    //            //    m.Invoke(webSocket4Net, new object[0]);
    //            //    XLog.Information("oooook");
    //            //    XLog.Information(webSocket4Net);
    //            //    //       var eventInfo = webSocket4Net.GetType().GetEvent("Error");
    //            //    //       Delegate handler =
    //            //    //Delegate.CreateDelegate(eventInfo.EventHandlerType,
    //            //    //                        webSocket4Net,
    //            //    //                        TestMethod);
    //            //    //       eventInfo.AddEventHandler(webSocket4Net, delegate (SuperSocket.ClientEngine.ErrorEventArgs e)
    //            //    //       {
    //            //    //           //XLog.Information(err.ToString());
    //            //    //           XLog.Information(e.ToString());
    //            //    //       });
    //            //}
    //            //catch (Exception e)
    //            //{
    //            //    XLog.Information("xxxxxx");

    //            //    XLog.Information(e);
    //            //}
    //            webSocket4Net = new WebSocket4Net.WebSocket(IP);
    //            webSocket4Net.Opened += WebSocket4Net_Opened;
    //            //webSocket4Net.GetEvent().ad(WebSocket4Net_Opened);
    //            webSocket4Net.MessageReceived += WebSocket4Net_MessageReceived;
    //            webSocket4Net.Open();
    //            webSocket4Net.Error += WebSocket4Net_Error;
    //            webSocket4Net.Closed += WebSocket4Net_Close;
    //            ScreenLog.Info(webSocket4Net.Version);
    //        }
    //    }
    //    public void WebSocket4Net_Error(object err, EventArgs e)
    //    {
    //        XLog.Information(err.ToString());
    //        XLog.Information(e.ToString());
    //    }
    //    public void WebSocket4Net_Close(object err, EventArgs e)
    //    {

    //        XLog.Information("close");
    //        XLog.Information(err.ToString());
    //        XLog.Information(e.ToString());
    //    }
    //    public void ClientSendMsgToServer(object message)
    //    {
    //        XLog.Information(message);

    //        webSocket4Net.Send(message.ToString());
    //    }
    //    private void WebSocket4Net_MessageReceived(object sender, MessageReceivedEventArgs e)
    //    {
    //        XLog.Information($"{sender}: {e.Message}");
    //    }
    //    private void WebSocket4Net_Opened(object sender, EventArgs e)
    //    {
    //        webSocket4Net.Send($"客户端准备发送数据！");
    //    }
    //}

}
