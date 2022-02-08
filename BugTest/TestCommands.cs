using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(BugTest.TestCommands))]

namespace BugTest
{
    public static class TestCommands
    {
        public static Point2d Point3DTo2D(Point3d p)
        {
            return new Point2d(p.X, p.Y);
        }

        [CommandMethod("AddLine")]
        public static void AddLine() // This method can have any name
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var opt = new PromptPointOptions("Pick point") { AllowNone = false };
            var res = ed.GetPoint(opt);
            if (res.Status != PromptStatus.OK) return;

            var pt = res.Value;

            using (var trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                var pline1 = new Polyline(2);
                var pline2 = new Polyline(2);

                pline1.AddVertexAt(0, Point3DTo2D(pt), 0, 0, 0);
                pline1.AddVertexAt(1, Point3DTo2D(pt + Vector3d.XAxis * 100), 0, 0, 0);

                pline2.AddVertexAt(0, Point3DTo2D(pt), 0, 0, 0);
                pline2.AddVertexAt(1, Point3DTo2D(pt + Vector3d.YAxis * 100), 0, 0, 0);

                var ms = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false, true);
                ms.AppendEntity(pline1);
                ms.AppendEntity(pline2);

                trans.AddNewlyCreatedDBObject(pline1, true);
                trans.AddNewlyCreatedDBObject(pline2, true);

                XDataClass.WriteRegAppName(pline1);
                XDataClass.WriteRegAppName(pline2);

                trans.Commit();
            }
        }
    }
}
