//---------------------------------------------------------------------------

#ifndef uCoordinationPointH
#define uCoordinationPointH
//---------------------------------------------------------------------------

#include "RSAGeography_TLB.h"
//---------------------------------------------------------------------------

class CoordinationPoint
{
public:
    int countryId;
    AnsiString countryName;
    bool inZone;
    TRSAGeoPoint point;
    int sector;

    CoordinationPoint(TRSAGeoPoint& point, bool inZone, int countryId, int sector);
    CoordinationPoint(const CoordinationPoint& coordinationPoint);
};
//---------------------------------------------------------------------------
#endif
 