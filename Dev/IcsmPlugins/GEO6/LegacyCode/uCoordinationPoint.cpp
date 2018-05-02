//---------------------------------------------------------------------------

#pragma hdrstop

#include "uCoordinationPoint.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

CoordinationPoint::CoordinationPoint(TRSAGeoPoint& point, bool inZone, int countryId, int sector)
{
    this->countryId = countryId;
    this->inZone = inZone;
    this->point = point;
    this->sector = sector;
}
//---------------------------------------------------------------------------

CoordinationPoint::CoordinationPoint(const CoordinationPoint& coordinationPoint)
{
    countryId = coordinationPoint.countryId;
    countryName = coordinationPoint.countryName;
    inZone = coordinationPoint.inZone;
    point = coordinationPoint.point;
    sector = coordinationPoint.sector;
}
//---------------------------------------------------------------------------
