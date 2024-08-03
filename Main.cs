// Game.ComponentGameSystem
using CefSharp;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;
using ETerminal;
using Game;
using GameEntitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
//using System.Diagnostics;
//using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using TemplatesDatabase;
//using System.Drawing;

public class QuitHook : ModLoader
{
    public override void OnProjectDisposed()
    {
        base.OnProjectDisposed();
        ScreenLog.Info("正在停止浏览器...");
        browserData.Close();

    }
}
public class LogSinkOverride : ModLoader
{
    //public override void proj
    //public override void OnProjectDisposed()
    //{
        
    //    //this.on
    //    base.OnProjectDisposed();
    //    ScreenLog.Info("正在停止浏览器...");
    //    browserData.Close();

    //}
    public override void OnLoadingStart(List<Action> actions)
    {
        Log.Information("OnLoadingStart");
        Log.Information("正在应用日志转发...");
        ScreenLog.Info("正在应用日志转发...");
        Log.AddLogSink(new ET_LogSink());
        base.OnLoadingStart(actions);


        var hook = new ModsManager.ModHook("QuitHook");
        hook.Add(new QuitHook());
        ModsManager.ModHooks.Add("OnProjectDisposed", hook);
        ScreenLog.Info("QuitHook added");
    }
    public override void OnLoadingFinished(List<Action> actions)
    {
        Log.Information("OnLoadingFinished");
        actions.Add(delegate
        {
            try
            {
                Entity.GetAssetsFile("Fonts/Pericles.lst", delegate (Stream stream)
                {
                    Entity.GetAssetsFile("Fonts/Pericles.png", delegate (Stream stream2)
                    {
                        LabelWidget.BitmapFont = BitmapFont.Initialize(stream2, stream);
                        Log.Information("字体Fonts/Pericles.png已加载");
                    });
                });
            }
            catch (Exception e)
            {
                Log.Information("字体文件 Fonts/Pericles.png 丢失，已恢复原始字体.");
            }
        });
    }
}

namespace Game
{
    //玩家进入
    //public class SubsystemTest : Subsystem
    //{
    //    public PlayerData m_playerData;

    //    //当玩家第一次添加时执行
    //    public void OnPlayerFirstAdded(ComponentPlayer componentPlayer)
    //    {
    //        //清空衣服
    //        ComponentClothing componentClothing = componentPlayer.Entity.FindComponent<ComponentClothing>();
    //        if (componentClothing != null)
    //        {
    //            foreach (var slot in componentClothing.m_clothes.Keys)
    //            {
    //                componentClothing.m_clothes[slot].Clear();
    //            }
    //        }
    //        //背包添加物品
    //        ComponentInventory componentInventory = componentPlayer.Entity.FindComponent<ComponentInventory>();
    //        if (componentInventory != null)
    //        {
    //            componentInventory.AddSlotItems(slotIndex: 0, value: 46, count: 3);
    //            componentInventory.AddSlotItems(slotIndex: 10, value: 126, count: 1);
    //        }
    //    }

    //    public override void OnEntityAdded(Entity entity)
    //    {
    //        ComponentPlayer componentPlayer = entity.FindComponent<ComponentPlayer>();
    //        if (componentPlayer != null && m_playerData != null && m_playerData == componentPlayer.PlayerData)
    //        {
    //            OnPlayerFirstAdded(componentPlayer);
    //            m_playerData = null;
    //        }
    //    }

    //    public override void Load(ValuesDictionary valuesDictionary)
    //    {
    //        base.Load(valuesDictionary);
    //        base.Project.FindSubsystem<SubsystemPlayers>(true).PlayerAdded += (PlayerData playerData) => { m_playerData = playerData; };
    //    }
    //}


    public class ComponentGameSystem : ComponentInventoryBase, IUpdateable
    {
        public ET_F3 et_F3;

        public TickTimer keyboardTickTimer = new(10);

        public ComponentPlayer Player;
        //public ButtonWidget AButton;
        public static Dictionary<string, ButtonWidget> CustomButtons = new();
        public SubsystemSky subsystemSky;
        public SubsystemBodies m_subsystemBodies;
        public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();
        public ComponentPlayer componentPlayer;
        public SubsystemBlockEntities Entities;
        public ComponentOnFire componentOnFire;
        public ComponentMiner componentMiner;

        public SubsystemTime subsystemTime;
        //public ComponentLocomotion componentLocomotion;
        //public Screen componentScreen;

        public float m_frequency = 0.5f;

        //public SubsystemGameInfo m_subsystemGameInfo;
        //public ComponentCreature m_componentCreature;
        //public SubsystemPlayers m_subsystemPlayers;

        public static void HandleInput(string input)
        {
            if (input == null || input == string.Empty) return;//当输入为空: 确定:input=string.empty  取消:input=nulll

            CommandInput.Exec(input, false);
        }

        UpdateOrder IUpdateable.UpdateOrder => ((IUpdateable)componentPlayer).UpdateOrder;

        public void AddButton(string Name,
                    string Text,
                    Vector2 Size,
                    BitmapFont Font = null,
                    bool IsEnabled = true
                    )
        {

            if (Font == null) Font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
            if (Size == null) Size = new Engine.Vector2(110f, 60f);
            try
            {
                CustomButtons[Name] = Player.ViewWidget.GameWidget.Children.Find<ButtonWidget>(Name);
                //XLog.Information("按钮已存在" + Name);
            }
            catch (Exception e)
            {
                //XLog.Information(e);
                //XLog.Information("按钮不存在" + Name);
                var btn2 = new BevelledButtonWidget
                {
                    Name = Name,
                    Text = Text,
                    Font = Font,
                    Size = Size,
                    IsEnabled = IsEnabled
                };
                Player.ViewWidget.GameWidget.Children.Find<StackPanelWidget>("RightControlsContainer").Children.Add(btn2);
                //value1;
                if (!CustomButtons.TryGetValue(Name, out ButtonWidget _)) CustomButtons.Add(Name, btn2);
                else CustomButtons[Name] = btn2;


            }
        }


        public static TextBoxDialog commandbox;
        public void Update(float dt)
        {
            //if (mapMod != null)
            //    mapMod.Update(dt);
            //_ = componentPlayer.ComponentBody.Position;
            //_ = componentLocomotion.WalkSpeed;

            //if (CustomButtons["AButton"].IsClicked)
            //{
            //componentPlayer.ComponentGui.ModalPanelWidget = new VitalStatsWidget(componentPlayer);
            //var vec3 = Player.ComponentBody.m_position;
            //var rot = Player.ComponentBody.m_rotation;
            //var fv = rot.GetForwardVector();
            //var uv = rot.GetRightVector();
            //ScreenLog.Info(" === === === === === ");
            //ScreenLog.Info($"Coordinate: {vec3.X} {vec3.Y} {vec3.Z}");
            //ScreenLog.Info($"Rotation: {rot.X} {rot.Y} {rot.Z} {rot.W}");
            //ScreenLog.Info($"Forward Vector: {fv.X} {fv.Y} {fv.Z}");
            //ScreenLog.Info($"Right Vector: {uv.X} {uv.Y} {uv.Z}");
            //ScreenLog.Info("GameMode: " + HammerData.GameMode);

            //Player.ComponentBody.m_subsystemBodies.
            //PlaceBlock.place.p((int)MathUtils.Floor(vec3.X), (int)MathUtils.Floor(vec3.Y), (int)MathUtils.Floor(vec3.Z), 1);

            //}


            //else if (Keyboard.IsKeyDownOnce(Key.G))
            //{
            //    ComponentInventory componentInventory = componentPlayer.Entity.FindComponent<ComponentInventory>();
            //    if (componentInventory != null)
            //    {
            //        componentInventory.AddSlotItems(slotIndex: 0, value: 186, count: 1);
            //    }
            //}

            //如果正在显示命令输入界面
            if (DialogsManager.HasDialogs(componentPlayer.GuiWidget))
            {
                if (Keyboard.IsKeyDownOnce(Key.Enter))
                {
                    commandbox.Dismiss(commandbox.m_textBoxWidget.Text);
                }
                else if (Keyboard.IsKeyDownOnce(Key.UpArrow))
                {

                }
                else if (Keyboard.IsKeyDownOnce(Key.DownArrow))
                {

                }
                //Has had dislog, do nothing.
            }
            //else if (Keyboard.IsKeyDownOnce(Key.I))
            //{
            //    //ScreenLog.Info(componentPlayer.ComponentInput.PlayerInput.EditItem);
            //    //ScreenLog.Info(ComponentScreenOverlays.m_drawOrders.Length);
            //    //ScreenLog.Info(ScreensManager.m_screens.Count);
            //    try
            //    {
            //        //ScreenLog.Info(ScreensManager.CurrentScreen.Name);
            //        ScreenLog.Info(DialogsManager.HasDialogs(componentPlayer.GuiWidget));
            //        ScreenLog.Info(componentPlayer.GuiWidget.Children.Count);
            //        //ScreenLog.Info(componentPlayer.Entity.Components.Count);
            //        //ScreenLog.Info(componentPlayer.GetOwnedEntities().ToDynamicArray().Count);
            //        ScreenLog.Info(componentPlayer.m_subsystemPickables.m_pickables.Count);
            //        ScreenLog.Info(componentPlayer.Project.m_entities.Count);
            //    }
            //    catch (Exception e)
            //    {
            //        ScreenLog.Info(e);
            //    }
            //    //ContentManager.Get
            //    //XLog.Information(" --- --- --- --- --- ");
            //    //var componentInventory = Player.Entity.FindComponent<ComponentInventory>();
            //    //var creativeInventory = Player.Entity.FindComponent<ComponentCreativeInventory>();
            //}
            else if (Keyboard.IsKeyDownOnce(Key.K))
            {
                try
                {
                    var wid = new BrowserWidget(componentPlayer);
                    wid.Update();
                    bool flag = componentPlayer.ComponentGui.ModalPanelWidget is BrowserWidget;
                    if (flag)
                    {
                        componentPlayer.ComponentGui.ModalPanelWidget = null;
                    }
                    else
                    {
                        componentPlayer.ComponentGui.ModalPanelWidget = new BrowserWidget(componentPlayer);
                    }
                }
                catch (Exception e)
                {
                    ScreenLog.Info(e);
                }
            }
            else if (Keyboard.IsKeyDownOnce(Key.Slash) || CustomButtons["ChatButton"].IsClicked)
            {
                //try
                //{
                //    Dictionary<string, string> Notes = new Dictionary<string, string>();
                //    componentPlayer.ComponentGui.ModalPanelWidget = new NotesWidget(componentPlayer, Notes);
                //}
                //catch (Exception e)
                //{
                //    ScreenLog.Info(e);
                //}
                commandbox = new TextBoxDialog("ETerminal: 聊天和命令", string.Empty, 256, HandleInput/* delegate (string input)
                {
                    if (input == null || input == string.Empty) return;//当输入为空: 确定:input=string.empty  取消:input=nulll

                    XLog.Information($">> {input}");
                    CommandInput.Exec(input);
                }*/);
                DialogsManager.ShowDialog(componentPlayer.GuiWidget, commandbox);
            }
            else if (Keyboard.IsKeyDown(Key.UpArrow) || CustomButtons["BButton"].IsClicked)
            {
                if (keyboardTickTimer.Next()) ScreenLog.up();
            }
            else if (Keyboard.IsKeyDown(Key.DownArrow) || CustomButtons["CButton"].IsClicked)
            {
                if (keyboardTickTimer.Next()) ScreenLog.down();
            }
            else if (Keyboard.IsKeyDownOnce(Key.F1) || CustomButtons["F1"].IsClicked)
            {
                bool visible = ScreenLog.label.IsVisible = !ScreenLog.label.IsVisible;
                CustomButtons["BButton"].IsVisible = visible;
                CustomButtons["CButton"].IsVisible = visible;
            }
            else if (Keyboard.IsKeyDownOnce(Key.F3) || CustomButtons["F3"].IsClicked)
            {
                et_F3.SwitchState();
            }
            //else if (Keyboard.IsKeyDownOnce(Key.J))
            //{
            //    //DialogsManager.HideDialog(this);
            //    //if (CommonLib.WorkType == WorkType.Client)
            //    //{
            //    //    ScreensManager.SwitchScreen("NetPlay");
            //    //}
            //    //else
            //    //{
            //    ScreensManager.SwitchScreen("Play");
            //    GameManager.SaveProject(waitForCompletion: true, showErrorDialog: true);
            //    //}
            //    GameManager.DisposeProject();
            //    //CommonLib.Net.Stop();
            //    //    try
            //    //    {
            //    //        var screen = ScreensManager.FindScreen<PlayScreen>("Play");
            //    //        ScreenLog.Info(screen == null);
            //    //        ScreenLog.Info(screen);
            //    //        ScreenLog.Info(screen.IsVisible);
            //    //        screen.
            //    //        //ScreenLog.Info(screen.Leave());
            //    //        screen.Leave();
            //    //        ScreenLog.Info(screen.IsVisible);
            //    //        ScreenLog.Info("===");
            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        ScreenLog.Info(ex);
            //    //    }
            //}
            else if (browserData.Browser != null)
            {
                if (Keyboard.IsKeyDown(Key.Y))
                {
                    //ScreenLog.Info("按键: Y, 向上滚动");
                    browserData.Browser.SendMouseWheelEvent(0, 0, 0, 6, CefEventFlags.None);
                }
                else if (Keyboard.IsKeyDown(Key.U))
                {
                    //ScreenLog.Info("按键: U, 向下滚动");
                    browserData.Browser.SendMouseWheelEvent(0, 0, 0, -6, CefEventFlags.None);
                }
                else if (Keyboard.IsKeyDown(Key.N))
                {
                    //ScreenLog.Info("按键: N, 向左滚动");
                    browserData.Browser.SendMouseWheelEvent(0, 0, 6, 0, CefEventFlags.None);
                }
                else if (Keyboard.IsKeyDown(Key.M))
                {
                    //ScreenLog.Info("按键: M, 向右滚动");
                    browserData.Browser.SendMouseWheelEvent(0, 0, -6, 0, CefEventFlags.None);
                }
            }



            et_F3.Update();
        }
        //public override void sa
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            base.Save(valuesDictionary, entityToIdMap);
            //ScreenLog.Info("Save");
        }
        public override void OnEntityAdded()
        {
            base.OnEntityAdded();
            //ScreenLog.Info("OnEntityAdded");
        }
        //玩家移除(死亡)
        public override void OnEntityRemoved()
        {
            base.OnEntityRemoved();
            //Log.Information("OnEntity")
            //ScreenLog.Info("OnEntityRemoved");
            ScreenLog.Info("\n\n");

            et_F3.Dispose();
            ScreenLog.RemoveLabel();
            //fpsTimer1s.Dispose();
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);

            Player = base.Entity.FindComponent<ComponentPlayer>();
            componentPlayer = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            et_F3 = new ET_F3(base.Entity);

            var screenLogLabel = new LabelWidget()
            {
                FontScale = 0.6f,
                Color = Color.LightGray,
                Margin = new Vector2(300f, 0f),
                VerticalAlignment = WidgetAlignment.Near
            };
            componentPlayer.GuiWidget.Children.Add(screenLogLabel);
            ScreenLog.label = screenLogLabel;
            //ScreenLog.label.IsVisible = false;
            ScreenLog.Info("Debug Log Screen View Loaded.\n");

            //var input = new TouchInputWidget()
            //{
            //    IsVisible = true,
            //    IsEnabled = true,
            //    Name = "Input",
            //    Margin = new Vector2(10f, 30f),
            //    VerticalAlignment = WidgetAlignment.Far,
            //    DesiredSize = new Vector2(100f, 100f)
            //};
            //componentPlayer.GuiWidget.Children.Add(input);


            //fpsTimer1s = new Timer((state) =>
            //{
            //    fps = fpsCumulative;
            //    fpsCumulative = 0;
            //}, "FPS", 1000, 1000);



            Player = base.Entity.FindComponent<ComponentPlayer>();
            EGlobal.setPlayer(Player);
            HammerData.GameMode = Player.m_subsystemGameInfo.WorldSettings.GameMode;
            if (!HammerData.isDebug) return;
            componentPlayer = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            componentMiner = base.Entity.FindComponent<ComponentMiner>(throwOnError: true);
            //componentLocomotion = base.Entity.FindComponent<ComponentLocomotion>(throwOnError: true);
            subsystemTime = base.Project.FindSubsystem<SubsystemTime>(throwOnError: true);
            //m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
            //m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(throwOnError: true);
            m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
            subsystemSky = base.Project.FindSubsystem<SubsystemSky>(throwOnError: true);
            EGlobal.terrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
            var ButtonSize = new Vector2(60f, 50f);
            AddButton("F1", "F1", ButtonSize);
            AddButton("F3", "F3", ButtonSize);
            AddButton("BButton", "↑", ButtonSize);
            AddButton("CButton", "↓", ButtonSize);


            StackPanelWidget stackPanelWidget = componentPlayer.GameWidget.Children.Find<StackPanelWidget>("MoreContents");
            var mapButton = new BevelledButtonWidget
            {
                Name = "ChatButton",
                Text = "/",
                Size = new Vector2(68f, 64f),
                Margin = new Vector2(4f, 0f),
                CenterColor = new Color(127, 127, 127, 180)
            };
            try
            {
                stackPanelWidget.Children.Find<ButtonWidget>(mapButton.Name);
            }
            catch
            {
                stackPanelWidget.Children.Add(mapButton);
                try
                {
                    CustomButtons[mapButton.Name] = mapButton;
                }
                catch
                {
                    CustomButtons.Add(mapButton.Name, mapButton);
                }
            }
            //var LookVector = componentPlayer.ComponentBody.Matrix.Forward.XZ;
            //foreach (Widget widget in stackPanelWidget.Children.m_widgets)
            //{
            //    if (widget.Name == mapButton.Name)
            //    {
            //        stackPanelWidget.Children.Remove(widget);
            //    }
            //}
            //stackPanelWidget.Children.Add(mapButton);


            //Player.ViewWidget.GameWidget.Children.Find<StackPanelWidget>("RightControlsContainer").Children.Add(AInput);
            //AButton.Name = "";
            //Player.ViewWidget.GameWidget.Children.Find<StackPanelWidget>("RightControlsContainer").Children.Remove(AButton);
            //}


            //if (EGlobal.isFirstLoad)
            //{
            //    EGlobal.AssemblyInit();
            //    //XLog.Information("---- List of all dependency files ----");
            //}
            EGlobal.isFirstLoad = false;
            //ScreenLog.Info("\n");
            ScreenLog.Info("WebTV Mod - SurvivalCraft2.3 API 1.5/1.44");
            ScreenLog.Info("按 / 以打开控制台，按 k 打开浏览器菜单");
            ScreenLog.Info("WebTV 模组讨论群: 328170928");
            //componentPlayer.m_subsystemGameInfo.
            //ScreenLog.Info($"{EGlobal.Version} - EltanceX {EGlobal.Date}");
            //try
            //{
            //    foreach (var item in ScreensManager.m_screens)
            //    {
            //        ScreenLog.Info(item.Key);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ScreenLog.Info(e);
            //}
        }
    }
}