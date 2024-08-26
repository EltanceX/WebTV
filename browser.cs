//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Web02
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//        }
//    }
//}
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;

using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using System.Threading;

using System.Threading.Tasks;

using CefSharp;
using CefSharp.DevTools.DOM;
//using CefSharp.DevTools.DOM;
using CefSharp.DevTools.Page;
using CefSharp.Enums;
//using CefSharp.DevTools.SystemInfo;
using CefSharp.OffScreen;
using CefSharp.Structs;
using Rect = CefSharp.Structs.Rect;
//using SixLabors.ImageSharp;
using Size = CefSharp.Structs.Size;
using Point = CefSharp.Structs.Point;
//using SixLabors.ImageSharp.Advanced;
//using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using System.Drawing.Imaging;
using Engine.Graphics;
//using static ConsoleApp_HtmlScreenshot.Program;
using System.Runtime.ConstrainedExecution;
using Viewport = CefSharp.DevTools.Page.Viewport;
using Game;
using CefSharp.Core;
using static Game.ComponentMap;
using Engine;
//using System.range
//using System.Drawing;
//[DllImport("CefSharp.Core.Runtime.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
//static extern string GetFileData(string fileName);


namespace Game
{
    public class browserData
    {

        //public static TickTimer MessageTick = new TickTimer(1000);


        //public static MemoryStream memoryStream = new();

        //public static bool Close()
        //{
        //    try
        //    {
        //        if (Browser == null || p == null) return false;
        //        Browser.GetBrowser().CloseBrowser(true);
        //        Browser.Dispose();
        //        Browser = null;
        //        p.Texture.Dispose();
        //        p = null;
        //        initPreview = false;
        //        //Cef.Shutdown();
        //        //Cef.
        //    }
        //    catch (Exception e)
        //    {
        //        ScreenLog.Info(e);
        //    }
        //    return true;
        //}
        //public static bool Create(string url = null)
        //{
        //    try
        //    {
        //        EGlobal.Player.ComponentGui.DisplaySmallMessage("正在依据预览信息创建浏览器...", Engine.Color.Orange, false, true);
        //        CEF_Browser.CreateBrowser(url == null ? link : url);
        //    }
        //    catch (Exception e2)
        //    {
        //        ScreenLog.Info(e2);
        //        return false;
        //    }
        //    return true;
        //}
    }
    public class CEF_Browser

    {
        public ChromiumWebBrowser Browser;
        public string link = "https://www.bing.com/search?q=bing";
        public Pattern pattern;
        public bool initPreview = false;
        public bool updated = false;
        public double lastUpdatedTime = 0;
        public bool isTextureDrawingCompleted = true;
        public bool isFocused = true;
        public float IngameHeight = 6;
        public float IngameWidth = 10;
        public bool hasMouseDown = false;
        public bool hasRightMouseDown = false;
        public Vector3 ScreenNormalVector = new Vector3(0, 0, 1);
        public Vector3 posVec = new Vector3(105, 75, 189);
        public Bitmap bitmap;
        public int height = 600;
        public int width = 1000;
        public bool MouseEventEnabled = true;

        public Vector3 U;//Normal vector
        public Vector3 V;//...
        public float DiagonalLength;



        public bool Close()
        {
            if (Browser == null && pattern == null) return false;
            try
            {
                if (Browser == null && pattern != null)
                {
                    pattern.Texture.Dispose();
                    pattern = null;
                    initPreview = false;
                    WebTV.RemoveInstanece(this);
                    return true;
                }
                Browser.GetBrowser().CloseBrowser(true);
                Browser.Dispose();
                Browser = null;
                pattern.Texture.Dispose();
                pattern = null;
                initPreview = false;
                WebTV.RemoveInstanece(this);
                return true;
                //Cef.Shutdown();
            }
            catch (Exception e)
            {
                ScreenLog.Info(e);
                return false;
            }
        }
        public bool Create(string url = null)
        {
            try
            {
                EGlobal.Player.ComponentGui.DisplaySmallMessage("正在依据预览信息创建浏览器...", Engine.Color.Orange, false, true);
                this.CreateBrowser(url == null ? link : url);
            }
            catch (Exception e2)
            {
                ScreenLog.Info(e2);
                return false;
            }
            return true;
        }
        public void updateTexture()
        {

            //ScreenLog.Info("Render_OnDraw");
            this.isTextureDrawingCompleted = false;
            try
            {
                PerformanceStatistic performance = new PerformanceStatistic();
                //MemoryStream pngStream2 = new();
                //browserData.bitmap = ImageScaler.ScaleImage(browserData.bitmap, 0.5f);
                //this.bitmap.Save(pngStream2, ImageFormat.Png);
                //pngStream2.Position = 0;
                //if (WebTV.settings.DebugMode)
                //{
                //    performance.end();
                //    ScreenLog.Info($"Copying time: {Math.Round(performance.runningTime, 2)}ms {Math.Round(1000 / performance.runningTime, 1)}fps");
                //}


                //browserData.p.Texture.Dispose();

                //Texture2D.Load(pngStream2);//.Dispose();
                //browserData.p.Texture = Texture2D.Load(pngStream2);

                var tex = this.pattern.Texture;
                //Engine.Media.Image image = Engine.Media.Image.Load(pngStream2);


                var image2 = new Engine.Media.Image(width, height);
                System.Drawing.Rectangle rect = new(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
                byte[] pixelValues = new byte[bytes];
                Marshal.Copy(bmpData.Scan0, pixelValues, 0, bytes);
                //image2.Pixels = bitmap. //readonly
                var bmpSize = bitmap.Width * bitmap.Height;
                Engine.Color[] Pixels = new Engine.Color[bmpSize];
                for (int i = 0; i < bmpSize; i++)
                {
                    int start = i * 4;
                    Pixels[i] = new Engine.Color(pixelValues[start + 2], pixelValues[start + 1], pixelValues[start], pixelValues[start + 3]);
                }//~20ms


                //Engine.Media.Image[] array = Engine.Media.Image.GenerateMipmaps(image, 1).ToArray();
                //ScreenLog.Info($"{tex.Height} {tex.Width} {tex.Height * tex.Width}");
                tex.SetData(0, Pixels);
                //for (int i = 0; i < array.Length; i++)
                //{
                //    tex.SetData(i, array[i].Pixels);
                //}
                if (WebTV.settings.DebugMode)
                {
                    performance.end();
                    ScreenLog.Info($"Drawing time: {Math.Round(performance.runningTime, 2)}ms {Math.Round(1000 / performance.runningTime, 1)}fps");
                }



                bitmap.UnlockBits(bmpData);
                this.bitmap.Dispose();
                //pngStream2.Dispose();
            }
            catch (Exception e)
            {

                ScreenLog.Info("Bitmap Render Error " + e);
            }
            this.isTextureDrawingCompleted = true;
        }
        public class CefLifeSpanHandler : ILifeSpanHandler
        {
            public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                //ScreenLog.Info("OnAfterCreated");

                //chromiumWebBrowser.Load("about:blank");

            }
            public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                //ScreenLog.Info("DoClose");
                return true;
            }
            //省略其他ILifeSpanHandler接口实现方法...
            public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                //ScreenLog.Info("OnBeforeClose");
            }

            //构造函数传入窗口对象,方便控制窗口中的控件
            //MainWindow mainWindow;
            public CefLifeSpanHandler()
            {
                //this.mainWindow = _mainWindow;
            }
            //浏览器新窗口弹出时调用方法
            public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
            {
                //chromiumWebBrowser.Load("about:blank");
                //chromiumWebBrowser.GetBrowserHost().CloseBrowser(true);
                chromiumWebBrowser.Load(targetUrl);
                //ScreenLog.Info("OnBeforePopup" + targetUrl);
                //为新弹窗创建一个浏览器对象
                //ChromiumWebBrowser newChromeBrowser = new ChromiumWebBrowser();
                //为新弹窗复制窗口处理
                //newBrowser.LifeSpanHandler = this;
                //省略赋值其他处理器...

                //赋值标题修改事件
                //newChromeBrowser.TitleChanged += mainWindow.Chrome_TitleChange;
                //省略赋值其他事件....

                //留下创建的浏览器对象引用,windowList(List<ChromiumWebBrowser>)是在MainWindow下声明的变量，窗口关闭时要释放,不然退出程序会跨线程访问控件错误
                //mainWindow.windowList.Add(newChromeBrowser);
                //为浏览器赋值
                //newBrowser = newChromeBrowser;

                //....
                newBrowser = null;
                return true;//禁止弹出新窗口
            }
        }
        public class RenderHandler2 : IRenderHandler
        {
            public ChromiumWebBrowser browser;
            public CEF_Browser caller;

            public Size popupSize;
            public Point popupPosition;

            /// <summary>
            /// Need a lock because the caller may be asking for the bitmap
            /// while Chromium async rendering has returned on another thread.
            /// </summary>
            public readonly object BitmapLock = new object();

            /// <summary>
            /// Gets or sets a value indicating whether the popup is open.
            /// </summary>
            /// <value>
            /// <c>true</c> if popup is opened; otherwise, <c>false</c>.
            /// </value>
            public bool PopupOpen { get; protected set; }

            /// <summary>
            /// Contains the last bitmap buffer. Direct access
            /// to the underlying buffer - there is no locking when trying
            /// to access directly, use <see cref="BitmapBuffer.BitmapLock" /> where appropriate.
            /// </summary>
            /// <value>The bitmap.</value>
            public BitmapBuffer BitmapBuffer { get; private set; }

            /// <summary>
            /// The popup Bitmap.
            /// </summary>
            public BitmapBuffer PopupBuffer { get; private set; }

            /// <summary>
            /// Gets the size of the popup.
            /// </summary>
            public Size PopupSize
            {
                get { return popupSize; }
            }

            /// <summary>
            /// Gets the popup position.
            /// </summary>
            public Point PopupPosition
            {
                get { return popupPosition; }
            }

            /// <summary>
            /// Create a new instance of DefaultRenderHadler
            /// </summary>
            /// <param name="browser">reference to the ChromiumWebBrowser</param>
            public RenderHandler2(ChromiumWebBrowser browser, CEF_Browser caller)
            {
                this.browser = browser;

                popupPosition = new Point();
                popupSize = new Size();

                BitmapBuffer = new BitmapBuffer(BitmapLock);
                PopupBuffer = new BitmapBuffer(BitmapLock);
                this.caller = caller;
            }

            /// <summary>
            /// Dispose of this instance.
            /// </summary>
            public void Dispose()
            {
                browser = null;
                BitmapBuffer = null;
                PopupBuffer = null;
            }

            /// <summary>
            /// Called to allow the client to return a ScreenInfo object with appropriate values.
            /// If null is returned then the rectangle from GetViewRect will be used.
            /// If the rectangle is still empty or invalid popups may not be drawn correctly. 
            /// </summary>
            /// <returns>Return null if no screenInfo structure is provided.</returns>	
            public virtual ScreenInfo? GetScreenInfo()
            {
                var deviceScaleFactor = browser?.DeviceScaleFactor;

                if (deviceScaleFactor == null)
                {
                    return null;
                }

                var screenInfo = new ScreenInfo { DeviceScaleFactor = deviceScaleFactor.Value };

                return screenInfo;
            }

            /// <summary>
            /// Called to retrieve the view rectangle which is relative to screen coordinates.
            /// This method must always provide a non-empty rectangle.
            /// </summary>
            /// <returns>Return a ViewRect strict containing the rectangle.</returns>
            public virtual Rect GetViewRect()
            {
                //TODO: See if this can be refactored and remove browser reference
                var size = browser?.Size;

                if (size == null)
                {
                    return new Rect(0, 0, 1, 1);
                }

                var viewRect = new Rect(0, 0, size.Value.Width, size.Value.Height);

                return viewRect;
            }

            /// <summary>
            /// Called to retrieve the translation from view coordinates to actual screen coordinates. 
            /// </summary>
            /// <param name="viewX">x</param>
            /// <param name="viewY">y</param>
            /// <param name="screenX">screen x</param>
            /// <param name="screenY">screen y</param>
            /// <returns>Return true if the screen coordinates were provided.</returns>
            public virtual bool GetScreenPoint(int viewX, int viewY, out int screenX, out int screenY)
            {
                screenX = viewX;
                screenY = viewY;

                return false;
            }

            /// <summary>
            /// Called when an element has been rendered to the shared texture handle.
            /// This method is only called when <see cref="IWindowInfo.SharedTextureEnabled"/> is set to true
            /// </summary>
            /// <param name="type">indicates whether the element is the view or the popup widget.</param>
            /// <param name="dirtyRect">contains the set of rectangles in pixel coordinates that need to be repainted</param>
            /// <param name="sharedHandle">is the handle for a D3D11 Texture2D that can be accessed via ID3D11Device using the OpenSharedResource method.</param>
            public virtual void OnAcceleratedPaint(PaintElementType type, Rect dirtyRect, IntPtr sharedHandle)
            {
                //NOT USED
            }

            /// <summary>
            /// Called when an element should be painted. Pixel values passed to this method are scaled relative to view coordinates based on the
            /// value of <see cref="ScreenInfo.DeviceScaleFactor"/> returned from <see cref="GetScreenInfo"/>.
            /// This method is only called when <see cref="IWindowInfo.SharedTextureEnabled"/> is set to false.
            /// Called on the CEF UI Thread
            /// </summary>
            /// <param name="type">indicates whether the element is the view or the popup widget.</param>
            /// <param name="dirtyRect">contains the set of rectangles in pixel coordinates that need to be repainted</param>
            /// <param name="buffer">The bitmap will be will be  width * height *4 bytes in size and represents a BGRA image with an upper-left origin</param>
            /// <param name="width">width</param>
            /// <param name="height">height</param>
            public virtual void OnPaint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
            {
                if (caller.isFocused && caller.isTextureDrawingCompleted)
                {
                    //ScreenLog.Info(":Browser_Render:");
                    try
                    {
                        var NumberOfBytes = width * height * 4;
                        var my_buffer = new byte[NumberOfBytes];
                        Marshal.Copy(buffer, my_buffer, 0, NumberOfBytes);
                        Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                        BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
                        Marshal.Copy(my_buffer, 0, bitmapData.Scan0, NumberOfBytes);
                        //Marshal.Copy(buffer, bitmapData.Scan0, 0, NumberOfBytes);
                        //bitmap.Save(browserData.memoryStream, ImageFormat.Png);
                        caller.bitmap = bitmap;
                        bitmap.UnlockBits(bitmapData);
                        caller.updated = true;
                        caller.lastUpdatedTime = util.getTime();

                    }
                    catch (Exception e)
                    {
                        ScreenLog.Info(e);
                    }
                }
                //updateTexture();

                //bitmap.Save("C:\\Users\\Administrator\\Desktop\\001.png", System.Drawing.Imaging.ImageFormat.Png);


                //    Cef.Shutdown();
                //    Console.ReadLine();
                //}
                //if (false && start)
                //{
                //    string storage = "";
                //    for (int i = 0; i < 1000; i++)
                //    {
                //        int number = Marshal.ReadByte(IntPtr.Add(buffer, i)); //使用IntPtr和Marshal读取内存中的整数数据
                //                                                              //Console.WriteLine(number);
                //        storage += number;
                //    }
                //    Console.WriteLine(height * width);
                //    File.WriteAllText("C:\\Users\\Administrator\\Desktop\\0.txt", storage);
                //    Cef.Shutdown();
                //    Console.ReadLine();
                //}
                var isPopup = type == PaintElementType.Popup;

                var bitmapBuffer = isPopup ? PopupBuffer : BitmapBuffer;

                bitmapBuffer.UpdateBuffer(width, height, buffer, dirtyRect);
            }

            /// <summary>
            /// Called when the browser's cursor has changed.
            /// </summary>
            /// <param name="cursor">If type is Custom then customCursorInfo will be populated with the custom cursor information</param>
            /// <param name="type">cursor type</param>
            /// <param name="customCursorInfo">custom cursor Information</param>
            public virtual void OnCursorChange(IntPtr cursor, CursorType type, CursorInfo customCursorInfo)
            {

            }

            /// <summary>
            /// Called when the user starts dragging content in the web view. Contextual information about the dragged content is
            /// supplied by dragData. OS APIs that run a system message loop may be used within the StartDragging call.
            /// Don't call any of the IBrowserHost.DragSource*Ended* methods after returning false.
            /// Return true to handle the drag operation. Call <see cref="IBrowserHost.DragSourceEndedAt"/> and <see cref="IBrowserHost.DragSourceSystemDragEnded"/> either synchronously or asynchronously to inform
            /// the web view that the drag operation has ended. 
            /// </summary>
            /// <param name="dragData">drag data</param>
            /// <param name="mask">operation mask</param>
            /// <param name="x">combined x and y provide the drag start location in screen coordinates</param>
            /// <param name="y">combined x and y provide the drag start location in screen coordinates</param>
            /// <returns>Return false to abort the drag operation.</returns>
            public virtual bool StartDragging(IDragData dragData, DragOperationsMask mask, int x, int y)
            {
                return false;
            }

            /// <summary>
            /// Called when the web view wants to update the mouse cursor during a drag &amp; drop operation.
            /// </summary>
            /// <param name="operation">describes the allowed operation (none, move, copy, link). </param>
            public virtual void UpdateDragCursor(DragOperationsMask operation)
            {

            }

            /// <summary>
            /// Called when the browser wants to show or hide the popup widget.  
            /// </summary>
            /// <param name="show">The popup should be shown if show is true and hidden if show is false.</param>
            public virtual void OnPopupShow(bool show)
            {
                PopupOpen = show;
            }

            /// <summary>
            /// Called when the browser wants to move or resize the popup widget. 
            /// </summary>
            /// <param name="rect">contains the new location and size in view coordinates. </param>
            public virtual void OnPopupSize(Rect rect)
            {
                ScreenLog.Info($"popupPosition.X = rect.X :{rect.X}");
                ScreenLog.Info($"popupPosition.Y = rect.Y :{rect.Y}");
                ScreenLog.Info($"popupPosition.Width = rect.X :{rect.Width}");
                ScreenLog.Info($"popupPosition.Height = rect.Height :{rect.Height}");
                //popupPosition.X = rect.X;
                //popupPosition.Y = rect.Y;
                //popupSize.Width = rect.Width;
                //popupSize.Height = rect.Height;
            }

            /// <summary>
            /// Called when the IME composition range has changed.
            /// </summary>
            /// <param name="selectedRange">is the range of characters that have been selected</param>
            /// <param name="characterBounds">is the bounds of each character in view coordinates.</param>
            public virtual void OnImeCompositionRangeChanged(Range selectedRange, Rect[] characterBounds)
            {

            }

            /// <summary>
            /// Called when an on-screen keyboard should be shown or hidden for the specified browser. 
            /// </summary>
            /// <param name="browser">the browser</param>
            /// <param name="inputMode">specifies what kind of keyboard should be opened. If <see cref="TextInputMode.None"/>, any existing keyboard for this browser should be hidden.</param>
            public virtual void OnVirtualKeyboardRequested(IBrowser browser, TextInputMode inputMode)
            {

            }
        }
        public async Task CreateBrowser(string URL = "https://www.bing.com/search?q=bing")

        {
            link = URL;

#if ANYCPU

           //Only required for PlatformTarget of AnyCPU

           CefRuntime.SubscribeAnyCpuAssemblyResolver();

#endif
            //ScreenLog.Info("Point 1");
            var settings = new CefSettings()
            {
                WindowlessRenderingEnabled = true
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                //CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };
            settings.CommandLineArgsDisabled = false;
            settings.EnableAudio();

            //Perform dependency check to make sure all relevant resources are in our output directory.
            ScreenLog.Info(Cef.IsInitialized);
            ScreenLog.Info(Cef.IsShutdown);
            if (!Cef.IsInitialized) Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            // Create the offscreen Chromium browser.
            var browser = new ChromiumWebBrowser(URL);
            //browser.SendMouseWheelEvent(0, 0, 0, 10, CefEventFlags.None);
            this.Browser = browser;
            browser.Size = new System.Drawing.Size(this.width, this.height);
            IRenderHandler renderHandler = new RenderHandler2(browser, this);
            browser.RenderHandler = renderHandler;

            browser.LifeSpanHandler = new CefLifeSpanHandler();
            //等待内容完成加载

            await browser.WaitForInitialLoadAsync();
            ScreenLog.Info("CEFSHARP: Browser加载完成 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //browser.SetZoomLevel(0.1);
        }
    }

}