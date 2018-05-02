//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "CustomMap.h"
#include "uAnalyzer.h"
#include "uParams.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "BaseMap"
#pragma resource "*.dfm"

TCustomMapFrame *CustomMapFrame;

using namespace Lis_map;
//---------------------------------------------------------------------------
__fastcall TCustomMapFrame::TCustomMapFrame(TComponent* Owner)
    : TFrame(Owner)
{
    cSta            = 0x00CC3333;
    cLink           = 0x00006699;
    cRelLine        = 0x000033FF;
    cCoordZone      = 0x00339900;
    cCovZone        = 0x00990099;
    cAntPat         = 0x00FF9999;
    cStaH           = 0x00FFFF66;
    cLinkH          = 0x000066FF;
    cRelLineH       = 0x009999FF;
    cCoordZoneH     = clRed; //0x0033FF99;
    cCovZoneH       = clGreen; //0x00FF00FF;
    cAntPatH        = 0x00FFCC99;
    cAllot          = clNavy;
    cAllotH         = cStaH;

    iWidthSta       = 5;
    iWidthLink      = 1;
    iWidthRelLine   = 1;
    iWidthCoordZone = 2;
    iWidthCovZone   = 1;
    iWidthAntPat    = 1;

    pstLink         = psSolid;
    pstRelLine      = psDash;
    pstCoordZone    = psSolid;
    pstCovZone      = psSolid;
    pstAntPat       = psDash;

    mColorUH[cotStation]    = &cSta;
    mColorUH[cotLink]       = &cLink;
    mColorUH[cotRelLine]    = &cRelLine;
    mColorUH[cotCoordZone]  = &cCoordZone;
    mColorUH[cotCovZone]    = &cCovZone;
    mColorUH[cotAntPattern] = &cAntPat;
    mColorUH[cotAllotCont]  = &cAllot;

    mColorH[cotStation]    = &cStaH;
    mColorH[cotLink]       = &cLinkH;
    mColorH[cotRelLine]    = &cRelLineH;
    mColorH[cotCoordZone]  = &cCoordZoneH;
    mColorH[cotCovZone]    = &cCovZoneH;
    mColorH[cotAntPattern] = &cAntPatH;
    mColorH[cotAllotCont]  = &cAllotH;

    omsCallBack = NULL;

    cc = new TCoordinateConvertor(this);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::SetLayerParam(unsigned int __index, bool _visible, AnsiString _caption, bool _selectable)
{
    int n = bmf->GetLayersCount();
    if(__index >= n)
        throw *(new Exception("SetLayerParam: заданный слой не существует."));
    bmf->SetLayerVisible(__index, _visible);
    bmf->SetLayerCaption(__index, _caption);
    bmf->SetLayerSelectable(__index, _selectable);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::Init()
{
    bmf->doInitDelay = BCCalcParams.doMapInitDelay;
    bmf->initDelay = BCCalcParams.mapInitDelay;
    bmf->doInitInfo = BCCalcParams.doMapInitInfo;

    bmf->Init();
    bmf->LoadConf("");
    bmf->Visible = true;

    oldMouseDown = bmf->Map->OnMouseDown;
    oldMouseMove = bmf->Map->OnMouseMove;
    oldMouseUp   = bmf->Map->OnMouseUp;

    bmf->Map->OnMouseDown = FrameMouseDown;
    bmf->Map->OnMouseMove = FrameMouseMove;
    bmf->Map->OnMouseUp   = FrameMouseUp;

    bmf->SetLayersCount(layer_last);
    SetLayerParam(layer_default, true,"По умолчанию", false);
    SetLayerParam(layer_station, true,"Станции", true);
    SetLayerParam(layer_link, true,"Линки", false);
    SetLayerParam(layer_zone, true,"Зоны покрытия", false);
    SetLayerParam(layer_coordZone, true,"Координационные зоны", false);
    SetLayerParam(layer_DuelResult, true,"Результаты дуэлей", false);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::FrameMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (oldMouseDown)
        oldMouseDown(Sender, Button, Shift, X, Y);

    if(omsCallBack != NULL)
    {
        TBaseMapFrame::Shapes vObj = bmf->SelectObjects(X, Y, 1);
        if(vObj.size()> 0)
        try
        {
            omsCallBack(Sender, vObj, Button, Shift);
        }
        catch(...)
        {;}
    }
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::FrameMouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y)
{
    if (oldMouseMove)
        oldMouseMove(Sender, Shift, X, Y);


    float screenX = X;
    float screenY = Y;
    double mapX, mapY;
    bmf->Map->ConvertCoord(&screenX, &screenY, &mapX, &mapY, miScreenToMap);
    if (cc)
        sb->Panels->Items[0]->Text = cc->CoordToStr(mapX, 'X') + " : " + cc->CoordToStr(mapY, 'Y');

    ShowZoom();
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::FrameMouseUp(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (oldMouseUp)
        oldMouseUp(Sender, Button, Shift, X, Y);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::Clear(int layer, bool refresh)
{
    //Очищается либо заданный уровень, либо вся карта.
    bmf->ClearData(layer);//ё... все данные (-1), или заданный слой
    if (refresh)
        //#ifdef _DEBUG
        //ShowMessage("Calling Map->Refresh() from "__FILE__":"+IntToStr(__LINE__)+", "__FUNC__+", layer = "+IntToStr(layer)),
        //#endif // _DEBUG
        bmf->Map->Refresh();
}
//---------------------------------------------------------------------------

MapPoint* __fastcall TCustomMapFrame::ShowPoint(double lon, double lat, TColor _color, int _width,
            PointType type, AnsiString label, AnsiString hint)
{
    MapPoint *pnt = dynamic_cast<MapPoint *>(bmf->NewShape(stPoint));
    pnt->x = lon;
    pnt->y = lat;
    pnt->color = _color;
    pnt->width = _width;
    pnt->pointType = type;
    pnt->name = label;
    pnt->desc = hint;
    return pnt;
}
//---------------------------------------------------------------------------

MapPoint* __fastcall TCustomMapFrame::ShowStation(double lon, double lat, AnsiString label, AnsiString hint)
{
//рисуем станцию с координатами и цветом...
    MapPoint *pnt = dynamic_cast<MapPoint *>(bmf->NewShape(stPoint));
    pnt->x = lon;
    pnt->y = lat;
    pnt->color = cSta;
    pnt->width = iWidthSta;
    pnt->pointType = ptPoint;
    pnt->name = label;
    pnt->userTag = (int)cotStation;
    pnt->desc = hint;
    pnt->SetLayer(layer_station);
    return pnt;
}
//---------------------------------------------------------------------------

MapPolygon* __fastcall TCustomMapFrame::ShowContour(Lis_map::Points points, AnsiString label, AnsiString hint)
{
    MapPolygon* pgn = dynamic_cast<MapPolygon*>(bmf->NewShape(stPolygon));
    pgn->points = points;
    pgn->width = 2;
    pgn->color = cAllot;
    pgn->style = psSolid;
    pgn->name = label;
    pgn->desc = hint;
    pgn->SetLayer(layer_station);
    pgn->userTag = cotAllotCont;
    return pgn;
}
//---------------------------------------------------------------------------

MapLink* __fastcall TCustomMapFrame::ShowLink(double lon1, double lat1, double lon2,
    double lat2, int trim, MapArrowType mat)
{
//рисуем соединение между двумя точками
    return ShowLine(lon1, lat1, lon2, lat2, trim, layer_link, cLink, iWidthLink, (int)cotLink, mat);
}
//---------------------------------------------------------------------------

MapLink* __fastcall TCustomMapFrame::ShowLine(double lon1, double lat1, double lon2,
             double lat2, int trim, int layer, TColor _color, int _width, int _tag,
             MapArrowType mat)
{
//рисуем линию
    MapLink *lnk = dynamic_cast<MapLink *>(bmf->NewShape(stLink));
    lnk->x1 = lon1;
    lnk->y1 = lat1;
    lnk->x2 = lon2;
    lnk->y2 = lat2;
    lnk->color = _color;
    lnk->width = _width;
    lnk->trim = trim;
    lnk->style = pstLink;
    lnk->userTag = _tag;
    lnk->arrow = mat;
    lnk->SetLayer(layer);
    return lnk;
}
//---------------------------------------------------------------------------

MapPolygon* __fastcall TCustomMapFrame::ShowZone(double centLon, double centLat, Zone zone,
                                         int width, TColor color, TPenStyle style,
                                         CustomObjectType objType, int layer)
{
    double n = (double)zone.size();//число шагов
    if (n == 0)
        return NULL;
        
    MapPolygon* pgn = dynamic_cast<MapPolygon*>(bmf->NewShape(stPolygon));
    Lis_map::Point pnt;
    Lis_map::Points pnts;
    double step = 360.0/n;//величина шага в градусах
    double step_r = (step*M_PI)/180.;//величина шага в радианах
    double c_x = 0.0, c_y =0.0;
    c_x = centLon;
    c_y = centLat;
    double x = 0.0, y = 0.0;
    double xt, yt;
    for(unsigned int i = 0; i < zone.size(); i++)
    {
        ////////////////////////////////////////////////////////////////////////
        ////                                                                ////
        ////                                                                ////
        /*
            сие добро выведено по уравнению круга в тригонометрической форме
            111.2 - количество километров в одном географическом градусе
            cos((c_y*M_PI)/180)) - правка для долготы, взависимости от широты

        */
        txAnalyzer.GetPoint(c_x, c_y, step * i, zone[i], &xt, &yt);
        x = c_x + ((zone[i] /111.2)/cos((c_y*M_PI)/180)) * sin((double)step_r*i);
        y = c_y + (zone[i] /111.2) * cos((double)step_r*i);
        ////                                                                ////
        ////                                                                ////
        ////////////////////////////////////////////////////////////////////////
        pnt.first  = xt;
        pnt.second = yt;
        pnts.push_back(pnt);
    }
    pgn->points  = pnts;
    pgn->width   = width;
    pgn->color   = color;
    pgn->style   = style;
    pgn->userTag = (int)objType;
    if(layer > 0)
        pgn->SetLayer(layer);
    else
        pgn->SetLayer(layer_default);
    return pgn;
}
//---------------------------------------------------------------------------

MapPolygon* __fastcall TCustomMapFrame::ShowCoordZone(double centLon, double centLat, Zone zone)
{
//показываем координационную зону
    //КЗ обычно помещаются в отдельный слой, чтобы легче было управлять отображением КЗ от разных станций.
    Clear(layer_coordZone);
    return ShowZone(centLon,
                    centLat,
                    zone,
                    iWidthCoordZone,
                    cCoordZone,
                    pstCoordZone,
                    cotCoordZone,
                    layer_coordZone);
}
//---------------------------------------------------------------------------

MapPolygon* __fastcall TCustomMapFrame::ShowCoverageZone(double centLon, double centLat, Zone zone)
{
//какая-то там диаграмма (поле, покрытие...)
    //В отдельном слое.
    return ShowZone(centLon,
                    centLat,
                    zone,
                    iWidthCovZone,
                    cCovZone,
                    pstCovZone,
                    cotCovZone,
                    layer_zone);
}
//---------------------------------------------------------------------------

MapLink* __fastcall TCustomMapFrame::ShowRelLine(double lon1, double lat1, double lon2,
    double lat2, MapArrowType mat)
{
//показываем линию между двумя точками
    MapLink *rline = dynamic_cast<MapLink *>(bmf->NewShape(stLink));
    rline->x1      = lon1;
    rline->y1      = lat1;
    rline->x2      = lon2;
    rline->y2      = lat2;
    rline->color   = cRelLine;
    rline->width   = iWidthRelLine;
    rline->trim    = 5;
    rline->style   = pstRelLine;
    rline->userTag = (int)cotRelLine;
    rline->arrow   = mat;

    return rline;
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::RemoveRelLine()
{
//удалякм линию между двумя точками
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::Highlight(int objId)
{
    //подсвечиваем некий объект
    MapShape* tmp_shp = bmf->GetShapeById(objId);
    if(tmp_shp == NULL)
        throw *(new Exception("Нельзя подсветить объект - объект не найден."));
    if (tmp_shp->userTag > cotNone && tmp_shp->userTag < cotLast)
    {
        tmp_shp->color = *mColorH[tmp_shp->userTag];
        vH.push_back(objId);
    }
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::UnHighliht(int objId)
{
    //выключаем подсветку данного объекта
    MapShape* tmp_shp = bmf->GetShapeById(objId);
    if(tmp_shp != NULL && (tmp_shp->userTag > cotNone && tmp_shp->userTag < cotLast))
        tmp_shp->color = *mColorUH[tmp_shp->userTag];
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::UnHighlihtAll()
{
    //выключаем подсветку всех объектов
    for(unsigned int i = 0; i < vH.size(); i++)
        UnHighliht(vH[i]);
    vH.clear();
}
//---------------------------------------------------------------------------

MapPolygon* __fastcall TCustomMapFrame::ShowAntennaPattern(double centLon, double centLat, Zone zone)
{
    //показываем ДН антенны
    return ShowZone(centLon, centLat, zone, iWidthAntPat, cAntPat, pstAntPat, cotAntPattern, -1);
    //В отдельном слое.
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::tb2Click(TObject *Sender)
{
    bmf->actPanExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::SetCenter(double lon, double lat)
{
    //позиционируем центр карты
    bmf->SetCenter(lon, lat);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::SetScale(double scale)
{
    //устанавливаем масштаб
    bmf->SetScale(scale);
    ShowZoom();
}
//---------------------------------------------------------------------------
void __fastcall TCustomMapFrame::FrameMouseWheelDown(TObject *Sender,
      TShiftState Shift, TPoint &MousePos, bool &Handled)
{
    bmfactZoomInTwiceExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::FrameMouseWheelUp(TObject *Sender,
      TShiftState Shift, TPoint &MousePos, bool &Handled)
{
    bmfactZoomOutTwiceExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::ShowZoom()
{
    double zoom = bmf->Map->Zoom;
    AnsiString formatStr = "0.";
    if (zoom < 100) formatStr += '0';
    if (zoom < 10) formatStr += '0';
    if (zoom < 1) formatStr += '0';
    sb->Panels->Items[1]->Text = "M: " + FormatFloat(formatStr, zoom);
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::bmfactZoomInTwiceExecute(TObject *Sender)
{
    bmf->actZoomInTwiceExecute(Sender);
    ShowZoom();
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::bmfactZoomOutTwiceExecute(TObject *Sender)
{
    bmf->actZoomOutTwiceExecute(Sender);
    ShowZoom();
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::bmfactZoomDefaultExecute(TObject *Sender)
{
    bmf->actZoomDefaultExecute(Sender);
    ShowZoom();
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::bmfactZoomCustExecute(TObject *Sender)
{
    bmf->actZoomCustExecute(Sender);
    ShowZoom();
}
//---------------------------------------------------------------------------

void __fastcall TCustomMapFrame::RemoveObject(int id)
{
    bmf->RemoveObject(id);
}
//---------------------------------------------------------------------------

TBaseMapFrame::Shapes __fastcall TCustomMapFrame::ShowDuelResult(Lis_map::Point a, Lis_map::Point b, Zone aNoise, Zone aInterf,
                                   Zone bNoise, Zone bInterf, Lis_map::Point c, Lis_map::Point d)
{
//Показываем результаты дуэлей
    TBaseMapFrame::Shapes sResult;
    Clear(layer_DuelResult);
    sResult.push_back(ShowLine(a.first, a.second, b.first, b.second, 0, layer_DuelResult, 0x00003366, 1, 0, matNone));
    sResult.push_back(ShowLine(c.first, c.second, a.first, a.second, 0, layer_DuelResult, 0x00003366, 1, 0, matNone));
    sResult.push_back(ShowLine(d.first, d.second, b.first, b.second, 0, layer_DuelResult, 0x00003366, 1, 0, matNone));
    sResult.push_back(ShowZone(a.first, a.second, aNoise, 1, 0x00FF0000, psSolid, cotNone, layer_DuelResult));
    sResult.push_back(ShowZone(b.first, b.second, bNoise, 1, 0x00FF0000, psSolid, cotNone, layer_DuelResult));
    sResult.push_back(ShowZone(a.first, a.second, aInterf, 1, 0x000000FF, psSolid, cotNone, layer_DuelResult));
    sResult.push_back(ShowZone(b.first, b.second, bInterf, 1, 0x000000FF, psSolid, cotNone, layer_DuelResult));
    return sResult;
}
//---------------------------------------------------------------------------


