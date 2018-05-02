// LISBCTXLISTIMPL : Implementation of TLISBCTxListImpl (CoClass: LISBCTxList, Interface: ILISBCTxList)

#include <vcl.h>
#pragma hdrstop

#include "LISBCTXLISTIMPL.H"

/////////////////////////////////////////////////////////////////////////////
// TLISBCTxListImpl

TLISBCTxListImpl::~TLISBCTxListImpl()
{
    for (TxVector::iterator i = txVector.begin(); i < txVector.end(); i++)
        try { delete *i; } catch (...) {};
}

STDMETHODIMP TLISBCTxListImpl::AddTx(ILISBCTx* Tx, long* idx)
{
    //  добавить передтчик.
    //  сначала пробегаемся по списку передатчков - проверяем налиие такого по Ид
    //  если нет - добавляем в конец
  try {
    TxVector::iterator vi;
    long TxId, curTxId;
    OLECHECK(Tx->get_id(&TxId));
    long i = 0;
    for(vi = txVector.begin(); vi < txVector.end(); vi++) {
        long id = 0;
        (*vi)->get_tx()->get_id(&id);
        if (TxId == id) {
            *idx = i;
            return S_OK;
        }
        i++;
    }

    txVector.push_back(new TxListRecord(TxId, Tx));
    *idx = i;

  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);

  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::RemoveTx(ILISBCTx* Tx)
{
  try {
    for(int i = 0; i < txVector.size(); i++)
    {
        TxVector::iterator vi = &txVector[i];
        if ((*vi)->get_tx() == Tx) {
            delete *vi;
            txVector.erase(vi);
            i--;
            // не выходим - ищем возможные повторения
        }
    }
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);

  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::RemoveId(long id)
{
  try {
    for(int i = 0; i < txVector.size(); i++)
    {
        TxVector::iterator vi = &txVector[i];
        long txId = 0;
        (*vi)->get_tx()->get_id(&txId);
        if (txId == id) {
            delete *vi;
            txVector.erase(vi);
            i--;
            // не выходим - ищем возможные повторения
        }
    }

  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);

  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTxList);
  }
  return S_OK;
}

STDMETHODIMP TLISBCTxListImpl::Clear()
{
  try {

    txVector.clear();

  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);

  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_Size(long* Value)
{
  try
  {
    *Value = txVector.size();
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxId(long idx, long* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->id;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_Tx(long idx, ILISBCTx** Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size()) {
        *Value = txVector[idx]->get_tx();
    } else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxUseInCalc(long idx,
  VARIANT_BOOL* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = VARIANT_BOOL(txVector[idx]->usedInCalc);
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxListImpl::set_TxUseInCalc(long idx,
  VARIANT_BOOL Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->usedInCalc = bool(Value);
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxWantInterfere(long idx, double* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->wantedInterfere;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxUnwantInterfere(long idx,
  double* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->unwantedInterfere;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxWantInterfere(long idx, double Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->wantedInterfere = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxUnwantInterfere(long idx,
  double Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->unwantedInterfere = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::SortByUnwantInterfere()
{
  try
  {
    //  bubble sorting algorithm
    for(TxVector::iterator vcur = txVector.begin() + 1; vcur < txVector.end(); vcur++) {
        for(TxVector::iterator vshuttle = vcur + 1; vshuttle < txVector.end(); vshuttle++) {
            if ((*vcur)->unwantedInterfere < (*vshuttle)->unwantedInterfere) {
                TxListRecord *temp = *vcur;
                *vcur = *vshuttle;
                *vshuttle = temp;
            }
        }
    }

  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);

  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::SortByWantInterfere()
{
  try
  {
    //  bubble sorting algorithm
    for(TxVector::iterator vcur = txVector.begin() + 1; vcur < txVector.end(); vcur++) {
        for(TxVector::iterator vshuttle = vcur + 1; vshuttle < txVector.end(); vshuttle++) {
            if ((*vcur)->wantedInterfere < (*vshuttle)->wantedInterfere) {
                TxListRecord *temp = *vcur;
                *vcur = *vshuttle;
                *vshuttle = temp;
            }
        }
    }

  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);

  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTxList);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxListImpl::get_TxUnwantedKind(long idx,
  signed_char* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->unwantedKind;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxWantedKind(long idx,
  signed_char* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->wantedKind;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxUnwantedKind(long idx,
  signed_char Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->unwantedKind = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxWantedKind(long idx,
  signed_char Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->wantedKind = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxListImpl::set_TxAzimuth(long idx, double Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->azimuth = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxDistance(long idx, double Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->distance = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxZoneOverlapping(long idx,
  double Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->zoneOverlapping = Value;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxAzimuth(long idx, double* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->azimuth;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxDistance(long idx, double* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->distance;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::get_TxZoneOverlapping(long idx,
  double* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = txVector[idx]->zoneOverlapping;
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxListImpl::get_TxShowOnMap(long idx,
  VARIANT_BOOL* Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        *Value = VARIANT_BOOL(txVector[idx]->showOnMap);
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxListImpl::set_TxShowOnMap(long idx,
  VARIANT_BOOL Value)
{
  try
  {
    if (idx >= 0 && idx < txVector.size())
        txVector[idx]->showOnMap = bool(Value);
    else
        return Error("Индекс передатчика вне границ списка", IID_ILISBCTxList);
  } catch(std::exception &e) {
    return Error(e.what(), IID_ILISBCTxList);
  }
  return S_OK;
};



