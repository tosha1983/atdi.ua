#include <vcl.h>
#pragma hdrstop

#include "BcDrawer.h"
#include <memory>
#include <math.h>
#include "uMainDm.h"

using namespace std;

void ContourDrawer::FitContours()
{
    if (showBoundries && dmMain->bordSectors.empty())
        dmMain->loadBordSectors();

    // obtain min and max coordinates
    minlon = showBoundries ? dmMain->minlon : 180.0;
    minlat = showBoundries ? dmMain->minlat : 90.0;
    maxlon = showBoundries ? dmMain->maxlon : -180.0;
    maxlat = showBoundries ? dmMain->maxlat : -90.0;

    if (contours != NULL)
    {

        for (int i = 0; i < (*contours).size(); i++)
        {
            for (int j = 0; j < (*contours)[i].contour.size(); j++)
            {
                BcCoord bcCurr = (*contours)[i].contour[j];
                if (minlon > bcCurr.lon)
                    minlon = bcCurr.lon;
                if (minlat > bcCurr.lat)
                    minlat = bcCurr.lat;
                if (maxlon < bcCurr.lon)
                    maxlon = bcCurr.lon;
                if (maxlat < bcCurr.lat)
                    maxlat = bcCurr.lat;
            }
        }
    }

    if (maxlon < minlon)
    {
        maxlon = 35.0;
        minlon = 25.0;
    }
    if (maxlat < minlat)
    {
        maxlat = 55.0;
        minlat = 45.0;
    }

    if (maxlon == minlon)
    {
        maxlon += 0.5;
        minlon -= 0.5;
    }
    if (maxlat == minlat)
    {
        maxlat += 0.5;
        minlat -= 0.5;
    }

    if (wc == NULL)
        return;

    scalex = ( maxlon - minlon ) / ((double)wc->ClientWidth - 2*xMargin);
    if (scalex == 0)
        scalex = 0.01;
    scaley = ( maxlat - minlat ) / ((double)wc->ClientHeight - 2*yMargin);
    if (scaley == 0)
        scaley = 0.01;
    origScalex = scalex;
    origScaley = scaley;

    AjustScale();

    origWidth = wc->ClientWidth;
    origHeight = wc->ClientHeight;
}


void ContourDrawer::DrawContours(int boldIdx)
{
    if (cnv.get() == NULL)
    {
        cnv.reset(new TControlCanvas());
        cnv->Control = wc;
    }
    // clear all;
    cnv->Brush->Color = clWhite;
    cnv->Brush->Style = bsSolid;
    cnv->Pen->Color = clBlack;
    cnv->Pen->Width = 1;
    cnv->Rectangle(0, 0, wc->ClientWidth, wc->ClientHeight);

    // draw state border
    if (showBoundries)
    {
        if (dmMain->bordSectors.empty())
            dmMain->loadBordSectors();

        cnv->Pen->Color = clLtGray;
        cnv->Pen->Width = 1;

        for (TdmMain::BordSectors::iterator ssi = dmMain->bordSectors.begin(); ssi < dmMain->bordSectors.end(); ssi++)
        {
            if (ssi->size() > 0)
            {
                for (TdmMain::BordSector::iterator si = ssi->begin(); si < ssi->end(); si++)
                {
                    int xpos = GetXpos(si->lon);
                    int ypos = GetYpos(si->lat);
                    if (si == ssi->begin())
                        cnv->MoveTo(xpos, ypos);
                    else
                        cnv->LineTo(xpos, ypos);
                }
            }
        }
    }
    // draw zones

    cnv->Pen->Style = psSolid;
    cnv->Pen->Width = 3;

    cnv->Font->Size = 6;
    //cnv->Font->Handle
    cnv->Brush->Style = bsClear;

    if (contours != NULL)
    {
        int selCont = -1;
        for (int i = 0; i < (*contours).size(); i++)
        {
            DrawContour((*contours)[i], clBlue, clDkGray);
            if ((*contours)[i].id == boldIdx)
                selCont = i;
        }
        if (selCont > -1)
            DrawContour((*contours)[selCont], clLime, clWindowText);
    }
}

void ContourDrawer::DrawContour(DrawContourData& dcd, TColor contColor, TColor textColor)
{
    if (cnv.get() == NULL)
    {
        cnv.reset(new TControlCanvas());
        cnv->Control = wc;
    }

    cnv->Pen->Color = contColor;
    cnv->Font->Color = textColor;

    long pn = dcd.contour.size();
    if (pn > 0)
    {
        BcCoord bcFirst = dcd.contour[0];
        int xpos = GetXpos(bcFirst.lon);
        int ypos = GetYpos(bcFirst.lat);

        int totalx = xpos;
        int totaly = ypos;

        cnv->MoveTo(xpos, ypos);

        for (int j = 1; j < pn; j++)
        {
            BcCoord bcCurr = dcd.contour[j];
            xpos = GetXpos(bcCurr.lon);
            ypos = GetYpos(bcCurr.lat);

            totalx += xpos;
            totaly += ypos;

            cnv->LineTo(xpos, ypos);
        }

        xpos = GetXpos(bcFirst.lon);
        ypos = GetYpos(bcFirst.lat);

        cnv->LineTo(xpos, ypos); // to first point

        // centerpoint
        totalx /= pn; totaly /= pn;
        SIZE ext = cnv->TextExtent(dcd.name);
        cnv->TextOut(totalx - ext.cx / 2, totaly - ext.cy / 2, dcd.name);
    }
}

void __fastcall ContourDrawer::AjustScale()
{
    if (wc)
    {
        if (origWidth == 0.0)
            origWidth = wc->ClientWidth;
        scalex = origScalex * (origWidth / wc->ClientWidth);

        if (origHeight == 0.0)
            origHeight = wc->ClientHeight;
        scaley = origScaley * (origHeight / wc->ClientHeight);
    }

    // average latitude correction
    double corrCoeff = cos(( maxlat + minlat ) / 2.0 * M_PI / 180.0);
    if (scalex * corrCoeff > scaley)
        scaley = scalex * corrCoeff;
    else
        scalex = scaley / corrCoeff;
}

int __fastcall ContourDrawer::GetXpos(double lon)
{
    return (lon - (minlon + maxlon) / 2) / scalex + wc->ClientWidth / 2;
}

int __fastcall ContourDrawer::GetYpos(double lat)
{
    return wc->ClientHeight / 2 - (lat - (minlat + maxlat) / 2) / scaley;
}

double __fastcall ContourDrawer::GetLon(int xpos)
{
    return (xpos - wc->ClientWidth / 2) * scalex + (minlon + maxlon) / 2;
}

double __fastcall ContourDrawer::GetLat(int ypos)
{
    return (wc->ClientHeight / 2 - ypos) * scaley + (minlat + maxlat) / 2;
}

void __fastcall ContourDrawer::SetWc(TWinControl *src)
{
    if (wc == NULL && src != NULL)
    {
        wc = src;
        origHeight = wc->ClientHeight;
        origWidth = wc->ClientWidth;
    }
}

void __fastcall ContourDrawer::SetScaleX(double val)
{
    if (val > 0)
    {
        origScalex *= (val / scalex);
        scalex = val;
    }
}

void __fastcall ContourDrawer::SetScaleY(double val)
{
    if (val > 0)
    {
        origScaley *= (val / scaley);
        scaley = val;
    }
}
