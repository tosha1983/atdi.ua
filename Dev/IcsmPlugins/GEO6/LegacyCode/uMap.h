//---------------------------------------------------------------------------

#ifndef uMapH
#define uMapH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <ToolWin.hpp>
#include <OleCtrls.hpp>
#include <Menus.hpp>
#include <ImgList.hpp>

#include "RSAGeography_TLB.h"
#include "rsMapUniversalX_OCX.h"

#include "uCoordinationPoint.h"
//---------------------------------------------------------------------------
#include <vector>
using namespace std;
enum ObjectType {
    otAll = 0,
    otTx = 1,
    otCoverage = -1,
    otZone = -2
};

extern TColor ccTx;
extern TColor ccTxSelected;
extern TColor ccTxZero;
extern TColor ccZoneCover;
extern TColor ccZoneCoverNotUsed;
extern TColor ccZoneNoise;
extern TColor ccECover;
extern TColor ccEPoint;
extern TColor ccZoneCoord;

#define BC_MAP_MASK_ZONECOVER       0x80000000
#define BC_MAP_MASK_ZONENOISE       0x40000000
#define BC_MAP_MASK_ZONEINTERFERE   0x20000000
#define BC_MAP_MASK_ZONEINTERFERE2  0x10000000

#define MT_GET_RELIEF 101

class TfrmSelection;
class TfrmTxBaseAir;

class TfmMap : public TFrame
{
__published:	// IDE-managed Components
    TStatusBar *sbMap;
    TMapUniversalX *theMap;
    TPopupMenu *pmnTx;
    TMenuItem *mniTxEdit;
    TMenuItem *mniUseInCalc;
    TMenuItem *mniAnalyze;
    TMenuItem *mniShowTestPoints;
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall theMapMouseDown(TObject *Sender, TxMouseButton Button, short Shift, long X, long Y);
    void __fastcall theMapMouseMove(TObject *Sender, short Shift, long X, long Y);
    void __fastcall theMapMouseUp(TObject *Sender, TxMouseButton Button, short Shift, long X, long Y);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall btnCloseClick(TObject *Sender);
    void __fastcall theMapSelectedZone(TObject *Sender, long ABegX, long ABegY, long AEndX, long AEndY);
private:	// User declarations
    bool FConnected;   // Признак загрузки карты
    // интерфейс связи с блоком управления
    TCOMIrsMapXXMap FrsCustomXMap;
    TCOMIrsVectorXMap1 FVectorXMap;
    // интерфейс преобразования координат
    TCOMIrsCoordTransformMap FCoordTrans;
    TCOMIrsAdvanceCoordTransformMap FAdvCoordTrans;
    // интерфейс географических функций
    TCOMIrsGeoFunctionsMap FGeoFuncs;
    // режим выбора мыши
    TCOMIrsSelectMouseMode m_mouseSelectMode;
    // Началоотсчета вычисления дистанции
    int FBegX;
    int FBegY;
    // выбранный обьект карты
    TCOMIrsMapObj FSelMapObj[3];
    // текущая точка
    TCOMIrsMapObj FEPointObj;
    // последняя нарисованная координационная зона
    TCOMIrsMapObj FLastCoordZone;
    // полигон - ось через точки стояния дуэлянтов (три линии)
    TCOMIrsMapObj FDuelAxe;
    // контрольные точки
    TCOMIrsMapObj checkPoints[360];
    // Интерфейс списка обьектов
    TCOMIrsMapObjectList FMapObjectList;

    std::vector<IrsMapObj*> coordinationPoints;
    TrsGeoPoint CentrePos;
    double Scale;

    TfrmSelection *theSelection;
    TfrmTxBaseAir* theTxCard;
    void __fastcall setObjColor(IrsMapObjPtr &mapObj, long);

protected:
    //long LastIdUserObject;
    //long FirstIdUserObject;
    bool m_mouseDown;
    int lastSelectedId;

public:		// User declarations
    void __fastcall highlightTx(long TxId, int txNo);
    __fastcall TfmMap(TComponent* Owner);
    __fastcall ~TfmMap();
    IrsMapObj* __fastcall drawCircle(double Long, double Lat, int Radius, TColor LColor, int id = 0);
    IrsMapObj* __fastcall drawEPoint(double Long, double Lat, int Id, TColor LColor, AnsiString Name, AnsiString Hint, int id = 0);
    IrsMapObj* __fastcall drawLine(double Long, double Lat, double Long2, double Lat2, int Size, TColor LColor, int id = 0);
    IrsMapObj* __fastcall drawPoint(double Long, double Lat, int Id, TColor LColor, AnsiString Name, AnsiString Hint, char symbol = 0, int id = 0);
    IrsMapObj* __fastcall drawPoligon(double Long, double Lat, double* InArray, TColor LColor, int LineWeight, int numberOfPoints = 36, int id = 0);
    IrsMapObj* __fastcall drawZone(double *array, bool gradient, double Emin, int id = 0);
    IrsMapObj* __fastcall drawContour(std::vector<double>& coords, WideString name, TColor LColor, int LineWeight, int id);
    void __fastcall centerMap(double Lon, double Lat);
    AnsiString PrintFileName;
    int Quality;
    TrsPixPoint zoneBeg;
    TrsPixPoint zoneEnd;

    TrsGeoPoint zoneBegGeo;
    TrsGeoPoint zoneEndGeo;

    void __fastcall deleteObjectById(int id);
    void __fastcall deleteObjects();
    void __fastcall setScale(double scale);
    void __fastcall getPureCoverage();
    void __fastcall loadParams();
    void __fastcall setHint(IrsMapObj* mapObject, AnsiString& value);
    void __fastcall setLabel(IrsMapObj* mapObject, AnsiString& value);
    void __fastcall setLabelById(int id, AnsiString& value);
    void __fastcall setColor(IrsMapObj* mapObject, TColor newColor);
    void __fastcall setColorById(int id, TColor newColor);
    void __fastcall drawCoordZone(double lon, double lat, double * zone);
    __property bool connected = {read = FConnected};
    double minLon, minLat, maxLon, maxLat;
    void __fastcall fitAllObjects();
    void __fastcall drawDuelAxe(const TRSAGeoPoint& pointA, const TRSAGeoPoint& pointB, const TRSAGeoPoint& point1, const TRSAGeoPoint& point2);
    void __fastcall setVisible(long id, bool visible);
    void __fastcall setVisible(IrsMapObj* pMo, bool visible);
    void __fastcall coordinationPointsDelete();
    void __fastcall coordinationPointsShow(std::vector<CoordinationPoint>& cordinationPoints);

    void __fastcall saveToBmp();
    void __fastcall reload();
    void __fastcall unload();
    void __fastcall configureLayers();
    void __fastcall showCheckPoints(LPSAFEARRAY sa, int txType);
    void __fastcall deleteCheckPoints();
    int getLastSelectedId() { return lastSelectedId; };
};
//---------------------------------------------------------------------------
extern TCOMIRSASpherics FSpherics;
//---------------------------------------------------------------------------
#endif
