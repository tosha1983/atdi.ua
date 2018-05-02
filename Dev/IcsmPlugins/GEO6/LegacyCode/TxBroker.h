//---------------------------------------------------------------------------
//  брокер объектов передатчиков.
//  управляет наличием объектов в памяти и доступом к ним
//  каждый, кто нуждается в доступе к объекту передатчика,
//  должен обращаться к брокеру
//
#ifndef TxBrokerH
#define TxBrokerH
#include <map.h>
#include <ComCtrls.hpp>
#include "LISBCTxServer_TLB.h"
class TxBroker {
private:
    void EnsureListInternal(ILISBCTxList* pTxList, TProgressBar *pb, GUID iid);
public:
    ILISBCTx* GetTx(long objId, GUID clsid = CLSID_LISBCTx); //  дать ссылку на передатчик с id = objId
    void EnsureList(ILISBCTxList* pTxList, TProgressBar *pb);
    TxBroker();
    ~TxBroker();
    void __fastcall Unload(ILISBCTx *tx);
    void __fastcall UnloadAll(bool force = false);
    long __fastcall GetCount();
    String __fastcall GetInfo();
};

extern TxBroker txBroker;
//---------------------------------------------------------------------------
#endif
