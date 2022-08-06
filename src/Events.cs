using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace VSNats.Events
{
    public abstract class NatsEvent
    {
        public string Type { get => "Event"; }
    }

    public class Vec3iWrapper
    {
        public int X;
        public int Y;
        public int Z;
        public Vec3iWrapper() { }
        public Vec3iWrapper(Vec3i v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
        public Vec3iWrapper(BlockPos v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
    }

    public class Vec3dWrapper
    {
        public double X;
        public double Y;
        public double Z;
        public Vec3dWrapper() { }
        public Vec3dWrapper(Vec3d v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
        public Vec3dWrapper(BlockPos v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
    }

    public class Vec2iWrapper
    {
        public Vec2iWrapper() { }
        public Vec2iWrapper(Vec2i v)
        {
            X = v.X;
            Y = v.Y;
        }
        public int X;
        public int Y;
    }

    public class Vec2dWrapper
    {
        public Vec2dWrapper() { }
        public Vec2dWrapper(Vec2d v)
        {
            X = v.X;
            Y = v.Y;
        }
        public double X;
        public double Y;
    }

    public class BlockSelectionWrapper
    {
        public Vec3dWrapper Position;
        public string Face;
        public Vec3dWrapper HitPosition;
        public int SelectionBoxIndex;
        public bool DidOffset;

        public BlockSelectionWrapper(BlockSelection bs)
        {
            Position = new Vec3dWrapper(bs.Position);
            Face = bs.Face.Code;
            HitPosition = new Vec3dWrapper(bs.HitPosition);
            SelectionBoxIndex = bs.SelectionBoxIndex;
            DidOffset = bs.DidOffset;
        }
    }
}