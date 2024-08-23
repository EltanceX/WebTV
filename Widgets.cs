using CefSharp.DevTools.LayerTree;
using Engine;
using Engine.Graphics;
using Engine.Input;
using GameEntitySystem;



//using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Game
{

    public class BrowserWidget : CanvasWidget
    {
        public void RefreshPosition()
        {
            playerPosition = m_componentPlayer.ComponentBody.Position;
            ScreenPositionTextbox.Text = $"{MathUtils.Floor(playerPosition.X)}, {MathUtils.Floor(playerPosition.Y)}, {MathUtils.Floor(playerPosition.Z)}";
        }
        //public GridPanelWidget m_inventoryGrid;

        //public ButtonWidget m_chooseButton;

        public ButtonWidget ResolvingPowerBtn;
        public ButtonWidget ResizeBtn;
        public LabelWidget ResolvingPowerLabel;
        public TextBoxWidget ScreenPositionTextbox;
        public TextBoxWidget SiteAddrDialog;
        public ButtonWidget CreatePreviewButton;
        public ButtonWidget CreateBrowserButton;
        public ButtonWidget ClosePageButton;
        public ButtonWidget CloseBrowserButton;

        public RectangleWidget m_facing;
        public ButtonWidget FacingBtn;


        public ComponentPlayer m_componentPlayer;
        public IInventory m_inventory;

        public int ResolvingPower_Height = 600;

        public int ResolvingPower_Width = 1000;

        public float ScreenSize = 10.0f;

        public Vector3 playerPosition;

        public enum Facing
        {
            X, Y, Z, Eye
        }
        public Facing FacingState = Facing.Eye;

        public BrowserWidget(/*ComponentShop componentShop,*/ ComponentPlayer componentPlayer/*, SubsystemFinancial subsystemFinancial*/)
        {
            m_componentPlayer = componentPlayer;
            //m_componentShop = componentShop;
            //m_subsystemFinancial = subsystemFinancial;
            m_inventory = componentPlayer.ComponentMiner.Inventory;
            XElement node = ContentManager.Get<XElement>("Widgets/BrowserWidget");
            LoadContents(this, node);
            //m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
            //m_moneyLabel = Children.Find<LabelWidget>("MoneyLabel");
            //m_chooseButton = Children.Find<ButtonWidget>("ChooseButton");
            //m_buyButton = Children.Find<ButtonWidget>("BuyButton");
            //m_itemvalueicon = Children.Find<BlockIconWidget>("Icon");
            //m_soldButton = Children.Find<ButtonWidget>("SoldButton");
            //m_chooseitemLabel = Children.Find<LabelWidget>("ChooseItemLabel");
            //m_chooseitemcountLabel = Children.Find<LabelWidget>("ChooseItemCountLabel");
            ResolvingPowerBtn = Children.Find<ButtonWidget>("ResolvingPowerBtn");
            ResizeBtn = Children.Find<ButtonWidget>("ResizeBtn");
            ResolvingPowerLabel = Children.Find<LabelWidget>("ResolvingPowerLabel");
            ScreenPositionTextbox = Children.Find<TextBoxWidget>("ScreenPositionTextbox");
            SiteAddrDialog = Children.Find<TextBoxWidget>("SiteAddrDialog");
            CreatePreviewButton = Children.Find<ButtonWidget>("CreatePreviewButton");
            CreateBrowserButton = Children.Find<ButtonWidget>("CreateBrowserButton");
            ClosePageButton = Children.Find<ButtonWidget>("ClosePageButton");
            CloseBrowserButton = Children.Find<ButtonWidget>("CloseBrowserButton");
            //m_soldSlot = Children.Find<InventorySlotWidget>("SoldSlot");
            //m_returnmoneyLabel = Children.Find<LabelWidget>("ReturnMoneyLabel");
            //m_upgradeshopButton = Children.Find<ButtonWidget>("UpgradeShopButton");
            //m_shoplevelLabel = Children.Find<LabelWidget>("ShopLevelLabel");
            //m_soldSlot.AssignInventorySlot(componentShop, componentShop.SoldSlotIndex);
            RefreshPosition();
            SiteAddrDialog.Text = WebTV.settings.defaultLink;

            m_facing = Children.Find<RectangleWidget>("Facing");

            FacingBtn = Children.Find<ButtonWidget>("FacingBtn");
            //m_facing.
        }

        //public override void MeasureOverride(Vector2 parentAvailableSize)
        //{
        //    int max = ((m_inventory is ComponentCreativeInventory) ? 10 : 7);
        //    m_inventory.VisibleSlotsCount = MathUtils.Clamp((int)((parentAvailableSize.X - 320f - 25f) / 72f), 7, max);
        //    if (m_inventory.VisibleSlotsCount != m_inventoryGrid.Children.Count)
        //    {
        //        m_inventoryGrid.Children.Clear();
        //        m_inventoryGrid.RowsCount = 1;
        //        m_inventoryGrid.ColumnsCount = m_inventory.VisibleSlotsCount;
        //        for (int i = 0; i < m_inventoryGrid.ColumnsCount; i++)
        //        {
        //            InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
        //            inventorySlotWidget.AssignInventorySlot(m_inventory, i);
        //            inventorySlotWidget.Size = new Vector2(54f, 54f);
        //            inventorySlotWidget.BevelColor = new Color(181, 172, 154) * 0.6f;
        //            inventorySlotWidget.CenterColor = new Color(181, 172, 154) * 0.33f;
        //            m_inventoryGrid.Children.Add(inventorySlotWidget);
        //            m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(i, 0));
        //        }
        //    }
        //    base.MeasureOverride(parentAvailableSize);
        //}

        public class ResizeDialog : Dialog
        {
            public ButtonWidget m_confirmButton;

            public ButtonWidget m_addButton;

            public ButtonWidget m_reduceButton;

            public SliderWidget m_countSlider;


            public Action<float> m_handler;

            public float ScreenSize = 10f;

            public ResizeDialog(float Size, Action<float> handler)
            {
                this.ScreenSize = Size;
                XElement node = ContentManager.Get<XElement>("Dialogs/ResizeDialog");
                LoadContents(this, node);
                m_handler = handler;
                m_confirmButton = Children.Find<ButtonWidget>("ConfirmButton");
                m_countSlider = Children.Find<SliderWidget>("CountSlider");
                m_addButton = Children.Find<ButtonWidget>("AddButton");
                m_reduceButton = Children.Find<ButtonWidget>("ReduceButton");
                m_countSlider.MinValue = 0.1f;
                m_countSlider.MaxValue = 30f;
                m_countSlider.Granularity = .1f;
                m_countSlider.Value = Size;

            }

            public override void Update()
            {
                m_countSlider.Text = "Size (m):" + ScreenSize;
                ScreenSize = MathUtils.Clamp(m_countSlider.Value, 0f, 30f);
                m_addButton.IsEnabled = m_countSlider.Value < 30f;
                m_reduceButton.IsEnabled = m_countSlider.Value > 0.1f;
                if (m_reduceButton.IsClicked) m_countSlider.Value -= .1f;
                else if (m_addButton.IsClicked) m_countSlider.Value += .1f;
                else if (m_confirmButton.IsClicked)
                {
                    DialogsManager.HideDialog(this);
                    if (m_handler != null)
                    {
                        m_handler(ScreenSize);
                    }
                }
            }
        }

        public class ResolvingPowerDialog : Dialog
        {
            public ButtonWidget m_confirmButton;

            public ButtonWidget m_addButton;
            public ButtonWidget m_addButton2;

            public ButtonWidget m_reduceButton;
            public ButtonWidget m_reduceButton2;

            public SliderWidget m_countSlider;
            public SliderWidget m_countSlider2;


            public Action<int, int> m_handler;

            public int Width = 1000;
            public int Height = 600;

            public ResolvingPowerDialog(int ResolvingPower_Width, int ResolvingPower_Height, Action<int, int> handler)
            {
                Width = ResolvingPower_Width;
                Height = ResolvingPower_Height;
                XElement node = ContentManager.Get<XElement>("Dialogs/ResolvingPowerDialog");
                LoadContents(this, node);
                m_handler = handler;
                m_confirmButton = Children.Find<ButtonWidget>("ConfirmButton");
                m_countSlider = Children.Find<SliderWidget>("CountSlider");
                m_addButton = Children.Find<ButtonWidget>("AddButton");
                m_reduceButton = Children.Find<ButtonWidget>("ReduceButton");
                m_countSlider.MinValue = 100f;
                m_countSlider.MaxValue = 2000f;
                m_countSlider.Granularity = 100f;
                m_countSlider.Value = Width;

                m_countSlider2 = Children.Find<SliderWidget>("CountSlider2");
                m_addButton2 = Children.Find<ButtonWidget>("AddButton2");
                m_reduceButton2 = Children.Find<ButtonWidget>("ReduceButton2");
                m_countSlider2.MinValue = 100f;
                m_countSlider2.MaxValue = 2000f;
                m_countSlider2.Granularity = 100f;
                m_countSlider2.Value = Height;

            }

            public override void Update()
            {
                m_countSlider2.Text = "Height:" + Height;
                Height = MathUtils.Clamp((int)m_countSlider2.Value, 100, 2000);
                m_addButton2.IsEnabled = m_countSlider2.Value < 2000f;
                m_reduceButton2.IsEnabled = m_countSlider2.Value > 100f;
                if (m_reduceButton2.IsClicked) m_countSlider2.Value -= 100;
                else if (m_addButton2.IsClicked) m_countSlider2.Value += 100;

                m_countSlider.Text = "Width:" + Width;
                Width = MathUtils.Clamp((int)m_countSlider.Value, 100, 2000);
                m_addButton.IsEnabled = m_countSlider.Value < 2000f;
                m_reduceButton.IsEnabled = m_countSlider.Value > 100f;
                if (m_reduceButton.IsClicked) m_countSlider.Value -= 100;
                else if (m_addButton.IsClicked) m_countSlider.Value += 100;
                else if (m_confirmButton.IsClicked)
                {
                    DialogsManager.HideDialog(this);
                    if (m_handler != null)
                    {
                        m_handler(Width, Height);
                    }
                }
            }
        }
        public override void Update()
        {
            //m_componentPlayer.ComponentInput.AllowHandleInput
            //Keyboard.
            //m_moneyLabel.Text = "余额:" + 1001;//m_subsystemFinancial.Money;
            //m_shoplevelLabel.Text = "商店等级:" + 1002;//m_subsystemFinancial.ShopLevel;
            //m_chooseitemcountLabel.Text = "购买个数:" + ItemCount;
            //int num = Terrain.ExtractContents(ItemValue);
            //Block block = BlocksManager.Blocks[num];
            //m_chooseitemLabel.Text = "己选择物品:" + block.GetDisplayName(null, ItemValue);
            //m_itemvalueicon.Value = ItemValue;
            //if (m_chooseButton.IsClicked)
            //{
            //DialogsManager.ShowDialog(null, new ChooseItemsDialog(m_subsystemFinancial, delegate (int value)
            //{
            //    ItemValue = value;
            //}));
            //}



            ResolvingPowerLabel.Text = $"分辨率: {ResolvingPower_Width}*{ResolvingPower_Height}";

            if (ResolvingPowerBtn.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ResolvingPowerDialog(ResolvingPower_Width, ResolvingPower_Height, delegate (int width, int height)
                {
                    ResolvingPower_Height = height;
                    ResolvingPower_Width = width;
                }));
            }
            else if (ResizeBtn.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ResizeDialog(ScreenSize, delegate (float ScreenSize)
                {
                    this.ScreenSize = ScreenSize;
                }));
            }
            else if (CreatePreviewButton.IsClicked)
            {
                //CEF_Browser CefIns = WebTV.getInstance();

                CEF_Browser CefIns;
                try
                {
                    CefIns = WebTV.CreateInstanece();
                    //ScreenLog.Info(ScreenPositionTextbox.Text);
                    string[] posArr = ScreenPositionTextbox.Text.Split(',');
                    if (posArr.Length != 3) throw new Exception("非法的坐标长度！");
                    playerPosition.X = float.Parse(posArr[0]);
                    playerPosition.Y = float.Parse(posArr[1]);
                    playerPosition.Z = float.Parse(posArr[2]);
                    CefIns.link = SiteAddrDialog.Text;
                    CefIns.width = ResolvingPower_Width;
                    CefIns.height = ResolvingPower_Height;
                }
                catch (Exception e)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage(e.ToString(), Color.Red, false, true);
                    return;
                }
                CefIns.posVec = playerPosition;


                try
                {
                    float size = ScreenSize;//82;
                    float UpProportion = (float)ResolvingPower_Height / (float)ResolvingPower_Width;

                    Vector3 eyePosition;
                    Vector3 u = new Vector3(0f, 1f/*UpProportion*/, 0f);
                    Vector3 v = new Vector3(1f, 0f, 0f);
                    if (FacingState == Facing.Eye)
                    {
                        eyePosition = m_componentPlayer.ComponentCreatureModel.EyeRotation.GetForwardVector();
                        CefIns.ScreenNormalVector = eyePosition;
                        Vector3[] uv = util.GetUVByPlaneNormal(eyePosition);
                        u = uv[0];
                        v = uv[1];
                    }
                    else if (FacingState == Facing.X)
                    {
                        CefIns.ScreenNormalVector = new Vector3(1, 0, 0);
                        v = new Vector3(0, 0, 1);
                    }
                    else if (FacingState == Facing.Y)
                    {
                        CefIns.ScreenNormalVector = new Vector3(0, -1, 0);
                        u = new Vector3(0, 0, 1/*UpProportion*/);
                    }
                    else if (FacingState == Facing.Z)
                    {
                        CefIns.ScreenNormalVector = new Vector3(0, 0, 1);
                    }
                    CefIns.U = u;
                    CefIns.V = v;


                    Pattern pattern = new Pattern
                    {
                        Point = new Point3(CefIns.posVec),
                        Color = Color.White,
                        Size = size,// / 20.418f,
                        TexName = "ICON",
                        Position = new Vector3(CefIns.posVec.X, CefIns.posVec.Y, CefIns.posVec.Z),
                        //Up = new Vector3(0f, UpProportion, 0f),
                        //Up = eyePosition,
                        Up = u * UpProportion,
                        //Right = new Vector3(1f, 0f, 0f)
                        Right = v
                    };
                    MemoryStream pngStream = new();
                    int cef_width = CefIns.width;
                    int cef_height = CefIns.height;
                    CefIns.IngameWidth = size;
                    CefIns.IngameHeight = size * UpProportion;

                    CefIns.DiagonalLength = MathUtils.Sqrt(CefIns.IngameWidth * CefIns.IngameWidth + CefIns.IngameHeight * CefIns.IngameHeight);

                    //CefIns.IngameHeight = size * UpProportion;
                    var bmPNG = new System.Drawing.Bitmap(cef_width, cef_height);

                    //绘制文字
                    var g = System.Drawing.Graphics.FromImage(bmPNG);
                    var textArea = new System.Drawing.RectangleF(0, 0, bmPNG.Width, bmPNG.Height);
                    var font = new System.Drawing.Font(new System.Drawing.FontFamily("微软雅黑"), 12.0f, System.Drawing.FontStyle.Regular);
                    var brush = new System.Drawing.SolidBrush(System.Drawing.Color.AliceBlue);
                    //蓝色画笔
                    System.Drawing.Pen bluePen = new(System.Drawing.Color.GreenYellow, 5);
                    g.DrawRectangle(bluePen, 10, 10, cef_width - 10, cef_height - 10);
                    g.DrawString($"\n  WebTV Mod - {DateTime.Now}\n  Screen Siz: {ScreenSize}\n  Resolution: {cef_width} * {cef_height} ({CefIns.posVec.X}, {CefIns.posVec.Y}, {CefIns.posVec.Z})\n  Site Link: {CefIns.link}\n  Browser screen ready.", font, brush, textArea);
                    g.Dispose();

                    System.Drawing.Image image = bmPNG;// System.Drawing.Bitmap.FromFile("D:\\SurvivalCraft\\Temp\\icon.png");
                    image.Save(pngStream, System.Drawing.Imaging.ImageFormat.Png);
                    pngStream.Position = 0;
                    pattern.Texture = Texture2D.Load(pngStream);
                    pngStream.Position = 0;
                    pattern.DataTexture = Texture2D.Load(pngStream);
                    pngStream.Dispose();
                    image.Dispose();

                    CefIns.pattern = pattern;
                    CefIns.initPreview = true;
                }
                catch (Exception e2)
                {
                    ScreenLog.Info(e2);
                }
                m_componentPlayer.ComponentGui.DisplaySmallMessage("预览已创建，位置等信息已保存至预览", Color.Green, false, false);
            }
            else if (CreateBrowserButton.IsClicked)
            {
                CEF_Browser CefIns = WebTV.GetNoneBrowserCef();
                if (CefIns == null || !CefIns.initPreview)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("错误: 浏览器需要在预览屏幕上进行创建", Color.Red, false, true);
                }
                else
                {
                    if (CefIns.Browser != null)
                    {
                        CefIns.link = SiteAddrDialog.Text;
                        m_componentPlayer.ComponentGui.DisplaySmallMessage("浏览器已存在，正在尝试加载页面...", Color.Orange, false, true);
                        try
                        {
                            CefIns.Browser.Load(CefIns.link);
                        }
                        catch (Exception e)
                        {
                            ScreenLog.Error(e);
                        }
                        return;
                    }
                    CefIns.Create();
                }
            }
            else if (ClosePageButton.IsClicked)
            {
                CEF_Browser CefIns = WebTV.GetLastElement();

                if (CefIns != null && CefIns.Browser != null)
                {
                    CefIns.Browser.Load("about:blank");
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("当前标签页已关闭", Color.Green, false, true);
                    return;
                }
                m_componentPlayer.ComponentGui.DisplaySmallMessage("浏览器未开启！未找到可关闭的标签", Color.Orange, false, true);
            }
            else if (CloseBrowserButton.IsClicked)
            {
                CEF_Browser CefIns = WebTV.GetLastElement();

                if (CefIns != null)// && CefIns.Browser != null)
                {
                    CefIns.Close();
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("浏览器已成功关闭", Color.Green, false, true);
                    return;
                }
                m_componentPlayer.ComponentGui.DisplaySmallMessage("浏览器未开启！未找到可关闭的浏览器", Color.Orange, false, true);

            }
            else if (FacingBtn.IsClicked)
            {
                if ((int)FacingState < 3) FacingState = FacingState + 1;
                else FacingState = Facing.X;
                m_facing.Subtexture = ContentManager.Get<Subtexture>($"Textures/Facing_{FacingState}");
            }
            //m_componentPlayer.ComponentGui.DisplaySmallMessage("未选择购买商品", Color.White, blinking: true, playNotificationSound: true);
        }
    }

}
