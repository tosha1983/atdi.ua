//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "BaseMap.h"
#include "MSXML_OCX.h"
#include <memory>
#include <Dialogs.hpp>
#include <math.h>
#include <float.h>
#include "tempvalues.h"
#include "uDlgMapConf.h"

#define min(a, b)  (((a) < (b)) ? (a) : (b))
#define max(a, b)  (((a) > (b)) ? (a) : (b))

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"

namespace Lis_map {

void MapShape::CheckIsValid()
{
    if (owner == NULL)
        throw *(new Exception("MapShape::CheckIsValid(): no owner"));
    owner->CheckIsShapeValid(this);
}

void MapShape::SetLayer(int l)
{
    layer = l;
}

HGDIOBJ MapShape::CreateFont(HDC hMapDC, AnsiString name, int size, TColor color, TFontStyles style)
{
    LOGFONT lf;
    memset(&lf, 0, sizeof(LOGFONT));
    lf.lfCharSet = DEFAULT_CHARSET;
    memmove(lf.lfFaceName, name.c_str(), name.Length());
    lf.lfHeight = -MulDiv(size, GetDeviceCaps(hMapDC, LOGPIXELSY), 72);
    if (style.Contains(fsBold))
        lf.lfWeight = FW_DEMIBOLD;
    HGDIOBJ newFont = ::CreateFontIndirect(&lf);

    ::SetTextColor(hMapDC, color);
    ::SetBkMode(hMapDC, TRANSPARENT);

    return newFont;
}

inline void DrawLine(HDC &hMapDC, int x1, int y1, int x2, int y2)
{
    ::MoveToEx(hMapDC, x1, y1, NULL);
    ::LineTo(hMapDC, x2, y2);
}

void MapPoint::Paint(HDC hMapDC)
{
//DSN
//старый код:
////////////////////////////////////////////////////////////////////////////////
/*
    int x, y;
    owner->CoordMapToScreen(this->x, this->y, &x, &y);
    HGDIOBJ oldPen = ::SelectObject(hMapDC, ::CreatePen(0, this->width, this->color));
    ::MoveToEx(hMapDC, x, y, NULL);
    ::LineTo(hMapDC, x, y);
    ::DeleteObject(::SelectObject(hMapDC, oldPen));

    if (this->name.Length() > 0)
    {
        HGDIOBJ captionFont = CreateFont(hMapDC, "Arial", 10, clBlack, TFontStyles());
        HGDIOBJ oldFont = ::SelectObject(hMapDC, captionFont);
        ::TextOut(hMapDC, x + 3, y + 3, this->name.c_str(), this->name.Length());
        ::SelectObject(hMapDC, oldFont);
        ::DeleteObject(captionFont);
    }
*/
////////////////////////////////////////////////////////////////////////////////
    int _x, _y;
    owner->CoordMapToScreen(this->x, this->y, &_x, &_y);
    HGDIOBJ oldPen = NULL;
    int delta = this->width/2;
    if(pointType == ptPoint)//точка
    {
        oldPen = ::SelectObject(hMapDC, ::CreatePen(0, this->width, this->color));
        DrawLine(hMapDC, _x, _y, _x, _y);
    }
    else if(pointType == ptXCross)//X
    {
        oldPen = ::SelectObject(hMapDC, ::CreatePen(0, 1, this->color));
        DrawLine(hMapDC, _x-delta, _y-delta, _x+delta+1, _y+delta+1);
        DrawLine(hMapDC, _x+delta, _y-delta, _x-delta-1, _y+delta+1);
    }
    else if(pointType == ptPlusCross)//+
    {
        oldPen = ::SelectObject(hMapDC, ::CreatePen(0, 1, this->color));
        DrawLine(hMapDC, _x, _y-delta, _x, _y+delta+1);
        DrawLine(hMapDC, _x-delta, _y, _x+delta+1, _y);
    }
    else if(pointType == ptSymbol)//символ
    {

        SIZE size;
        HGDIOBJ pointFont = NULL;
        TFontStyles fsTemp;
        if (isBold)
            fsTemp << fsBold;
        pointFont = CreateFont(hMapDC, fontName.c_str(), fontSize, this->color, fsTemp);
        HGDIOBJ oldFont = ::SelectObject(hMapDC, pointFont);
        ::GetTextExtentPoint32(hMapDC, AnsiString(char(symbol)).c_str(), 1, &size);
        ::TextOut(hMapDC, _x - size.cx / 2, _y - size.cy / 2, AnsiString(char(symbol)).c_str(), 1);
        ::SelectObject(hMapDC, oldFont);
        ::DeleteObject(pointFont);
    }
    else
        ;//ну так лажа где-то если влетели сюда...
    if(pointType != ptSymbol)
        ::DeleteObject(::SelectObject(hMapDC, oldPen));
////DSN
////Перенесено в OnDrawUserLayer()
/*    if (this->name.Length() > 0)
    {
        HGDIOBJ captionFont = CreateFont(hMapDC, "Arial", 8, clBlack, TFontStyles());
        HGDIOBJ oldFont = ::SelectObject(hMapDC, captionFont);
        ::TextOut(hMapDC, _x + 3, _y + 3, this->name.c_str(), this->name.Length());
        ::SelectObject(hMapDC, oldFont);
        ::DeleteObject(captionFont);
    }
*/
}

bool __fastcall MapPoint::IsIntersect(int x, int y, int trim)
{
    int tx, ty;
    owner->CoordMapToScreen(this->x, this->y, &tx, &ty);

    return abs(x - tx) <= trim && abs(y - ty) <= trim;
}

bool __fastcall MapPoint::GetExtent(Point& lb, Point& rt)
{
    lb.first = x;
    lb.second = y;
    rt.first = x;
    rt.second = y;
    return true;
}

inline void DrawArrow(HDC hMapDC, int x1, int y1, int x2, int y2, double h, double w)
{
    double dx = (double)(x2 - x1);
    double dy = (double)(y2 - y1);
    double l = sqrt(dx*dx+dy*dy);
    if(l <= (h*2))
        h = l / 3;
    dx = dx/l;
    dy = dy/l;
    double nx = dy;
    double ny = -dx;
    double x3 = x2-h*dx;
    double y3 = y2-h*dy;
    double x4 = x3 + w*nx;
    double y4 = y3 + w*ny;
    double x5 = x3 - w*nx;
    double y5 = y3 - w*ny;
    ::MoveToEx(hMapDC, x4, y4, NULL);
    ::LineTo(hMapDC, x2, y2);
    ::LineTo(hMapDC, x5, y5);
}

void MapLink::Paint(HDC hMapDC)
{
    int x1, x2, y1, y2;
    owner->CoordMapToScreen(this->x1, this->y1, &x1, &y1);
    owner->CoordMapToScreen(this->x2, this->y2, &x2, &y2);

    // make lines don't touch site points
    if (trim > 0)
    {
        int x = x1 - x2; int y = y1 - y2;
        double d = sqrt(x*x + y*y);
        x1 -= (this->trim * x / d);
        x2 += (this->trim * x / d);
        y1 -= (this->trim * y / d);
        y2 += (this->trim * y / d);
    }

    HGDIOBJ oldPen = ::SelectObject(hMapDC, ::CreatePen(this->style, this->width, this->color));
    ::MoveToEx(hMapDC, x1, y1, NULL);
    ::LineTo(hMapDC, x2, y2);

    if(arrow == matPoint)
    {
        LOGBRUSH newBrush;
        newBrush.lbStyle = BS_SOLID;
        newBrush.lbColor = this->color;
        HGDIOBJ oldBrush = ::SelectObject(hMapDC, ::CreateBrushIndirect(&newBrush));
        ::Ellipse(hMapDC, x1-this->width*2, y1-this->width*2, x1+this->width*2, y1+this->width*2);
        ::Ellipse(hMapDC, x2-this->width*2, y2-this->width*2, x2+this->width*2, y2+this->width*2);
        ::DeleteObject(::SelectObject(hMapDC, oldBrush));
    }
    else if(arrow == matArrow)
    {
        DrawArrow(hMapDC, x1, y1, x2, y2, 10, 4);
        DrawArrow(hMapDC, x2, y2, x1, y1, 10, 4);
    }
    ::DeleteObject(::SelectObject(hMapDC, oldPen));
    //MapShape::Paint(hMapDC);
}

bool __fastcall MapLink::IsIntersect(int x, int y, int trim)
{
    return false;
}

bool __fastcall MapLink::GetExtent(Point& lb, Point& rt)
{
    lb.first = min(x1, x2);
    lb.second = min(y1, y2);
    rt.first = max(x1, x2);
    rt.second = max(y1, y2);
    return true;
}

void MapPolygon::Paint(HDC hMapDC)
{
    if (points.size() > 0)
    {
        // first point
        Points::iterator pi = points.begin();
        pointsInt.clear();

        int xcur, ycur, xstart, ystart;
        owner->CoordMapToScreen(pi->first, pi->second, &xcur, &ycur);
        xstart = xcur;
        ystart = ycur;
        pointsInt.push_back(TPoint(xcur, ycur));
        xmin = xmax = xcur;
        ymin = ymax = ycur;

        HGDIOBJ oldPen = ::SelectObject(hMapDC, ::CreatePen(this->style, this->width, this->color));
        ::MoveToEx(hMapDC, xcur, ycur, NULL);

        while (pi < points.end())
        {
            owner->CoordMapToScreen(pi->first, pi->second, &xcur, &ycur);
            ::LineTo(hMapDC, xcur, ycur);
            pi++;
            pointsInt.push_back(TPoint(xcur, ycur));

            xmin = min(xcur, xmin);
            xmax = max(xcur, xmax);
            ymin = min(ycur, ymin);
            ymax = max(ycur, ymax);

        }

        ::LineTo(hMapDC, xstart, ystart);
        ::DeleteObject(::SelectObject(hMapDC, oldPen));
    }

////DSN
////Перенесено в OnDrawUserLayer()
/*
    if (name.Length() > 0)
    {
        double clon, clat;
        if (GetCenterPoint(clon, clat))
        {
            int cx, cy;
            owner->CoordMapToScreen(clon, clat, &cx, &cy);
            SIZE size;

            HGDIOBJ captionFont = CreateFont(hMapDC, "Arial", 8, clBlack, TFontStyles());
            HGDIOBJ oldFont = ::SelectObject(hMapDC, captionFont);

            ::GetTextExtentPoint32(hMapDC, name.c_str(), name.Length(), &size);

            ::TextOut(hMapDC, cx - size.cx / 2, cy - size.cy / 2, this->name.c_str(), this->name.Length());
            ::SelectObject(hMapDC, oldFont);
            ::DeleteObject(captionFont);
        }
    }
*/
}

bool MapPolygon::GetCenterPoint(double& cx, double& cy)
{
    int size = points.size();
    if (size == 0)
        return false;

    // todo: хуйня это, нужно центр тяжести по треугольникам высчитывать.
    else
    {
        double minx = points[0].first; double miny = points[0].second;
        double maxx = points[0].first; double maxy = points[0].second;
        for (int i = 1; i < size; i++)
        {
            maxx = max(points[i].first, maxx);
            maxy = max(points[i].second, maxy);
            minx = min(points[i].first, minx);
            miny = min(points[i].second, miny);
        }
        cx = (maxx + minx) / 2;
        cy = (maxy + miny) / 2;

        return true;
    }
}

bool __fastcall MapPolygon::IsIntersect(int x, int y, int trim)
{
    std::vector<TPoint>& p = pointsInt;
    if (p.size() == 0)
        return false;

    // сначала грубо - по Extent'у
    if (x < xmin || x > xmax || y < ymin || y > ymax)
        return false;

    int N = p.size();
    bool inside = false;
    // разбиваем полигон на треугольники по очереди из каждой точки
    for (int n = 0; n < N; n++)
    {
        // задача: выяснить, попадает ли при данной разбивке точка хоть в один из треугольников
        inside = false;
        // начальная точка обхода
        int i1 = n < N-1 ? n + 1 : 0;
        // пошли по всем треугольникам
        while (!inside)
        {
            // третья точка треугольника
            int i2 = i1 + 1;
            if (i2 >= N)
                i2 = 0;
            // если третья упёрлась в начало обхода, то хватит
            if (i2 == (n < N-1 ? n + 1 : 0))
                break;
            int S = abs(p[i1].x * (p[i2].y - p[n ].y) +
                        p[i2].x * (p[n ].y - p[i1].y) +
                        p[n ].x * (p[i1].y - p[i2].y));
            int S1 = abs(p[i1].x * (p[i2].y - y) +
                        p[i2].x * (y       - p[i1].y) +
                        x       * (p[i1].y - p[i2].y));
            int S2 = abs(p[n ].x * (p[i2].y - y) +
                        p[i2].x * (y       - p[n ].y) +
                        x       * (p[n ].y - p[i2].y));
            int S3 = abs(p[i1].x * (p[n ].y - y) +
                        p[n ].x * (y       - p[i1].y) +
                        x       * (p[i1].y - p[n ].y));

            if (S == S1 + S2 + S3)
            {
                // при этом разбиении треугольник с точкой внутри нашёлся
                inside = true;
                break;
            }
            i1 = i1 + 1;
            if (i1 >= N)
                i1 = 0;
        }
        if (!inside)
            // нашлось разбиение полигона на треугольники,
            // при котором точка не попала ни в один - всё, выходим
            break;
    }
    return inside;
}

bool __fastcall MapPolygon::GetExtent(Point& lb, Point& rt)
{
    if (points.size() == 0)
        return false;
    else
    {
        lb.first = points[0].first;
        lb.second = points[0].second;
        rt.first = points[0].first;
        rt.second = points[0].second;
        Points::iterator i = points.begin();
        for (i++; i < points.end(); i++)
        {
            lb.first =  min(lb.first, i->first);
            lb.second = min(lb.second, i->second);
            rt.first =  max(rt.first, i->first);
            rt.second = max(rt.second, i->second);
        }
        return true;
    }
}

void MapLabel::Paint(HDC hMapDC)
{
    //MapShape::Paint(hMapDC);
}

//---------------------------------------------------------------------------
__fastcall TBaseMapFrame::TBaseMapFrame(TComponent* Owner)
    : TFrame(Owner)
{
    m_map = NULL;
    notifObj = NULL;
    mapMenu = NULL;
    staticLayers = 0;
    lastId = 0;
}
//---------------------------------------------------------------------------

__fastcall TBaseMapFrame::~TBaseMapFrame()
{
    ClearData(-1);
}

//---------------------------------------------------------------------------
TMxMap* __fastcall TBaseMapFrame::GetMap()
{
    if (m_map == NULL)
        throw *(new Exception("Карта не инициализирована"));
    else
        return m_map;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::Init()
{
    if (m_map != NULL)
        throw *(new Exception("Карта уже инициализирована"));

    TempCursor tempCur(crHourGlass);

    try {
        m_map = new TMxMap(this);
    } catch (Exception &e) {
        throw *(new Exception("Ошибка создания компонента цифровой карты\n(возможно, не установлен MapInfo MapX):\n\n" + e.Message));
    }
    m_map->Parent = this;
    m_map->SendToBack();
    m_map->Align = alClient;
    m_map->ShowHint = true;
    m_map->CreateCustomTool(miDistanceTool, miToolTypeLine, TVariant(miCrossCursor), TVariant(NULL), TVariant(NULL), TVariant(0));
    m_map->CreateCustomTool(miReliefTool, miToolTypeLine, TVariant(miCrossCursor), TVariant(NULL), TVariant(NULL), TVariant(0));
    m_map->MapUnit = miUnitKilometer;
    m_map->CenterX = 31.3;
    m_map->CenterY = 48.3;
    m_map->Zoom = 1500;
    m_map->OnMouseMove = MapMouseMove;
    m_map->OnMouseDown = MapMouseDown;
    m_map->OnMouseUp = MapMouseUp;
    m_map->OnToolUsed = MapToolUsed;
    m_map->OnDrawUserLayer = MapDrawUserLayer;

    if (mapMenu)
    {
        mapMenu->Clear();
        for (int i = 0; i < al->ActionCount; i++)
        {
            if (al->Actions[i]->Category == "Layers")
            {
                TMenuItem *mni = new TMenuItem(mapMenu->Owner);
                mni->Action = al->Actions[i];
                mapMenu->Add(mni);
            }
        }
    }
    SetLayersCount(1);
}
//---------------------------------------------------------------------------

bool __fastcall TBaseMapFrame::getIsOverlapping(std::vector<tagRECT> &vRect, tagRECT _r)
{
//проверяем перекрытие прямоугольников
//если перекрываются более чем на 1/3 - возвращаем false.
//иначе - добавляем _r в vRect и возвращаем true
    tagRECT t_r;
    for(unsigned int i = 0; i < vRect.size(); i++)
    {
        t_r = vRect[i];
        if(//проверяем величину перектрытия
           (
            ((t_r.bottom - t_r.top)/3 <= (_r.bottom - t_r.top) && t_r.bottom > _r.top)//наплыв сверху вниз
            ||
            ((t_r.bottom - t_r.top)/3 <= (t_r.bottom - _r.top) && _r.bottom > t_r.top)//наплыв снизу вверх
           )
           &&
           (
            ((t_r.right - t_r.left)/3 <= (t_r.right - _r.left) && _r.right > t_r.left)//наплыв справа налево
            ||
            ((t_r.right - t_r.left)/3 <= (_r.right - t_r.left) && t_r.right > _r.left)//наплыв слева направо
           )
          )
        return false;
    }
    vRect.push_back(_r);
    return true;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::MapDrawUserLayer(System::TObject * Sender, LPDISPATCH Layer, OLE_HANDLE hOutputDC,
        OLE_HANDLE hAttributeDC, LPDISPATCH RectFull, LPDISPATCH RectInvalid)
{
    // per level
    HDC hMapDC = (HDC)hOutputDC;
    SetMapMode(hMapDC, MM_TEXT);

    int n = GetLayersCount();

    for (int i = 0; i < n; i++)  // iterate through levels
    {
        // if level visible
        if (GetLayerVisible(i))
        {
            for (Shapes::iterator sh = shapes.begin(); sh < shapes.end(); sh++)
                if (*sh != NULL && (*sh)->layer == i)
                    (*sh)->Paint(hMapDC);
        }
    }

////////////////////////////////////////////////////////////////////////////////
//______________________________________DSN___________________________________//
//рисуем надписи (имена объектов)

    tagRECT tmp_rect;
    std::vector<tagRECT> vRect;
    AnsiString tmp_name = "";
    double clon = 0.0, clat = 0.0;
    int x = 0, y = 0;
    SIZE size;
    if (shapes.size() > 0)
    for (int i = n-1; i >= 0; i--)
    {
        if (GetLayerVisible(i))
        {
            HGDIOBJ captionFont = MapShape::CreateFont(hMapDC, "Arial", 8, clBlack, TFontStyles());
            HGDIOBJ oldFont = ::SelectObject(hMapDC, captionFont);
            try
            {
                Shapes::reverse_iterator sh = shapes.rend();
                for (sh = shapes.rbegin(); sh != shapes.rend(); sh++)
                {
                    if (*sh != NULL && (*sh)->layer == i)
                    {
                        MapShape *t_sh = (*sh);
                        tmp_name = t_sh->name;
                        try
                        {
                            MapLabel *tmp_lbl = dynamic_cast<MapLabel*>(t_sh);
                            if(tmp_lbl != NULL)
                                tmp_name = tmp_lbl->label;
                        }
                        catch(...)
                        {;}
                        if(tmp_name.Length() > 0)
                        {
                            ::GetTextExtentPoint32(hMapDC, tmp_name.c_str(), tmp_name.Length(), &size);
                            if(t_sh->GetCenterPoint(clon, clat))
                            {
                                CoordMapToScreen(clon, clat, &x, &y);
                                // todo: set coordinates according to position of label, not to shape type
                                if (dynamic_cast<MapPolygon*>(t_sh))
                                {
                                    tmp_rect.left = x - size.cx / 2;
                                    tmp_rect.top = y - size.cy / 2;
                                }
                                else
                                {
                                    tmp_rect.left = x + 3;
                                    tmp_rect.top = y + 3;
                                }
                                tmp_rect.right = tmp_rect.left + size.cx;
                                tmp_rect.bottom = tmp_rect.top + size.cy;

                                if(getIsOverlapping(vRect, tmp_rect))
                                {//рисуем надпись
                                    ::TextOut(hMapDC,
                                              tmp_rect.left,
                                              tmp_rect.top,
                                              tmp_name.c_str(),
                                              tmp_name.Length());
                                }
                            }
                        }
                    }
                }
            }
            __finally
            {
                ::SelectObject(hMapDC, oldFont);
                ::DeleteObject(captionFont);
            }
        }
    }
//--------------------------------------DSN-----------------------------------//
////////////////////////////////////////////////////////////////////////////////


    // convert coordinates;
    /*
    for (Stations::iterator si = stations.begin(); si != stations.end(); si++)
    {
        float scrX = 0;
        float scrY = 0;
        StatData* sd = si->second;
        Map->ConvertCoord(&scrX, &scrY, &(sd->longt), &(sd->latit), miMapToScreen);
        si->second->x = scrX;
        si->second->y = scrY;
    }


    // shift sites if any pair are the same
    for(Stations::iterator si = stations.begin(); si != stations.end(); si ++)
        for(Stations::iterator sj = si; sj != stations.end(); sj ++)
            if(( si->second->x == sj->second->x ) && ( si->second->y == sj->second->y ))
            {
                si->second->x -=2;
                si->second->y -=1;
                sj->second->x +=2;
                sj->second->y +=1;
            }


    // go
    HDC hMapDC = (HDC)hOutputDC;
    SetMapMode(hMapDC, MM_TEXT);

    // draw links
    if (actViewLinks->Checked)
    for (Links::iterator li = links.begin(); li != links.end(); li++)
    {
        LinkData* ld = *li;
        Stations::iterator si1 = stations.find(ld->stat1);
        Stations::iterator si2 = stations.find(ld->stst2);
        if (si1 != stations.end() && si2 != stations.end())
        {
            // make lines don't touch site points
            int x = si2->second->x - si1->second->x; int y = si2->second->y - si1->second->y;
            double d = sqrt(x*x + y*y);
            int x1 = si1->second->x + (5 * x / d);
            int x2 = si2->second->x - (5 * x / d);
            int y1 = si1->second->y + (5 * y / d);
            int y2 = si2->second->y - (5 * y / d);

            HGDIOBJ oldPen = ::SelectObject(hMapDC, ::CreatePen(ld->style, ld->width, ld->color));
            ::MoveToEx(hMapDC, x1, y1, NULL);
            ::LineTo(hMapDC, x2, y2);
            ::DeleteObject(::SelectObject(hMapDC, oldPen));
        }
    }

    // stations

    LOGFONT lf;
    memset(&lf, 0, sizeof(LOGFONT));
    lf.lfHeight = -MulDiv(8, GetDeviceCaps(hMapDC, LOGPIXELSY), 72);
    lf.lfCharSet = DEFAULT_CHARSET;
    memmove(lf.lfFaceName, "Arial", 5);

    HGDIOBJ detailFont = ::CreateFontIndirect(&lf);

    lf.lfHeight = -MulDiv(10, GetDeviceCaps(hMapDC, LOGPIXELSY), 72);
    lf.lfWeight = FW_DEMIBOLD;

    HGDIOBJ captionFont = ::CreateFontIndirect(&lf);

    ::SetTextColor(hMapDC, clBlack);
    ::SetBkMode(hMapDC, TRANSPARENT);

    HGDIOBJ oldFont = ::SelectObject(hMapDC, detailFont);
    TEXTMETRIC tm;
    GetTextMetrics(hMapDC, &tm);
    int detailCharHeight = tm.tmHeight;

    for(Stations::iterator si = stations.begin(); si != stations.end(); si ++)
    {
        StatData *sdi = si->second;
        HGDIOBJ oldPen = ::SelectObject(hMapDC, ::CreatePen(0, sdi->width, sdi->color));
        ::MoveToEx(hMapDC, sdi->x, sdi->y, NULL);
        ::LineTo(hMapDC, sdi->x, sdi->y);
        ::DeleteObject(::SelectObject(hMapDC, oldPen));


        if (actViewStations->Checked)
        {
            //Позиціонування конролів. pt - базова точка
            double dist = DBL_MAX;
            StatData *sdnearest = NULL;
            for(Stations::iterator sj = stations.begin(); sj != stations.end(); sj ++)
            {
                StatData *sdj = sj->second;
                //знаходимо найближчу станцію
                double dx = sqrt((sdj->x - sdi->x) * (sdj->x - sdi->x) + (sdj->y - sdi->y) * (sdj->y - sdi->y));
                if(dist > dx && sdj != sdi)
                {
                    dist = dx;
                    sdnearest = sdj;
                }
            }

            POINT pt;

            if(sdnearest && sdi->x > sdnearest->x)
                pt.x = sdi->x + 3;
            else
                if(sdnearest&& sdi->x == sdnearest->x && sdi->id > sdnearest->id)
                    pt.x = sdi->x + 3;
                else
                    pt.x = sdi->x - 121;

            if(sdnearest && sdi->y > sdnearest->y)
                pt.y = sdi->y + 3;
            else
                pt.y = sdi->y - 17;

            //if(ClientWidth - pt.x < 121)
            //   pt.x = pt.x - 121 + (ClientWidth - pt.x);

            ::SelectObject(hMapDC, captionFont);
            ::TextOut(hMapDC, pt.x, pt.y, sdi->name.c_str(), sdi->name.Length());

            if (actViewStationDetails->Checked)
            {
                ::SelectObject(hMapDC, detailFont);
                AnsiString bw = FormatFloat("0.###", sdi->channel);
                ::TextOut(hMapDC, pt.x + 20, pt.y + tm.tmInternalLeading + detailCharHeight, bw.c_str(), bw.Length());
                int yoffset = pt.y + tm.tmInternalLeading + detailCharHeight;
                for (unsigned i = 0; i < sdi->freqs.size(); i++)
                {
                    AnsiString freq = FormatFloat("0.000", sdi->freqs[i]);
                    ::TextOut(hMapDC, pt.x + 40, yoffset + detailCharHeight * i, freq.c_str(), freq.Length());
                }


                //vect[i]->edChannel->Left = vect[i]->edName->Left + 20;
                //vect[i]->lstFreqs->Left = vect[i]->edChannel->Left + vect[i]->edChannel->Width - 1;
                //if(ClientHeight - pt.y < vect[i]->lstFreqs->Height + 20)
                //{
                //    vect[i]->edChannel->Top = vect[i]->edName->Top - 13;
                //    vect[i]->lstFreqs->Top = vect[i]->edName->Top - vect[i]->lstFreqs->Height + 1;
                //}
                //else
                //{
                //    vect[i]->edChannel->Top = vect[i]->edName->Top + 14;
                //    vect[i]->lstFreqs->Top = vect[i]->edChannel->Top;
                //}
            }
        }
    }

    ::SelectObject(hMapDC, oldFont);
    ::DeleteObject(detailFont);
    ::DeleteObject(captionFont);
    */
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::MapToolUsed(System::TObject * Sender, short ToolNum, double X1, double Y1, double X2, double Y2, double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl, VARIANT_BOOL* EnableDefault)
{
    if ( ToolNum == miDistanceTool )
    {
    }
    else if ( ToolNum == miReliefTool )
    {
        //TfrmRelief *frmRelief = new TfrmRelief(Application);
        //frmRelief->Caption = "Профіль траси " + dmMain->coordToStr(Y1, 'Y') + ':' + dmMain->coordToStr(X1, 'X')
        //                                      + " - "
        //                                      + dmMain->coordToStr(Y2, 'Y') + ':' + dmMain->coordToStr(X2, 'X');
        //frmRelief->fmProfileView1->RetreiveProfile(X1, Y1, X2, Y2);
        //frmRelief->Show();
    }
    else if ( ToolNum == miZoomOutTool )
    {
        if ( m_map->Zoom > 100000 )
            m_map->Zoom = 100000;
    }
    else if ( ToolNum == miZoomInTool )
    {
        if ( m_map->Zoom < 0.0001 )
            m_map->Zoom = 0.0001;
    }
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::MapMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    //
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::MapMouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y)
{
    bool hintShown = false;
    for (int i = GetLayersCount() - 1; i >= 0  && !hintShown; i--)  // iterate through levels
    {
        // if level visible
        if (GetLayerVisible(i))
        {
            for (Shapes::iterator sh = shapes.begin(); sh < shapes.end() && !hintShown; sh++)
                if (*sh != NULL && (*sh)->layer == i && (*sh)->desc.Length() > 0 && (*sh)->IsIntersect(X, Y, 1))
                {
                    m_map->Hint = (*sh)->desc;
                    //Application->CancelHint();
                    Application->ActivateHint(m_map->ClientToScreen(TPoint(X, Y)));
                    hintShown = true;
                }
        }
    }
    if (!hintShown)
        Application->CancelHint();
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::MapMouseUp(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    //
}
//---------------------------------------------------------------------------


void __fastcall TBaseMapFrame::LoadConf(AnsiString filename)
{
    if (filename.IsEmpty())
        filename = ChangeFileExt(Application->ExeName, ".mapconf");

    mapIniName = filename;

    if (mapMenu)
    try {
        int i = 0;
        while (i < mapMenu->Count)
            if (mapMenu->Items[i]->Name.Pos("mniMapStatic") == 1 || mapMenu->Items[i]->IsLine())
                mapMenu->Delete(i);
            else
                i++;

    } catch(...) {}

    try {
        
        m_map->Layers->RemoveAll();

        CMapXLayerInfoPtr li;
        li.CreateInstance(CLSID_LayerInfo);
        li->Type = miLayerInfoTypeUserDraw;
        li->AddParameter(L"Name", TVariant(L"Objects"));

        //CMapXLayerPtr lyr = m_map->Layers->AddUserDrawLayer(L"Objects", 1);
        m_map->Layers->Add(TVariant((CMapXLayerInfo*)li), TVariant(1));
        CMapXLayerPtr lyr = m_map->Layers->get_Item(TVariant(1));
        if (!lyr.IsBound())
            MessageBox(NULL, "User Draw Layer is not created", "Error", MB_ICONEXCLAMATION);

        if (FileExists(filename))
        {

            TDOMDocument *conf = new TDOMDocument(this);
            if (!(bool)conf->load(TVariant(WideString(filename))))
                throw *(new Exception("Файл не читается"));
            IXMLDOMElementPtr root = conf->documentElement;
            if (!root.IsBound())
                throw *(new Exception("Корневой узел отсутствует"));

            IXMLDOMNodePtr section = root->firstChild;
            while (section.IsBound() && (
                   section->nodeType != Msxml_tlb::NODE_ELEMENT ||
                   wcscmp(section->nodeName, L"layers") != 0))
                section = section->nextSibling;

            if (section.IsBound())
            {
                IXMLDOMNodePtr node = section->firstChild;
                while (node)
                {
                    if (wcscmp(node->nodeName, L"layer") == 0 && node->nodeType == Msxml_tlb::NODE_ELEMENT)
                    {
                        IXMLDOMElementPtr element = node;
                        TVariant vv = TVariant(element->getAttribute(L"file"));
                        if (vv.vt != VT_EMPTY)
                        {
                            AnsiString fn = ExpandFileName((WideString)vv);
                            if (FileExists(fn))
                            {
                                int position = m_map->Layers->Count + 1;
                                CMapXLayer *layer = m_map->Layers->Add(TVariant(WideString(fn)), TVariant(position));

                                AnsiString name = "default" + IntToStr(position);
                                TVariant v = TVariant(element->getAttribute(L"name"));
                                if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                                    name = WideString(v.bstrVal);

                                bool visible = true;
                                v = TVariant(element->getAttribute(L"visible"));
                                if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                                {
                                    visible = (WideString(v.bstrVal) != WideString(L"0"));
                                    if (layer != NULL)
                                        layer->Visible = visible;
                                }

                                v = TVariant(element->getAttribute(L"autolabel"));
                                if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                                {
                                    bool autolabel = (WideString(v.bstrVal) != WideString(L"0"));
                                    if (layer != NULL)
                                        layer->AutoLabel = autolabel;
                                }

                                v = TVariant(element->getAttribute(L"zoomMin"));
                                if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                                {
                                    double zoomMin = StrToFloat(WideString(AnsiString(v.bstrVal)));
                                    if (layer != NULL)
                                    {
                                        layer->ZoomLayer = 1;
                                        layer->ZoomMin = zoomMin;
                                    }
                                }

                                v = TVariant(element->getAttribute(L"zoomMax"));
                                if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                                {
                                    double zoomMax = StrToFloat(WideString(AnsiString(v.bstrVal)));
                                    if (layer != NULL)
                                    {
                                        layer->ZoomLayer = 1;
                                        layer->ZoomMax = zoomMax;
                                    }
                                }

                                if (mapMenu)
                                {
                                    TMenuItem *mni = new TMenuItem(mapMenu->Owner);
                                    mni->Name = "mniMapStatic"+IntToStr(position);
                                    mni->Caption = name;
                                    mni->Checked = visible;
                                    mni->OnClick = MapMenuClick;
                                    mni->Tag = position;
                                    if (m_map->Layers->Count == 2) // first layer
                                        mapMenu->NewTopLine();

                                    mapMenu->Insert(position - 2, mni);
                                }

                                if (notifObj)
                                {
                                    notifObj->layerAdded(layer, name);
                                }
                            }
                        }
                    }
                    node = node->nextSibling;
                }
                section = section->nextSibling;
            }

            section = root->firstChild;
            while (section.IsBound() && (
                   section->nodeType != Msxml_tlb::NODE_ELEMENT ||
                   wcscmp(section->nodeName, L"position") != 0))
                section = section->nextSibling;

            if (section.IsBound())
            {
                IXMLDOMElementPtr element = section;
                char oldDecSep = ::DecimalSeparator;
                ::DecimalSeparator = '.';
                try {
                    TVariant v = TVariant(element->getAttribute(L"centerx"));
                    if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                        m_map->CenterX = StrToFloat(AnsiString(WideString(v)));
                    v = TVariant(element->getAttribute(L"centery"));
                    if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                        m_map->CenterY = StrToFloat(AnsiString(WideString(v)));
                    v = TVariant(element->getAttribute(L"zoom"));
                    if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                    {
                        double zoom = StrToFloat(AnsiString(WideString(v)));
                        if (zoom > 0)
                            m_map->Zoom = zoom;
                        else
                            m_map->Zoom = 1000;
                    }
                } __finally {
                    ::DecimalSeparator = oldDecSep;
                }
                section = section->nextSibling;
            }

        } // if (FileExist(filename))

        staticLayers = m_map->Layers->Count;

        Map->Refresh();

    } catch (Exception &e) {
        throw *(new Exception("Ошибка при создании слоёв карты:\n\n"+e.Message));
    }
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::MapMenuClick(TObject *Sender)
{
    TMenuItem *mni = dynamic_cast<TMenuItem*>(Sender);
    if (mni->Name.Pos("mniMapStatic") == 1)
    {
        mni->Checked = !mni->Checked;
        Map->Layers->get_Item(TVariant(mni->Tag))->Visible = mni->Checked;
        Map->Refresh();
    }
}
//---------------------------------------------------------------------------
void __fastcall TBaseMapFrame::btnUClick(TObject *Sender)
{
    Map->CenterY = Map->CenterY + Map->Bounds->Height / 3;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnLClick(TObject *Sender)
{
    Map->CenterX = Map->CenterX - Map->Bounds->Width / 3;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnRClick(TObject *Sender)
{
    Map->CenterX = Map->CenterX + Map->Bounds->Width / 3;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnDClick(TObject *Sender)
{
    Map->CenterY = Map->CenterY - Map->Bounds->Height / 3;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnULClick(TObject *Sender)
{
    btnUClick(Sender);
    btnLClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnURClick(TObject *Sender)
{
    btnUClick(Sender);
    btnRClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnDLClick(TObject *Sender)
{
    btnDClick(Sender);
    btnLClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::btnDRClick(TObject *Sender)
{
    btnDClick(Sender);
    btnRClick(Sender);
}
//---------------------------------------------------------------------------
void __fastcall TBaseMapFrame::actNoneExecute(TObject *Sender)
{
    Map->CurrentTool = miArrowTool;
    Map->Cursor = crCross;
    actNone->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actPanExecute(TObject *Sender)
{
    Map->CurrentTool = miPanTool;
    actPan->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actDistanceExecute(TObject *Sender)
{
    Map->CurrentTool = miDistanceTool;
    Map->Cursor = crDefault;
    actDistance->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actZoomInTwiceExecute(TObject *Sender)
{
    Map->Zoom = Map->Zoom / 2;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actZoomOutTwiceExecute(TObject *Sender)
{
    Map->Zoom = Map->Zoom * 2;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actZoomDefaultExecute(TObject *Sender)
{
    Map->Zoom = 1000;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actZoomCustExecute(TObject *Sender)
{
    AnsiString val = FloatToStr(Map->Zoom);
    if (InputQuery("Произвольный масштаб", "Новое значение:", val))
        Map->Zoom = StrToFloat(val);
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actPanButtonsExecute(TObject *Sender)
{
    btnUL->Enabled = actPanButtons->Checked;
    btnU->Enabled = actPanButtons->Checked;
    btnUR->Enabled = actPanButtons->Checked;
    btnR->Enabled = actPanButtons->Checked;
    btnDR->Enabled = actPanButtons->Checked;
    btnD->Enabled = actPanButtons->Checked;
    btnDL->Enabled = actPanButtons->Checked;
    btnL->Enabled = actPanButtons->Checked;
    btnUL->Visible = actPanButtons->Checked;
    btnU->Visible = actPanButtons->Checked;
    btnUR->Visible = actPanButtons->Checked;
    btnR->Visible = actPanButtons->Checked;
    btnDR->Visible = actPanButtons->Checked;
    btnD->Visible = actPanButtons->Checked;
    btnDL->Visible = actPanButtons->Checked;
    btnL->Visible = actPanButtons->Checked;

    actPanButtons->Checked = !actPanButtons->Checked;
}
//---------------------------------------------------------------------------


void __fastcall TBaseMapFrame::ClearData(int lr)
{
    for (Shapes::iterator si = shapes.begin(); si < shapes.end(); si++)
    {
        if (*si != NULL && (lr == -1 || (*si)->layer == lr))
        {
            delete *si;
            //si is nullified in destructor
        }
        //erasing vector element takes time to rearrange vector;
        //so taking into account that pointer is nullified in destructor, leave it alone
        //shapes.erase(si);
    }
    if (lr == -1)
    shapes.clear();
}

//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::SavePosition(AnsiString filename)
{
    try {

        TDOMDocument *conf = new TDOMDocument(this);
        if (!(bool)conf->load(TVariant(WideString(filename))))
            throw *(new Exception("Файл не читается"));
        IXMLDOMElementPtr root = conf->documentElement;
        if (!root.IsBound())
            throw *(new Exception("Корневой узел отсутствует"));

        IXMLDOMNodePtr section = root->firstChild;
        while (section.IsBound() && (
               wcscmp(section->nodeName, L"position") != 0 ||
               section->nodeType != Msxml_tlb::NODE_ELEMENT))
            section = section->nextSibling;

        if (!section.IsBound())
        {
            OLECHECK(section = conf->createElement(L"position"));
            root->appendChild(section);
        }
        {
            IXMLDOMElementPtr element = section;
            char oldDecSep = ::DecimalSeparator;
            ::DecimalSeparator = '.';
            try {
                element->setAttribute(L"centerx", TVariant(WideString(FormatFloat("0.0", m_map->CenterX))));
                element->setAttribute(L"centery", TVariant(WideString(FormatFloat("0.0", m_map->CenterY))));
                if(m_map->Zoom > 0)
                    element->setAttribute(L"zoom",    TVariant(WideString(FormatFloat("0.0", m_map->Zoom))));
                else
                    element->setAttribute(L"zoom",    TVariant(WideString(FormatFloat("0.0", 100.0))));
            } __finally {
                ::DecimalSeparator = oldDecSep;
            }
        }

        conf->save(TVariant(WideString(filename)));

    } catch (Exception &e) {
        throw *(new Exception("Ошибка записи в конфигурационный файл карты '"+filename+"':\n\n"+e.Message));
    }
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actProfileExecute(TObject *Sender)
{
    Map->CurrentTool = miReliefTool;
    Map->Cursor = crDefault;
    actDistance->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::FitObjects()
{
    double minX = 180;
    double maxX = -180;
    double minY = 90;
    double maxY = -90;

    for (Shapes::iterator sh = shapes.begin(); sh < shapes.end(); sh++)
    {
        if ((*sh)->visible && GetLayerVisible((*sh)->layer))
        {
            Point lb, rt;
            if ((*sh)->GetExtent(lb, rt))
            {
                minX = min(minX, lb.first);
                maxX = max(maxX, rt.first);
                minY = min(minY, lb.second);
                maxY = max(maxY, rt.second);
            }
        }
    }

    if (minX < DBL_MAX)
    {
        m_map->CenterX = (minX + maxX) / 2;
        m_map->CenterY = (minY + maxY) / 2;

        double dist = m_map->Distance(minX, minY, maxX, maxY);
        m_map->Zoom = (dist < 50) ? (100) : (dist * 1.5);
        m_map->Refresh();
    }
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actFitObjectsExecute(TObject *Sender)
{
    FitObjects();
}
//---------------------------------------------------------------------------

void TBaseMapFrame::RemoveShapeReferences(MapShape* shape)
{
    // erase references from shape indices and zero reference in main container
    shapeIdx.erase(shapeIdx.find(shape->GetId()));
    ShapePosIdx::iterator posi = shapePosIdx.find(shape);
    if (posi != shapePosIdx.end())
    {
        shapes[posi->second] = NULL;
        shapePosIdx.erase(posi);
    }
}

MapShape* TBaseMapFrame::NewShape(ShapeType sht)
{
    MapShape* sh = NULL;
    switch (sht) {
        case stPoint: sh = new MapPoint; break;
        case stLink: sh = new MapLink; break;
        case stPolygon: sh = new MapPolygon; break;
        case stLabel: sh = new MapLabel; break;
        default: return NULL;
    }
    sh->id = GetNewShapeId();
    sh->layer = 0;
    sh->owner = this;

    shapes.push_back(sh);
    shapeIdx[sh->GetId()] = sh;
    shapePosIdx[sh] = shapes.size() - 1;

    return sh;
}

void TBaseMapFrame::CheckIsShapeValid(MapShape *sh)
{
    TBaseMapFrame::ShapePosIdx::iterator posi = shapePosIdx.find(sh);
    if (posi == shapePosIdx.end())
        throw *(new Exception("MapShape::CheckIsValid(): shape is not indexed"));
}


void __fastcall TBaseMapFrame::CoordMapToScreen(double X, double Y, int* x, int* y)
{
    float xf, yf;
    Map->ConvertCoord(&xf, &yf, &X, &Y, miMapToScreen);
    *x = xf;
    *y = yf;
}

void __fastcall TBaseMapFrame::CoordScreenToMap(int x, int y, double *X, double *Y)
{
    float xf = x;
    float yf = y;
    Map->ConvertCoord(&xf, &yf, X, Y, miScreenToMap);
}

MapShape* __fastcall TBaseMapFrame::GetShapeById(int id)
{
    ShapeIdx::iterator shi = shapeIdx.find(id);
    if (shi != shapeIdx.end())
        return shi->second;
    else
        return NULL;
}

void __fastcall TBaseMapFrame::SetLayersCount(int val)
{
    layers.resize(val);
    while (layers.size() < val)
        layers.push_back(Layer());
}

int __fastcall TBaseMapFrame::GetLayersCount()
{
    return layers.size();
}

void __fastcall TBaseMapFrame::SetLayerVisible(int __index, bool value)
{
    CheckIndex(__index, layers.size(), "слоя ");
    if(layers[__index].visible != value)
        layers[__index].visible = value;
}

bool __fastcall TBaseMapFrame::GetLayerVisible(int __index)
{
    CheckIndex(__index, layers.size(), "слоя ");
    return layers[__index].visible;
}

void __fastcall TBaseMapFrame::SetLayerSelectable(int __index, bool value)
{
    CheckIndex(__index, layers.size(), "слоя ");
    if(layers[__index].selectable != value)
        layers[__index].selectable = value;
}

bool __fastcall TBaseMapFrame::GetLayerSelectable(int __index)
{
    CheckIndex(__index, layers.size(), "слоя ");
    return layers[__index].selectable;
}

void __fastcall TBaseMapFrame::SetLayerCaption(int __index, AnsiString value)
{
    CheckIndex(__index, layers.size(), "слоя ");
    if(layers[__index].caption != value)
        layers[__index].caption != value;
}

AnsiString __fastcall TBaseMapFrame::GetLayerCaption(int __index)
{
    CheckIndex(__index, layers.size(), "слоя ");
    return layers[__index].caption;
}

void __fastcall TBaseMapFrame::CheckIndex(int idx, int size, AnsiString msg)
{
    if(idx < 0 || idx >= size)
        throw *(new Exception("Неправильный индекс "+msg+'('+IntToStr(idx)+')'));
}

TBaseMapFrame::Shapes __fastcall TBaseMapFrame::SelectObjects(int x, int y, int trim)
{
    Shapes res;
    for (int i = GetLayersCount() - 1; i >= 0; i--)
        if (GetLayerVisible(i) && GetLayerSelectable(i))
            for (Shapes::iterator shi = shapes.begin(); shi < shapes.end(); shi++)
            {
                if((*shi) != NULL && (*shi)->layer == i && (*shi)->IsIntersect(x, y, trim))
                    res.push_back(*shi);
            }
    return res;
}

void __fastcall TBaseMapFrame::SetCenter(double lon, double lat)
{
    Map->CenterX = lon;
    Map->CenterY = lat;
    Map->Refresh();
}

void __fastcall TBaseMapFrame::SetScale(double scale)
{
    Map->Zoom = scale;
    Map->Refresh();
}

//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::RemoveObject(int id)
{
//Это работает:


//    RemoveShapeReferences();

    MapShape* tmp_shape;
    int tmp_id = 0;
    //чистим вручную !!!
    Shapes tmp_shapes = shapes;
    shapes.clear();
    for(unsigned int i = 0; i < tmp_shapes.size(); i++)
    {
        tmp_shape = tmp_shapes[i];
        tmp_id = tmp_shape->GetId();
        if(tmp_id == id)
            delete tmp_shape;
        else
            shapes.push_back(tmp_shape);
    }

//Это не работает:
/*
    MapShape* tmp_shape;
    int tmp_id = 0;
    for(unsigned int i = 0; i < shapes.size(); i++)
    {
        tmp_shape = shapes[i];
        tmp_id = tmp_shape->GetId();
        if(tmp_id == id)
        {
            //delete shapes[i];//шо так
            delete tmp_shape;  //шо так...
            return;
        }
    }
/**/
}
//---------------------------------------------------------------------------
void __fastcall TBaseMapFrame::SetCursor(TCursor value)
{
    if (m_map)
        m_map->Cursor = value;
}

TCursor __fastcall TBaseMapFrame::GetCursor()
{
    if (m_map)
        return m_map->Cursor;
    else
        return crDefault;
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actConfExecute(TObject *Sender)
{
    std::auto_ptr<TdlgMapConf> dlg(new TdlgMapConf(Application, ChangeFileExt(Application->ExeName, ".mapconf")));
    if (dlg->ShowModal() == mrOk)
    {
        double cenx = Map->CenterX;
        double ceny = Map->CenterY;
        double zoom = Map->Zoom;

        LoadConf("");

        Map->CenterX = cenx;
        Map->CenterY = ceny;
        Map->Zoom    = zoom;
    }
}
//---------------------------------------------------------------------------

void __fastcall TBaseMapFrame::actArrowsExecute(TObject *Sender)
{
    bool checked = actArrows->Checked;
    btnUL->Visible = checked;
    btnU->Visible = checked;
    btnUR->Visible = checked;
    btnR->Visible = checked;
    btnDR->Visible = checked;
    btnD->Visible = checked;
    btnDL->Visible = checked;
    btnL->Visible = checked;
    actArrows->Checked = !checked;
}

}; // namespace



