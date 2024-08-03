using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETerminal
{
    public class ET_F3
    {
        public bool enabled = true;
        public LabelWidget label;
        public ComponentPlayer m_componentPlayer;
        public ComponentHealth m_componentHealth;
        public ComponentBody m_componentBody;
        public ComponentInventory m_componentInventory;
        public ComponentCreativeInventory m_componentCreativeInventory;
        public Entity entity;
        public TickTimer tickTimer;
        public ET_F3(Entity entity)
        {
            this.entity = entity;
            tickTimer = new TickTimer(50);
            m_componentPlayer = entity.FindComponent<ComponentPlayer>(throwOnError: true);
            m_componentHealth = entity.FindComponent<ComponentHealth>(throwOnError: true);
            m_componentBody = entity.FindComponent<ComponentBody>(throwOnError: true);
            m_componentInventory = m_componentPlayer.Entity.FindComponent<ComponentInventory>();
            m_componentCreativeInventory = m_componentPlayer.Entity.FindComponent<ComponentCreativeInventory>();


            label = new LabelWidget()
            {
                FontScale = 0.7f,
                Color = Color.White,
                Margin = new Vector2(4f, 0f),
                VerticalAlignment = WidgetAlignment.Stretch,
                Text = "F3 Label"
            };
            m_componentPlayer.GuiWidget.Children.Add(label);
        }
        public void Update()
        {
            if (enabled && tickTimer.Next())
            {
                var position = m_componentPlayer.ComponentBody.Position;
                var itemId = m_componentInventory.GetSlotValue(m_componentInventory.m_activeSlotIndex);
                var citemId = m_componentCreativeInventory.GetSlotValue(m_componentCreativeInventory.m_activeSlotIndex);
                var rotate = m_componentBody.Rotation;
                var f_vec = EGlobal.Player.ComponentCreatureModel.EyeRotation.GetForwardVector();
                var cpu_usage = Game.PerformanceManager.m_averageCpuFrameTime.m_value / Game.PerformanceManager.m_averageFrameTime.m_value;
                var health = m_componentHealth.Health;

                string text = "";
                text += $"Key: [F1]Log [F3]Info [/]Cmd\n";
                text += $"FPS: {1f / PerformanceManager.AverageFrameTime:0.0}\n";
                text += $"Location: {MathUtils.Floor(position.X)}, {MathUtils.Floor(position.Y)}, {MathUtils.Floor(position.Z)}";
                text += "\n";
                text += "RAM Total: " + Game.PerformanceManager.m_totalMemoryUsed / 1024 / 1024 + " MB";
                text += "\n";
                text += "GPU MEM  : " + Display.GetGpuMemoryUsage() / 1024 / 1024 + " MB";
                //text += "\n";
                //text += "GPU Total RAM: " + Game.PerformanceManager.m_totalGpuMemoryUsed / 1024 / 1024 + " MB";
                text += "\n";
                text += $"CPU: {util.Progress(cpu_usage, 20)} {Math.Round(cpu_usage, 4) * 100f} %";
                text += "\n";
                text += "System Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                text += "\n";
                text += "Game Time: " + m_componentBody.m_subsystemTime.GameTime + " s";
                text += "\n";
                text += "\n";
                text += "Game Mode: " + m_componentPlayer.PlayerData.m_subsystemGameInfo.WorldSettings.GameMode;
                text += "\n";
                text += $"Health: {util.Progress(health > 1 ? 1 : health, 20)} {Math.Round(health, 2)}";
                text += "\n";
                text += "Humidity: " + m_componentPlayer.m_subsystemTerrain.Terrain.GetHumidity((int)position.X, (int)position.Z);
                text += "\n";
                text += "Temperature: " + m_componentPlayer.m_subsystemTerrain.Terrain.GetTemperature((int)position.X, (int)position.Z);
                text += "\n";
                text += "Seed    : " + m_componentPlayer.PlayerData.m_subsystemGameInfo.WorldSeed;
                text += "\n";
                text += "Items   : " + m_componentPlayer.m_subsystemPickables.m_pickables.Count;
                text += "\n";
                text += "Entities: " + m_componentPlayer.Project.m_entities.Count;
                text += "\n";
                text += $"Position: {position.X}, {position.Y}, {position.Z}";
                text += "\n";
                text += $"Rotation: {rotate.X}, {rotate.Y}, {rotate.Z}, {rotate.W}";
                text += "\n";
                text += $"Facing  : {f_vec.X}, {f_vec.Y}, {f_vec.Z}";
                text += "\n";
                text += $"Slot Index/Value [Sur]: {m_componentInventory.m_activeSlotIndex} / {itemId}";
                text += "\n";
                text += $"Slot Index/Value [Cre]: {m_componentCreativeInventory.m_activeSlotIndex} / {citemId}";
                text += "\n";
                text += $"V {EGlobal.Version} [{EGlobal.Date}]";

                label.Text = text;
            }
        }
        public void Dispose()
        {
            m_componentPlayer.GuiWidget.Children.Remove(label);

        }

        public void SwitchState()
        {
            if (enabled) Disable();
            else Enable();
        }
        public void Enable()
        {
            this.enabled = true;
            label.IsVisible = true;
        }
        public void Disable()
        {
            this.enabled = false;
            label.IsVisible = false;
        }
    }
}
