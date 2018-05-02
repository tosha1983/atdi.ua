inherited frmListBlockDAB: TfrmListBlockDAB
  Tag = 7
  Left = 298
  Caption = #1063#1072#1089#1090#1086#1090#1085#1110' '#1073#1083#1086#1082#1080' '#1062#1056#1052
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1063#1072#1089#1090#1086#1090#1085#1110' '#1073#1083#1086#1082#1080' '#1062#1056#1052
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from BLOCKDAB'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into BLOCKDAB'
      
        '  (ID, NAME, CENTREFREQ, FREQFROM, FREQTO, LOWERGUARDBAND, UPPER' +
        'GUARDBAND)'
      'values'
      
        '  (:ID, :NAME, :CENTREFREQ, :FREQFROM, :FREQTO, :LOWERGUARDBAND,' +
        ' :UPPERGUARDBAND)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAME,'
      '  CENTREFREQ,'
      '  FREQFROM,'
      '  FREQTO,'
      '  LOWERGUARDBAND,'
      '  UPPERGUARDBAND'
      'from BLOCKDAB '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, NAME, CENTREFREQ, FREQFROM, FREQTO, LOWERGUARDBAND, U' +
        'PPERGUARDBAND from BLOCKDAB'
      'order by 2')
    ModifySQL.Strings = (
      'update BLOCKDAB'
      'set'
      '  NAME = :NAME,'
      '  CENTREFREQ = :CENTREFREQ,'
      '  FREQFROM = :FREQFROM,'
      '  FREQTO = :FREQTO,'
      '  LOWERGUARDBAND = :LOWERGUARDBAND,'
      '  UPPERGUARDBAND = :UPPERGUARDBAND'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'BLOCKDAB.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'BLOCKDAB.NAME'
      Size = 4
    end
    object dstListCENTREFREQ: TFloatField
      DisplayLabel = #1062#1077#1085#1090#1088'.'#1095#1072#1089#1090#1086#1090#1072
      DisplayWidth = 7
      FieldName = 'CENTREFREQ'
      Origin = 'BLOCKDAB.CENTREFREQ'
    end
    object dstListFREQFROM: TFloatField
      DisplayLabel = #1054#1090' '
      DisplayWidth = 7
      FieldName = 'FREQFROM'
      Origin = 'BLOCKDAB.FREQFROM'
    end
    object dstListFREQTO: TFloatField
      DisplayLabel = #1044#1086
      DisplayWidth = 7
      FieldName = 'FREQTO'
      Origin = 'BLOCKDAB.FREQTO'
    end
    object dstListLOWERGUARDBAND: TFloatField
      DisplayLabel = #1053#1080#1078'. '#1084#1077#1078#1072
      DisplayWidth = 7
      FieldName = 'LOWERGUARDBAND'
      Origin = 'BLOCKDAB.LOWERGUARDBAND'
    end
    object dstListUPPERGUARDBAND: TFloatField
      DisplayLabel = #1042#1077#1088#1093'. '#1084#1077#1078#1072
      DisplayWidth = 7
      FieldName = 'UPPERGUARDBAND'
      Origin = 'BLOCKDAB.UPPERGUARDBAND'
    end
  end
end
