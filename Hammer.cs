// Game.SubsystemCoalBlockBehavior
using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Game;
using TemplatesDatabase;
namespace Game
{
    public class SubsystemStoneBlockBehavior : SubsystemBlockBehavior
    {
        //public struct TreasureData
        //{
        //    public int Value;

        //    public float Probability;

        //    public int MaxCount;
        //}

        //[Serializable]
        //public sealed class c
        //{
        //    public static readonly c _ = new c();

        //    public static Func<TreasureData, float> __6_0;

        //    public float OnNeighborBlockChanged_b__6_0(TreasureData t)
        //    {
        //        return t.Probability;
        //    }
        //}

        public SubsystemPickables m_subsystemPickables;

        public Game.Random m_random = new Game.Random();

        //public static TreasureData[] m_treasureData;

        public override int[] HandledBlocks => new int[1] { 16 };

        public short[] pos = null;
        public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
        {
            if (pos != null) return;

            var Player = EGlobal.getPlayer();
            var PlayerBody = Player.ComponentBody;

            var coordinate = PlayerBody.m_position;
            if (HammerData.GameMode == GameMode.Creative)
            {
                var creativeInventory = Player.Entity.FindComponent<ComponentCreativeInventory>();
                var activeValue = creativeInventory.GetSlotValue(creativeInventory.m_activeSlotIndex);
                if (activeValue != HammerData.SteelHammerValue) return;
            }
            else
            {
                var survivalInventory = Player.Entity.FindComponent<ComponentInventory>();
                var activeValue = survivalInventory.GetSlotValue(survivalInventory.m_activeSlotIndex);
                if (activeValue != HammerData.SteelHammerValue) return;
            }

            var fvec3 = PlayerBody.m_rotation.GetForwardVector();
            var blockUnitVector = new Vector3(x - MathUtils.Floor(coordinate.X), y - MathUtils.Floor(coordinate.Y), z - MathUtils.Floor(coordinate.Z));
            var projectionY = blockUnitVector.Y / blockUnitVector.Length();
            //ScreenLog.Info($"{y} // {coordinate.Y} // {projectionY}");
            //XLog.Information("........");
            //XLog.Information("projectionY: " + projectionY);
            //XLog.Information($"PlayerCoor: {coordinate.X} {coordinate.Y} {coordinate.Z}");
            //XLog.Information($"relative: {relative[0]} {relative[1]} {relative[2]}");
            //XLog.Information($"blockPos: {x} {y} {z}");
            if (projectionY > 0.75 || projectionY < -0.7)
            {
                //y
                for (pos = new short[2] { -1, -1 }; pos[1] <= 1;)
                {
                    //base.SubsystemTerrain.Terrain.GetCellContents
                    base.SubsystemTerrain.DestroyCell(10, x + pos[0], y, z + pos[1], 0, false, true);
                    if (++pos[0] > 1)
                    {
                        pos[0] = -1;
                        pos[1]++;
                    }
                }
            }
            else if (MathUtils.Abs(fvec3.Z) > MathUtils.Abs(fvec3.X))
            {
                //z
                for (pos = new short[2] { -1, -1 }; pos[1] <= 1;)
                {
                    base.SubsystemTerrain.DestroyCell(10, x + pos[0], y + pos[1], z, 0, false, true);
                    if (++pos[0] > 1)
                    {
                        pos[0] = -1;
                        pos[1]++;
                    }
                }

            }
            else
            {
                //x
                for (pos = new short[2] { -1, -1 }; pos[1] <= 1;)
                {
                    base.SubsystemTerrain.DestroyCell(10, x, y + pos[1], z + pos[0], 0, false, true);
                    if (++pos[0] > 1)
                    {
                        pos[0] = -1;
                        pos[1]++;
                    }
                }
            }


            //3x3x3
            //for (pos = new short[] { -1, -1, -1 }; pos[2] < 2;)
            //{
            //    base.SubsystemTerrain.DestroyCell(10, x + pos[0], y + pos[1], z + pos[2], 0, false, true);
            //    if (++pos[0] == 2)
            //    {
            //        pos[0] = -1;
            //        if (++pos[1] == 2)
            //        {
            //            pos[2]++;
            //            pos[1] = -1;
            //        }
            //    }
            //}


            pos = null;
        }

        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            return;
            int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(neighborX, neighborY, neighborZ);
            if (cellContents != 0 && cellContents != 18)
            {
                return;
            }
            x = neighborX;
            y = neighborY;
            z = neighborZ;
            //base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(0));

            //if (!m_random.Bool(1f))
            //{
            //    return;
            //}
            //int num = 0;
            //int num2 = 0;
            ////float max = ((IEnumerable<TreasureData>)m_treasureData).Sum((Func<TreasureData, float>)c._.OnNeighborBlockChanged_b__6_0);
            //float max = m_treasureData.Sum(c._.OnNeighborBlockChanged_b__6_0);
            //float num3 = m_random.Float(0f, max);
            //TreasureData[] treasureData = m_treasureData;
            //for (int i = 0; i < treasureData.Length; i++)
            //{
            //    TreasureData treasureData2 = treasureData[i];
            //    num3 -= treasureData2.Probability;
            //    if (num3 <= 0f)
            //    {
            //        num = treasureData2.Value;
            //        num2 = m_random.Int(1, treasureData2.MaxCount);
            //        break;
            //    }
            //}
            //if (num != 0 && num2 > 0)
            //{
            //    for (int j = 0; j < num2; j++)
            //    {
            //        m_subsystemPickables.AddPickable(num, 1, new Vector3(x, y, z) + m_random.Vector3(0.1f, 0.4f) + new Vector3(0.5f), Vector3.Zero, null);
            //    }
            //    int num4 = m_random.Int(3, 4);
            //    for (int k = 0; k < num4; k++)
            //    {
            //        m_subsystemPickables.AddPickable(248, 1, new Vector3(x, y, z) + m_random.Vector3(0.1f, 0.4f) + new Vector3(0.5f), Vector3.Zero, null);
            //    }
            //}
        }

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
        }

        //static SubsystemCoalBlockBehavior()
        //{
        //TreasureData[] array = new TreasureData[60];
        //TreasureData treasureData = (array[59] = new TreasureData
        //{
        //    Value = 22,
        //    Probability = 4f,
        //    MaxCount = 4
        //});
        //m_treasureData = array;
        //}
    }

}