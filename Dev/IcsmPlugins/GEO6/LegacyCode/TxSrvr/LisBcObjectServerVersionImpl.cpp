// COLISBCOBJECTSERVERVERSIONIMPL : Implementation of TCoLisBcObjectServerVersionImpl (CoClass: CoLisBcObjectServerVersion, Interface: ILisBcObjectServerVersion)

#include <vcl.h>
#pragma hdrstop

#include "LisBcObjectServerVersionImpl.h"

/////////////////////////////////////////////////////////////////////////////
// TCoLisBcObjectServerVersionImpl

STDMETHODIMP TLisBcObjectServerVersionImpl::GetVersion(double* Value)
{
    *Value = (double)BC_TX_SERVER_VERSION;
    return S_OK;
}

