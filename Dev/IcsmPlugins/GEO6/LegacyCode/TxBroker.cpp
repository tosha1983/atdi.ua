//---------------------------------------------------------------------------
//  брокер объектов передатчиков.
//  управляет наличием объектов в памяти и доступом к ним
//  каждый, кто нуждается в доступе к объекту передатчика,
//  должен обращаться к брокеру
//
#pragma hdrstop
#include <atl\atlmod.h>
#include "TxBroker.h"
#include "uMainDm.h"
#include <memory>
#include "LISBC_TLB.h"
#include <Registry.hpp>
#include <ComObj.hpp>
//---------------------------------------------------------------------------
#pragma package(smart_init)
//  карта брокера
typedef std::map<long, ILISBCTx*> TxMap;
static TxMap txMap;
//  сам брокер
TxBroker txBroker;
ILisBcStoragePtr storage;

const unsigned extraRef = 0;  // protection extra references

//---------------------------------------------------------------------------

TxBroker::TxBroker()
{
}

//  дать ссылку на передатчик с id = objId
ILISBCTx* TxBroker::GetTx(long objId, GUID clsid)
{
    ILISBCTx* & value = txMap[objId];
    if (value == NULL) {
        ILISBCTxPtr tx;
        OleCheck(tx.CreateInstance(clsid));
        value = tx;
        long c1 = value->AddRef() - 1;
        tx.Unbind();
        for (unsigned i = 0; i < extraRef; i++)  // protection extra references
            value->AddRef();
    }

    long id = 0;
    value->get_id(&id);
    if (id == 0)
    {
        if (!storage.IsBound())
            if (storage.CreateInstance(CLSID_CoLisBcStorage) != S_OK)
                MessageBox(NULL, "TxBroker(): Не могу создать объект CLSID_CoLisBcStorage", "Ошибка", MB_ICONHAND);

        OleCheck(value->init((long)(ILisBcStorage*)storage, objId));
    }

    return value;
}

TxBroker::~TxBroker()
{
    // Unbind() должны вызываться в деструкторах
    /*
    std::map<int, TCOMILISBCTx>::iterator i;
    for (i = txMap.begin(); i != txMap.end(); i++)
        if (i->second.IsBound())
            i->second.Unbind();
    */
}

void TxBroker::EnsureList(ILISBCTxList* pTxList, TProgressBar *pb)
{
    TCOMILISBCTxList txList(pTxList, true);
    if (pb) {
        pb->Min = 0;
        pb->Position = 0;
        pb->Max = txList.Size;
        pb->Visible = true;
        pb->Update();
    }

    try {
        EnsureListInternal(pTxList, pb, IID_ILISBCTx);
        EnsureListInternal(pTxList, pb, IID_ILisBcDigAllot);
    } __finally {
        if (pb) {
            pb->Visible = false;
            pb->Update();
        }
    }

    #ifdef _DEBUG
        /*
        int notFetchedNum = 0;
        for (int i = 0; i < txList.Size; i++) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            if (tx.IsBound() && !(bool)tx.is_fetched)
                notFetchedNum++;
        }
        */
        //MessageBox(NULL, AnsiString(notFetchedNum).c_str(), "EnsureList: not fecthed", MB_ICONINFORMATION);
    #endif
}

void TxBroker::EnsureListInternal(ILISBCTxList* pTxList, TProgressBar *pb, GUID iid)
{
    String qryObjClause;
    String qryDetlClause;

    if (iid == IID_ILISBCTx)
    {
        qryObjClause = selectTxClause;
        qryDetlClause = selectTxDetlClause;
    }
    else if (iid == IID_ILisBcDigAllot)
    {
        qryObjClause = selectAllotClause;
        qryDetlClause = selectAllotDetlClause;
    }
    else
        throw *(new Exception("Unknow object type GUID in TxBroker::EnsureListInternal()"));

    TCOMILISBCTxList txList(pTxList, true);
    int maxNo = 1499; //  for interbase 6.0

    std::auto_ptr<TIBSQL> sqlObj(new TIBSQL(Application));
    sqlObj->Database = dmMain->dbMain;
    std::auto_ptr<TIBSQL> sqlDetl(new TIBSQL(Application));
    sqlDetl->Database = dmMain->dbMain;

    int i = 0;
    while (i < txList.Size) {
        AnsiString asTxIds(" (");
        int start = i;
        int j = 0;
        while (j < maxNo && i < txList.Size) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            bool isTx = false;
            if (iid == IID_ILISBCTx)
            {
                TCOMILisBcDigAllot allot;
                tx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot);
                // load only txs, no allotments
                isTx = !(allot.IsBound());
            } else if (iid == IID_ILisBcDigAllot) {
                TCOMILisBcDigAllot allot;
                tx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot);
                // load only allots
                isTx = allot.IsBound();
            }
            if (isTx && !(bool)tx.is_fetched) {
                asTxIds = asTxIds + tx.id + ",";
                j++;
            }
            i++;
        }

        if (j > 0) {
            asTxIds[asTxIds.Length()] = ')';
            sqlObj->Close();
            sqlDetl->Close();

            try {
                sqlObj->SQL->Text = qryObjClause + asTxIds + " order by TX.ID";
                if (iid == IID_ILISBCTx)
                    sqlDetl->SQL->Text = qryDetlClause + asTxIds + " order by STA_ID";
                else if (iid == IID_ILisBcDigAllot)
                    sqlDetl->SQL->Text = qryDetlClause + asTxIds + " order by ALLOT_ID, s.CONTOUR_ID, POINT_NO";

                for (sqlObj->ExecQuery(), sqlDetl->ExecQuery(); !sqlObj->Eof; sqlObj->Next()) {

                    int id = sqlObj->FieldByName("ID")->AsInteger;
                    TCOMILISBCTx tx(GetTx(id), true); // no need to pick class - object is already initialized
                    if (!(bool)tx.is_fetched)
                    {
                        HRESULT hr = tx.loadFromQuery((long)sqlObj.get());
                        if (!SUCCEEDED(hr))
                            ;
                        // results are sorted - synchronize them
                        while (!sqlDetl->Eof && sqlDetl->FieldByName("OBJ_ID")->AsInteger < id)
                            sqlDetl->Next();
                        if (!sqlDetl->Eof && sqlDetl->FieldByName("OBJ_ID")->AsInteger == id)
                        {
                            bool detlsLoaded = false; //tx->get_???
                            if (!detlsLoaded)
                                hr = tx.loadFromQuery((long)sqlDetl.get());
                        }
                    }
                    if (pb)
                    {
                        pb->StepBy(1);
                        pb->Update();
                    }
                }
            } catch(Exception &e) {
                Application->MessageBox((e.Message+"\n\nSQL obj:\n"+sqlObj->SQL->Text+
                                                   "\n\nSQL details:\n"+sqlDetl->SQL->Text).c_str(),
                                        "Error loading list",
                                        MB_ICONHAND);
            }
        }
    }
}

void __fastcall TxBroker::Unload(ILISBCTx *tx)
{
    long id;
    tx->get_id(&id);
    if (txMap.find(id) != txMap.end())
    {
        long cnt = tx->AddRef();
        tx->Release();
    }
}

void __fastcall TxBroker::UnloadAll(bool force)
{
    TxMap::iterator i;
    String rep;
    for (i = txMap.begin(); i != txMap.end(); i++)
    {
        ILISBCTx *tx = i->second;
        if (tx)
        {
            try {
                if ((tx->AddRef() <= 2 + extraRef) || force)
                {
                    txMap.erase(i);
                    while(tx->Release() > 1)
                        ; // ссылка карты + protection extra references
                }
                tx->Release(); // ссылка AddRef()
            } catch (...) {
                // сбой - скорее всего, память уже освобождена. выкинуть нахуй
                txMap.erase(i);
            }
        } else
            txMap.erase(i);
    }
}

long __fastcall TxBroker::GetCount()
{
    return txMap.size();
}

String __fastcall TxBroker::GetInfo()
{
    TxMap::iterator i;
    String rep;
    for (i = txMap.begin(); i != txMap.end(); i++)
    {
        ILISBCTx *tx = i->second;
        long id = i->first;
        String line = String().sprintf("#%d (0x%08x): ", id, (unsigned)tx);
        try {
            long cnt = tx->AddRef() - 1;
            tx->Release();
            line += String().sprintf("%d\n", cnt);
        } catch (Exception &e) {
            line += String().sprintf("exception '%s'\n", e.Message.c_str());
        } catch (...) {
            line += String("unknown exception\n");
        }
        rep += line;
    }
    return String().sprintf("Total: %d transmitters\nProtection extra ref = %d\n\n", txMap.size(), extraRef) + rep;
}
