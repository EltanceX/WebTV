using CefSharp;
using Engine;
using Engine.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class CommandParsed
    {
        public Dictionary<string, string> opts;
        public string origin;
        public string header;
        public string body;
        public ArrayList arr;
        public CommandParsed(string str)
        {
            this.origin = str;
        }
    }
    public class CommandInput
    {
        public static CommandParsed ParseCommand(string str)
        {
            var cmdp = new CommandParsed(str);
            cmdp.origin = str;
            var opts = new Dictionary<string, string>();
            ArrayList arr = new ArrayList();
            string optIndex = null;
            for (var i = 0; i < str.Length;)
            {
                int space = str.IndexOf(' ', i);
                if (space == -1) space = str.Length;
                string s = str.Substring(i, space - i);
                if (s == "")
                {
                    i++;
                    continue;
                }
                if (s[0] == '*')
                {
                    string optheader = s.Substring(1, s.Length - 1);
                    try
                    {
                        opts.Add(optheader, "true");
                    }
                    catch { }
                    optIndex = optheader;
                    i = space + 1;
                    continue;
                }
                else if (optIndex != null)
                {
                    opts[optIndex] = s;
                    optIndex = null;
                    i = space + 1;
                    continue;
                }
                if (cmdp.header == null)
                {
                    cmdp.header = s;
                    if (space + 1 > str.Length) cmdp.body = "";
                    else cmdp.body = str.Substring(space + 1, str.Length - space - 1);
                }
                else arr.Add(s);
                i = space + 1;
            }

            cmdp.arr = arr;
            cmdp.opts = opts;
            return cmdp;
        }
        public static void Exec(string cmd, bool isExternal)
        {
            var Player = EGlobal.Player;
            var playername = Player.PlayerData.Name;
            if (!isExternal) ScreenLog.Info($"<{playername}> {cmd}");
            if (cmd[0] != '/' && GameWebsocket.Opened != null)
            {
                GameWebsocket.Opened.send(cmd, websocket.MessageType.PlayerMessage);
            }
            if (cmd[0] != '/') return;
            var parsed = ParseCommand(cmd);
            var header = parsed.header;
            switch (header.ToLower())
            {
                case "/help":
                    int page = 1;
                    try
                    {
                        page = int.Parse(parsed.arr[0].ToString());
                    }
                    catch
                    {
                        page = 1;
                    }
                    switch (page)
                    {
                        case 1:
                            ScreenLog.Info(@"-- 显示帮助手册总 2 页中的第 1 页(/help [页码]) --
/help                显示帮助页面
/connect [IP]        尝试在所提供的 URL 上链接到 Websocket 服务器
         out         断开与 Websocket 服务器的链接
/clear               清空聊天栏
/title [Content]     向玩家展示标题
/gamemode [Int]      切换游戏模式 [0]Creative [1]Harmless [2]Survival
                                 [3]Challenging [4]Cruel [5]Adventure
/heal [Health:Float] 治疗玩家
/kill                杀死玩家
/spawnpoint          设置玩家的出生点为当前位置
/version             显示游戏版本
/about               关于ETerminal
/dll                 列出所有已加载的程序集
-- 查看下一页：/help 2 --");
                            break;
                        case 2:
                            ScreenLog.Info(@"-- 显示帮助手册总 2 页中的第 2 页(/help [页码]) --
/setblock [x: Int][y: Int][z: Int] [Value: Int] 在世界中放置方块
/tp [x: Float][y: Float][z: Float]              在世界中传送
/say [Message]              发送聊天信息
/destroy [x: Int] [y:Int] [z: Int]              在世界中破坏方块
/cef stop      停止浏览器
     new [url] 在浏览器上加载链接
");
                            break;
                        default:
                            ScreenLog.Warn($"页码 {page} 超出范围。");
                            break;
                    }

                    //                    / time[]

                    break;
                case "/setblock":
                    {
                        try
                        {
                            int x = int.Parse(parsed.arr[0].ToString());
                            int y = int.Parse(parsed.arr[1].ToString());
                            int z = int.Parse(parsed.arr[2].ToString());
                            int value = int.Parse(parsed.arr[3].ToString());
                            EGlobal.Player.m_subsystemTerrain.ChangeCell(x, y, z, value);
                            ScreenLog.Info($"已将坐标位置为 {x}, {y}, {z} 的方块Value设置为: {value}");
                        }
                        catch (Exception e)
                        {
                            ScreenLog.Info(e);
                            ScreenLog.Info("命令参数错误!");
                        }
                    }
                    break;
                case "/destroy":
                    {
                        try
                        {
                            int x = int.Parse(parsed.arr[0].ToString());
                            int y = int.Parse(parsed.arr[1].ToString());
                            int z = int.Parse(parsed.arr[2].ToString());
                            int value = int.Parse(parsed.arr[3].ToString());
                            EGlobal.Player.m_subsystemTerrain.DestroyCell(10, x, y, z, 0, false, false);
                            ScreenLog.Info($"已将摧毁位于 {x}, {y}, {z} 的方块");
                        }
                        catch (Exception e)
                        {
                            ScreenLog.Info(e);
                            ScreenLog.Info("命令参数错误!");
                        }
                    }
                    break;
                case "/say":
                    ScreenLog.Info((isExternal ? "[External] " : $"<{playername}> ") + parsed.body);
                    break;
                case "/tp":
                    try
                    {
                        float x = float.Parse(parsed.arr[0].ToString());
                        float y = float.Parse(parsed.arr[1].ToString());
                        float z = float.Parse(parsed.arr[2].ToString());
                        Player.ComponentBody.m_position.X = x;
                        Player.ComponentBody.m_position.Y = y;
                        Player.ComponentBody.m_position.Z = z;
                        ScreenLog.Info($"已将玩家 {playername} 传送至 {x} {y} {z}");
                    }
                    catch (Exception e)
                    {
                        ScreenLog.Warn(e);
                        ScreenLog.Warn("无效的坐标参数，格式: /tp [x] [y] [z]");
                    }
                    break;
                case "/spawnpoint":
                    var pos = Player.ComponentBody.Position;
                    Player.PlayerData.SpawnPosition = pos;
                    ScreenLog.Info($"将玩家 {playername} 的 spawnpoint 设置为: {pos.X} {pos.Y} {pos.Z}");
                    break;
                case "/version":
                    ScreenLog.Info($"Survival Craft: {Game.VersionsManager.Version}");
                    ScreenLog.Info($"ETerminal: {EGlobal.Version} [{EGlobal.Date}]");
                    break;
                case "/about":
                    ScreenLog.Info(@"WebTV by EltanceX 内置浏览器模组
ETerminal 控制台组件
按键绑定: [F3]实时信息 [F1]日志 [UP]上滚 [Down]下滚 [/]聊天和命令
禁用字体： 删除FontTemplate.dll
使用钻石镐采集石头以触发3x3效果。
本次更新: " + EGlobal.UpdateInfo);
                    break;
                case "/dll":
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        ScreenLog.Info(assembly.FullName);
                    }
                    ScreenLog.Info("---- List of all loaded dependency files ----");
                    break;
                case "/kill":
                    EGlobal.Player.Entity.FindComponent<ComponentHealth>(throwOnError: true).Health = -1;
                    ScreenLog.Info($"已清除: {playername}");
                    break;
                case "/title":
                    string content;
                    try
                    {
                        content = parsed.body;
                    }
                    catch { content = "null"; }
                    ScreenLog.Info($"向玩家 {playername} 的 actionbar 展示: {content}");
                    EGlobal.getPlayer().ComponentGui.DisplaySmallMessage(content, Color.White, true, true);
                    break;
                case "/heal":
                    float heal = 1;
                    if (parsed.arr.Count != 0)
                    {
                        try
                        {
                            heal = float.Parse(parsed.arr[0].ToString());
                        }
                        catch (Exception e)
                        {
                            ScreenLog.Info(e);
                            ScreenLog.Info("错误: 不是一个浮点数");
                            return;
                        }
                    }
                    EGlobal.Player.Entity.FindComponent<ComponentHealth>(throwOnError: true).Health += heal;
                    ScreenLog.Info($"已治愈玩家 {playername} 的血量: {heal}");
                    break;
                case "/gamemode":
                    string mode;
                    try { mode = parsed.arr[0].ToString(); }
                    catch
                    {
                        mode = "0";
                    }
                    switch (mode.ToLower())
                    {
                        case "0":
                        case "creative":
                            Player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Creative;
                            break;
                        case "1":
                        case "harmless":
                            Player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Harmless;
                            break;
                        case "2":
                        case "survival":
                            Player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Survival;
                            break;
                        case "3":
                        case "challenging":
                            Player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Challenging;
                            break;
                        case "4":
                        case "Cruel":
                            Player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Cruel;
                            break;
                        case "5":
                        case "Adventure":
                            Player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Adventure;
                            break;
                    }
                    ScreenLog.Info($"已将玩家 {playername} 的游戏模式设置为: {Player.m_subsystemGameInfo.WorldSettings.GameMode}");

                    break;
                //case "/pngdef":
                //    MemoryStream pngStream = new();
                //    System.Drawing.Image image = System.Drawing.Bitmap.FromFile("D:\\SurvivalCraft\\Temp\\icon.png");
                //    image.Save(pngStream, ImageFormat.Png);
                //    pngStream.Position = 0;
                //    ScreenLog.Info(string.Join(" ", pngStream.GetBuffer()));
                //    ScreenLog.Info(pngStream.Length);
                //    ScreenLog.Info(pngStream.GetBuffer().Length);
                //    browserData.p.Texture = Texture2D.Load(pngStream);
                //    pngStream.Dispose();
                //    image.Dispose();
                //    break;
                //case "/webbitmap":
                //    MemoryStream pngStream2 = new();
                //    browserData.bitmap.Save(pngStream2, ImageFormat.Png);
                //    pngStream2.Position = 0;
                //    browserData.p.Texture = Texture2D.Load(pngStream2);
                //    break;
                //case "/stop":
                //    browserData.browser.GetBrowser().CloseBrowser(true);
                //    browserData.browser.Dispose();
                //    browserData.browser = null;
                //    break;
                case "/cef":
                    string parameter1;
                    try
                    {
                        parameter1 = parsed.arr[0].ToString();
                    }
                    catch (Exception e)
                    {
                        ScreenLog.Info("错误的命令参数！");
                        return;
                    }
                    switch (parameter1)
                    {
                        case "create":

                            break;
                        case "new":

                            string cef_url = "www.baidu.com";
                            try
                            {
                                cef_url = parsed.arr[1].ToString();
                            }
                            catch
                            {
                                ScreenLog.Info("URL格式错误！自动设置为www.baidu.com");
                            }
                            if (browserData.Browser != null)
                            {
                                ScreenLog.Info("浏览器已存在！");
                                browserData.Browser.Load(cef_url);
                                return;
                            }
                            browserData.Create(cef_url);
                            break;
                        case "stop":
                            browserData.Browser.Load("about:blank");
                            break;
                        case "close":
                            browserData.Close();
                            break;
                        default:
                            ScreenLog.Info("未知的参数类型！");
                            break;
                    }
                    break;
                //case "/999":
                //    CEF_Browser.CreateBrowser("https://www.bilibili.com");
                //    //Task.Run(async delegate
                //    //{
                //    //    try
                //    //    {
                //    //        await Program2.MainB(new string[1]);
                //    //    }
                //    //    catch (Exception e)
                //    //    {
                //    //        Log.Information(e);
                //    //        ScreenLog.Info(e);
                //    //    }
                //    //}).ContinueWith(delegate
                //    //{
                //    //    ScreenLog.Info("Async Task: \"ws.Open()\" finished.");
                //    //});
                //    break;
                case "/connect":
                    //var url = cmd.Substring(9, cmd.Length - 9);
                    var url = (string)parsed.arr[0];
                    //ScreenLog.Info($"URL: {url}");
                    if (url == "out")
                    {
                        if (GameWebsocket.Opened == null)
                        {
                            ScreenLog.Info("Error: There is NO esisting websockets.");
                            return;
                        }
                        //待测
                        GameWebsocket.Opened.Close();
                        GameWebsocket.Opened = null;
                        //...null
                        return;
                    }
                    if (GameWebsocket.Opened != null)
                    {
                        ScreenLog.Info("Error: There has been an existing websocket connection !");
                    }
                    Task.Run(delegate
                    {
                        try
                        {
                            var ws = new websocket(url);
                            GameWebsocket.Opened = ws;
                        }
                        catch (Exception e)
                        {
                            ScreenLog.Error(e);
                        }
                    }).ContinueWith(delegate
                    {
                        ScreenLog.Info("Async Task: \"ws.Open()\" finished.");
                    });
                    //var ws = new websocket(url);
                    //GameWebsocket.Opened = ws;

                    break;
                case "/clear":
                    ScreenLog.logs.Clear();
                    ScreenLog.currentLine = 0;
                    ScreenLog.Refresh();
                    ScreenLog.Info("已清屏");
                    break;
                default:
                    ScreenLog.Info($"未知的命令: {header}。\n请检查命令是否存在，执行/help以显示帮助菜单。");
                    break;
            }

        }
    }
}
