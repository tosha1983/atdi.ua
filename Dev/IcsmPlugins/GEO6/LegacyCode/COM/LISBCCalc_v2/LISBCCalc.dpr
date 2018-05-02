library LISBCCalc;

{%File '..\bugz.txt'}

uses
  ComServ,
  LISBCCalc_TLB in 'LISBCCalc_TLB.pas',
  ULISBCCalcCOM in 'ULISBCCalcCOM.pas' {CoLISBCCalc: CoClass},
  UShare in '..\Share\UShare.pas',
  LISBCTxServer_TLB in 'c:\Program Files\Borland\Delphi6\Imports\LISBCTxServer_TLB.pas',
  RSAGeography_TLB in 'c:\Program Files\Borland\Delphi6\Imports\RSAGeography_TLB.pas',
  LISPropagation_TLB in 'c:\Program Files\Borland\Delphi6\Imports\LISPropagation_TLB.pas',
  UAntenna in 'UAntenna.pas',
  g1 in 'G1.pas',
  LISBCCoordDist_TLB in 'c:\Program Files\Borland\Delphi6\Imports\LISBCCoordDist_TLB.pas',
  LISProgress_TLB in 'C:\Program Files\Borland\Delphi6\Imports\LISProgress_TLB.pas',
  UReferenceNetwork in 'UReferenceNetwork.pas',
  UZRoutine in 'UZRoutine.pas',
  UZDialog in 'UZDialog.pas',
  UEminDialog in 'UEminDialog.pas' {EminDialog},
  UPR_DVBT2_Share in '..\Share\upr_dvbt2_share.pas',
  UPR_Share in '..\Share\upr_share.pas';

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer;

{$R *.TLB}

{$R *.RES}

begin
end.
