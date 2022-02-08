using System;
using System.Diagnostics;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace BugTest
{
    internal class TestGripOverrule : GripOverrule
    {
        public static TestGripOverrule Instance = new TestGripOverrule("BugTest");

        private TestGripOverrule(string regAppName)
        {
            SetXDataFilter(regAppName);
        }
        public override void GetGripPoints(Entity entity, GripDataCollection grips, double curViewUnitSize, int gripSize, Vector3d curViewDir, GetGripPointsFlags bitFlags)
        {
            try
            {
                if (entity.ObjectId == ObjectId.Null || entity.Id.Handle.Value == 0)
                {
                    base.GetGripPoints(entity, grips, curViewUnitSize, gripSize, curViewDir, bitFlags);
                    return;
                }

                var pline = (Polyline)entity;

                grips.Add(new LineMoveGrip(pline, pline.StartPoint, GripTypeEnum.MainStart));
                grips.Add(new LineMoveGrip(pline, pline.EndPoint, GripTypeEnum.MainEnd));
                //grips.Add(new LineMoveGrip(pline, pline.StartPoint + pline.StartPoint.GetVectorTo(pline.EndPoint).GetNormal() * 100, GripTypeEnum.MainEnd));

                /*
                var entMapPropInfo = GetType().GetProperty("EntityMap", BindingFlags.Instance | BindingFlags.NonPublic);
                if (entMapPropInfo?.GetValue(this) is Dictionary<IntPtr, List<GripData>> entityMap && entityMap.ContainsKey(entity.UnmanagedObject))
                    entityMap.Remove(entity.UnmanagedObject);
                */
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public override void MoveGripPointsAt(Entity entity, GripDataCollection grips, Vector3d offset, MoveGripPointsFlags bitFlags)
        {
            try
            {
                if (entity.ObjectId == ObjectId.Null || entity.Id.Handle.Value == 0)
                {
                    //base.MoveGripPointsAt(entity, grips, offset, bitFlags);
                    return;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }
}
