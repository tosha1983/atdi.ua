//---------------------------------------------------------------------------

#ifndef BaseMapH
#define BaseMapH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "MapXLib_OCX.h"
#include <Buttons.hpp>
#include <ActnList.hpp>
#include <ImgList.hpp>
#include <ComCtrls.hpp>
#include <ToolWin.hpp>
#include <vector>
#include <map>
#include <math.h>

//---------------------------------------------------------------------------
namespace Lis_map {

typedef std::pair<double, double> Point;
// let first will be X and second will be Y
typedef std::vector<Point> Points;

enum MapTools { miDistanceTool = 1, miReliefTool, miLastBmTool };

enum PointType {ptPoint, ptXCross, ptPlusCross, ptSymbol};//стили для MapPoint



struct MapCallback {
    virtual mouseMove(double lon, double lat) = 0;
    virtual mouseDown(double lon, double lat) = 0;
    virtual mouseUp(double lon, double lat) = 0;
    virtual layerAdded(CMapXLayer *l, AnsiString n) = 0;
};

class TBaseMapFrame;

enum ShapeType { stPoint, stLink, stPolygon, stLabel };

class MapShape
{
  protected:
    unsigned        id;
    unsigned        layer;      // just manage visibility
    //ShapeType       shapetype;
    TBaseMapFrame*  owner;

    MapShape(): layer(0), visible(true), color(1), width(1), userTag(0)
    {};
    static HGDIOBJ CreateFont(HDC hMapDC, AnsiString name, int size, TColor color, TFontStyles style);

  public:
    bool        visible;
    int         color;
    int         width;
    AnsiString  name;
    AnsiString  desc;
    int         userTag;

    int GetId()            { return id; }
    void CheckIsValid();
    int GetLayer() { return layer; }
    void SetLayer(int l);
    virtual void Paint(HANDLE hOutputDC) = 0;

    virtual ~MapShape(); // destructor needs to be virtual due to type-unknown deletion from shape vector 
    virtual bool GetCenterPoint(double& cx, double& cy) { return false; /* not realized by default */ }
    virtual bool __fastcall IsIntersect(int x, int y, int trim) { return false; }
    virtual bool __fastcall GetExtent(Point& lb, Point& rt) {return false;};

  friend class TBaseMapFrame;
};


class MapPoint: public MapShape
{
  public:
    double      x;
    double      y;
    PointType pointType;
    int symbol;
    AnsiString fontName;
    int fontSize;
    bool isBold;

  protected:
    MapPoint(): MapShape() {pointType=ptPoint; fontName="MapInfo Transportation", symbol=0x71; fontSize=16; isBold=false;};

  public:
    virtual void Paint(HANDLE hOutputDC);
    virtual bool GetCenterPoint(double& cx, double& cy)
    { cx = this->x, cy = this->y; return true; }
    bool __fastcall IsIntersect(int x, int y, int trim);
    virtual bool __fastcall GetExtent(Point& lb, Point& rt);


  friend class TBaseMapFrame;
};

enum MapArrowType {matNone, matArrow, matPoint};

class MapLink : public MapShape
{
  public:
    double      x1;
    double      y1;
    double      x2;
    double      y2;

    int         trim;
    TPenStyle   style;

    MapArrowType arrow;
  protected:
    MapLink(): MapShape(), trim(0), style(psSolid) {};

  public:
    virtual void Paint(HANDLE hOutputDC);
    virtual bool GetCenterPoint(double& cx, double& cy)
    { cx = this->x1 + this->x2 / 2, cy = this->y1 + this->y2 / 2; return true; }
    bool __fastcall IsIntersect(int x, int y, int trim);
    virtual bool __fastcall GetExtent(Point& lb, Point& rt);
    
  friend class TBaseMapFrame;
};


class MapPolygon : public MapShape
{
  public:
    Points points;
    std::vector<TPoint> pointsInt;

    int         trim;
    TPenStyle   style;
    bool        closed;
    bool        filled;

  protected:
    MapPolygon(): MapShape(), trim(0), style(psSolid), closed(true), filled(false) {};

  private:
    int xmin, ymin, xmax, ymax;

  public:
    ~MapPolygon() { points.clear(); };
    virtual void Paint(HANDLE hOutputDC);
    virtual bool GetCenterPoint(double& cx, double& cy);
    bool __fastcall IsIntersect(int x, int y, int trim);
    virtual bool __fastcall GetExtent(Point& lb, Point& rt);

  friend class TBaseMapFrame;
};


class MapLabel : public MapShape
{
  public:
    AnsiString      label;
    MapShape*       reference;
    int             offsetx;
    int             offsety;
  protected:
    MapLabel(): MapShape(), reference(NULL), offsetx(0), offsety(0) {};

  public:
    virtual void Paint(HANDLE hOutputDC);
    virtual bool GetCenterPoint(double& cx, double& cy) { return false; };

  friend class TBaseMapFrame;
};

class TBaseMapFrame : public TFrame
{
__published:	// IDE-managed Components
    TBitBtn *btnUL;
    TBitBtn *btnU;
    TBitBtn *btnUR;
    TBitBtn *btnR;
    TBitBtn *btnDR;
    TBitBtn *btnD;
    TBitBtn *btnDL;
    TBitBtn *btnL;
    TActionList *al;
    TImageList *iml;
    TAction *actNone;
    TAction *actDistance;
    TAction *actPan;
    TAction *actZoomInTwice;
    TAction *actZoomOutTwice;
    TAction *actZoomDefault;
    TAction *actZoomCust;
    TAction *actPanButtons;
    TAction *actProfile;
    TAction *actFitObjects;
    TAction *actConf;
    TAction *actArrows;
    void __fastcall btnUClick(TObject *Sender);
    void __fastcall btnLClick(TObject *Sender);
    void __fastcall btnRClick(TObject *Sender);
    void __fastcall btnDClick(TObject *Sender);
    void __fastcall btnULClick(TObject *Sender);
    void __fastcall btnURClick(TObject *Sender);
    void __fastcall btnDLClick(TObject *Sender);
    void __fastcall btnDRClick(TObject *Sender);
    void __fastcall actNoneExecute(TObject *Sender);
    void __fastcall actPanExecute(TObject *Sender);
    void __fastcall actDistanceExecute(TObject *Sender);
    void __fastcall actZoomInTwiceExecute(TObject *Sender);
    void __fastcall actZoomOutTwiceExecute(TObject *Sender);
    void __fastcall actZoomDefaultExecute(TObject *Sender);
    void __fastcall actZoomCustExecute(TObject *Sender);
    void __fastcall actPanButtonsExecute(TObject *Sender);                     
    void __fastcall MapMouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall MapMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
    void __fastcall MapMouseUp(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall MapToolUsed(System::TObject * Sender, short ToolNum, double X1, double Y1,
        double X2, double Y2, double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl, VARIANT_BOOL* EnableDefault);
    void __fastcall MapDrawUserLayer(System::TObject * Sender, LPDISPATCH Layer, OLE_HANDLE hOutputDC,
        OLE_HANDLE hAttributeDC, LPDISPATCH RectFull, LPDISPATCH RectInvalid);
    void __fastcall actProfileExecute(TObject *Sender);
    void __fastcall actFitObjectsExecute(TObject *Sender);
    void __fastcall actConfExecute(TObject *Sender);
    void __fastcall actArrowsExecute(TObject *Sender);
    void __fastcall MapDblClick(TObject *Sender);
public:

    // types
    typedef std::vector<MapShape*> Shapes;
    typedef std::map<int, MapShape*> ShapeIdx;
    typedef std::map<MapShape*, int> ShapePosIdx;

    struct Layer {
        AnsiString caption;
        bool visible;
        bool selectable;

        Layer(): visible(true), selectable(false) {};
        Layer(const Layer& pat): caption(pat.caption), visible(pat.visible), selectable(pat.selectable)
        {}
        Layer& operator= (const Layer& pat)
        {
            caption = pat.caption, visible = pat.visible; selectable = pat.selectable; return *this;
        }
    };

private:	// User declarations
    TMxMap* m_map;

    TMxMapToolUsed m_ToolUsed;

    MapCallback *notifObj;
    AnsiString mapIniName;
    int staticLayers;

    // support of shapes

    // main container
    Shapes shapes;

    // indices Id-to-Shape and Shape-to-Pos
    ShapePosIdx shapePosIdx;
    ShapeIdx shapeIdx;

    MapShape* distShape;

    // global Id counter
    unsigned lastId;

    typedef std::vector<Layer> Layers;
    Layers layers;
    void __fastcall CheckIndex(int idx, int size, AnsiString msg);
    void __fastcall SetCursor(TCursor value);
    TCursor __fastcall GetCursor();
    bool __fastcall getIsOverlapping(std::vector<tagRECT> &vRect, tagRECT _r);

public:		// User declarations

    TMenuItem *mapMenu;

    bool doInitDelay;
    int initDelay;
    bool doInitInfo;
    
    __fastcall TBaseMapFrame(TComponent* Owner);
    __fastcall ~TBaseMapFrame();

    TMxMap* __fastcall GetMap();
    void __fastcall Init();
    void __fastcall LoadConf(AnsiString filename);
    void __fastcall ClearData(int l);
    unsigned GetNewShapeId() { return ++lastId; };

    MapShape* NewShape(ShapeType sht);
    void CheckIsShapeValid(MapShape *sh);

    void __fastcall MapMenuClick(TObject *Sender);
    void __fastcall SavePosition(AnsiString filename);
    AnsiString GetDefConf() { return mapIniName; }
    void __fastcall CenterObjectsPos();
    void __fastcall FitObjects();
    void __fastcall CoordMapToScreen(double X, double Y, int* x, int* y);
    void __fastcall CoordScreenToMap(int x, int y, double* X, double* Y);
    MapShape* __fastcall GetShapeById(int id);

    void __fastcall SetLayersCount(int val);
    int __fastcall GetLayersCount();
    void __fastcall SetLayerVisible(int __index, bool value);
    bool __fastcall GetLayerVisible(int __index);
    void __fastcall SetLayerSelectable(int __index, bool value);
    bool __fastcall GetLayerSelectable(int __index);
    void __fastcall SetLayerCaption(int __index, AnsiString value);
    AnsiString __fastcall GetLayerCaption(int __index);

    Shapes __fastcall SelectObjects(int x, int y, int trim);

    __property TMxMap* Map = { read = GetMap };
    __property TMxMapToolUsed OnToolUsed = { read = m_ToolUsed, write = m_ToolUsed };

    void __fastcall SetCenter(double lon, double lat);
    void __fastcall SetScale(double scale);

    void __fastcall RemoveObject(int id);
    String GetConfFileName();
    __property TCursor Cursor  = { read=GetCursor, write=SetCursor };
protected:
    void RemoveShapeReferences(MapShape* shape);

    friend class MapShape;
};


// MapShape inline implementation

inline MapShape::~MapShape()
{
    if (owner)
        owner->RemoveShapeReferences(this);
};

} // namespace

//---------------------------------------------------------------------------
#endif

