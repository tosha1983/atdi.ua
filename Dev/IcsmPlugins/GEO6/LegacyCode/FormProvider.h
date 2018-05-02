//---------------------------------------------------------------------------
//  Обеспечиавет предоставление визуальных форм
//  для заданного типа объекта
//
#include <Windows.h>
#include <Forms.hpp>
#pragma hdrstop

#ifndef FormProviderH
#define FormProviderH


//---------------------------------------------------------------------------
class TfrmBaseList;
class TfrmTxBase;
class TfrmListTransmitters;
class TfrmSelection;
class TfrmSearch;
class TfmMap;
class TfrmExplorer;
class TfrmAllotment;

enum ObjType {
     otACCOUNT_STATES    = 1
    ,otTx                = 2
    ,otBLOCK_DAB         = 7
    ,otDocTemplate       = 19
    ,otEQUIP             = 20
    ,otDocument          = 21
    ,otLICENSES          = 22
    ,otOWNER             = 25
    ,otSITES             = 33
    ,otORGANIZATION      = 36
    ,otSFN               = 45
    ,otDIG_ALLOTMENT     = 48
    ,otDIG_SUBAREAS      = 49
    ,otTxSearch          = 50
};


namespace Lisbctxserver_tlb {
  class ILISBCTx;
};

class TFormProvider {
public:
    void __fastcall DisableApplicationControls(void);
    void __fastcall EnableApplicationControls(void);
    void __fastcall UpdateTransmitters(int ID);
    void __fastcall UpdateStands(int ID);
    TForm* __fastcall ShowList(ObjType dictId, HWND caller, int elementId,
                            String extraCond = String(), String extraCaption = String(), int extraTag = 0);
    TForm* __fastcall ShowTx(Lisbctxserver_tlb::ILISBCTx*);
    TfrmListTransmitters* __fastcall ShowTxList(HWND caller, int elementId, long TxFlags);
    TfrmSelection* __fastcall ShowSelection(int Id);
    TfrmSearch* __fastcall ShowSearch(ObjType ot);
    TfrmExplorer* __fastcall ShowExplorer();
    __fastcall ~TFormProvider();
    TForm * __fastcall ShowPlanning();
    int __fastcall ShowForm(ObjType objType, unsigned objId);
    void __fastcall UpdateViews();
    String __fastcall GetListName(ObjType objType);
    String __fastcall GetObjectName(ObjType objType);
    String __fastcall RemoveAmpersand(String);
    int __fastcall DelimitedPos(String s, String subs);
};


extern TFormProvider FormProvider;


#endif
