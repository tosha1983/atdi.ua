#include <vcl.h>
#include <values.h>
#include <memory>
#include <math.h>
#include "uParams.h"
#pragma hdrstop

#include "uProfileView.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfmProfileView *fmProfileView;
//---------------------------------------------------------------------------
__fastcall TfmProfileView::TfmProfileView(TComponent* Owner)
    : TFrame(Owner), m_txAntHeight(0), m_rcAntHeight(0), m_drawMode(1),
        m_showAngles(false), m_useEarthCurve(true), resetState(true),
        m_lon1(0), m_lat1(0), m_lon2(0), m_lat2(0), reliefStep(0), reliefDistance(0),
        normaHeightIdx(0), oldMouseX(-1), oldBmp(new Graphics::TBitmap)
{
    oldWndProc = WindowProc;
    WindowProc = newWndProc;
    oldBmp->Width = 1;
    chbEarthCurve->Checked = BCCalcParams.earthCurveInRelief;

    dist1 = 0; dist2 = 0; dist3 = 0;
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::SetTxAntHeight(int value)
{
    if (m_txAntHeight != value) {
        m_txAntHeight = value;
        resetState = false;
        Repaint();
    }
}

void __fastcall TfmProfileView::SetRcAntHeight(int value)
{
    if (m_rcAntHeight != value) {
        m_rcAntHeight = value;
        resetState = false;
        Repaint();
    }
}
void __fastcall TfmProfileView::SetDrawMode(int value)
{
    if (m_drawMode != value) {
        m_drawMode = value;
        resetState = false;
        Repaint();
    }
}
void __fastcall TfmProfileView::SetShowAngles(bool value)
{
    if (m_showAngles != value) {
        m_showAngles = value;
        resetState = false;
        Repaint();
    }
}
void __fastcall TfmProfileView::SetEarthCurve(bool value)
{
    if (m_useEarthCurve != value) {
        m_useEarthCurve = value;
        chbEarthCurve->Checked = m_useEarthCurve;
        resetState = false;
        Repaint();
    }
}
//---------------------------------------------------------------------------
void __fastcall TfmProfileView::chbEarthCurveClick(TObject *Sender)
{
    useEarthCurve = chbEarthCurve->Checked;
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::RetreiveProfile(double lon1, double lat1,
                                    double lon2, double lat2,
                                    double lon3, double lat3,
                                    double lon4, double lat4
                                   )
{
    TCOMIRSASpherics comSpherics;
    try {
        comSpherics.CreateInstance(CLSID_RSASpherics);
    } catch (Exception &e) {
        throw Exception(AnsiString("Помилка завантаження інтерфейсу перетворення координат.\n")+e.Message);
    }

    reliefHeight.clear();
    earthCurve.clear();

    TRSAGeoPoint GeoPoint1, GeoPoint2, GeoPoint3, GeoPoint4;
      GeoPoint1.H = lat1; GeoPoint1.L = lon1;
      GeoPoint2.H = lat2; GeoPoint2.L = lon2;
      GeoPoint3.H = lat3; GeoPoint3.L = lon3;
      GeoPoint4.H = lat4; GeoPoint4.L = lon4;

    {
        TRSAAzimuth azimuth23;
        comSpherics.Azimuth(GeoPoint2, GeoPoint3, &azimuth23);
        TRSAAzimuth azimuth13;
        comSpherics.Azimuth(GeoPoint1, GeoPoint3, &azimuth13);

        double distance13;
        comSpherics.Distance(GeoPoint1, GeoPoint3, &distance13);

        //  эти загадочные deltaX потом добавляются к общей длине

        deltaXLeft = 0;//distance13 - distance13*cos(azimuth13 - azimuth23);

        TRSAAzimuth azimuth34;
        comSpherics.Azimuth(GeoPoint3, GeoPoint4, &azimuth34);

        double distance34;
        comSpherics.Distance(GeoPoint3, GeoPoint4, &distance34);

        deltaXRight = 0;//distance34 - distance34*cos(azimuth34 - azimuth23);
    }
    TRSAAltitude h;
    if ( BCCalcParams.ReliefServerArrayGUID != "" )
        BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPoint1, &h);

    int normaHeight = h;
    normaHeightIdx = 0;

    reliefDistance = 0;
    double distance;

    TRSAAzimuth azimuth;
    comSpherics.Azimuth(GeoPoint1, GeoPoint2, &azimuth);
    angle12 = 90 - azimuth;

    if ( BCCalcParams.ReliefServerArrayGUID != "" )
        BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPoint1, &h);
    h_angle1 =  h + m_txAntHeight;

    comSpherics.Distance(GeoPoint1, GeoPoint2, &reliefDistance);
    dist1 = reliefDistance;

    comSpherics.Distance(GeoPoint2, GeoPoint3, &distance);
    reliefDistance += distance;
    dist2 = distance;

    comSpherics.Azimuth(GeoPoint3, GeoPoint4, &azimuth);
    angle34 = 90 - azimuth;

    // конечная точка
    if ( BCCalcParams.ReliefServerArrayGUID != "" )
        BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPoint4, &h);
    h_angle2 = h + m_rcAntHeight;

    comSpherics.Distance(GeoPoint3, GeoPoint4, &distance);
    reliefDistance += distance;
    dist3 = distance;

    reliefDistance += (deltaXLeft + deltaXRight);

    txtDist->Caption = txtDist->Caption.sprintf("%.1f км", reliefDistance);

    // вычисляем шаг снятия рельефа
    reliefStep = reliefDistance / (double)(reliefDistance / BCCalcParams.Step + 1);

    angle1 = -1; angle2 = -1;

    dist0 = 0;
    //RetreiveProfile(lon1, lat1, lon4, lat4, &normaHeight);

    RetreiveProfile(lon1, lat1, lon2, lat2, &normaHeight);
    dist0 += dist1;
    RetreiveProfile(lon2, lat2, lon3, lat3, &normaHeight);
    dist0 += dist2;
    RetreiveProfile(lon3, lat3, lon4, lat4, &normaHeight);

    Repaint();
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::RetreiveProfile(double lon1, double lat1, double lon2, double lat2, int *normaHeight)
{
    //if ((m_lon1 != lon1)&&(m_lat1 != lat1)&&(m_lon2 != lon2)&&(m_lat2 != lat2))
    {
        m_lon1 = lon1; m_lat1 = lat1; m_lon2 = lon2; m_lat2 = lat2;

        if ( (!BCCalcParams.FTerrInfoSrv.IsBound()) && (BCCalcParams.ReliefServerArrayGUID != "") )
            throw Exception("Інтерфейс серверу рельефу не проініціалізований.");

        TCOMIRSASpherics comSpherics;
        try {
            comSpherics.CreateInstance(CLSID_RSASpherics);
        } catch (Exception &e) {
            throw Exception(AnsiString("Помилка завантаження інтерфейсу перетворення координат.\n")+e.Message);
        }

        TRSAGeoPoint GeoPointA; GeoPointA.H = lat1; GeoPointA.L = lon1;
        TRSAGeoPoint GeoPointB; GeoPointB.H = lat2; GeoPointB.L = lon2;

        // азимут
        TRSAAzimuth azimuth;
        comSpherics.Azimuth(GeoPointA, GeoPointB, &azimuth);

        // дистанция
        double distance;
        comSpherics.Distance(GeoPointA, GeoPointB, &distance);

        double h1, h2 = h_angle2;

        TRSAGeoPoint GeoPointTmp;
        TRSAAltitude h;
        TRSAMorpho m;

        int stepCount = distance / BCCalcParams.Step + 1;

        if (stepCount == 0) throw *(new Exception("Нульова дистанція"));

        // поехали понемногу
        double curDist = 0.0;
        while ( stepCount > 0 )
        {
            comSpherics.PolarToGeo(curDist, azimuth, GeoPointA, &GeoPointTmp);
            if ( BCCalcParams.ReliefServerArrayGUID != "" )
            {
                BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPointTmp, &h);
                BCCalcParams.FTerrInfoSrv.Get_Morpho(GeoPointTmp, &m);
            }

            TReliefPoint reliefPoint;
            double dopHeight = 0.0;
            if (reliefDistance != 0.0)
                dopHeight = (((reliefDistance * (curDist+dist0)) / (2 * 6.371)) * (1 - ((curDist+dist0) / reliefDistance)));
                
            reliefPoint.height = h;
            reliefPoint.morpho = m.Kind;

            // нормированная высота
            if (*normaHeight > h) {
                *normaHeight = h;
                normaHeightIdx = reliefHeight.size();
            }

            reliefHeight.push_back(reliefPoint);
            earthCurve.push_back(dopHeight);

            double actualHeight = h + dopHeight;
            // углы закрытия
            if (curDist == 0.0)
                h1 = (h + m_txAntHeight);
            else {
                if (curDist > reliefStep) {
                    double curAng1 = sin(0.001 * (actualHeight - h1)/curDist);
                    if (curAng1 > angle1) {
                        angle1 = curAng1;
                        dist_gorb1 = curDist;
                        h_angle1 = actualHeight;
                    }
                }
                if ((distance - curDist) > reliefStep) {
                    double curAng2 = sin(0.001 * (actualHeight - h2)/(distance-curDist));
                    if (curAng2 > angle2) {
                        angle2 = curAng2;
                        dist_gorb2 = curDist;
                        h_angle2 = actualHeight;
                    }
                }
            }

            curDist += reliefStep;

            stepCount--;
        }
    }

   resetState = false;
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::RetreiveProfile(double lon1, double lat1, double lon2, double lat2)
{
    //if ((m_lon1 != lon1)&&(m_lat1 != lat1)&&(m_lon2 != lon2)&&(m_lat2 != lat2))
    {
        m_lon1 = lon1; m_lat1 = lat1; m_lon2 = lon2; m_lat2 = lat2;

        reliefStep = BCCalcParams.Step;

        if (!BCCalcParams.FTerrInfoSrv.IsBound())
            if ( BCCalcParams.ReliefServerArrayGUID != "" )
                new Exception("Інтерфейс серверу рельефу не проініціалізований.");

        TCOMIRSASpherics comSpherics;
        try {
            comSpherics.CreateInstance(CLSID_RSASpherics);
        } catch (Exception &e) {
            throw *(new Exception(AnsiString("Помилка завантаження інтерфейсу перетворення координат.\n")+e.Message));
        }

        // азимут
        TRSAAzimuth azimuth;
        TRSAGeoPoint GeoPointA;
        GeoPointA.H = lat1; GeoPointA.L = lon1;
        TRSAGeoPoint GeoPointB;
        GeoPointB.H = lat2; GeoPointB.L = lon2;
        comSpherics.Azimuth(GeoPointA, GeoPointB, &azimuth);

        // дистанция
        comSpherics.Distance(GeoPointA, GeoPointB, &reliefDistance);
        txtDist->Caption = txtDist->Caption.sprintf("%.1f км", reliefDistance);

        // инициализация массивов и переменных
        reliefHeight.clear();
        earthCurve.clear();
        double curDist = 0.0;

        double h1, h2;

        TRSAGeoPoint GeoPointTmp;
        TRSAAltitude h;
        TRSAMorpho m;

        // начальная точка
        if ( BCCalcParams.ReliefServerArrayGUID != "" )
            BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPointA, &h);
        int normaHeight = h;
        normaHeightIdx = 0;
        h_angle1 =  h + m_txAntHeight;

        // конечная точка
        if ( BCCalcParams.ReliefServerArrayGUID != "" )
            BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPointB, &h);
        h2 = h_angle2 = h + m_rcAntHeight;

        // вычисляем шаг снятия рельефа
        int stepCount = reliefDistance / reliefStep + 1;
        reliefStep = reliefDistance / (double)stepCount;
        angle1 = -1; angle2 = -1;

        // поехали понемногу
        if (stepCount > 0) {

            stepCount++;
           while (stepCount-- > 0) {
                comSpherics.PolarToGeo(curDist, azimuth, GeoPointA, &GeoPointTmp);
                if ( BCCalcParams.ReliefServerArrayGUID != "" )
                {
                    BCCalcParams.FTerrInfoSrv.Get_Altitude(GeoPointTmp, &h);
                    BCCalcParams.FTerrInfoSrv.Get_Morpho(GeoPointTmp, &m);
                }

                TReliefPoint reliefPoint;
                double dopHeight = 0.0;
                if (reliefDistance != 0.0)
                    dopHeight = (((reliefDistance * curDist) / (2 * 6.371)) * (1 - (curDist / reliefDistance)));

                reliefPoint.height = h;
                reliefPoint.morpho = m.Kind;
                //??? unsigned_char tmp = m.BuildingHeight;

                // нормированная высота
                if (normaHeight > h) {
                    normaHeight = h;
                    normaHeightIdx = reliefHeight.size();
                }

                reliefHeight.push_back(reliefPoint);
                earthCurve.push_back(dopHeight);

                double actualHeight = h + dopHeight;
                // углы закрытия
                if (curDist == 0.0)
                    h1 = (h + m_txAntHeight);
                else {
                    if (curDist > reliefStep) {
                        double curAng1 = sin(0.001 * (actualHeight - h1)/curDist);
                        if (curAng1 > angle1) {
                            angle1 = curAng1;
                            dist_gorb1 = curDist;
                            h_angle1 = actualHeight;
                        }
                    }
                    if ((reliefDistance - curDist) > reliefStep) {
                        double curAng2 = sin(0.001 * (actualHeight - h2)/(reliefDistance-curDist));
                        if (curAng2 > angle2) {
                            angle2 = curAng2;
                            dist_gorb2 = curDist;
                            h_angle2 = actualHeight;
                        }
                    }
                }

                curDist += reliefStep;
            }

         } else
            throw *(new Exception("Нульова дистанція"));
    }

    resetState = false;
    Repaint();
}

void __fastcall TfmProfileView::Reset()
{
    std::auto_ptr<TControlCanvas> cnv(new TControlCanvas());
    cnv->Control = this;
    TRect rect = GetClientRect();
    cnv->Brush->Color = this->Color;
    cnv->FillRect(rect);
    resetState = true;
}

void __fastcall TfmProfileView::newWndProc(Messages::TMessage &Message)
{
    bool SepIs = false;
    switch (Message.Msg) {
        case WM_PAINT:
            oldWndProc(Message);
            if (!resetState) {
                Paint();
            }
            break;
        default:
            oldWndProc(Message);
            break;
    }
}

void __fastcall TfmProfileView::Paint()
{
    if ((reliefStep == 0) || (reliefDistance == 0))
        return;

    std::auto_ptr<TControlCanvas> cnv(new TControlCanvas());
    cnv->Control = this;
    TRect rect = GetClientRect();
    //rect.Bottom = rect.Bottom - chbEarthCurve->Height - 2;
    cnv->Brush->Color = this->Color;
    cnv->FillRect(rect);

    int normaHeight = reliefHeight[normaHeightIdx].height;

    std::vector<TReliefPoint>::iterator ih = reliefHeight.begin();
    std::vector<int>::iterator ic = earthCurve.begin();

    // поиск максимальной высоты
    int maxHeight = ih->height;
    for (ih++, ic++; ih < reliefHeight.end() && ic < earthCurve.end(); ih++, ic++) {
        int actHeight = ih->height;
        if (m_useEarthCurve)
            actHeight += *ic;
        if (maxHeight < actHeight)
            maxHeight = actHeight;
    }

    // определение масштаба
    if (maxHeight - normaHeight == 0) maxHeight++;
    double h_scale = ((double)Height - 5 - chbEarthCurve->Height) / ((maxHeight - normaHeight) * 1.5);
    double l_scale = ((double)Width - 1) / reliefDistance;
    int start_h = Height - 4 - chbEarthCurve->Height;
    int start_l = deltaXLeft * l_scale;

    double curDist = 0;
    cnv->Pen->Color = COLOR_EARTH;
    cnv->Pen->Width = 1;

    // коробка
    cnv->MoveTo(start_l, 1); //start_h - (maxHeight - normaHeight) * h_scale);
    cnv->LineTo(start_l, start_h);
    cnv->LineTo(Width-1, start_h);
    cnv->LineTo(Width-1, 1); // start_h - (maxHeight - normaHeight) * h_scale);

    // кривизна земли по нормированной высоте
    if (m_useEarthCurve) {
        ic = earthCurve.begin();
        cnv->MoveTo(start_l,start_h);
        do {

            curDist += reliefStep;
            ic++;
            cnv->LineTo(start_l + curDist * l_scale, start_h - (*ic) * h_scale);

        } while (ic < earthCurve.end());
    }

    leftTxBaseHeight = 0; rightTxBaseHeight = 0;

    // перо - в первую точку
    cnv->MoveTo(start_l, start_h - (reliefHeight[0].height - normaHeight) * h_scale);
    curDist = reliefStep;

    ih = reliefHeight.begin();
    ic = earthCurve.begin();
    curDist = 0;

    do {
        if (ih->morpho == 0) {
            cnv->Pen->Color = COLOR_GROUND;
            cnv->Pen->Width = 1;
        } else if (ih->morpho == 0xC0) {
            cnv->Pen->Color = COLOR_SEA;
            cnv->Pen->Width = 4;
        } else if (ih->morpho == 64) {
            cnv->Pen->Color = COLOR_SEA;
            cnv->Pen->Width = 2;
        } else {
            cnv->Pen->Color = COLOR_GROUND;
            cnv->Pen->Width = 3;
        }


        ih++, ic++;
        curDist += reliefStep;

        double height = start_h - (ih->height - normaHeight + ((m_useEarthCurve)? (*ic) : 0)) * h_scale;
        cnv->LineTo(start_l + curDist * l_scale, height);

        if ( ( curDist * l_scale >= leftTxX ) && ( ( curDist - reliefStep )* l_scale < leftTxX) )
        {//место установки левого передатчика ( линейная интерполяция высоты )
         // ( x - x1 ) / ( x2 - x1 ) = ( y - y1 ) / ( y2 - y1 )
            int y1 = start_h - ((ih-1)->height - normaHeight + ((m_useEarthCurve)? (*ic) : 0)) * h_scale;
            int y2 = start_h - (ih->height - normaHeight + ((m_useEarthCurve)? (*ic) : 0)) * h_scale;
            leftTxBaseHeight = (leftTxX - (curDist-reliefStep)*l_scale)*(y2-y1)/(reliefStep*l_scale)+y1;
        }

        if ( ( curDist * l_scale >= rightTxX ) && ( ( curDist - reliefStep )* l_scale < rightTxX) )
        {//место установки правого передатчика
            int y1 = start_h - ((ih-1)->height - normaHeight + ((m_useEarthCurve)? (*ic) : 0)) * h_scale;
            int y2 = start_h - (ih->height - normaHeight + ((m_useEarthCurve)? (*ic) : 0)) * h_scale;
            rightTxBaseHeight = (rightTxX - (curDist-reliefStep)*l_scale)*(y2-y1)/(reliefStep*l_scale)+y1;
        }

    } while (ih < reliefHeight.end());

    if ( leftTxX != rightTxX )
    {
        cnv->Pen->Color = clRed;
        cnv->Pen->Width = 2;

        if ( leftTxHeight == 0 )
            leftTxHeight = 50;

        cnv->MoveTo(leftTxX, leftTxBaseHeight - leftTxHeight*h_scale);
        cnv->LineTo(leftTxX, leftTxBaseHeight);

        if ( rightTxHeight == 0 )
            rightTxHeight = 50;

        cnv->MoveTo(rightTxX, rightTxBaseHeight - rightTxHeight*h_scale);
        cnv->LineTo(rightTxX, rightTxBaseHeight);
    }


    //cnv->LineTo(relief_distance*l_scale+start_l,start_h-((height_array.back().height - normaHeight)*h_scale));

    if (m_showAngles) {

        cnv->Pen->Color = COLOR_ANTENNA;
        cnv->Pen->Width = 2;
        cnv->MoveTo(start_l,start_h - ((reliefHeight.front().height - normaHeight + m_txAntHeight) * h_scale));
        cnv->LineTo(start_l,start_h - ((reliefHeight.front().height - normaHeight) * h_scale));
        cnv->LineTo(reliefDistance * l_scale+start_l, start_h - ((reliefHeight.back().height - normaHeight) * h_scale));
        cnv->LineTo(reliefDistance * l_scale+start_l, start_h - ((reliefHeight.back().height - normaHeight + m_rcAntHeight) * h_scale));

        cnv->Pen->Color = COLOR_DIRECTLINE;
        cnv->Pen->Width = 1;
        cnv->LineTo(start_l,start_h - ((reliefHeight.front().height - normaHeight + m_txAntHeight) * h_scale));

        cnv->Pen->Color = COLOR_ANGLES;
        cnv->Pen->Width = 2;
        cnv->LineTo(start_l + (dist_gorb1)*l_scale, start_h - h_angle1 * h_scale);
        cnv->MoveTo(reliefDistance * l_scale + start_l, start_h - (reliefHeight.back().height - normaHeight + m_rcAntHeight) * h_scale);
        cnv->LineTo(dist_gorb2 * l_scale + start_l, start_h - h_angle2 * h_scale);
    }
    cnv->Font->Color = COLOR_TITLE;
    cnv->Font->Size = 4;
    cnv->TextOutA( 2, 2, AnsiString(reliefHeight.front().height));
    cnv->TextOutA(Width - AnsiString(reliefHeight.back().height).Length() * 7, 2, AnsiString(reliefHeight.back().height));

    oldMouseX = -1;
    txtHeight->Caption = "";
}

void __fastcall TfmProfileView::FrameResize(TObject *Sender)
{
    oldBmp->Height = Height;
    Repaint();
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::Frame_DoMouseMove(int X, int Y)
{
    int upIdx = reliefHeight.size() - 1;
    if (upIdx == -1) {
        txtHeight->Caption = "";
        return;
    }

    std::auto_ptr<TControlCanvas> cnv(new TControlCanvas());
    cnv->Control = this;
    cnv->Pen->Style = psDot;
    cnv->Brush->Style = bsClear;

    // стереть старую линейку
    if (oldMouseX > -1) {
        cnv->CopyRect(TRect(oldMouseX, 0, oldMouseX + 1, Height), oldBmp->Canvas, TRect(0, 0, 1, Height));
    }

    // запомнить область под линейкой
    oldBmp->Canvas->CopyRect(TRect(0, 0, 1, Height), cnv.get(), TRect(X, 0, X + 1, Height));

    // нарисовать новую линейку
    cnv->Pen->Mode = pmCopy;
    cnv->MoveTo(X, 0);
    cnv->LineTo(X, Height);
    oldMouseX = X;

    float pos = ((float)X) * upIdx / Width;
    int low = floor(pos);
    int hi = ceil(pos);

    if (low < 0) low = 0;
    if (hi > upIdx) hi = upIdx;

    // res = base + difY, difY = difX * hopY / hopX, hopX = 1
    int height = reliefHeight[low].height + (reliefHeight[hi].height - reliefHeight[low].height) * (pos - low);
    if ( dist1 == 0 )
    {
        float dist = reliefStep * pos;
        txtHeight->Caption = txtHeight->Caption.sprintf("%.1f км:  %d м", dist, height);
    }
    else
    {
        /*
        float distLeft = (reliefStep * pos) - dist1;
        float distRight = (reliefStep * pos) - dist1 - dist2;
        txtHeight->Caption = txtHeight->Caption.sprintf("%.1f км / %.1f км  %d м", distLeft, distRight, height);
        */
        float distLeft = (X - leftTxX)*reliefDistance / ((double)Width - 1);
        float distRight = (X - rightTxX)*reliefDistance / ((double)Width - 1);
        txtHeight->Caption = txtHeight->Caption.sprintf("%.1f км  /  %.1f км  %d м", distLeft, distRight, height);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::FrameMouseMove(TObject *Sender, TShiftState Shift, int X, int Y)
{
    Frame_DoMouseMove(X, Y);

    if ( Import_OnMouseMove )
        Import_OnMouseMove(X, Y);
}
//---------------------------------------------------------------------------

void __fastcall TfmProfileView::External_OnMouseMove(int X, int Y)
{
    Frame_DoMouseMove(X, Y);
}
//---------------------------------------------------------------------------
