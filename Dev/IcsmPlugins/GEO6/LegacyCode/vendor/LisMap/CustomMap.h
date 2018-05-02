//---------------------------------------------------------------------------


#ifndef CustomMapH
#define CustomMapH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <ToolWin.hpp>
#include <math.h>
#include "BaseMap.h"
#include "CoordConv.hpp"
#include <Menus.hpp>
#include <memory>
using namespace std;
using namespace Lis_map;

enum LayerEnum { layer_default, layer_coordZone, layer_zone, layer_link, layer_station, layer_DuelResult, layer_last };

//---------------------------------------------------------------------------

enum CustomObjectType {cotNone = 0, cotStation = 1, cotLink = 2, cotRelLine = 3,
                       cotCoordZone = 4, cotCovZone = 5, cotAntPattern = 6, cotAllotCont, cotLast};

//---------------------------------------------------------------------------

typedef void __fastcall (__closure *TOnMenuStation)
    (TObject *Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift);
//---------------------------------------------------------------------------

class TCustomMapFrame : public TFrame
{
__published:	// IDE-managed Components
    TStatusBar *sb;
    TBaseMapFrame *bmf;
    TToolBar *tb;
    TToolButton *tb1;
    TToolButton *tb2;
    TToolButton *tb3;
    TToolButton *tb4;
    TToolButton *tb5;
    TToolButton *tb6;
    void __fastcall FrameMouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall FrameMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
    void __fastcall FrameMouseUp(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall tb2Click(TObject *Sender);
    void __fastcall FrameMouseWheelDown(TObject *Sender, TShiftState Shift,
          TPoint &MousePos, bool &Handled);
    void __fastcall FrameMouseWheelUp(TObject *Sender, TShiftState Shift,
          TPoint &MousePos, bool &Handled);
    void __fastcall bmfactZoomInTwiceExecute(TObject *Sender);
    void __fastcall bmfactZoomOutTwiceExecute(TObject *Sender);
    void __fastcall bmfactZoomDefaultExecute(TObject *Sender);
    void __fastcall bmfactZoomCustExecute(TObject *Sender);
private:	// User declarations
    TCoordinateConvertor *cc;
    void __fastcall SetLayerParam(unsigned int __index, bool _visible, AnsiString _caption, bool _selectable);
    TMouseEvent oldMouseDown;
    TMouseMoveEvent oldMouseMove;
    TMouseEvent oldMouseUp;
public:		// User declarations
    __fastcall TCustomMapFrame(TComponent* Owner);
    __fastcall void Init();

    typedef std::vector<double> Zone;

    void __fastcall Clear(int layer, bool refresh = false);
    //Очищается либо заданный уровень, либо вся карта.

    MapPoint* __fastcall ShowPoint(double lon, double lat, TColor _color, int _width,
            PointType type, AnsiString label, AnsiString hint);
    MapLink* __fastcall ShowLine(double lon1, double lat1, double lon2,
            double lat2, int trim, int layer, TColor _color, int _width, int _tag,
            MapArrowType mat = matNone);
    MapPolygon* __fastcall ShowZone(double centLon, double centLat, Zone zone, int width,
                            TColor color, TPenStyle style, CustomObjectType objType, int layer);

    MapPoint* __fastcall ShowStation(double lon, double lat, AnsiString label, AnsiString hint);
    MapLink* __fastcall ShowLink(double lon1, double lat1, double lon2,
            double lat2, int trim, MapArrowType mat = matNone);
    MapLink* __fastcall ShowRelLine(double lon1, double lat1, double lon2,
            double lat2, MapArrowType mat = matNone);
    MapPolygon* __fastcall ShowContour(Lis_map::Points points, AnsiString label, AnsiString hint);
    MapPolygon* __fastcall ShowCoordZone(double centLon, double centLat, Zone zone);
    MapPolygon* __fastcall ShowCoverageZone(double centLon, double centLat, Zone zone);
    MapPolygon* __fastcall ShowAntennaPattern(double centLon, double centLat, Zone zone);
    TBaseMapFrame::Shapes __fastcall ShowDuelResult(Lis_map::Point a, Lis_map::Point b, Zone aNoise, Zone aInterf,
                                   Zone bNoise, Zone bInterf, Lis_map::Point c, Lis_map::Point d);

    void __fastcall RemoveRelLine();
    void __fastcall Highlight(int objId);
    void __fastcall UnHighliht(int objId);
    void __fastcall UnHighlihtAll();

    void __fastcall RemoveObject(int id);

private:
    //цвета объектов в обычном состоянии
    TColor cSta;
    TColor cLink;
    TColor cRelLine;
    TColor cCoordZone;
    TColor cCovZone;
    TColor cAntPat;
    TColor cAllot;

    //цвета объектов в подсвеченом состоянии
    TColor cStaH;
    TColor cLinkH;
    TColor cRelLineH;
    TColor cCoordZoneH;
    TColor cCovZoneH;
    TColor cAntPatH;
    TColor cAllotH;

    //толщина линии объекта
    int iWidthSta;
    int iWidthLink;
    int iWidthRelLine;
    int iWidthCoordZone;
    int iWidthCovZone;
    int iWidthAntPat;

    //стиль линии объекта
    TPenStyle pstLink;
    TPenStyle pstRelLine;
    TPenStyle pstCoordZone;
    TPenStyle pstCovZone;
    TPenStyle pstAntPat;

    //стиль заливки объекта
    //   !!! РЕАЛИЗОВАТЬ !!!

    std::map<int, TColor*> mColorUH;
    std::map<int, TColor*> mColorH;

    std::vector<int> vH;
    void __fastcall ShowZoom();

protected:
    TColor GetCSUH()        {return cSta; };
    TColor GetCSH()         {return cStaH; };
    TColor GetCLUH()        {return cLink; };
    TColor GetCLH()         {return cLinkH; };
    TColor GetCRLUH()       {return cRelLine; };
    TColor GetCRLH()        {return cRelLineH; };
    TColor GetCCDZUH()      {return cCoordZone; };
    TColor GetCCDZH()       {return cCoordZoneH; };
    TColor GetCCVZUH()      {return cCovZone; };
    TColor GetCCVZH()       {return cCovZoneH; };
    TColor GetCAPUH()       {return cAntPat; };
    TColor GetCAPH()        {return cAntPatH; };
    int GetWS()             {return iWidthSta; };
    int GetWL()             {return iWidthLink; };
    int GetWRL()            {return iWidthRelLine; };
    int GetWCDZ()           {return iWidthCoordZone; };
    int GetWCVZ()           {return iWidthCovZone; };
    int GetWAP()            {return iWidthAntPat; };
    TPenStyle GetSL()       {return pstLink; };
    TPenStyle GetSRL()      {return pstRelLine; };
    TPenStyle GetSCDZ()     {return pstCoordZone; };
    TPenStyle GetSCVZ()     {return pstCovZone; };
    TPenStyle GetSAP()      {return pstAntPat; };

    template <class T1>
    void __fastcall SetParam(T1& variable, T1 parametr)
    {
        if(parametr != variable)
        {
            variable = parametr;
            //...
        }
    };

    void __fastcall SetCSUH(TColor param)         {SetParam(cSta, param); };
    void __fastcall SetCSH(TColor param)          {SetParam(cStaH, param); };
    void __fastcall SetCLUH(TColor param)         {SetParam(cLink, param); };
    void __fastcall SetCLH(TColor param)          {SetParam(cLinkH, param); };
    void __fastcall SetCRLUH(TColor param)        {SetParam(cRelLine, param); };
    void __fastcall SetCRLH(TColor param)         {SetParam(cRelLineH, param); };
    void __fastcall SetCCDZUH(TColor param)       {SetParam(cCoordZone, param); };
    void __fastcall SetCCDZH(TColor param)        {SetParam(cCoordZoneH, param); };
    void __fastcall SetCCVZUH(TColor param)       {SetParam(cCovZone, param); };
    void __fastcall SetCCVZH(TColor param)        {SetParam(cCovZoneH, param); };
    void __fastcall SetCAPUH(TColor param)        {SetParam(cAntPat, param); };
    void __fastcall SetCAPH(TColor param)         {SetParam(cAntPatH, param); };
    void __fastcall SetWS(int param)              {SetParam(iWidthSta, param); };
    void __fastcall SetWL(int param)              {SetParam(iWidthLink, param); };
    void __fastcall SetWRL(int param)             {SetParam(iWidthRelLine, param); };
    void __fastcall SetWCDZ(int param)            {SetParam(iWidthCoordZone, param); };
    void __fastcall SetWCVZ(int param)            {SetParam(iWidthCovZone, param); };
    void __fastcall SetWAP(int param)             {SetParam(iWidthAntPat, param); };
    void __fastcall SetSL(TPenStyle param)        {SetParam(pstLink, param); };
    void __fastcall SetSRL(TPenStyle param)       {SetParam(pstRelLine, param); };
    void __fastcall SetSCDZ(TPenStyle param)      {SetParam(pstCoordZone, param); };
    void __fastcall SetSCVZ(TPenStyle param)      {SetParam(pstCovZone, param); };
    void __fastcall SetSAP(TPenStyle param)       {SetParam(pstAntPat, param); };

public:
    TOnMenuStation omsCallBack;

    __property TColor ColorStationUnH = {read = GetCSUH, write = SetCSUH};
    __property TColor ColorStationH = {read = GetCSH, write = SetCSH};
    __property TColor ColorLinkUnH = {read = GetCLUH, write = SetCLUH};
    __property TColor ColorLinkH = {read = GetCLH, write = SetCLH};
    __property TColor ColorRelLineUnH = {read = GetCRLUH, write = SetCRLUH};
    __property TColor ColorRelLineH = {read = GetCRLH, write = SetCRLH};
    __property TColor ColorCoordZoneUnH = {read = GetCCDZUH, write = SetCCDZUH};
    __property TColor ColorCoordZoneH = {read = GetCCDZH, write = SetCCDZH};
    __property TColor ColorCoverageZoneUnH = {read = GetCCVZUH, write = SetCCVZUH};
    __property TColor ColorCoverageZoneH = {read = GetCCVZH, write = SetCCVZH};
    __property TColor ColorAntennaPatternUnH = {read = GetCAPUH, write = SetCAPUH};
    __property TColor ColorAntennaPatternH = {read = GetCAPH, write = SetCAPH};
    __property int WidthStation = {read = GetWS, write = SetWS};
    __property int WidthLink = {read = GetWL, write = SetWL};
    __property int WidthRelLine = {read = GetWRL, write = SetWRL};
    __property int WidthCoordZone = {read = GetWCDZ, write = SetWCDZ};
    __property int WidthCoverageZone = {read = GetWCVZ, write = SetWCVZ};
    __property int WidthAntennaPattern = {read = GetWAP, write = SetWAP};
    __property TPenStyle StyleLink = {read = GetSL, write = SetSL};
    __property TPenStyle StyleRelLine = {read = GetSRL, write = SetSRL};
    __property TPenStyle StyleCoordZone = {read = GetSCDZ, write = SetSCDZ};
    __property TPenStyle StyleCoverageZone = {read = GetSCVZ, write = SetSCVZ};
    __property TPenStyle StyleAntennaPattern = {read = GetSAP, write = SetSAP};

    void __fastcall SetCenter(double lon, double lat);
    void __fastcall SetScale(double scale);
};
//---------------------------------------------------------------------------
extern PACKAGE TCustomMapFrame *CustomMapFrame;
//---------------------------------------------------------------------------
#endif
