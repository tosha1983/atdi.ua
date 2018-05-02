inherited frmListCarrierGuardInterval: TfrmListCarrierGuardInterval
  Tag = 8
  Left = 488
  Top = 184
  Width = 514
  Height = 311
  Caption = #1053#1077#1089#1091#1095#1080' '#1090#1072' '#1079#1072#1093#1080#1089#1085#1110' '#1110#1085#1090#1077#1088#1074#1072#1083#1080' '#1094#1080#1092#1088#1086#1074#1086#1075#1086' '#1084#1086#1074#1083#1077#1085#1085#1103
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited tbrList: TToolBar
    Width = 506
  end
  inherited panList: TPanel
    Width = 506
    Height = 259
    Caption = #1053#1077#1089#1091#1095#1080' '#1090#1072' '#1079#1072#1093#1080#1089#1085#1110' '#1110#1085#1090#1077#1088#1074#1072#1083#1080' '#1094#1080#1092#1088#1086#1074#1086#1075#1086' '#1084#1086#1074#1083#1077#1085#1085#1103
    inherited dgrList: TDBGrid
      Width = 506
      Height = 259
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from CARRIERGUARDINTERVAL'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into CARRIERGUARDINTERVAL'
      
        '  (ID, CODE, NUMBERCARRIER, TIMEUSEFULINTERVAL, FREQINTERVAL, FR' +
        'EQBOUNDINTERVAL, '
      '   TIMECURRIERGUARD, NAMECURRIERGUARD)'
      'values'
      
        '  (:ID, :CODE, :NUMBERCARRIER, :TIMEUSEFULINTERVAL, :FREQINTERVA' +
        'L, :FREQBOUNDINTERVAL, '
      '   :TIMECURRIERGUARD, :NAMECURRIERGUARD)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CODE,'
      '  NUMBERCARRIER,'
      '  TIMEUSEFULINTERVAL,'
      '  FREQINTERVAL,'
      '  FREQBOUNDINTERVAL,'
      '  TIMECURRIERGUARD,'
      '  NAMECURRIERGUARD'
      'from CARRIERGUARDINTERVAL '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, CODE, NUMBERCARRIER, TIMEUSEFULINTERVAL, FREQINTERVAL' +
        ', FREQBOUNDINTERVAL, TIMECURRIERGUARD, NAMECURRIERGUARD '
      'from CARRIERGUARDINTERVAL'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update CARRIERGUARDINTERVAL'
      'set'
      '  CODE = :CODE,'
      '  NUMBERCARRIER = :NUMBERCARRIER,'
      '  TIMEUSEFULINTERVAL = :TIMEUSEFULINTERVAL,'
      '  FREQINTERVAL = :FREQINTERVAL,'
      '  FREQBOUNDINTERVAL = :FREQBOUNDINTERVAL,'
      '  TIMECURRIERGUARD = :TIMECURRIERGUARD,'
      '  NAMECURRIERGUARD = :NAMECURRIERGUARD'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'CARRIERGUARDINTERVAL.ID'
      Required = True
      Visible = False
    end
    object dstListCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'CARRIERGUARDINTERVAL.CODE'
      Size = 4
    end
    object dstListNUMBERCARRIER: TSmallintField
      DisplayLabel = #1050#1110#1083#1100#1082'. '#1085#1077#1089#1091#1095#1080#1093
      FieldName = 'NUMBERCARRIER'
      Origin = 'CARRIERGUARDINTERVAL.NUMBERCARRIER'
    end
    object dstListTIMEUSEFULINTERVAL: TIntegerField
      DisplayLabel = #1063#1072#1089' '#1082#1086#1088#1080#1089'. '#1080#1085#1090'.'
      FieldName = 'TIMEUSEFULINTERVAL'
      Origin = 'CARRIERGUARDINTERVAL.TIMEUSEFULINTERVAL'
    end
    object dstListFREQINTERVAL: TIntegerField
      DisplayLabel = #1052#1110#1078' '#1085#1077#1089#1091#1095#1080#1084#1080
      FieldName = 'FREQINTERVAL'
      Origin = 'CARRIERGUARDINTERVAL.FREQINTERVAL'
    end
    object dstListFREQBOUNDINTERVAL: TFloatField
      DisplayLabel = #1052#1110#1078' '#1082#1088#1072#1081'. '#1085#1077#1089#1091#1095#1080#1084#1080
      FieldName = 'FREQBOUNDINTERVAL'
      Origin = 'CARRIERGUARDINTERVAL.FREQBOUNDINTERVAL'
      DisplayFormat = '0.000'
    end
    object dstListTIMECURRIERGUARD: TIntegerField
      DisplayLabel = #1063#1072#1089' '
      DisplayWidth = 7
      FieldName = 'TIMECURRIERGUARD'
      Origin = 'CARRIERGUARDINTERVAL.TIMECURRIERGUARD'
    end
    object dstListNAMECURRIERGUARD: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072' '
      DisplayWidth = 7
      FieldName = 'NAMECURRIERGUARD'
      Origin = 'CARRIERGUARDINTERVAL.NAMECURRIERGUARD'
      Size = 4
    end
  end
end
