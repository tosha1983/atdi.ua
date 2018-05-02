inherited frmListSystemCast: TfrmListSystemCast
  Tag = 35
  Left = 334
  Top = 152
  Caption = #1057#1080#1089#1090#1077#1084#1080' '#1084#1086#1074#1083#1077#1085#1085#1103
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1057#1080#1089#1090#1077#1084#1080' '#1084#1086#1074#1083#1077#1085#1085#1103
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from SYSTEMCAST'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into SYSTEMCAST'
      
        '  (ID, CODE, DESCRIPTION, TYPESYSTEM, CLASSWAVE, FREQFROM, FREQT' +
        'O, '
      'NUMDIAPASON, '
      '   RELATIONNAME, ENUMVAL, is_used )'
      'values'
      
        '  (:ID, :CODE, :DESCRIPTION, :TYPESYSTEM, :CLASSWAVE, :FREQFROM,' +
        ' '
      ':FREQTO, '
      '   :NUMDIAPASON, :RELATIONNAME, :ENUMVAL, :is_used )')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CODE,'
      '  DESCRIPTION,'
      '  TYPESYSTEM,'
      '  CLASSWAVE,'
      '  FREQFROM,'
      '  FREQTO,'
      '  NUMDIAPASON,'
      '  RELATIONNAME,'
      '  ENUMVAL,'
      '  is_used'
      'from SYSTEMCAST '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, CODE, DESCRIPTION, TYPESYSTEM, CLASSWAVE, FREQFROM, F' +
        'REQTO, NUMDIAPASON, RELATIONNAME, ENUMVAL, is_used'
      'from SYSTEMCAST')
    ModifySQL.Strings = (
      'update SYSTEMCAST'
      'set'
      '  CODE = :CODE,'
      '  DESCRIPTION = :DESCRIPTION,'
      '  TYPESYSTEM = :TYPESYSTEM,'
      '  CLASSWAVE = :CLASSWAVE,'
      '  FREQFROM = :FREQFROM,'
      '  FREQTO = :FREQTO,'
      '  NUMDIAPASON = :NUMDIAPASON,'
      '  RELATIONNAME = :RELATIONNAME,'
      '  ENUMVAL = :ENUMVAL,'
      '  is_used = :is_used'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'SYSTEMCAST.ID'
      Required = True
      Visible = False
    end
    object dstListCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'SYSTEMCAST.CODE'
      Size = 4
    end
    object dstListIS_USED: TSmallintField
      Alignment = taCenter
      DisplayLabel = #1042#1080#1082
      DisplayWidth = 7
      FieldName = 'IS_USED'
      OnGetText = dstListIS_USEDGetText
      OnSetText = dstListIS_USEDSetText
    end
    object dstListDESCRIPTION: TIBStringField
      DisplayLabel = #1044#1077#1089#1082#1088#1080#1087#1090#1086#1088
      FieldName = 'DESCRIPTION'
      Origin = 'SYSTEMCAST.DESCRIPTION'
      Size = 32
    end
    object dstListTYPESYSTEM: TSmallintField
      Alignment = taCenter
      DisplayLabel = #1058#1080#1087
      DisplayWidth = 3
      FieldName = 'TYPESYSTEM'
      Origin = 'SYSTEMCAST.TYPESYSTEM'
      OnGetText = dstListTYPESYSTEMGetText
    end
    object dstListCLASSWAVE: TIBStringField
      Alignment = taCenter
      DisplayLabel = #1050#1083#1072#1089' '#1093#1074#1080#1083#1100
      DisplayWidth = 3
      FieldName = 'CLASSWAVE'
      Origin = 'SYSTEMCAST.CLASSWAVE'
      Size = 4
    end
    object dstListFREQFROM: TFloatField
      DisplayLabel = #1044#1110#1072#1087#1072#1079#1086#1085' '#1074#1110#1076
      FieldName = 'FREQFROM'
      Origin = 'SYSTEMCAST.FREQFROM'
      DisplayFormat = '0.000'
    end
    object dstListFREQTO: TFloatField
      DisplayLabel = #1044#1110#1072#1087#1072#1079#1086#1085' '#1076#1086
      FieldName = 'FREQTO'
      Origin = 'SYSTEMCAST.FREQTO'
      DisplayFormat = '0.000'
    end
    object dstListNUMDIAPASON: TSmallintField
      DisplayLabel = #1053#1086#1084#1077#1088' '#1076#1110#1072#1087#1072#1079#1086#1085#1091
      DisplayWidth = 7
      FieldName = 'NUMDIAPASON'
      Origin = 'SYSTEMCAST.NUMDIAPASON'
    end
    object dstListRELATIONNAME: TIBStringField
      DisplayLabel = #1058#1072#1073#1083#1080#1094#1103
      DisplayWidth = 32
      FieldName = 'RELATIONNAME'
      Origin = 'SYSTEMCAST.RELATIONNAME'
      Required = True
      Size = 64
    end
    object dstListENUMVAL: TSmallintField
      DisplayLabel = #1047#1085'.'
      DisplayWidth = 3
      FieldName = 'ENUMVAL'
      Origin = 'SYSTEMCAST.ENUMVAL'
      Required = True
    end
  end
end
