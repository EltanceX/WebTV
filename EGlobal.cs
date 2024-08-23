using Engine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.IO;
using System.Text;
using System.Xml;
using Engine.Graphics;
using CefSharp.OffScreen;
using System.Drawing;
using GameEntitySystem;
using System.Collections;
//using OpenTK;
using System.Numerics;

namespace Game
{
    public static class WebTV
    {
        public static ArrayList cefInstances = new ArrayList();
        public static CEF_Browser GetLastTabElement()
        {
            for (int i = cefInstances.Count - 1; i >= 0; i--)
            {
                var cef = (CEF_Browser)cefInstances[i];
                if (cef.Browser != null) return cef;
            }
            return null;
        }
        public static CEF_Browser GetLastElement()
        {
            if (cefInstances.Count == 0) return null;
            CEF_Browser browser = (CEF_Browser)cefInstances[cefInstances.Count - 1];
            //if (willRemove) cefInstances.RemoveAt(cefInstances.Count - 1);
            return browser;
        }
        public static CEF_Browser GetNoneBrowserCef()
        {
            foreach (CEF_Browser item in cefInstances)
            {
                if (item.Browser == null) return item;
            }
            return null;
        }
        public static bool RemoveInstanece(CEF_Browser cef)
        {
            try
            {
                cefInstances.Remove(cef);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static CEF_Browser CreateInstanece()
        {
            CEF_Browser cef = new CEF_Browser();
            cefInstances.Add(cef);
            return cef;
        }
        public static void CloseAll()
        {
            //foreach (CEF_Browser item in cefInstances)
            //{
            //    item.Close();
            //    RemoveInstanece(item);
            //}
            while (cefInstances.Count > 0)
            {
                CEF_Browser toBeClosed = (CEF_Browser)cefInstances[0];
                toBeClosed.Close();
                RemoveInstanece(toBeClosed);
            }
        }
        public static CEF_Browser getInstance()
        {
            if (WebTV.cefInstances.Count > 0)
            {
                return (CEF_Browser)cefInstances[0];
            }
            else
            {
                return null;
            }
        }
        public static class settings
        {
            public static string defaultLink = "https://www.bing.com/search?q=bing";
            public static BrowserWidget KWidget;
            public static bool DebugMode = true;
        }
    }
    public class PerformanceStatistic
    {
        public double startTime;
        public double runningTime;
        public PerformanceStatistic()
        {
            this.startTime = util.getTime();
        }
        public double end()
        {
            return runningTime = util.getTime() - startTime;
        }
    }
    public class Pattern
    {
        public Point3 Point;

        public Vector3 Position;

        public Texture2D Texture;

        public Texture2D DataTexture;

        public string TexName;

        public Vector3 Right;

        public Vector3 Up;

        public Engine.Color Color;

        public float Size;
    }






    public class TickTimer
    {
        public double interval;
        public double LastUpdateTime = 0;

        public TickTimer(double interval = 100)
        {
            this.interval = interval;
        }
        public bool Next()
        {
            var tickCurrent = util.getTime();
            if (tickCurrent - LastUpdateTime >= interval)
            {
                LastUpdateTime = tickCurrent;
                return true;
            }
            return false;
        }
    }
    public enum Direction
    {
        X_Positive,
        X_Negative,
        Y_Positive,
        Y_Negative,
        Z_Positive,
        Z_Negative
    }

    public class EveryFps
    {
        public static void KeyboardOperation()
        {

        }
    }


    //public class DLL
    //{
    //    public class L_WebsocketSharp
    //    {
    //        public static Assembly assembly;
    //    }
    //}
    public class xml
    {
        public static string xmlToString(XmlDocument doc)
        {
            // 将 XML 转换为文本格式
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            doc.WriteTo(writer);
            writer.Flush();
            return sb.ToString();
        }
    }
    public class EGlobal
    {
        public static string Version = "0.1.5";
        public static string Date = "2024-8-21";
        public static string UpdateInfo = @"浏览器支持多标签多任务多角度运行\n新增音频支持";
        //public static void AssemblyInit()
        //{
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //    foreach (var assembly in assemblies)
        //    {
        //        //XLog.Information(assembly.FullName);
        //        switch (assembly.GetName().Name)
        //        {
        //            case "websocket-sharp":
        //                DLL.L_WebsocketSharp.assembly = assembly;
        //                ScreenLog.Info($"[Init] DLL Loaded: " + assembly.FullName);
        //                break;
        //        }
        //    }
        //}

        //public static string name = Process.GetCurrentProcess().ProcessName;
        //public static PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", name);
        //public static PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", name);

        public static Entity entity;
        public static ComponentPlayer Player;
        public static Random random = new Random(273817293);
        public static bool isFirstLoad = true;
        public static SubsystemTerrain terrain;
        public static void place(int x, int y, int z, int value = 2)
        {
            terrain?.ChangeCell(x, y, z, Terrain.MakeBlockValue(value));
        }
        public static void place(float x, float y, float z, int value = 2)
        {
            terrain?.ChangeCell((int)MathUtils.Floor(x), (int)MathUtils.Floor(y), (int)MathUtils.Floor(z), Terrain.MakeBlockValue(value));
        }
        public static ComponentPlayer getPlayer()
        {
            return EGlobal.Player;
        }
        public static void setPlayer(ComponentPlayer player)
        {
            EGlobal.Player = player;
        }
    }
    public class HammerData
    {
        public static int SteelHammerValue = 115;
        public static int DiamondHammerValue = 0;
        public static Game.GameMode GameMode;
        public static bool isDebug = true;
    }
    public class util
    {
        public static Vector3 GetUnitVector(Vector3 vec3)
        {
            var len = vec3.Length();
            return new Vector3(vec3.X / len, vec3.Y / len, vec3.Z / len);
        }
        private static System.Random random = new();
        public static double RandomD(double minimum, double maximum, int Len = 1)   //Len小数点保留位数
        {
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, Len);
        }
        public static float RandomF(float minimum, float maximum, int Len = 1)   //Len小数点保留位数
        {
            return (float)Math.Round(random.NextDouble() * (maximum - minimum) + minimum, Len);
        }
        public static string Progress(double per, int len)
        {
            if (per > 1) return per.ToString();
            return $"[{new String(' ', (int)(len * (1 - per))).PadLeft(len, '|')}]";
        }
        public static float VectorProjection(Vector3 vec, Vector3 target)
        {
            return (vec.X * target.X + vec.Y * target.Y + vec.Z * target.Z) / MathUtils.Sqrt(MathUtils.Pow(target.X, 2) + MathUtils.Pow(target.Y, 2) + MathUtils.Pow(target.Z, 2));
        }
        public static string uuid()
        {
            return Guid.NewGuid().ToString();
        }
        public static double getTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (dateTime - startTime).TotalMilliseconds;

        }
        //public static Vector3 crossProduct(Vector3 a, Vector3 b)
        //{
        //    return new Vector3(
        //        a.Y * b.Z - a.Z * b.Y,
        //        a.Z * b.X - a.X * b.Z,
        //        a.X * b.Y - a.Y * b.X
        //    );
        //}
        public static Vector3[] GetUVByPlaneNormal(Vector3 planeNormal)
        {
            Vector3 u = new Vector3(1, 0, 0);
            //if (MathUtils.Abs(planeNormal.X) < 1e-10 && MathUtils.Abs(planeNormal.Y) < 1e-10)
            //{
            //    u = new Vector3(1, 0, 0);  // If b is close to (0, 0, 1), use x-axis
            //}
            //else
            //{
            //    u = new Vector3(0, 0, 1);  // Otherwise, use z-axis
            //}
            if (planeNormal.X >= 0 && planeNormal.Z <= 0 || planeNormal.X <= 0 && planeNormal.Z <= 0) u = new Vector3(1, 0, 0); //+- --
            else if (planeNormal.X >= 0 && planeNormal.Z >= 0 || planeNormal.X <= 0 && planeNormal.Z >= 0) u = new Vector3(-1, 0, 0); //++ -+
            //u = new Vector3(-1, 0, 0);  // Otherwise, use z-axis

            u = Vector3.Cross(u, planeNormal);

            u = Vector3.Normalize(u);


            Vector3 v = Vector3.Cross(u, planeNormal);
            v = Vector3.Normalize(v);
            //ScreenLog.Info($"U: {u.X} {u.Y} {u.Z} || V: {v.X} {v.Y} {v.Z}");
            return new Vector3[] { u, v };
        }
    }
}