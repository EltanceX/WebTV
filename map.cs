// Game.ComponentMap
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;
using Engine.Serialization;
using Game;
using GameEntitySystem;
using Newtonsoft.Json.Linq;

//using System.Drawing;
//using Newtonsoft;
using TemplatesDatabase;

namespace Game
{
    public class ComponentMap : Component, IDrawable, IUpdateable
    {
        public enum MapTypes
        {
            close,
            open
        }
        public Camera camera;

        private int mapblockcount = 48;

        private float mapSize = 0.25f;

        private BitmapButtonWidget mapButton;

        private Point2 Point;

        //CB 
        public Texture2D Lightstick;
        public PrimitivesRenderer3D m_primitivesRenderer3D2 = new PrimitivesRenderer3D();


        private Texture2D MapTexture;

        private Texture2D m_mapButtonTexture;

        private Subtexture m_mapButton;

        private Subtexture m_mapButton_Pressed;

        private float lastf;

        private float currentf;

        public Color cursorColor;

        private Vector2 cursorCoordLeftTop;

        private Vector2 cursorCoordRightBotton;

        private Vector2 skullCoordLeftTop;

        private Vector2 skullCoordRightBotton;

        private readonly List<Vector2> deathLocations = new List<Vector2>();

        private float alpha = 1f;

        private readonly Dictionary<Point2, int> MapContents = new Dictionary<Point2, int>();

        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemTime m_subsystemTime;

        public SubsystemSky m_subsystemSky;

        public ComponentGui m_componentGui;

        public ComponentPlayer m_componentPlayer;

        public PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

        public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();



        public int[] m_drawOrders = new int[1] { 1101 };

        public MapTypes MapType = MapTypes.open;

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public int[] DrawOrders => m_drawOrders;

        public void Update(float dt)
        {
            //HandleInput
        }
        public void Draw(Camera camera, int drawOrder)
        {
            this.camera = camera;

            //CEF_Browser CefIns = ;
            if (WebTV.getInstance() == null) return;


            //foreach (CEF_Browser CefIns in WebTV.cefInstances)
            for (int i = 0; i < WebTV.cefInstances.Count; i++)
            {
                CEF_Browser CefIns = (CEF_Browser)WebTV.cefInstances[i];

                if (CefIns.isFocused && CefIns.Browser != null && CefIns.updated && CefIns.isTextureDrawingCompleted)//&& util.getTime() - browserData.lastUpdatedTime >= 10)
                {
                    //Task.Run(delegate
                    //{
                    //    CEF_Browser.updateTexture();
                    //});
                    CefIns.updateTexture();

                    CefIns.updated = false;
                }

                if (CefIns.pattern != null)
                {

                    //ScreenLog.Info(Mouse.MouseWheelMovement);
                    float cosPosV_ScreenNormalV = 2;
                    float cosScreenNormalV_EyeV = 2;
                    Vector3 forwardVector = EGlobal.Player.ComponentCreatureModel.EyeRotation.GetForwardVector();
                    //if (browserData.pos != null)
                    //{
                    //Vector3 v5 = new Vector3(browserData.pos.X, browserData.pos.Y, browserData.pos.Z);
                    FlatBatch3D flatBatch3D = m_primitivesRenderer3D2.FlatBatch();
                    BoundingBox boundingBox = new BoundingBox(CefIns.posVec, CefIns.posVec + new Vector3(1f));
                    flatBatch3D.QueueBoundingBox(boundingBox, Color.Orange);


                    //Vector3 unitForwardVec = Vector3.Normalize(forwardVector);
                    Vector3 unitForwardVec = forwardVector;
                    Vector3 eyePosition = EGlobal.Player.ComponentCreatureModel.EyePosition;
                    //视角到屏幕点的向量
                    Vector3 posV = CefIns.posVec - eyePosition;
                    cosPosV_ScreenNormalV = Vector3.Dot(posV, CefIns.ScreenNormalVector) / (posV.Length() * CefIns.ScreenNormalVector.Length());
                    //ScreenLog.Info(cosPosV_ScreenNormalV);
                    cosScreenNormalV_EyeV = Vector3.Dot(forwardVector, CefIns.ScreenNormalVector) / (forwardVector.Length() * CefIns.ScreenNormalVector.Length());
                    if (cosScreenNormalV_EyeV > 0.2 && cosPosV_ScreenNormalV > 0)
                    {
                        //ScreenLog.Info($"A {cosScreenNormalV_EyeV} {cosPosV_ScreenNormalV}");
                        //眼睛垂直于屏幕的距离
                        float eyeToScreen = posV.Length() * cosPosV_ScreenNormalV;
                        if (eyeToScreen < 50)
                        {
                            //眼睛沿视线射向屏幕的距离
                            float d_eyeV = eyeToScreen / cosScreenNormalV_EyeV;
                            Vector3 forwardVecLengthen = forwardVector * d_eyeV;
                            Vector3 targetPointVec = eyePosition + forwardVecLengthen;//屏幕焦点坐标
                            BoundingBox boundingBox2 = new BoundingBox(targetPointVec.X - 0.1f, targetPointVec.Y - 0.1f, targetPointVec.Z - 0.1f, targetPointVec.X + 0.1f, targetPointVec.Y + 0.1f, targetPointVec.Z + 0.1f);
                            flatBatch3D.QueueBoundingBox(boundingBox2, Color.Blue);
                            flatBatch3D.QueueLine(eyePosition, targetPointVec, Color.Red);

                            //if (Mouse.IsMouseButtonDown(MouseButton.Left))
                            //{
                            Vector3 relativeVec = targetPointVec - CefIns.posVec;
                            //ScreenLog.Info($"{relativeVec.X} {relativeVec.Y} {relativeVec.Z}");

                            float relativeLength = relativeVec.Length();
                            float ab = relativeLength * CefIns.U.Length();
                            float cos_Relative_U = Vector3.Dot(relativeVec, CefIns.U) / ab;
                            float cos_Relative_V = Vector3.Dot(relativeVec, CefIns.V) / ab;
                            float ULength = relativeLength * cos_Relative_U; //+ -
                            float VLength = relativeLength * cos_Relative_V; //+ -
                            //Vector3 ProjectionU = CefIns.U * (relativeLength * cos_Relative_U);
                            //Vector3 ProjectionV = CefIns.V * (relativeLength * cos_Relative_V);
                            //ScreenLog.Info($"{cos_Relative_U} {cos_Relative_V} / {ULength}({CefIns.IngameHeight}) {VLength}({CefIns.IngameWidth}) | {relativeLength}({CefIns.DiagonalLength})");
                            //if (CefIns.Browser != null && relativeVec.X > 0 && relativeVec.Y > 0 && relativeVec.X < CefIns.IngameWidth && relativeVec.Y < CefIns.IngameHeight)
                            if (CefIns.Browser != null && cos_Relative_U > 0 && cos_Relative_V > 0 && relativeLength <= CefIns.DiagonalLength && ULength < CefIns.IngameHeight && VLength < CefIns.IngameWidth)
                            {

                                float y = ULength / CefIns.IngameWidth * CefIns.width;
                                float x = VLength / CefIns.IngameHeight * CefIns.height;
                                int upper_left_X = (int)(CefIns.width - x);
                                int upper_left_Y = (int)(CefIns.height - y);
                                //ScreenLog.Info($"MouseMove Vector: {upper_left_X} {upper_left_Y}");
                                var mouseEvent = new CefSharp.MouseEvent(upper_left_X, upper_left_Y, CefEventFlags.None);
                                var browserHost = CefIns.Browser.GetBrowserHost();
                                browserHost.SendMouseMoveEvent(mouseEvent, false);
                                if (!CefIns.hasMouseDown && Mouse.IsMouseButtonDown(MouseButton.Left))
                                {
                                    ScreenLog.Info("Set Left Click State: True");
                                    browserHost.SendMouseClickEvent(mouseEvent, MouseButtonType.Left, false, 1);
                                    //browserHost.
                                    CefIns.hasMouseDown = true;
                                }
                                else if (CefIns.hasMouseDown && !Mouse.IsMouseButtonDown(MouseButton.Left))
                                {
                                    ScreenLog.Info("Set Left Click State: False");
                                    CefIns.hasMouseDown = false;
                                    browserHost.SendMouseClickEvent(mouseEvent, MouseButtonType.Left, true, 1);
                                }

                                if (!CefIns.hasRightMouseDown && Mouse.IsMouseButtonDown(MouseButton.Right))
                                {
                                    ScreenLog.Info("Set Right Click State: True");
                                    browserHost.SendMouseClickEvent(mouseEvent, MouseButtonType.Right, false, 1);
                                    //browserHost.
                                    CefIns.hasRightMouseDown = true;
                                }
                                else if (CefIns.hasRightMouseDown && !Mouse.IsMouseButtonDown(MouseButton.Right))
                                {
                                    ScreenLog.Info("Set Right Click State: False");
                                    CefIns.hasRightMouseDown = false;
                                    browserHost.SendMouseClickEvent(mouseEvent, MouseButtonType.Right, true, 1);
                                }

                                if (Mouse.MouseWheelMovement > 0)
                                {
                                    CefIns.Browser.SendMouseWheelEvent(0, 0, 0, 30, CefEventFlags.None);
                                }
                                else if (Mouse.MouseWheelMovement < 0)
                                {
                                    CefIns.Browser.SendMouseWheelEvent(0, 0, 0, -30, CefEventFlags.None);
                                }

                            }
                            //}

                        }
                    }
                    m_primitivesRenderer3D2.Flush(camera.ViewProjectionMatrix);
                    //}
                    //else
                    //{
                    //    m_primitivesRenderer3D2.Clear();
                    //}




                    var pattern = CefIns.pattern;
                    if (cosScreenNormalV_EyeV == 2 || cosPosV_ScreenNormalV < 0 || cosScreenNormalV_EyeV < -0.2)
                    {
                        CefIns.isFocused = false;
                        continue;
                        //return;
                    }
                    CefIns.isFocused = true;

                    Vector3 vector = pattern.Position - camera.ViewPosition;
                    if (vector.Length() < m_subsystemSky.ViewFogRange.Y)
                    {
                        //Vector3 vector2 = (0f - (0.01f + 0.02f * num)) / num2 * vector;
                        //Vector3 vector3 = pattern.Position - pattern.Size * (pattern.Right + pattern.Up);// + vector2; +1, +0.6
                        //Vector3 vector4 = pattern.Position + pattern.Size * (pattern.Right - pattern.Up);// + vector2; -1, +0.6
                        //var vector4 = pattern.Position;
                        //Vector3 vector5 = pattern.Position + pattern.Size * (pattern.Right + pattern.Up);// + vector2; -1, -0.6
                        //Vector3 vector6 = pattern.Position - pattern.Size * (pattern.Right - pattern.Up);// + vector2; +1, -0.6
                        //var vector6 = pattern.Position;
                        //RenderTarget2D r2d = Display.RenderTarget;

                        var vector3 = pattern.Position + pattern.Size * (pattern.Right + pattern.Up);
                        var vector4 = pattern.Position + pattern.Size * pattern.Up;
                        var vector5 = pattern.Position; //+ new Vector3(0, 0, 0);
                        var vector6 = pattern.Position + pattern.Size * pattern.Right;

                        var temp = m_primitivesRenderer3D.TexturedBatch(pattern.Texture, useAlphaTest: true, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.PointClamp);
                        //int count = temp.TriangleVertices.Count;
                        temp.QueueQuad(vector3, vector4, vector5, vector6, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), pattern.Color);

                        m_primitivesRenderer3D.Flush(camera.ViewProjectionMatrix);

                        //Engine.Graphics.TexturedBatch3D.
                        //m_primitivesRenderer3D.
                        //temp.Clear();
                        //temp.Clear();
                        //m_primitivesRenderer3D.Clear();

                    }
                    //m_primitivesRenderer3D.Flush(camera.ViewProjectionMatrix);



                }

                //return;
                //float number = 2f;
                //Vector3 eyePosition = EGlobal.Player.ComponentCreatureModel.EyePosition;
                //Vector3 posv = new(pos.X, pos.Y, pos.Z);
                //Vector3 v = Vector3.Normalize(posv - eyePosition);
                //Vector3 v2 = eyePosition + v * 50f;
                //Vector3 v3 = Vector3.Normalize(Vector3.Cross(v, Vector3.UnitY));
                //Vector3 v4 = Vector3.Normalize(Vector3.Cross(v, v3));
                //Vector3 p = v2 + number * (-v3 - v4);
                //Vector3 p2 = v2 + number * (v3 - v4);
                //Vector3 p3 = v2 + number * (v3 + v4);
                //Vector3 p4 = v2 + number * (-v3 + v4);
                //TexturedBatch3D texturedBatch3D = m_primitivesRenderer3D.TexturedBatch(Lightstick, useAlphaTest: false, 0, DepthStencilState.None);
                //int count = texturedBatch3D.TriangleVertices.Count;
                //texturedBatch3D.QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), Color.Red);
                //texturedBatch3D.TransformTriangles(camera.ViewMatrix, count);
                //m_primitivesRenderer3D.Flush(camera.ProjectionMatrix);

            }
            //if (m_componentPlayer.GameWidget == camera.GameWidget && MapType > MapTypes.close)
            //{
            //    DrawMap(camera, alpha);
            //}
        }
        public class image
        {
            public static int Height = 100;
            public static int Width = 100;
        };

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            Lightstick = ContentManager.Get<Texture2D>("Textures/Lightstick");
            m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(throwOnError: true);
            m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(throwOnError: true);
            m_componentGui = base.Entity.FindComponent<ComponentGui>(throwOnError: true);
            m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            foreach (PlayerStats.DeathRecord deathRecord in m_componentPlayer.PlayerStats.DeathRecords)
            {
                deathLocations.Add(new Vector2((int)deathRecord.Location.X, (int)deathRecord.Location.Z));
            }
            m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);

            m_mapButtonTexture = new Texture2D(image.Width, image.Height, 1, ColorFormat.Rgba8888);
            //m_mapButtonTexture.SetData(0, image.Pixels);
            cursorCoordLeftTop = new Vector2(134f / ((float)image.Width - 1f), 23f / ((float)image.Height - 1f));
            cursorCoordRightBotton = new Vector2(148f / ((float)image.Width - 1f), 37f / ((float)image.Height - 1f));
            if (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Male)
            {
                cursorCoordLeftTop = new Vector2(134f / ((float)image.Width - 1f), 39f / ((float)image.Height - 1f));
                cursorCoordRightBotton = new Vector2(148f / ((float)image.Width - 1f), 53f / ((float)image.Height - 1f));
            }
            skullCoordLeftTop = new Vector2(135f / ((float)image.Width - 1f), 0f / ((float)image.Height - 1f));
            skullCoordRightBotton = new Vector2(157f / ((float)image.Width - 1f), 22f / ((float)image.Height - 1f));

            MapTexture = ContentManager.Get<Texture2D>("Textures/Blocks");

            mapButton = m_componentPlayer.GuiWidget.Children.Find<BitmapButtonWidget>("MapButton", throwIfNotFound: false);
            if (mapButton == null)
            {
                //m_mapButton = new Subtexture(m_mapButtonTexture, new Vector2(0f / ((float)image.Width - 1f), 0f / ((float)image.Height - 1f)), new Vector2(67f / ((float)image.Width - 1f), 63f / ((float)image.Height - 1f)));
                //m_mapButton_Pressed = new Subtexture(m_mapButtonTexture, new Vector2(67f / ((float)image.Width - 1f), 0f / ((float)image.Height - 1f)), new Vector2(134f / ((float)image.Width - 1f), 63f / ((float)image.Height - 1f)));
                mapButton = new BitmapButtonWidget
                {
                    Name = "MapButton",
                    HorizontalAlignment = WidgetAlignment.Far,
                    Text = "M",
                    Color = Color.Black,
                    Size = new Vector2(68f, 64f),
                    Margin = new Vector2(4f, 0f),
                    NormalSubtexture = m_mapButton,
                    ClickedSubtexture = m_mapButton_Pressed
                };
                StackPanelWidget stackPanelWidget = m_componentPlayer.GuiWidget.Children.Find<StackPanelWidget>("MoreContents");
                stackPanelWidget.Children.Add(mapButton);
            }
        }

        public void DrawMap(Camera camera, float alpha)
        {
            return;
            Vector2 viewportSize = camera.ViewportSize;
            Vector2 vector = new Vector2(viewportSize.X, viewportSize.Y * mapSize);
            float num = vector.Y / (float)mapblockcount;
            Vector2 vector2 = new Vector2(viewportSize.X - 1.5f * vector.Y, 0f);
            FlatBatch2D flatBatch2D = m_primitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, BlendState.AlphaBlend);
            int count = flatBatch2D.TriangleVertices.Count;
            flatBatch2D.QueueQuad(vector2, vector, 0f, Color.Black * alpha);
            flatBatch2D.TransformTriangles(camera.ViewportMatrix, count);
            flatBatch2D.Flush();
            //TexturedBatch2D texturedBatch2D = m_primitivesRenderer2D.TexturedBatch(m_mapButtonTexture, useAlphaTest: false, 1, DepthStencilState.None, null, BlendState.AlphaBlend, SamplerState.PointWrap);
            //int count2 = texturedBatch2D.TriangleVertices.Count;
            TexturedBatch2D texturedBatch2D2 = m_primitivesRenderer2D.TexturedBatch(MapTexture, useAlphaTest: false, 0, DepthStencilState.None, null, BlendState.Additive, SamplerState.PointWrap);
            //m_primitivesRenderer2D.TexturedBatch()
            //int count3 = texturedBatch2D2.TriangleVertices.Count;
            //texturedBatch2D2.



            Vector4 vector5 = BlocksManager.m_slotTexCoords[20];
            Point2 mouse = (Point2)Mouse.MousePosition;
            if (mouse != null)
            {
                for (int i = 0; i < 30; i++)
                {
                    float x = mouse.X + util.RandomF(-30, 30, 0);
                    float y = mouse.Y + util.RandomF(-30, 30, 0);
                    Vector2 vec = new Vector2(x, y);
                    Vector2 vec2 = new Vector2(x + util.RandomF(1, 5, 0), x + util.RandomF(1, 5, 0));
                    texturedBatch2D2.QueueQuad(vec, vec2, 0f, new Vector2(vector5.X, vector5.W), new Vector2(vector5.Z, vector5.Y), Color.Yellow);
                }
            }
            texturedBatch2D2.Flush();
        }

    }
}