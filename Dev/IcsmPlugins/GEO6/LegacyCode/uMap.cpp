//---------------------------------------------------------------------------

#include <vcl.h>
#include <values.h>
#include <Registry.hpp>
#include <memory>
#pragma hdrstop

#pragma link "rsMapUniversalX_OCX"

#include "uLayoutMngr.h"
#include "uMap.h"
#include "uMainDm.h"
#include "uMainForm.h"
#include "uPrintMapDlg.h"
#include "uSelection.h"
#include "uFrmTxBaseAir.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)

#pragma resource "*.dfm"
#include "uParams.h"
#include <math.h>

//  Интерфейс геометрии на сфере
TCOMIRSASpherics FSpherics;

bool cancelDrawing;

//  color constants
TColor ccTx = clMaroon;
TColor ccTxSelected = clBlue;
TColor ccTxZero = clRed;
TColor ccZoneCover = clGreen;
TColor ccZoneCoverNotUsed = clSilver;
TColor ccZoneNoise = clBlue;
TColor ccECover = clRed;
TColor ccEPoint = clTeal;
TColor ccZoneCoord = clRed;
TColor ccDuelAxe = clMaroon;

WideString wsMapLayersFile(ExtractFileDir(Application->ExeName) + "\\MapLayers.dat");

char* szMapLayerLib   = "rsMapLayer.dll";
char* szMapLayerFunc  = "ExecMapLayerEx";
char* szCANNOTLOADFUNC  = "Не можу завантажити функцію: '%s' бібліотеки '&s'";
char* szCANNOTLOADLIB  = "Не можу завантажити бібліотеку: %s";
AnsiString MapPath;

//---------------------------------------------------------------------------

__fastcall TfmMap::TfmMap(TComponent* Owner)
    : TFrame(Owner), FConnected(false), m_mouseDown(false)
{
    loadParams();

    zoneBeg.ppX = 0;
    zoneBeg.ppY = 0;
    zoneEnd.ppX = 0;
    zoneEnd.ppY = 0;

    lastSelectedId = 0;

    theSelection = dynamic_cast<TfrmSelection*>(Owner);
    theTxCard = dynamic_cast<TfrmTxBaseAir*>(Owner);

    MapPath = AnsiString(sAppRegPath) + "\\Map";

    minLon = minLat = maxLon = maxLat = 0.0;
    
    std::auto_ptr<TRegistry> reg (new TRegistry);
    reg->Access = KEY_READ;
    reg->RootKey = HKEY_CURRENT_USER;
    if (reg->OpenKeyReadOnly(MapPath))  {
        try {
            CentrePos.gpL = reg->ReadFloat("GeoCentrePosL");
        } catch (...) {
        CentrePos.gpL = 50;
        }
        try {
             CentrePos.gpH = reg->ReadFloat("GeoCentrePosH");
        } catch (...) {
        CentrePos.gpH = 30;
        }
        try {
            Scale = reg->ReadFloat("Scale");
        } catch (...) {
        Scale = 400;
        }
        sbMap->Panels->Items[2]->Text = AnsiString(Scale);
        }
    reg->CloseKey();
    //LastIdUserObject = 1;
    //FirstIdUserObject = 1;
    if (!FSpherics.IsBound())
        FSpherics.CreateInstance(CLSID_RSASpherics);

    IrsMapUniversalAdvangePtr ptrIMapUniversalAdvange;
    theMap->ControlInterface->QueryInterface(IID_IrsMapUniversalAdvange, (void**)&ptrIMapUniversalAdvange);
    if (ptrIMapUniversalAdvange.IsBound()) {
        ptrIMapUniversalAdvange->Set_CursorHandle(crGetE, (long)LoadCursor(HInstance, "CR_GET_E"));
        ptrIMapUniversalAdvange->Set_CursorHandle(crGetTx, (long)LoadCursor(HInstance, "CR_SEL_TX"));
        ptrIMapUniversalAdvange->Set_CursorHandle(crGetRelief, (long)LoadCursor(HInstance, "CR_GET_RELIEF"));
    }

    /*
    for (int i = 0; i < 360; i++) {
        checkPoints[i] = (IrsMapObj*)NULL; //TCOMIrsMapObj();
    }
    */
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::loadParams()
{
    ccZoneCover = (TColor)(BCCalcParams.lineColorZoneCover);
    ccZoneNoise = (TColor)(BCCalcParams.lineColorZoneNoise);
}
//---------------------------------------------------------------------------

__fastcall TfmMap::~TfmMap()
{
    try
    {
        if (FConnected)
        {
            AnsiString StringVal;
            TrsGeoPoint CentrePos;
            double Scale;
            std::auto_ptr<TRegistry> reg(new TRegistry);

            FrsCustomXMap->Get_Scale(&Scale);
            FrsCustomXMap->Get_GeoCenterMap(&CentrePos);
            reg->Access = KEY_WRITE;
            reg->RootKey = HKEY_CURRENT_USER;
            if (reg->OpenKey(MapPath, true))  {
                reg->WriteFloat("GeoCentrePosL", (double)CentrePos.gpL);
                reg->WriteFloat("GeoCentrePosH", (double)CentrePos.gpH);
                reg->WriteFloat("Scale",Scale);
            }
            reg->CloseKey();

            unload();
            FConnected = false;
        }
    }
    catch(...)
    {
    }
}

//---------------------------------------------------------------------------
void __fastcall TfmMap::FormClose(TObject *Sender, TCloseAction &Action)
{
    Action = caFree;
    //frmMap = NULL;
    //frmMain->actMap->Checked = false;
    try {
        //LayoutManager.saveLayout(this);
    } catch(...) {}
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::highlightTx(long TxId, int txNo)
{
    IrsMapObjPtr ptrSelMapObj;
    FMapObjectList->FindByID(TxId, &ptrSelMapObj);
    if (ptrSelMapObj.IsBound())
    {
        long selId = 0;
        ptrSelMapObj->Get_ID(&selId);
        if (selId > 0)
            if (FEPointObj.IsBound())
            {
                FEPointObj->Delete();
                FEPointObj.Unbind();
                //setVisible((IrsMapObj*)FEPointObj, false);
            }
        IrsColorMapObjPtr ptrColorMapObj;

        TCOMIrsMapObj SelMapObj;
        if ( FSelMapObj[txNo].IsBound() )
            SelMapObj = FSelMapObj[txNo];

        setObjColor(SelMapObj, ccTx);

        if ( (txNo != 0) && (FSelMapObj[0].IsBound()) )
        {
            long id;
            FSelMapObj[0]->Get_ID(&id);

            if ( id == TxId )//если пытаемся перекрасить помехой занятый передатчик -- выйти
                return;
        }

        FSelMapObj[txNo] = ptrSelMapObj;

        setObjColor(ptrSelMapObj, txNo == 0 ? ccTxSelected : txNo == 1 ? ccTxZero : BCCalcParams.lineColorZoneInterfere2);

    }
}


void __fastcall TfmMap::theMapMouseDown(TObject *Sender, TxMouseButton Button, short Shift, long X, long Y)
{
    m_mouseDown = true;

    if ((theMap->LeftMouseMode == mmSelectObject || theMap->Cursor == crGetTx) && theMap->Cursor != crGetE)
    {
        IrsMapObjPtr ptrSelMapObj;
        FMapObjectList->SelectObject(X, Y, &ptrSelMapObj);
        if (ptrSelMapObj.IsBound())
        {
            long selId = 0;
            ptrSelMapObj->Get_ID(&selId);
            if (selId > 0)
            {
                if ( theSelection )
                    theSelection->selectTxByID(selId);
            }
            else
            {//контрольная точка
                TrsPixPoint LPix;
                TrsGeoPoint LGeo;
                LPix.ppX = X;
                LPix.ppY = Y;
                // Преобразовать пиксельные координаты в географические
                FAdvCoordTrans->PixToGeoSM(LPix, &LGeo, tcAbsolute);

                if ( theSelection )
                {
                    theSelection->showCheckPoint(LGeo.gpL, LGeo.gpH);
                }
            }

            if (selId > 0 && Button == Rsmapuniversalx_tlb::mbRight)
            {
                POINT point;
                if ( GetCursorPos(&point) )
                    pmnTx->Popup(point.x, point.y);
            }
        }
        return;
    }

    if (theMap->LeftMouseMode == mmNo && theMap->Cursor == crGetE) {
        TrsPixPoint LPix;
        TrsGeoPoint LGeo;
        LPix.ppX = X;
        LPix.ppY = Y;
        // Преобразовать пиксельные координаты в географические
        FAdvCoordTrans->PixToGeoSM(LPix, &LGeo, tcAbsolute);

        if ( theSelection )
            theSelection->getE(LGeo.gpL, LGeo.gpH);
    }

    /*
    ShowMessage(
        AnsiString("zoneBeg.ppX = ") + zoneBeg.ppX +
        "\nzoneBeg.ppY = " + zoneBeg.ppY
        );
        */

    if (theMap->LeftMouseMode == mmSelect) {
        /*
        zoneBeg.ppX = X;
        zoneBeg.ppY = Y;
        ShowMessage(
            AnsiString("zoneBeg.ppX = ") + zoneBeg.ppX +
            "\nzoneBeg.ppY = " + zoneBeg.ppY
            );
        */
    }

}
//---------------------------------------------------------------------------
void __fastcall TfmMap::theMapMouseMove(TObject *Sender, short Shift, long X, long Y)
{
    if (FConnected) {
        TrsPixPoint LPix;
        TrsGeoPoint LGeo;
        WideString LStr;
        if (FAdvCoordTrans) {
            LPix.ppX = X;
            LPix.ppY = Y;
            // Преобразовать пиксельные координаты в географические
            FAdvCoordTrans->PixToGeoSM(LPix, &LGeo, tcAbsolute);
            // Преобразовать географические координаты в строку
            sbMap->Panels->Items[0]->Text = dmMain->coordToStr(LGeo.gpL, 'X') + " : " + dmMain->coordToStr(LGeo.gpH, 'Y');
            sbMap->Panels->Items[1]->Text = AnsiString().sprintf("%d:%d px", X, Y);
            short a;
            TRSAGeoPoint gp;
            if (BCCalcParams.FTerrInfoSrv.IsBound()) {
                gp.L = LGeo.gpL;
                gp.H = LGeo.gpH;
                try {
                BCCalcParams.FTerrInfoSrv.Get_Altitude(gp, &a);
                sbMap->Panels->Items[5]->Text = AnsiString(a) + " m";
                } catch(...) {}
            } else {
                sbMap->Panels->Items[5]->Text = "";
            }
        }
        FrsCustomXMap->Get_Scale(&Scale);
        sbMap->Panels->Items[2]->Text = "Мірило: " + FormatFloat("0.00",Scale);

    }
}
//---------------------------------------------------------------------------
void __fastcall TfmMap::theMapMouseUp(TObject *Sender, TxMouseButton Button, short Shift, long X, long Y)
{
    m_mouseDown = false;
    double LDist;
    if (theMap->LeftMouseMode == mmToolDistance || theMap->LeftMouseMode == MT_GET_RELIEF) {
        // получить суммирующую дистанцию
        FrsCustomXMap->Get_Distance(&LDist);
        sbMap->Panels->Items[4]->Text = AnsiString().sprintf("%f km", LDist);
    }
    if (theMap->LeftMouseMode == mmSelect) {
        if (theSelection)
        {
            FAdvCoordTrans->PixToGeoSM(zoneBeg, &zoneBegGeo, tcAbsolute);
            FAdvCoordTrans->PixToGeoSM(zoneEnd, &zoneEndGeo, tcAbsolute);
            theSelection->doSelection();
        }
        else if (theTxCard)
        {
            FAdvCoordTrans->PixToGeoSM(zoneBeg, &zoneBegGeo, tcAbsolute);
            FAdvCoordTrans->PixToGeoSM(zoneEnd, &zoneEndGeo, tcAbsolute);
            theTxCard->doSelection();
        }

    }
}
//---------------------------------------------------------------------------
void __fastcall TfmMap::unload()
{
try
{
    if (FSelMapObj[0].IsBound()) {
        FSelMapObj[0].Unbind();
    }
    if (FSelMapObj[1].IsBound()) {
        FSelMapObj[1].Unbind();
    }

    // Удалить все обьекты карты
    if (FMapObjectList.IsBound()) {
        FMapObjectList->Clear(tpMapObject);
        FMapObjectList.Unbind();
    }
    // Выгрузить слои карты
    FVectorXMap->UnLoadLayers();
    // Disconnect карты
    FrsCustomXMap->Set_Connect(false);
    // Diactive карты
    FrsCustomXMap->Set_Active(false);
    //theMap->MapSource->Release();
    FAdvCoordTrans.Unbind();
    FCoordTrans.Unbind();
    FrsCustomXMap.Unbind();
    FConnected = false;
    FVectorXMap.Unbind();
    FGeoFuncs.Unbind();
}
catch(...)
{
}
}

void __fastcall TfmMap::reload()
{
    TCursor oldCursor = Cursor;
    Cursor = crHourGlass;
    try {
        if (FConnected) {
            unload();
        }
        // создать и получить обьект блока управления
        try {
            FrsCustomXMap.CreateInstance(IID_IrsMapXXMap);
            FrsCustomXMap->QueryInterface(IID_IrsVectorXMap1, (void**)&FVectorXMap);
        } catch (Exception &e) {
               Application->MessageBox((AnsiString("Помилка створення об'єкту блоку керування картою:\n") + e.Message).c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }
        // получить интерфейс преобразования координат
        try {
            FrsCustomXMap->Get_MapTransform(&FCoordTrans);
            FCoordTrans.AddRef();
            FCoordTrans->QueryInterface(IID_IrsAdvanceCoordTransformMap, (void**)&FAdvCoordTrans);
            if (!FAdvCoordTrans) {
                throw *(new Exception("Помилка завантаження інтерфейсу розширеного перетворення координат"));
            }
        } catch (Exception &e) {
               Application->MessageBox((AnsiString("Помилка завантаження інтерфейсу перетворення координат:\n") + e.Message).c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

        // получить интерфейс географических функций
        try {
            FrsCustomXMap->Get_GeoFuncs(&FGeoFuncs);
            FGeoFuncs.AddRef();
        } catch (Exception &e) {
               Application->MessageBox((AnsiString("Помилка завантаження інтерфейсу географічних функцій:\n") + e.Message).c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

        // Загрузить слои из файла в карту
        try {
            FVectorXMap->LoadFromFile(wsMapLayersFile);
        } catch (Exception &e) {
            Application->MessageBox((AnsiString("Помилка завантаження файлу шарів:\n")+ wsMapLayersFile + e.Message).c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

        // Получить список обьктов карты
        try {
            FrsCustomXMap->Get_MapObjectList(&FMapObjectList);
            FMapObjectList.AddRef();
        } catch (Exception &e) {
            Application->MessageBox((AnsiString("Помилка отримання об'єктів карти:\n") + e.Message).c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

        theMap->Visible = false;
        // Связать блок управления картой с экраном отображения
        theMap->MapSource = FrsCustomXMap;

        // Connect карты
        try {
            FrsCustomXMap->Set_Connect(True);
        } catch (Exception &e) {
            #ifndef _DEBUG
            Application->MessageBox(AnsiString("Помилка підключення карти:\n" + e.Message).c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
            #endif
        }

        // Active карты

        FConnected = true;
        // Установить текущий масштаб
        FrsCustomXMap->Set_GeoCenterMap(CentrePos);
        if ( Scale > 50000 )
            Scale = 50000;
        FrsCustomXMap->Set_Scale(Scale);

        theMap->Visible = true;

        sbMap->Panels->Items[2]->Text = FormatFloat("0.00",Scale);
        sbMap->Panels->Items[3]->Text = ExtractFileName(wsMapLayersFile);

        FrsCustomXMap->Set_Active(True);

        theMap->LeftMouseMode = mmNo;
        theMap->Cursor = crGetTx;

        IrsMapToolUsedPtr ptrMapTool;
        theMap->ControlInterface->QueryInterface(IID_IrsMapToolUsed, (void**)&ptrMapTool);
        if (ptrMapTool)
            ptrMapTool->CreateCustomTool(MT_GET_RELIEF, Rscontextmaps_tlb::ttToolTypeLine, 0, 0, 0);

        theMap->ControlInterface->QueryInterface(IID_IrsSelectMouseMode, (void**)&m_mouseSelectMode);
        if (m_mouseSelectMode)
            m_mouseSelectMode->RegionMode = rmRectangle;

    } __finally {
        Cursor = oldCursor;
    }
}
//---------------------------------------------------------------------------
HANDLE GrsMapLayerDllHandle;
typedef int (__stdcall *TExecMapLayer) (IrsVectorXMap* AIVectorXMap, HANDLE parentWindow) ;
TExecMapLayer ExecMapLayer;

void __fastcall TfmMap::configureLayers()
{
    if (FConnected) {
        //  Загрузить Dll для работы со слоями
        if (GrsMapLayerDllHandle <= 0)
            GrsMapLayerDllHandle = LoadLibrary(szMapLayerLib);
        if (GrsMapLayerDllHandle > 0) {
            if (!(ExecMapLayer = (TExecMapLayer)GetProcAddress(GrsMapLayerDllHandle, szMapLayerFunc)))
                throw *(new Exception(AnsiString().sprintf(szCANNOTLOADFUNC, szMapLayerFunc, szMapLayerLib)));

            HANDLE hnd = Parent ? Parent->Handle : NULL;
            if (ExecMapLayer(FVectorXMap, hnd)) {
                // Сохранить слои в файл
                FVectorXMap->SaveToFile(wsMapLayersFile);
                // Переинициализировать карту
                FVectorXMap->InvalidateLayers();
            }
         }
    } else throw *(new Exception(AnsiString().sprintf(szCANNOTLOADLIB,szMapLayerLib)));
}
//---------------------------------------------------------------------------

IrsMapObj* __fastcall TfmMap::drawPoint(double Long, double Lat, int Size, TColor LColor, AnsiString Name, AnsiString Hint, char symbol, int id)
{
    IrsMapObjPtr ptrMapObject;
    FMapObjectList->CreateMapObject(tpPoint, &ptrMapObject);
    if (ptrMapObject)
    {
        IrsPointMapObjPtr ptrPointMapObj;
        ptrMapObject->QueryInterface(IID_IrsPointMapObj, (void**)&ptrPointMapObj);
        if (ptrPointMapObj)
        {
            ptrMapObject->Set_ID(id);

            TrsGeoPoint geo_coord;
            geo_coord.gpH = Lat; geo_coord.gpL = Long;
            ptrPointMapObj->InitPointGeo(geo_coord, LColor, Size);

            if (minLon = 0.0 || minLon > Long) minLon = Long;
            if (minLat = 0.0 || minLat > Lat)  minLat = Lat;
            if (maxLon = 0.0 || maxLon < Long) maxLon = Long;
            if (maxLat = 0.0 || maxLat < Lat)  maxLat = Lat;

            if (!Hint.IsEmpty())
                setHint(ptrMapObject, Hint);

            if (!Name.IsEmpty())
                if ( BCCalcParams.ShowTxNames )
                    setLabel(ptrMapObject, Name + " " + Hint);
                else
                    setLabel(ptrMapObject, Name);

            if (symbol)
            {
                IrsSymbolPtr pSymbol;
                ptrMapObject->QueryInterface(IID_IrsSymbol, (void**)&pSymbol);
                if (pSymbol.IsBound()) {
                    pSymbol->Set_Symbol(symbol);
                    std::auto_ptr<TFont> symbolFont(new TFont());
                    symbolFont->Size = Size;
                    symbolFont->Name = "Map Symbols";//"Courier New";
                    symbolFont->Style = symbolFont->Style << fsBold;

                    _di_IFontDisp OleFont;

                    GetOleFont(symbolFont.get(), OleFont);
                    if (OleFont) {
                        IrsFontPtr pFont;
                        pSymbol->QueryInterface(IID_IrsFont, (void**)&pFont);
                        pFont->Set_Font(OleFont);
                    }
                }
            }

            ptrPointMapObj->Set_Visible(true);
            ptrPointMapObj->Invalidate();
        }
    }
    return ptrMapObject;
}

double smplArray[36];
double smplL;
double smplH;

IrsMapObj* __fastcall TfmMap::drawContour(std::vector<double>& coords, WideString name, TColor LColor, int LineWeight, int id)
{
    if (coords.size() < 2)
        return NULL;

    IrsMapObjPtr ptrMapObject;
    FMapObjectList->CreateMapObject(tpPolyLine, &ptrMapObject);
    if (ptrMapObject) {
        ptrMapObject->Set_ID(id);

        IrsPolyLineMapObjPtr ptrPolygonMapObj;
        ptrMapObject->QueryInterface(IID_IrsPolyLineMapObj, (void **)&ptrPolygonMapObj);

        if (ptrPolygonMapObj) {

            int numberOfPoints = coords.size() / 2;
            ptrPolygonMapObj->Set_Count(numberOfPoints);

            //AnsiString msg("  Lon              Lat\n-----------------\n");

            for (int i = 0; i < numberOfPoints; i++) {

                TrsGeoPoint geo_coord = {coords[i*2], coords[i*2+1]};
                //geo_coord.gpL = rezGeoPoint.L; geo_coord.gpH = rezGeoPoint.H;
                //msg += ("\n"+dmMain->coordToStr(coords[i*2],'X') + "   "+dmMain->coordToStr(coords[i*2+1],'Y'));

                ptrPolygonMapObj->Set_Coord(i, geo_coord);

                if (minLon == 0.0 || minLon > geo_coord.gpL)
                    minLon = geo_coord.gpL;
                if (minLat == 0.0 || minLat > geo_coord.gpH)
                    minLat = geo_coord.gpH;
                if (maxLon == 0.0 || maxLon < geo_coord.gpL)
                    maxLon = geo_coord.gpL;
                if (maxLat == 0.0 || maxLat < geo_coord.gpH)
                    maxLat = geo_coord.gpH;

            }

            //ShowMessage(msg);
            
            ptrPolygonMapObj->Set_DefColor(LColor);
            ptrPolygonMapObj->Set_LineWidth(LineWeight);
            ptrPolygonMapObj->Set_DrawStyle(ldsSolid);
            ptrPolygonMapObj->Set_ConnStyle(lcsLoop);

            //if (!name.IsEmpty())
            //    setLabel(ptrMapObject, name);

            ptrPolygonMapObj->Set_Bland(0);
            ptrPolygonMapObj->Invalidate();

        }
    }
    return ptrMapObject;
}

IrsMapObj* __fastcall TfmMap::drawPoligon(double Long, double Lat, double* InArray, TColor LColor, int LineWeight, int numberOfPoints, int id)
{
    IrsMapObjPtr ptrMapObject;
    FMapObjectList->CreateMapObject(tpPolyLine, &ptrMapObject);

    if (ptrMapObject) {
        ptrMapObject->Set_ID(id);

        IrsPolyLineMapObjPtr ptrPolygonMapObj;
        ptrMapObject->QueryInterface(IID_IrsPolyLineMapObj, (void **)&ptrPolygonMapObj);

        if (ptrPolygonMapObj) {
            TRSAGeoPoint rezGeoPoint, centreGeoPoint;
            centreGeoPoint.H = Lat; centreGeoPoint.L = Long;
            ptrPolygonMapObj->Set_Count(numberOfPoints);

            for (int i = 0; i < numberOfPoints; i++) {
                FSpherics->PolarToGeo(InArray[i], i * 360 / numberOfPoints, centreGeoPoint, &rezGeoPoint);
                TrsGeoPoint geo_coord;
                geo_coord.gpL = rezGeoPoint.L; geo_coord.gpH = rezGeoPoint.H;

                ptrPolygonMapObj->Set_Coord(i, geo_coord);

                if (minLon == 0.0 || minLon > geo_coord.gpL)
                    minLon = geo_coord.gpL;
                if (minLat == 0.0 || minLat > geo_coord.gpH)
                    minLat = geo_coord.gpH;
                if (maxLon == 0.0 || maxLon < geo_coord.gpL)
                    maxLon = geo_coord.gpL;
                if (maxLat == 0.0 || maxLat < geo_coord.gpH)
                    maxLat = geo_coord.gpH;

            }
            ptrPolygonMapObj->Set_DefColor(LColor);
            ptrPolygonMapObj->Set_LineWidth(LineWeight);
            ptrPolygonMapObj->Set_DrawStyle(ldsSolid);
            ptrPolygonMapObj->Set_ConnStyle(lcsLoop);

            ptrPolygonMapObj->Set_Bland(0);
            ptrPolygonMapObj->Invalidate();

            /*
            if (chbSaveZone->Checked && Application->MessageBox("Сохранить зону?", "Вопрос", MB_YESNO | MB_ICONQUESTION) == IDYES) {
                smplL = Long;
                smplH = Lat;
                for (int i = 0; i < numberOfPoints; i++)
                    smplArray[i] = InArray[i];
            }
            */
        }
    }
    return ptrMapObject;
}

IrsMapObj* __fastcall TfmMap::drawCircle(double Long, double Lat, int Radius, TColor LColor, int id)
{
    IrsMapObjPtr            ptrMapObject;
    FMapObjectList->CreateMapObject(tpEllipse, &ptrMapObject);
    if (ptrMapObject) {
        ptrMapObject->Set_ID(id);

        IrsAdvanceRectangleMapObjPtr ptrEllipseMapObj;
        ptrMapObject->QueryInterface(IID_IrsAdvanceRectangleMapObj, (void**)&ptrEllipseMapObj);

        if (ptrEllipseMapObj) {
            TRSAGeoPoint rezGeoPoint,rezGeoPoint2, centreGeoPoint;
            centreGeoPoint.H = Lat; centreGeoPoint.L = Long;

            FSpherics->PolarToGeo(Radius, 225.0, centreGeoPoint, &rezGeoPoint);
            FSpherics->PolarToGeo(Radius, 45.0, centreGeoPoint, &rezGeoPoint2);

            TrsGeoPoint geo_coord, geo_coord2;

            geo_coord.gpH = rezGeoPoint.H; geo_coord.gpL = rezGeoPoint.L;
            geo_coord2.gpH = rezGeoPoint2.H; geo_coord2.gpL = rezGeoPoint2.L;

            ptrEllipseMapObj->Set_RectGeo(geo_coord, geo_coord2);
            ptrEllipseMapObj->Set_Width(4);
            ptrEllipseMapObj->Set_Color(LColor);

            ptrEllipseMapObj->Set_Visible(true);
            ptrEllipseMapObj->Invalidate();
            ptrEllipseMapObj.Unbind();

            if (minLon > geo_coord.gpL) minLon = geo_coord.gpL;
            if (minLat > geo_coord.gpH) minLat = geo_coord.gpH;
            if (maxLon < geo_coord.gpL) maxLon = geo_coord.gpL;
            if (maxLat < geo_coord.gpH) maxLat = geo_coord.gpH;

            if (minLon > geo_coord2.gpL) minLon = geo_coord2.gpL;
            if (minLat > geo_coord2.gpH) minLat = geo_coord2.gpH;
            if (maxLon < geo_coord2.gpL) maxLon = geo_coord2.gpL;
            if (maxLat < geo_coord2.gpH) maxLat = geo_coord2.gpH;

        }
    }
    return ptrMapObject;
}


IrsMapObj* __fastcall TfmMap::drawLine(double Long, double Lat, double Long2, double Lat2, int Size, TColor LColor, int id)
{
    IrsMapObjPtr ptrMapObject;
    FMapObjectList->CreateMapObject(tpLine, &ptrMapObject);
    if (ptrMapObject) {
        ptrMapObject->Set_ID(id);

        IrsLineMapObjPtr    ptrLineMapObj;
        ptrMapObject->QueryInterface(IID_IrsLineMapObj, (void**)&ptrLineMapObj);

        if (ptrLineMapObj) {
            TrsGeoPoint geo_beg;
            TrsGeoPoint geo_end;
            geo_beg.gpH = Lat; geo_beg.gpL = Long;
            geo_end.gpH = Lat2; geo_end.gpL = Long2;

            ptrLineMapObj->InitLineGeo(geo_beg, geo_end, LColor, Size);
            ptrLineMapObj->Invalidate();

            if (minLon == 0.0 || minLon > Long) minLon = Long;
            if (minLat == 0.0 || minLat > Lat)  minLat = Lat;
            if (maxLon == 0.0 || maxLon < Long) maxLon = Long;
            if (maxLat == 0.0 || maxLat < Lat)  maxLat = Lat;

            if (minLon > Long2) minLon = Long2;
            if (minLat > Lat2)  minLat = Lat2;
            if (maxLon < Long2) maxLon = Long2;
            if (maxLat < Lat2)  maxLat = Lat2;

        }
    }
    return ptrMapObject;
}


IrsMapObj* __fastcall TfmMap::drawZone(double *array, bool gradient, double eMin, int id)
{
    //double *data = array + 6;
    double maxE = MINDOUBLE, minE = MAXDOUBLE;
    for(int i = 0; i <  ((int)array[4])*((int)array[7]); i++) {
        if (array[8+i] > maxE)
            maxE = array[8+i];
        if (array[8+i] > 0 && array[8+i] < minE)
            minE = array[8+i];
    }

    //double color_koef = maxE -= minE;
    double LongBeg, LatBeg, LongEnd, LatEnd;
    TRSAGeoPoint rezGeoPoint, centerGeoPoint;
    TRSAGeoPoint point1, point2, point3, point4;

    //  форма с прогресс-ьаром и кнопкой отмены
    std::auto_ptr<TForm> frmProgress(new TForm(Application));
    frmProgress->Width = 250;
    frmProgress->Height = 90;
    frmProgress->BorderStyle = bsDialog;
    frmProgress->Position = poMainFormCenter;
    frmProgress->OnClose = FormClose;
    frmProgress->Caption = "Отрисовка зоны...";
    frmProgress->FormStyle = fsStayOnTop;

    TProgressBar *pb = new TProgressBar(frmProgress.get());
    pb->Parent = frmProgress.get();
    pb->Step = 1;
    pb->Top = 20;
    pb->Left = 4;
    pb->Width = frmProgress->ClientWidth - pb->Left * 2;

    TButton *btn = new TButton(frmProgress.get());
    btn->Parent = frmProgress.get();
    btn->Top = 40;
    btn->Height = 20;
    btn->Left = (frmProgress->ClientWidth - btn->Width) / 2;
    btn->Caption = "Отменить";
    btn->OnClick = btnCloseClick;

    TLabel *lbl = new TLabel(frmProgress.get());
    lbl->Parent = frmProgress.get();
    lbl->Left = 8;
    lbl->Top = 4;
    lbl->Caption = "...";

    frmProgress->Visible = true;
    Application->ProcessMessages();

    centerGeoPoint.L = *(array); centerGeoPoint.H = *(array + 1);

    cancelDrawing = false;
    pb->Max = (int)array[4] * (int)array[7];

    for (int azimuth = 0; azimuth < (int)array[4]; azimuth ++) {
        int radius = 0, radiusBeg = 0;
        while( radius < (int)array[7] )
        {
            pb->StepIt();
            lbl->Caption = "["+AnsiString(azimuth)+":"+AnsiString(radius) + "] "
                            "з [" + AnsiString((int)array[4])+":"+AnsiString((int)array[7])+"]  ";
                            //"("+(int)+"%)";
            Application->ProcessMessages();
            if (cancelDrawing)
                break;

            //пропускаем зону с "нулевой" напряженностью
            while( (radius < (int)array[7]) && (array[8 + (int)array[7] * azimuth + radius]<eMin) )
            {
                radius++;
                pb->StepIt();
                lbl->Caption = "["+AnsiString(azimuth)+":"+AnsiString(radius) + "] "
                                "з [" + AnsiString((int)array[4])+":"+AnsiString((int)array[7])+"]  ";
                                //"("+(int)+"%)";
                Application->ProcessMessages();
            }

            radiusBeg = radius;

            //проходим зону с "еденичной" напряженностью или доходим до конца тестируемого отрезка
            while( (radius < (int)array[7]) && (array[8 + (int)array[7] * azimuth + radius]>eMin) )
            {
                radius++;
                pb->StepIt();
                lbl->Caption = "["+AnsiString(azimuth)+":"+AnsiString(radius) + "] "
                                "з [" + AnsiString((int)array[4])+":"+AnsiString((int)array[7])+"]  ";
                                //"("+(int)+"%)";
                Application->ProcessMessages();
            }
            radius--;//мы за пределами сектора

            //пошла отрисовка
            int pos = 8 + (int)array[7] * azimuth + radius;
            if ( !( (radius+1 == radiusBeg) && (radius+1 == (int)array[7]) ) )
            {
                //  нарисовать вокруг точки трапецию
                //  array[2] - стартовая позиция азимута
                //  array[3] - шаг азимута
                //  array[5] - стартовая позиция радиуса
                //  array[6] - шаг радиуса
                FSpherics->PolarToGeo(array[5] + (array[6]* radiusBeg) - array[6] / 2,
                                      array[2] + array[3] * azimuth - array[3] / 2, centerGeoPoint, &point1);
                FSpherics->PolarToGeo(array[5] + (array[6]* radius) + array[6] / 2,
                                      array[2] + array[3] * azimuth - array[3] / 2, centerGeoPoint, &point2);
                FSpherics->PolarToGeo(array[5] + (array[6]* radius) + array[6] / 2,
                                      array[2] + array[3] * azimuth + array[3] / 2, centerGeoPoint, &point3);
                FSpherics->PolarToGeo(array[5] + (array[6]* radiusBeg) - array[6] / 2,
                                      array[2] + array[3] * azimuth + array[3] / 2, centerGeoPoint, &point4);

                int LineColor = (gradient) ?
                                (255.0 * array[pos] / maxE) :
                                (255.0);
                                /*
                                (255.0 * (array[pos] - minE) / maxE) :
                                (255.0 * (maxE - minE) / maxE);
                                */
                                //(200);

                IrsMapObjPtr ptrMapObject;
                FMapObjectList->CreateMapObject(tpPolygon, &ptrMapObject);

                if (ptrMapObject) {
                    ptrMapObject->Set_ID(id);

                    IrsPolygonMapObjPtr ptrPolygonMapObj;
                    ptrMapObject->QueryInterface(IID_IrsPolygonMapObj, (void **)&ptrPolygonMapObj);

                    if (ptrPolygonMapObj) {
                        ptrPolygonMapObj->Set_Count(4);

                        TrsGeoPoint geoCoord;

                        geoCoord.gpL = point1.L; geoCoord.gpH = point1.H;
                        ptrPolygonMapObj->Set_Coord(0, geoCoord);

                        geoCoord.gpL = point2.L; geoCoord.gpH = point2.H;
                        ptrPolygonMapObj->Set_Coord(1, geoCoord);

                        geoCoord.gpL = point3.L; geoCoord.gpH = point3.H;
                        ptrPolygonMapObj->Set_Coord(2, geoCoord);

                        geoCoord.gpL = point4.L; geoCoord.gpH = point4.H;
                        ptrPolygonMapObj->Set_Coord(3, geoCoord);

                        ptrPolygonMapObj->Set_DefColor(RGB(255, 255 - LineColor, 255 - LineColor));

                        if (gradient) {
                            ptrPolygonMapObj->Set_Bland(255);
                        } else {
                            ptrPolygonMapObj->Set_Bland(100);
                        }

                        ptrPolygonMapObj->Invalidate();
                    }
                }
            }
        radius ++;
        }
        if (cancelDrawing)
            break;
    }
    return NULL;
}

void __fastcall TfmMap::saveToBmp()
{
    double Scale;
    TrsGeoPoint Centre;
    FrsCustomXMap->Get_GeoCenterMap(&Centre);
    FrsCustomXMap->Get_Scale(&Scale);
    Quality = 0;
    PrintFileName = "";
/*
    if (!dlgPrintMap)
        dlgPrintMap = new TdlgPrintMap(this);
*/
    std::auto_ptr<TdlgPrintMap> dlgPrintMap(new TdlgPrintMap(this));
    dlgPrintMap->ShowModal();
    if (PrintFileName != "") {
        theMap->Visible = false;
        theMap->Align = alNone;
        theMap->Width  = (theMap->Width) * Quality;
        theMap->Height = (theMap->Height) * Quality;
        FrsCustomXMap->Set_Scale(Scale);
        FrsCustomXMap->Set_GeoCenterMap(Centre);
        try {
            FVectorXMap->DrawToBitmap(WideString(PrintFileName), 0, 0, theMap->Width, theMap->Height);
        } catch (Exception &e) {
            Application->MessageBox("Помилка збереження файлу", Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }
        theMap->Align = alClient ;
        FrsCustomXMap->Set_Scale(Scale);
        FrsCustomXMap->Set_GeoCenterMap(Centre);
        theMap->Visible = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::centerMap(double Long, double Lat)
{
    if (connected) {
        CentrePos.gpL = Long;
        CentrePos.gpH = Lat;
        FrsCustomXMap->Set_GeoCenterMap(CentrePos);
    }
}


void __fastcall TfmMap::FormShow(TObject *Sender)
{
    try {
        //LayoutManager.loadLayout(this);
    } catch(...) {};
    if (theSelection != NULL) {
        //frmMap = this;
        //theSelection->drawTxs();
        //theSelection->drawCoverage();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::deleteObjectById(int id)
{
    if (!connected)
        return;
    IrsMapObjPtr mapObject;
    FMapObjectList->FindByID(id, &(mapObject));
    if (mapObject) {
        mapObject->Delete();
        mapObject->Invalidate();
        mapObject.Unbind();
    }
}

void __fastcall TfmMap::deleteObjects()
{
    Cursor = crHourGlass;
    sbMap->SimpleText = "Видалення об'єктів з карти...";
    sbMap->SimplePanel = true;
    sbMap->Update();

    try {
        if (FSelMapObj[0].IsBound()) {
            FSelMapObj[0]->Delete();
            FSelMapObj[0].Unbind();
        }
        if (FSelMapObj[1].IsBound()) {
            FSelMapObj[1]->Delete();
            FSelMapObj[1].Unbind();
        }
        if (FSelMapObj[2].IsBound()) {
            FSelMapObj[2]->Delete();
            FSelMapObj[2].Unbind();
        }

        if (FEPointObj.IsBound()) {
            FEPointObj->Delete();
            FEPointObj.Unbind();
        }
        if (FLastCoordZone.IsBound()) {
            FLastCoordZone->Delete();
            FLastCoordZone.Unbind();
        }
        /*
        if (FDuelAxe.IsBound()) {
            FDuelAxe->Delete();
            FDuelAxe.Unbind();
        }
        */
        for (int i = 0; i < 360; i++) {
            if (checkPoints[i].IsBound()) {
                checkPoints[i]->Delete();
                checkPoints[i].Unbind();
            }
        }

        minLat = minLon = maxLat = maxLon = 0.0;

        FMapObjectList->Clear(tpMapObject);
        FrsCustomXMap->Repaint();
        
        //coordinationPointsDelete();
        coordinationPoints.clear();

    } __finally {

        Cursor = crDefault;
        sbMap->SimplePanel = false;

    }
}

void __fastcall TfmMap::setScale(double scale)
{
    Scale = scale;
    if ( Scale > 50000 )
        Scale = 50000;
    if (connected) {
        FrsCustomXMap->Set_Scale(Scale);
    }
}

void __fastcall TfmMap::setHint(IrsMapObj* mapObject, AnsiString& value)
{
    if (!connected)
        return;
    IrsHintMapObjPtr ptrPointHint;
    mapObject->QueryInterface(IID_IrsHintMapObj, (void**)&ptrPointHint);
    if (ptrPointHint) {
        ptrPointHint->Set_Hint(WideString(value));
    }
}

void __fastcall TfmMap::setLabel(IrsMapObj* mapObject, AnsiString& value)
{
    if (!connected)
        return;
    IrsLabelMapObjPtr ptrPointLabel;
    mapObject->QueryInterface(IID_IrsLabelMapObj, (void**)&ptrPointLabel);
    if (ptrPointLabel) {
        // подпись
        ptrPointLabel->Set_Label(WideString(value));
        ptrPointLabel->Set_Color(clBlack);
        IrsLabelPtr ptrLabel;
        ptrPointLabel->QueryInterface(IID_IrsLabel, (void**)&ptrLabel);
        if (ptrLabel) {
            // Шрифт текста (можно его не устанавливать)
            try {
                //_di_IFontDisp iFontDisp;
                //theMap->Font->Color = clWindowText;
                //GetOleFont(theMap->Font, iFontDisp);
                //iLabel->Set_Font(iFontDisp);
            } catch (...) {}
            // если 0 то цвет фона на котором написан текст будет желтым
            // если 255 - прозрачным
            ptrLabel->Set_BlandBackGround(255);
        }
    }
}

void __fastcall TfmMap::setLabelById(int id, AnsiString& value)
{
    if (!connected)
        return;
    IrsMapObjPtr LMapObject;
    FMapObjectList->FindByID(id, &(LMapObject));
    if (LMapObject) {
        setLabel(LMapObject, value);
        LMapObject.Unbind();
    }
}                

void __fastcall TfmMap::setColor(IrsMapObj* mapObject, TColor newColor)
{
    if (!connected)
        return;
    IrsColorMapObj *ptrColor;
    mapObject->QueryInterface(IID_IrsColorMapObj, (void**)&ptrColor);
    if (ptrColor) {
        ptrColor->Set_Color(newColor);
        ptrColor->Invalidate();
    } else {
        //  может это полилиния
        IrsPolyLineMapObj *ptrPoligon;
        mapObject->QueryInterface(IID_IrsPolyLineMapObj, (void**)&ptrPoligon);
        if (ptrPoligon) {
            ptrPoligon->Set_DefColor(newColor);
            ptrPoligon->Invalidate();
        }
    }
}

void __fastcall TfmMap::setColorById(int id, TColor newColor)
{
    if (!connected)
        return;
    IrsMapObj *ptrMapObject = NULL;
    FMapObjectList->FindByID(id, &ptrMapObject);
    if (ptrMapObject) 
        setColor(ptrMapObject, newColor);
}

IrsMapObj* __fastcall TfmMap::drawEPoint(double Long, double Lat, int Id, TColor color, AnsiString Name, AnsiString Hint, int id)
{
    if (FEPointObj.IsBound()) {
        //FEPointObj->Delete();
        setVisible((IrsMapObj*)FEPointObj, false);
    }
    FEPointObj.Bind(drawPoint(Long, Lat, 7, color, Name, Hint, 'X', id), true);
    return FEPointObj;
}

void __fastcall TfmMap::drawCoordZone(double lon, double lat, double * zone)
{
    if (FLastCoordZone.IsBound())
        FLastCoordZone->Delete();

    FLastCoordZone.Bind(drawPoligon(lon, lat, zone, ccZoneCoord, 2), true);
}

void __fastcall TfmMap::fitAllObjects()
{
    //  отцентрируем карту
    centerMap((minLon + maxLon) / 2, (minLat + maxLat) / 2);

    double scaleLat = 200;
    double scaleLon = 200;

    TRSAGeoPoint point1, point2;
    point1.H = minLat; point1.L = (minLon + maxLon) / 2;
    point2.H = maxLat; point2.L = (minLon + maxLon) / 2;
    FSpherics->Distance(point1, point2, &scaleLat);

    point1.H = (minLat + maxLat) / 2; point1.L = minLon;
    point2.H = (minLat + maxLat) / 2; point2.L = maxLon;
    FSpherics->Distance(point1, point2, &scaleLon);

    /*  по 50 миль со всех сторон накинем */
    scaleLat += 200;
    scaleLon += 200;

    if (scaleLon > 0) {
        /* масштаб указывает размер изображения карты по долготе в милях */
        double scale = scaleLon;
        /* если при этом все передатчики не пместятся по широте, то масштаб нужно скорректировать */
        if (scaleLat / scaleLon > theMap->Height * 1.0 / theMap->Width)
            scale = scaleLat / (theMap->Height * 1.0 / theMap->Width);
        if (scale < 200) scale = 200;
        setScale(scale);
    } else {
        setScale(200);
    }
}

void __fastcall TfmMap::drawDuelAxe(const TRSAGeoPoint& pointA, const TRSAGeoPoint& pointB, const TRSAGeoPoint& point1, const TRSAGeoPoint& point2)
{
    /*
    if (FDuelAxe.IsBound()) {
        FDuelAxe->Delete();
        //setVisible((IrsMapObj*)FDuelAxeM, false);
        //FDuelAxe.Unbind();
    }
    */


    IrsMapObjPtr ptrMapObject;
    FMapObjectList->CreateMapObject(tpPolyLine, &ptrMapObject);

    if (ptrMapObject) {
        ptrMapObject->Set_ID(0);

        IrsPolyLineMapObjPtr ptrPolygonMapObj;
        ptrMapObject->QueryInterface(IID_IrsPolyLineMapObj, (void **)&ptrPolygonMapObj);

        if (ptrPolygonMapObj) {
            ptrPolygonMapObj->Set_Count(4);

            TrsGeoPoint point;

            point.gpL = point1.L;
            point.gpH = point1.H;
            ptrPolygonMapObj->Set_Coord(0, point);

            point.gpL = pointA.L;
            point.gpH = pointA.H;
            ptrPolygonMapObj->Set_Coord(1, point);

            point.gpL = pointB.L;
            point.gpH = pointB.H;
            ptrPolygonMapObj->Set_Coord(2, point);

            point.gpL = point2.L;
            point.gpH = point2.H;
            ptrPolygonMapObj->Set_Coord(3, point);

            ptrPolygonMapObj->Set_DefColor(ccDuelAxe);
            ptrPolygonMapObj->Set_LineWidth(1);
            ptrPolygonMapObj->Set_DrawStyle(ldsSolid);
            ptrPolygonMapObj->Set_ConnStyle(lcsLines);

            ptrPolygonMapObj->Set_Bland(100);
            ptrPolygonMapObj->Invalidate();

            /*
            FDuelAxe.Bind(ptrPolygonMapObj);
            */

        }
    } 
}

void __fastcall TfmMap::setVisible(long id, bool visible)
{
    IrsMapObj *ptrMapObject;
    FMapObjectList->FindByID(id, &ptrMapObject);
    if (ptrMapObject)
        setVisible(ptrMapObject, visible);
}

void __fastcall TfmMap::setVisible(IrsMapObj* pMo, bool visible)
{
    if (pMo) {
        pMo->Set_Visible(visible);
        pMo->Invalidate();
    }
}

void __fastcall TfmMap::btnCloseClick(TObject *Sender)
{
    cancelDrawing = true;
    TForm *frm = dynamic_cast<TForm*>(Sender);
    if (frm)
        frm->Close();
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::theMapSelectedZone(TObject *Sender, long ABegX,
      long ABegY, long AEndX, long AEndY)
{
    zoneBeg.ppX = ABegX;
    zoneBeg.ppY = ABegY;
    zoneEnd.ppX = AEndX;
    zoneEnd.ppY = AEndY;
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::showCheckPoints(LPSAFEARRAY psa, int txType)
{
    deleteCheckPoints();

    TSafeArrayT<TVariant, VT_VARIANT, 2> saCp;
    saCp.Attach(psa);
    long num = saCp.get_BoundsLength(0);

    //  ищем точку с максимальной разницей напряженностей помехи
    //  она будет выделена жирно
    int maxDifIdx = 0;
    double dif, maxDif;

    for (int i = 0; i < num; i++)
    {
        dif = (double)(TVariant)saCp[i][cpdiEu2] - (double)(TVariant)saCp[i][cpdiEu1];
        if (i == 0 || maxDif < dif)
        {
            maxDifIdx = i;
            maxDif = dif;
        }
    }

    //  рисуем каждую точку
    for (int i = 0; i < num; i++) {

        AnsiString hint;

        //  coord
        double longitude = (double)(TVariant)saCp[i][cpdiLongitude];
        double latitude = (double)(TVariant)saCp[i][cpdiLatitude];

        double eu1 = (double)(TVariant)saCp[i][cpdiEu1];
        double eu2 = (double)(TVariant)saCp[i][cpdiEu2];

        AnsiString name = (wchar_t*)(TVariant)saCp[i][cpdiStandName];
        AnsiString regnum = (wchar_t*)(TVariant)saCp[i][cpdiNumReg];
        AnsiString txnum = (wchar_t*)(TVariant)saCp[i][cpdiTxNum];

        int color = ((TBCTxType)txType == ttTV || (TBCTxType)txType == ttDVB) ?
                ((eu2 - eu1 > BCCalcParams.treshVideo) ? clRed : clBlue) :
                ((eu2 - eu1 > BCCalcParams.treshAudio) ? clRed : clBlue);

        int weight = i == maxDifIdx ? 9 : 5;
        char symbol = '\0';

        hint = AnsiString().sprintf("Азімут, град = %.1f\n"
                                    "Відстань, км = %.1f\n"
                                    "ЕВП (без) = %.2f дбкВт\n"
                                    "ЕВП (зав) = %.2f дбкВт\n"
                                    "Е  1%% = %.2f дБмкВ/м\n"
                                    "Е 10%% = %.2f дБмкВ/м\n"
                                    "Е 50%% = %.2f дБмкВ/м\n"
                                    "ЗО c = %.1f дБ\n"
                                    "ЗО t = %.1f дБ\n"
                                    "D ант = %.1f дБ\n"
                                    "D ант орт = %.1f дБ\n"
                                    "Опора '%s'\n"
                                    "Регіон, №Tx '%s %s'\n"
                                    "Довгота = %s\n"
                                    "Широта = %s\n"
                                    "Нефф, м = %d\n"
                                    "Пол = '%c'"
                                     , (double)(TVariant)saCp[i][cpdiAzimuth]
                                     , (double)(TVariant)saCp[i][cpdiDist]
                                     , eu1
                                     , eu2
                                     , (double)(TVariant)saCp[i][cpdiE01]
                                     , (double)(TVariant)saCp[i][cpdiE10]
                                     , (double)(TVariant)saCp[i][cpdiE50]
                                     , (double)(TVariant)saCp[i][cpdiPrC]
                                     , (double)(TVariant)saCp[i][cpdiPrT]
                                     , (double)(TVariant)saCp[i][cpdiDa]
                                     , (double)(TVariant)saCp[i][cpdiDaO]
                                     , name.c_str()
                                     , regnum.c_str(), txnum.c_str()
                                     , dmMain->coordToStr(longitude, 'X').c_str()
                                     , dmMain->coordToStr(latitude, 'Y').c_str()
                                     , (long)(TVariant)saCp[i][cpdiHeff]
                                     , (char)(TVariant)saCp[i][cpdiPol]
                                    );

        checkPoints[i].Bind(drawPoint(longitude, latitude, weight, color, AnsiString(), hint, symbol, 0), true);
    }
    saCp.Detach();
}

void __fastcall TfmMap::deleteCheckPoints()
{
    for (int i = 0; i < 360; i++) {
        if (checkPoints[i].IsBound()) {
            checkPoints[i]->Delete();
            checkPoints[i].Unbind();
        }
    }
}

void __fastcall TfmMap::setObjColor(IrsMapObjPtr &mapObj, long value)
{
    if (mapObj.IsBound())
    {
        IrsColorMapObjPtr ptrColorMapObj;
        mapObj->QueryInterface(IID_IrsColorMapObj, (void**)&ptrColorMapObj);
        if (ptrColorMapObj.IsBound())
        {
            ptrColorMapObj->Set_Color(value);
            ptrColorMapObj->Invalidate();
            ptrColorMapObj.Unbind();
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::coordinationPointsDelete()
{
    if ( !coordinationPoints.empty() )
    {
        for ( std::vector<IrsMapObj*>::iterator coordinationPoint = coordinationPoints.begin(); coordinationPoint != coordinationPoints.end(); coordinationPoint++ )
           if ( (*coordinationPoint) )
                setVisible((IrsMapObj*)(*coordinationPoint), false);

        coordinationPoints.clear();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmMap::coordinationPointsShow(std::vector<CoordinationPoint>& cPoints)
{
    if ( !coordinationPoints.empty() )
        coordinationPointsDelete();

    int cordinationPointIndex = 0;
    for ( std::vector<CoordinationPoint>::const_iterator cordinationPoint = cPoints.begin(); cordinationPoint != cPoints.end(); cordinationPoint++, cordinationPointIndex++ )
    {
        if ( cordinationPoint->inZone )
            coordinationPoints.push_back(drawPoint(cordinationPoint->point.L, cordinationPoint->point.H, 10, BCCalcParams.coordinationPointsInZoneColor, "", IntToStr(cordinationPoint->sector) + " / " + cordinationPoint->countryName, 0x32, cordinationPointIndex));
        else
            coordinationPoints.push_back(drawPoint(cordinationPoint->point.L, cordinationPoint->point.H, 10, BCCalcParams.coordinationPointsOutZoneColor, "", IntToStr(cordinationPoint->sector) + " / " + cordinationPoint->countryName, 0x32, cordinationPointIndex));
    }
}
//---------------------------------------------------------------------------

