//---------------------------------------------------------------------------

#ifndef uBcAutoUpgradeH
#define uBcAutoUpgradeH
//---------------------------------------------------------------------------

#include "uAutoUpgrade.h"
//---------------------------------------------------------------------------

class BcAutoUpgrade : public TDBAutoUpgrade
{
public:
    BcAutoUpgrade(TIBDatabase *db, double version);
    void __fastcall UpgradeDb();
    void __fastcall CreateOriginalSchema(double &tmp_version);
};
//---------------------------------------------------------------------------

#endif
