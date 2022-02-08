using System;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace BugTest
{
    internal class LineEndMoveJig : DrawJig, IDisposable
    {
        private Point3d _pnt1;
        internal Point3d Pnt1;
        private Polyline _pline;
        private Point3d _position;
        private Point3d _basePoint;
        private readonly Editor _ed;
        private readonly GripTypeEnum _gripType;

        public LineEndMoveJig(Polyline pline, GripTypeEnum gripType)
        {
            _pline = pline;
            _gripType = gripType;
            _ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            _ed.PointMonitor += ed_PointMonitor;
        }

        public Editor Editor => _ed;

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            switch (_gripType)
            {
                case GripTypeEnum.MainStart:
                    _basePoint = _pline.StartPoint;
                    break;

                case GripTypeEnum.MainEnd:
                    _basePoint = _pline.EndPoint;
                    break;
            }
            var optPoint = new JigPromptPointOptions("Pick new point")
            {
                UserInputControls = UserInputControls.GovernedByOrthoMode | UserInputControls.GovernedByUCSDetect
            };
            var resPoint = prompts.AcquirePoint(optPoint);
            if (resPoint.Status == PromptStatus.OK)
            {
                if (_pnt1.DistanceTo(resPoint.Value) < Tolerance.Global.EqualPoint)
                    return SamplerStatus.NoChange;

                _pnt1 = resPoint.Value;
                Pnt1 = _position;
            }
            return SamplerStatus.OK;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            if (draw.Geometry != null)
            {
                switch (_gripType)
                {
                    case GripTypeEnum.MainStart:
                        _pline.SetPointAt(0, TestCommands.Point3DTo2D(Pnt1));
                        break;

                    case GripTypeEnum.MainEnd:
                        _pline.SetPointAt(1, TestCommands.Point3DTo2D(Pnt1));
                        break;
                }

                draw.Geometry.WorldLine(_pline.StartPoint, _pline.EndPoint);
            }

            return true;
        }

        void ed_PointMonitor(object sender, PointMonitorEventArgs e)
        {
            _position = e.Context.ComputedPoint;
        }

        public void Dispose()
        {
            _ed.PointMonitor -= ed_PointMonitor;
        }
    }
}
