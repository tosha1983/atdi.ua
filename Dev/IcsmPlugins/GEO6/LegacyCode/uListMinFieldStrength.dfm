inherited frmListMinFieldStrength: TfrmListMinFieldStrength
  Tag = 23
  Caption = #1052#1110#1085#1110#1084#1072#1083#1100#1085#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1052#1110#1085#1110#1084#1072#1083#1100#1085#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from MINSTRENGTHFIELD'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into MINSTRENGTHFIELD'
      '  (ID, TYPEAREA, TYPESERVICE, MINFIELDSTENGTH, SYSTEMCAST_ID)'
      'values'
      
        '  (:ID, :TYPEAREA, :TYPESERVICE, :MINFIELDSTENGTH, :SYSTEMCAST_I' +
        'D)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  SYSTEMCAST_ID,'
      '  TYPESERVICE,'
      '  TYPEAREA,'
      '  MINFIELDSTENGTH'
      'from MINSTRENGTHFIELD '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, TYPEAREA, TYPESERVICE, MINFIELDSTENGTH, SYSTEMCAST_ID' +
        ' '
      'from MINSTRENGTHFIELD'
      'where SYSTEMCAST_ID = :GRP_ID '
      'order by TYPEAREA ')
    ModifySQL.Strings = (
      'update MINSTRENGTHFIELD'
      'set'
      '  TYPEAREA = :TYPEAREA,'
      '  TYPESERVICE = :TYPESERVICE,'
      '  MINFIELDSTENGTH = :MINFIELDSTENGTH,'
      '  SYSTEMCAST_ID = :SYSTEMCAST_ID'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'MINSTRENGTHFIELD.ID'
      Required = True
      Visible = False
    end
    object dstListTYPEAREA: TSmallintField
      DisplayLabel = #1058#1080#1087' '#1079#1072#1089#1090#1088#1086#1081#1082#1080
      FieldName = 'TYPEAREA'
      Origin = 'MINSTRENGTHFIELD.TYPEAREA'
    end
    object dstListTYPESERVICE: TSmallintField
      DisplayLabel = #1052#1086#1085#1086'/'#1057#1090#1077#1088#1077#1086
      FieldName = 'TYPESERVICE'
      Origin = 'MINSTRENGTHFIELD.TYPESERVICE'
    end
    object dstListMINFIELDSTENGTH: TFloatField
      DisplayLabel = #1052#1080#1085'. '#1085#1072#1087#1088#1103#1078'.'
      FieldName = 'MINFIELDSTENGTH'
      Origin = 'MINSTRENGTHFIELD.MINFIELDSTENGTH'
      DisplayFormat = '0.000'
    end
    object dstListSYSTEMCAST_ID: TIntegerField
      FieldName = 'SYSTEMCAST_ID'
      Origin = 'MINSTRENGTHFIELD.SYSTEMCAST_ID'
      Required = True
      Visible = False
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select SYSTEMCAST_ID from MINSTRENGTHFIELD'
      'where ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'select ID, CODE from SYSTEMCAST')
  end
end
