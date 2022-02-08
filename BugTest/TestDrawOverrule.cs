using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace BugTest
{
    class TestDrawOverrule : DrawableOverrule
    {
        public static TestDrawOverrule Instance = new TestDrawOverrule("BugTest");

        private TestDrawOverrule(string regAppName)
        {
            SetXDataFilter(regAppName);
        }

        public override void ViewportDraw(Drawable d, ViewportDraw vd)
        {
            var pline = d as Polyline;
            //vd.Geometry.WorldLine(pline.StartPoint, pline.StartPoint + pline.StartPoint.GetVectorTo(pline.EndPoint).GetNormal() * 100);
            vd.Geometry.WorldLine(pline.StartPoint, pline.EndPoint);
        }

        public override bool WorldDraw(Drawable d, WorldDraw wd)
        {
            return false;
        }
    }
}
