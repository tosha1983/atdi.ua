// Borland C++ Builder
// Copyright (c) 1995, 2002 by Borland Software Corporation
// All rights reserved

// (DO NOT EDIT: machine generated header) 'uConnect.pas' rev: 6.00

#ifndef uConnectHPP
#define uConnectHPP

#pragma delphiheader begin
#pragma option push -w-
#pragma option push -Vx
#include <IBDatabase.hpp>	// Pascal unit
#include <ComCtrls.hpp>	// Pascal unit
#include <Buttons.hpp>	// Pascal unit
#include <StdCtrls.hpp>	// Pascal unit
#include <Controls.hpp>	// Pascal unit
#include <Forms.hpp>	// Pascal unit
#include <Graphics.hpp>	// Pascal unit
#include <Classes.hpp>	// Pascal unit
#include <SysUtils.hpp>	// Pascal unit
#include <Windows.hpp>	// Pascal unit
#include <SysInit.hpp>	// Pascal unit
#include <System.hpp>	// Pascal unit

//-- user supplied -----------------------------------------------------------

namespace Uconnect
{
//-- type declarations -------------------------------------------------------
class DELPHICLASS TdlgConnect;
class PASCALIMPLEMENTATION TdlgConnect : public Forms::TForm 
{
	typedef Forms::TForm inherited;
	
__published:
	Stdctrls::TLabel* Label1;
	Stdctrls::TEdit* edtPassword;
	Stdctrls::TButton* btnOk;
	Stdctrls::TButton* btnCancel;
	Stdctrls::TListBox* lbxBases;
	Stdctrls::TLabel* Label2;
	Stdctrls::TButton* btnAdd;
	Stdctrls::TButton* btnDelete;
	Stdctrls::TButton* btnChange;
	Stdctrls::TButton* btnCreate;
	Stdctrls::TEdit* edtName;
	Stdctrls::TEdit* edtRole;
	Stdctrls::TLabel* Label3;
	Stdctrls::TLabel* Label4;
	Comctrls::TStatusBar* StatusBar1;
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall lbxBasesClick(System::TObject* Sender);
	void __fastcall btnAddClick(System::TObject* Sender);
	void __fastcall btnDeleteClick(System::TObject* Sender);
	void __fastcall btnChangeClick(System::TObject* Sender);
	void __fastcall btnCreateClick(System::TObject* Sender);
	void __fastcall btnOkClick(System::TObject* Sender);
	void __fastcall FormShow(System::TObject* Sender);
	void __fastcall FormClose(System::TObject* Sender, Forms::TCloseAction &Action);
	
private:
	Ibdatabase::TIBDatabase* TargetDB;
	
public:
	AnsiString sDatabaseRegPath;
	void __fastcall Init(AnsiString AppRegPath, Ibdatabase::TIBDatabase* DB);
public:
	#pragma option push -w-inl
	/* TCustomForm.Create */ inline __fastcall virtual TdlgConnect(Classes::TComponent* AOwner) : Forms::TForm(AOwner) { }
	#pragma option pop
	#pragma option push -w-inl
	/* TCustomForm.CreateNew */ inline __fastcall virtual TdlgConnect(Classes::TComponent* AOwner, int Dummy) : Forms::TForm(AOwner, Dummy) { }
	#pragma option pop
	#pragma option push -w-inl
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TdlgConnect(void) { }
	#pragma option pop
	
public:
	#pragma option push -w-inl
	/* TWinControl.CreateParented */ inline __fastcall TdlgConnect(HWND ParentWindow) : Forms::TForm(ParentWindow) { }
	#pragma option pop
	
};


//-- var, const, procedure ---------------------------------------------------
extern PACKAGE TdlgConnect* dlgConnect;

}	/* namespace Uconnect */
using namespace Uconnect;
#pragma option pop	// -w-
#pragma option pop	// -Vx

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// uConnect
