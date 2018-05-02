//---------------------------------------------------------------------------
#include <math.h>
#pragma hdrstop
#include "uDiagramPanel.h"
#include "uDuelResult.h"
#include "uParams.h"
#include "uProfileView.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

//---------------------------------------------------------------------------
__fastcall TPolarDiagramPanel::TPolarDiagramPanel(TComponent* AOwner)
 : TPanel(AOwner),
    m_showCenter(true),
    m_showAxis(true),
    m_norma(0.0),
    m_showMode(smZone),
    m_duelAzimuth(0.0),
    m_duelDistance(0.0),
    mouseX(-1),
    oldBmp(new Graphics::TBitmap),
    oldMouseX(-1)
{
    clear();
    Owner = AOwner;
    fmProfileView = NULL;
    oldBmp->Width = 1;
}
//---------------------------------------------------------------------------

void __fastcall TPolarDiagramPanel::Paint(void)
{
    oldBmp->Height = Height;
    
    Canvas->Brush->Style = bsSolid;
    Canvas->Brush->Color = Color;
    Canvas->FillRect(ClientRect);

    int centerX1 = Width / 2;
    int centerX2 = Width / 2;
    int centerY1 = Height / 2;
    int centerY2 = Height / 2;

    if (m_showMode == smDuel) {
        //  начало координат - точка установки первого передатчика
        //  найдём макс. значения X и Y в этих координатах.
        double minX = 0.0;
        double minY = 0.0;
        double maxX = 0.0;
        double maxY = 0.0;

        double duelCorrX = 0.0;
        double duelCorrY = 0.0;

        for (int zoneIdx = 0; zoneIdx < 5; zoneIdx++) {
            double* zone = NULL;
            int num = 0;
            switch (zoneIdx) {
                case 0: zone = &coverageZone[0];
                        num = coverageZone.size();
                        break;
                case 1: zone = &noiseLimitedZone[0];
                        num = noiseLimitedZone.size();
                        break;
                case 2: zone = &interfereLimitedZone[0];
                        num = interfereLimitedZone.size();
                        break;
                case 3: zone = &noiseLimitedZone2[0];
                        num = noiseLimitedZone2.size();
                        break;
                case 4: zone = &interfereLimitedZone2[0];
                        num = interfereLimitedZone2.size();
                        break;
                default: return;
            }
            if (zoneIdx == 3 || zoneIdx == 4) {
                /*
                duelCorrX = m_duelDistance * sin(m_duelAzimuth / 180.0 * M_PI);
                duelCorrY = - m_duelDistance * cos(m_duelAzimuth / 180.0 * M_PI);
                */
                //  развернём ось дуэли вдоль горизонтального края экрана
                duelCorrX = m_duelDistance;
            }

            int i = 0;
            for (double *f = zone; f < zone + num; f++, i++) {
                //  double angle = i / 18.0 * M_PI;
                //  развернём ось дуэли вдоль горизонтального края экрана
                double angle = (i * 360 / num - m_duelAzimuth + 90) / 180.0 * M_PI;
                double x = (*f) * sin(angle) + duelCorrX;
                double y = - (*f) * cos(angle) + duelCorrY;
                if (minX > x) minX = x;
                if (minY > y) minY = y;
                if (maxX < x) maxX = x;
                if (maxY < y) maxY = y;
            }
        }

        double width = maxX - minX;
        double height = maxY - minY;
        if (width == 0 || height == 0 || Width == 0 || Height == 0)
            return;

        //  нормирующее значение и корректировка точек установки передатчикрв
        //  относительно левого верхнего угла всей площадки
        if (width / (Parent->ClientWidth-2) > height / (Parent->ClientHeight - 2 )) {
            m_norma = width / (Parent->ClientWidth - 2);
            Width = Parent->ClientWidth - 2;
            Left = 1;
            Height = height / m_norma;
            Top = (Parent->ClientHeight - 2 - Height) / 2;
            //int corrY = (Height - height / norma) / 2;
            //centerX1 = - minX / norma;
            //centerY1 = - minY / norma + corrY;
            //centerX2 = (- minX + duelCorrX) / norma;
            //centerY2 = (- minY + duelCorrY) / norma + corrY;
        } else {
            m_norma = height / (Parent->ClientHeight - 2);
            Height = Parent->ClientHeight - 2;
            Top = 1;
            Width = width / m_norma;
            Left = (Parent->ClientWidth - 2 - Width) / 2;
            //int corrX = (Width - width / norma) / 2;
            //centerX1 = - minX / norma + corrX;
            //centerY1 = - minY / norma;
            //centerX2 = (- minX + duelCorrX) / norma + corrX;
            //centerY2 = (- minY + duelCorrY) / norma;
        }
        centerX1 = - minX / norma;
        centerY1 = - minY / norma;
        centerX2 = (- minX + duelCorrX) / norma;
        centerY2 = (- minY + duelCorrY) / norma;
    }

    if (showCenter) {
        Canvas->Pen->Color = clBlack;
        Canvas->Pen->Width = 5;
        Canvas->MoveTo(centerX1, centerY1);
        Canvas->LineTo(centerX1, centerY1);
        if (m_showMode == smDuel) {
            Canvas->MoveTo(centerX2, centerY2);
            Canvas->LineTo(centerX2, centerY2);
        }
    }

    {//ось через места установки передатчиков
        Canvas->Pen->Color = clBlack;
        Canvas->Pen->Width = 1;
        if (m_showMode == smDuel)
        {
            Canvas->MoveTo(0, centerY1);
            Canvas->LineTo(ClientWidth, centerY2);
        }
    }

    if ( dynamic_cast<TfmProfileView*>(fmProfileView) != NULL )
    {
        dynamic_cast<TfmProfileView*>(fmProfileView)->leftTxX = 0;
        dynamic_cast<TfmProfileView*>(fmProfileView)->rightTxX  = 0;
    }

    switch (m_showMode) {
        case smZone:
            if (showAxis) {
                Canvas->Pen->Color = clBlack;
                Canvas->Pen->Width = 1;
                Canvas->MoveTo(Width / 2, 0);
                Canvas->LineTo(Width / 2, Height);
                Canvas->MoveTo(0, Height / 2);
                Canvas->LineTo(Width, Height / 2);
            }

            drawZone(coverageZone, centerX1, centerY1, colCoverage, thickCoverage);
            drawZone(interfereLimitedZone, centerX1, centerY1, colInterf, thickInterf);
            drawZone(noiseLimitedZone, centerX1, centerY1, colNoise, thickNoise);
            break;
        case smLine:
            Canvas->Pen->Color = clGreen;
            Canvas->Pen->Width = 3;
            Canvas->MoveTo(3, Height / 2);
            Canvas->LineTo(Width / 2 - 7, Height / 2);
            Canvas->MoveTo(Width / 2 + 7, Height / 2);
            Canvas->LineTo(Width - 4, Height / 2);

            Canvas->Pen->Color = clGreen;
            //Canvas->Pen->Width = 1;
            Canvas->MoveTo(2, Height / 2 - 5);
            Canvas->LineTo(2, Height / 2 + 5);
            Canvas->MoveTo(Width - 3, Height / 2 - 5);
            Canvas->LineTo(Width - 3, Height / 2 + 5);

            break;
        case smDuel:
            Canvas->Pen->Style = psSolid;

            drawZone(interfereLimitedZone, centerX1, centerY1, BCCalcParams.lineColorZoneInterfere, BCCalcParams.lineThicknessZoneInterfere);
            drawZone(noiseLimitedZone, centerX1, centerY1, BCCalcParams.lineColorZoneNoise, BCCalcParams.lineThicknessZoneNoise);
            drawZone(interfereLimitedZone2, centerX2, centerY2, BCCalcParams.lineColorZoneInterfere, BCCalcParams.lineThicknessZoneInterfere);
            drawZone(noiseLimitedZone2, centerX2, centerY2, BCCalcParams.lineColorZoneNoise, BCCalcParams.lineThicknessZoneNoise);
            if (label2.Length() > 0) {
                Canvas->TextOut(centerX2 + 3, centerY2 + 3, label2);
            }

            if ( dynamic_cast<TfmProfileView*>(fmProfileView) != NULL )
            {
                dynamic_cast<TfmProfileView*>(fmProfileView)->leftTxX = centerX1;
                dynamic_cast<TfmProfileView*>(fmProfileView)->rightTxX  = centerX2;

                int max = 0, min = Width;
                int num = interfereLimitedZone.size();
                for (int i = 1; i < num; i++)
                {
                    double angle;
                    //  развернём ось дуэли вдоль горизонтального края экрана
                    angle = (i * 360 / num - m_duelAzimuth + 90) / 180.0 * M_PI;

                    int val = centerX1 + screenValue(interfereLimitedZone[i])*sin(angle);

                    if ( max < val )
                        max = val;

                    if ( min > val )
                        min = val;
                }

                max = 0; min = Width;
                num = interfereLimitedZone.size();
                for (int i = 1; i < num; i++)
                {
                    double angle;
                    //  развернём ось дуэли вдоль горизонтального края экрана
                    angle = (i * 360 / num - m_duelAzimuth + 90) / 180.0 * M_PI;

                    int val = centerX2 + screenValue(interfereLimitedZone2[i])*sin(angle);

                    if ( max < val )
                        max = val;

                    if ( min > val )
                        min = val;
                }

                dynamic_cast<TfmProfileView*>(fmProfileView)->Paint();
            }

            if ( mouseX >= 0 )
                DrawMarker(mouseX);
            
            break;
    }
    if (label.Length() > 0) {
        Canvas->TextOut(centerX1 + 3, centerY1 + 3, label);
    }
}

void __fastcall TPolarDiagramPanel::DrawMarker(int X)
{
    mouseX = X;

    Canvas->Pen->Color = clBlack;
    Canvas->Pen->Style = psDot;
    Canvas->Pen->Width = 1;
    Canvas->Brush->Style = bsClear;

    // стереть старую линейку
    if ( oldMouseX > -1 )
        Canvas->CopyRect(TRect(oldMouseX, 0, oldMouseX + 1, Height), oldBmp->Canvas, TRect(0, 0, 1, Height));

    // запомнить область под линейкой
    oldBmp->Canvas->CopyRect(TRect(0, 0, 1, Height), Canvas, TRect(X, 0, X + 1, Height));

    // нарисовать новую линейку
    Canvas->Pen->Mode = pmCopy;
    Canvas->MoveTo(X, 0);
    Canvas->LineTo(X, Height);

    oldMouseX = X;
}

void __fastcall TPolarDiagramPanel::drawZone(std::vector<double>& zone, int centerX, int centerY, TColor color, int lineWeight)
{
    int num = zone.size();

    if (num == 0)
        return;

    if (norma > 0) {
        Canvas->Pen->Width = lineWeight;
        Canvas->Pen->Color = color;
        if (m_showMode == smDuel) {
            double angle = (90 - m_duelAzimuth) / 180.0 * M_PI;
            Canvas->MoveTo(centerX + screenValue(zone[0])*sin(angle), centerY - screenValue(zone[0])/**4.0/3.0*/ * cos(angle));
        } else {
            Canvas->MoveTo(centerX, centerY - screenValue(zone[0]) /**4.0/3.0*/);
        }
        for (int i = 1; i < num; i++) {
            double angle;
            if (m_showMode == smDuel) {
                //  развернём ось дуэли вдоль горизонтального края экрана
                angle = (i * 360 / num - m_duelAzimuth + 90) / 180.0 * M_PI;
            } else {
                angle = i * 360 / num / 180.0 * M_PI;
            }
            Canvas->LineTo(centerX + screenValue(zone[i])*sin(angle), centerY - screenValue(zone[i])/**4.0/3.0*/ * cos(angle));
        }
        if (m_showMode == smDuel) {
            double angle = (90 - m_duelAzimuth) / 180.0 * M_PI;
            Canvas->LineTo(centerX + screenValue(zone[0])*sin(angle), centerY - screenValue(zone[0])/**4.0/3.0*/ * cos(angle));
        } else {
            Canvas->LineTo(centerX, centerY - screenValue(zone[0])/**4.0/3.0*/);
        }
    }
}

void __fastcall TPolarDiagramPanel::clear()
{
    coverageZone.clear();
    noiseLimitedZone.clear();
    interfereLimitedZone.clear();
    noiseLimitedZone2.clear();
    interfereLimitedZone2.clear();
    norma = 0.0;
    m_duelAzimuth = 0.0;
    m_duelDistance = 0.0;
    Invalidate();
}
void __fastcall TPolarDiagramPanel::setCoverage(double *zone, TColor col, int thickness, int num)
{
    colCoverage = col;
    thickCoverage = thickness;
    coverageZone.resize(num, 0);
    for (int i = 0; i < num; i++)
        coverageZone[i] = zone[i];
    findNorma();
    Invalidate();
}

void __fastcall TPolarDiagramPanel::setNoiseLimited(double *zone, TColor col, int thickness, int num)
{
    colNoise = col;
    thickNoise = thickness;
    noiseLimitedZone.resize(num, 0);
    for (int i = 0; i < num; i++)
        noiseLimitedZone[i] = zone[i];
    findNorma();
    Invalidate();
}

void __fastcall TPolarDiagramPanel::setInterfereLimited(double *zone, TColor col, int thickness, int num)
{
    colInterf = col;
    thickInterf = thickness;
    interfereLimitedZone.resize(num, 0);
    for (int i = 0; i < num; i++)
        interfereLimitedZone[i] = zone[i];
    findNorma();
    Invalidate();
}

void __fastcall TPolarDiagramPanel::setNoiseLimited2(double* zone, int num)
{
    noiseLimitedZone2.resize(num, 0);
    for (int i = 0; i < num; i++)
        noiseLimitedZone2[i] = zone[i];
    findNorma();
    Invalidate();
}

void __fastcall TPolarDiagramPanel::setInterfereLimited2(double* zone, int num)
{
    interfereLimitedZone2.resize(num, 0);
    for (int i = 0; i < num; i++)
        interfereLimitedZone2[i] = zone[i];
    findNorma();
    Invalidate();
}

void __fastcall TPolarDiagramPanel::findNorma()
{
    norma = 0.0;
    for (int i = 0; i < coverageZone.size(); i++)
        if (norma < coverageZone[i])
            norma = coverageZone[i];
    for (int i = 0; i < noiseLimitedZone.size(); i++)
        if (norma < noiseLimitedZone[i])
            norma = noiseLimitedZone[i];
    for (int i = 0; i < interfereLimitedZone.size(); i++)
        if (norma < interfereLimitedZone[i])
            norma = interfereLimitedZone[i];
    int minDim = Width < Height ? Width : Height;
    if (minDim > 1)
        norma /= (minDim / 2 );
}

void __fastcall TPolarDiagramPanel::SetShowCenter(bool value)
{
    if(m_showCenter != value) {
        m_showCenter = value;
        Invalidate();
    }
}

void __fastcall TPolarDiagramPanel::SetShowAxis(bool value)
{
    if(m_showAxis != value) {
        m_showAxis = value;
        Invalidate();
    }
}

void __fastcall TPolarDiagramPanel::SetNorma(double value)
{
    if(m_norma != value) {
        m_norma = value;
        Invalidate();
    }
}

void __fastcall TPolarDiagramPanel::SetShowMode(DiagramShowMode newShowMode)
{
    if(m_showMode != newShowMode) {
        m_showMode = newShowMode;
        Invalidate();
    }
}

void __fastcall TPolarDiagramPanel::SetDuelDistance(double value)
{
    if(m_duelDistance != value) {
        m_duelDistance = value;
        findNorma();
        Invalidate();
    }
}

void __fastcall TPolarDiagramPanel::SetDuelAzimuth(double value)
{
    if(m_duelAzimuth != value) {
        m_duelAzimuth = value;
        findNorma();
        Invalidate();
    }
}


