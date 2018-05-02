// COCALCPROGRESSIMPL : Implementation of TCoCalcProgressImpl (CoClass: CoCalcProgress, Interface: ILISProgress)

#include <vcl.h>
#pragma hdrstop

#include "COCALCPROGRESSIMPL.H"
#include "uAnalyzer.h"
#pragma link "LISProgress_OCX"

/////////////////////////////////////////////////////////////////////////////
// TCoCalcProgressImpl

STDMETHODIMP TCoCalcProgressImpl::Progress(long* perc)
{ 
    if ( txAnalyzer.DoProgress(*perc) )
        *perc = 101;
    return S_OK;
}

