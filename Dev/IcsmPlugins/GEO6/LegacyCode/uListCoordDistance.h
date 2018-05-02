//---------------------------------------------------------------------------

#ifndef uListCoordDistanceH
#define uListCoordDistanceH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseListTree.h"
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <IBSQL.hpp>
//---------------------------------------------------------------------------
class TfrmListCoordDistance : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIntegerField *dstListEFFECTRADIATEPOWER;
    TIntegerField *dstListHEIGHTANTENNA;
    TSmallintField *dstListOVERLAND;
    TSmallintField *dstListOVERCOLDSEA;
    TSmallintField *dstListOVERWARMSEA;
    TSmallintField *dstListGENERALLYSEA20;
    TSmallintField *dstListGENERALLYSEA40;
    TSmallintField *dstListGENERALLYSEA60;
    TSmallintField *dstListGENERALLYSEA80;
    TSmallintField *dstListGENERALLYSEA100;
    TSmallintField *dstListMEDITERRANEANSEA20;
    TSmallintField *dstListMEDITERRANEANSEA40;
    TSmallintField *dstListMEDITERRANEANSEA60;
    TSmallintField *dstListMEDITERRANEANSEA80;
    TSmallintField *dstListMEDITERRANEANSEA100;
    TIntegerField *dstListSYSTEMCAST_ID;
private:	// User declarations
    __fastcall TfrmListCoordDistance(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListCoordDistance(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListCoordDistance *frmListCoordDistance;
//---------------------------------------------------------------------------
#endif
