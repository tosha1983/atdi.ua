// COLISBCSTORAGEIMPL : Implementation of TCoLisBcStorageImpl (CoClass: CoLisBcStorage, Interface: ILisBcStorage)

#include <vcl.h>
#pragma hdrstop

#include "CoStorageImpl.h"                  
#include "TxBroker.h"
#include "uMainDm.h"

/////////////////////////////////////////////////////////////////////////////
// TCoLisBcStorageImpl

STDMETHODIMP TCoLisBcStorageImpl::LoadObject(LPUNKNOWN obj)
{
    return dmMain->LoadObject(obj);
}

STDMETHODIMP TCoLisBcStorageImpl::SaveObject(LPUNKNOWN obj)
{
    return dmMain->SaveObject(obj);
}

STDMETHODIMP TCoLisBcStorageImpl::LoadDetails(LPUNKNOWN obj)
{
    return dmMain->LoadDetails(obj);
}


