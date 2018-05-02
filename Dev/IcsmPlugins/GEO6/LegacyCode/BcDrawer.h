#ifndef BcDrawerH
#define BcDrawerH

#include "LISBCTxServer_TLB.h"
#include <vector>
#include <memory>
#include <Controls.hpp>

struct DrawContourData
{
    int id;
    AnsiString name;
    std::vector<BcCoord> contour;
    DrawContourData(): id(0) {};
    DrawContourData(const DrawContourData& src): id(src.id), name(src.name), contour(src.contour) {};
    DrawContourData& operator=(DrawContourData& src) { id=src.id; name=src.name; contour=src.contour; return *this; };
};

class ContourDrawer
{
public:
    int xMargin;
    int yMargin;

    double minlon;
    double minlat;
    double maxlon;
    double maxlat;

private:

    double scalex;
    double scaley;

    double origWidth;
    double origHeight;
    double origScalex;
    double origScaley;
    TWinControl* wc;

public:

    bool showBoundries;
    std::vector<DrawContourData> *contours;
    std::auto_ptr<TControlCanvas> cnv;

    ContourDrawer():
        xMargin(10),
        yMargin(10),
        showBoundries(true),
        contours(NULL),
        wc(NULL),
        scalex(1), scaley(1),
        origScalex(1), origScaley(1),
        origWidth(0.0), origHeight(0.0)
    {};

    void DrawContours(int);
    void FitContours();
    void DrawContour(DrawContourData& dcd, TColor contColor, TColor textColor);

    void __fastcall AjustScale();
    int __fastcall GetXpos(double lon);
    int __fastcall GetYpos(double lat);
    double __fastcall GetLon(int xpos);
    double __fastcall GetLat(int ypos);
    void __fastcall SetWc(TWinControl *);
    TWinControl * __fastcall GetWc() {return wc; };
    void __fastcall SetScaleX(double);
    void __fastcall SetScaleY(double);
    double __fastcall GetScaleX() { return scalex; };
    double __fastcall GetScaleY() { return scaley; };
};

#endif BcDrawerH
