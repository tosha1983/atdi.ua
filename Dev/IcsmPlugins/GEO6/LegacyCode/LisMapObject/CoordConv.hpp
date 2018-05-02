// Borland C++ Builder
// Copyright (c) 1995, 2002 by Borland Software Corporation
// All rights reserved

// (DO NOT EDIT: machine generated header) 'CoordConv.pas' rev: 6.00

#ifndef CoordConvHPP
#define CoordConvHPP

#pragma delphiheader begin
#pragma option push -w-
#pragma option push -Vx
#include <Dialogs.hpp>	// Pascal unit
#include <Windows.hpp>	// Pascal unit
#include <Math.hpp>	// Pascal unit
#include <Classes.hpp>	// Pascal unit
#include <SysUtils.hpp>	// Pascal unit
#include <SysInit.hpp>	// Pascal unit
#include <System.hpp>	// Pascal unit

//-- user supplied -----------------------------------------------------------

namespace Coordconv
{
//-- type declarations -------------------------------------------------------
class DELPHICLASS TCoordinateConvertor;
class PASCALIMPLEMENTATION TCoordinateConvertor : public Classes::TComponent 
{
	typedef Classes::TComponent inherited;
	
private:
	AnsiString m_Text;
	double m_Value;
	char m_Direction;
	AnsiString E;
	AnsiString W;
	AnsiString N;
	AnsiString S;
	char axesX;
	char axesY;
	bool IsInit;
	bool isNoDividers;
	bool isSignMandatory;
	void __fastcall SetText(AnsiString _text);
	AnsiString __fastcall GetText();
	void __fastcall SetValue(double val);
	double __fastcall GetValue(void);
	void __fastcall SetDirection(char direction);
	char __fastcall GetDirection(void);
	void __fastcall SetE(AnsiString val);
	void __fastcall SetW(AnsiString val);
	void __fastcall SetN(AnsiString val);
	void __fastcall SetS(AnsiString val);
	void __fastcall SetAxesX(char val);
	void __fastcall SetAxesY(char val);
	AnsiString __fastcall GetE();
	AnsiString __fastcall GetW();
	AnsiString __fastcall GetN();
	AnsiString __fastcall GetS();
	char __fastcall GetAxesX(void);
	char __fastcall GetAxesY(void);
	void __fastcall corrector(AnsiString oldVal, AnsiString newVal);
	double __fastcall fabs(double val);
	
public:
	AnsiString __fastcall CoordToStr(double coord, char _direction);
	double __fastcall StrToCoord(AnsiString TextCoord);
	__fastcall virtual TCoordinateConvertor(Classes::TComponent* AOwner);
	__fastcall virtual ~TCoordinateConvertor(void);
	
__published:
	__property AnsiString Text = {read=GetText, write=SetText};
	__property double Value = {read=GetValue, write=SetValue};
	__property char Direction = {read=GetDirection, write=SetDirection, nodefault};
	__property AnsiString EastLongitude = {read=GetE, write=SetE};
	__property AnsiString WestLongitude = {read=GetW, write=SetW};
	__property AnsiString NorthLatitude = {read=GetN, write=SetN};
	__property AnsiString SouthLatitude = {read=GetS, write=SetS};
	__property char AxesXName = {read=GetAxesX, write=SetAxesX, nodefault};
	__property char AxesYName = {read=GetAxesY, write=SetAxesY, nodefault};
	__property bool NoDividers = {read=isNoDividers, write=isNoDividers, nodefault};
	__property bool SignMandatory = {read=isSignMandatory, write=isSignMandatory, nodefault};
};


//-- var, const, procedure ---------------------------------------------------
extern PACKAGE void __fastcall Register(void);

}	/* namespace Coordconv */
using namespace Coordconv;
#pragma option pop	// -w-
#pragma option pop	// -Vx

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// CoordConv
