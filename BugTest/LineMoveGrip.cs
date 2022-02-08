using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace BugTest
{
    internal class LineMoveGrip : GripData
    {
        private GripTypeEnum _gripType;
        private Polyline _pline;
        public LineMoveGrip(Polyline pline, Point3d point, GripTypeEnum gripType)
        {
            GripPoint = point;
            _gripType = gripType;
            _pline = pline;
        }
        public override bool WorldDraw(WorldDraw worldDraw, ObjectId entityId, DrawType type, Point3d? imageGripPoint, double dGripSize)
        {
            return false;
        }

        private static Point3dCollection GripSquare(ViewportDraw wd, ObjectId entid, int gsip, Point3d grippoint, double angle = 0)
        {
            var unit = wd.Viewport.GetNumPixelsInUnitSquare(grippoint);
            var dR = gsip / unit.X;
            var dr = dR / 1.0;

            using (var pl = new Polyline(4))
            {
                pl.AddVertexAt(0, new Point2d(grippoint.X + dr, grippoint.Y + dr), 0, 0, 0);
                pl.AddVertexAt(1, new Point2d(grippoint.X - dr, grippoint.Y + dr), 0, 0, 0);
                pl.AddVertexAt(2, new Point2d(grippoint.X - dr, grippoint.Y - dr), 0, 0, 0);
                pl.AddVertexAt(3, new Point2d(grippoint.X + dr, grippoint.Y - dr), 0, 0, 0);

                pl.TransformBy(Matrix3d.Rotation(angle, Vector3d.ZAxis, grippoint));

                return new Point3dCollection
                {
                    pl.GetPoint3dAt(0),
                    pl.GetPoint3dAt(1),
                    pl.GetPoint3dAt(2),
                    pl.GetPoint3dAt(3)
                };
            }
        }

        public override bool ViewportDraw(ViewportDraw wd, ObjectId entid, DrawType type, Point3d? imageGripPoint, int gripSizeInPixels)
        {
            if (entid == ObjectId.Null) return false;

            using (var pnts = GripSquare(wd, entid, gripSizeInPixels, GripPoint, 0))
            {
                wd.SubEntityTraits.FillType = FillType.FillAlways;
                wd.Geometry.Polygon(pnts);

                wd.SubEntityTraits.FillType = FillType.FillNever;
                //wd.SubEntityTraits.TrueColor = ConstantClass.BorderColor;
                wd.Geometry.Polygon(pnts);
            }
            return true;
        }

        public override ReturnValue OnHotGrip(ObjectId entid, Context contextFlags)
        {
            //using (Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (var trans = entid.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var pline = trans.GetObject(entid, OpenMode.ForWrite, false, true) as Polyline;
                if (pline == null) return ReturnValue.Failure;


                using (var jig = new LineEndMoveJig(pline, _gripType))
                {
                    var res0 = jig.Editor.Drag(jig);

                    if (res0.Status != PromptStatus.OK)
                    {
                        //trans.Abort();
                        return ReturnValue.GripHotToWarm;
                    }

                    //Autodesk.AutoCAD.Internal.Utils.RegenEntity(entid);
                    trans.Commit();
                    return ReturnValue.GetNewGripPoints;
                }
            }
            return ReturnValue.GripHotToWarm;
        }
    }
}
