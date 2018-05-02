inherited frmListLicense: TfrmListLicense
  Tag = 22
  Left = 270
  Top = 271
  Width = 679
  Height = 256
  Caption = #1051#1110#1094#1077#1085#1079#1110#1111
  PixelsPerInch = 96
  TextHeight = 13
  inherited splTree: TSplitter
    Height = 204
  end
  inherited tbrList: TToolBar
    Width = 671
  end
  inherited panList: TPanel
    Width = 547
    Height = 204
    Caption = #1051#1110#1094#1077#1085#1079#1110#1111
    inherited dgrList: TDBGrid
      Width = 547
      Height = 146
    end
    inherited stPath: TStaticText
      Width = 547
    end
    inherited panSearch: TPanel
      Top = 163
      Width = 547
    end
  end
  inherited panTree: TPanel
    Height = 204
    inherited trvList: TTreeView
      Height = 187
    end
    inherited stxListQuantity: TStaticText
      Top = 187
    end
  end
  inherited imlList: TImageList
    Left = 276
    Top = 0
  end
  inherited aclList: TActionList
    Top = 4
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from LICENSE'
      'where  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into LICENSE'
      '  (ID, CODE, DATEFROM, DATETO, NUMLICENSE, OWNER_ID,  '
      '   TIMEFROM, TIMETO, CALLSIGN, ANNUL)'
      'values'
      '  (:ID, :CODE, :DATEFROM, :DATETO, :NUMLICENSE, :OWNER_ID, '
      '   :TIMEFROM, :TIMETO, :CALLSIGN, :ANNUL)')
    RefreshSQL.Strings = (
      'select '#9'L.ID, '
      #9'L.CODE, '
      #9'L.DATEFROM, '
      #9'L.DATETO, '
      #9'L.NUMLICENSE, '
      #9'L.OWNER_ID, '
      #9'O.NAMEORGANIZATION,'
      #9'L.TIMEFROM, '
      #9'L.TIMETO, '
      #9'L.CALLSIGN,'
      #9'L.ANNUL'
      'from LICENSE L'
      'left outer join OWNER O on (O.ID = L.OWNER_ID)'
      'where  L.ID = :ID'
      'order by L.CODE')
    SelectSQL.Strings = (
      'select '#9'L.ID, '
      #9'L.CODE, '
      #9'L.DATEFROM, '
      #9'L.DATETO, '
      #9'L.NUMLICENSE, '
      #9'L.OWNER_ID, '
      #9'O.NAMEORGANIZATION,'
      #9'L.TIMEFROM, '
      #9'L.TIMETO,'
      #9'L.CALLSIGN,'
      #9'L.ANNUL'
      'from LICENSE L'
      'left outer join OWNER O on (O.ID = L.OWNER_ID)'
      'where CODE = :GRP_ID')
    ModifySQL.Strings = (
      'update LICENSE'
      'set'
      '  CODE = :CODE,'
      '  DATEFROM = :DATEFROM,'
      '  DATETO = :DATETO,'
      '  NUMLICENSE = :NUMLICENSE,'
      '  OWNER_ID = :OWNER_ID,'
      '  TIMEFROM = :TIMEFROM,'
      '  TIMETO = :TIMETO,'
      '  CALLSIGN = :CALLSIGN,'
      '  ANNUL = :ANNUL'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'OWNER.ID'
      Visible = False
    end
    object dstListCODE: TIntegerField
      FieldName = 'CODE'
      Origin = 'LICENSE.CODE'
      Required = True
      Visible = False
    end
    object dstListNAMEORGANIZATION: TIBStringField
      Tag = 5
      DisplayLabel = #1042#1083#1072#1089#1085#1080#1082
      DisplayWidth = 32
      FieldKind = fkLookup
      FieldName = 'NAMEORGANIZATION'
      LookupDataSet = ibqOwner
      LookupKeyFields = 'ID'
      LookupResultField = 'NAMEORGANIZATION'
      KeyFields = 'OWNER_ID'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
      Lookup = True
    end
    object dstListNUMLICENSE: TIBStringField
      DisplayLabel = #1053#1086#1084#1077#1088' '#1083#1110#1094#1077#1085#1079#1110#1111
      DisplayWidth = 32
      FieldName = 'NUMLICENSE'
      Origin = 'LICENSE.NUMLICENSE'
      Size = 64
    end
    object dstListCALLSIGN: TIBStringField
      DisplayLabel = #1055#1086#1079#1099#1074#1085#1086#1081
      DisplayWidth = 20
      FieldName = 'CALLSIGN'
      Origin = 'LICENSE.CALLSIGN'
      Size = 32
    end
    object dstListDATEFROM: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1087#1086#1095#1072#1090#1082#1091
      FieldName = 'DATEFROM'
      Origin = 'LICENSE.DATEFROM'
    end
    object dstListDATETO: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1082#1110#1085#1094#1103
      FieldName = 'DATETO'
      Origin = 'LICENSE.DATETO'
    end
    object dstListOWNER_ID: TIntegerField
      FieldName = 'OWNER_ID'
      Origin = 'LICENSE.OWNER_ID'
      Required = True
      Visible = False
    end
    object dstListTIMEFROM: TTimeField
      DisplayLabel = #1055#1086#1095#1072#1090#1086#1082' '#1084#1086#1074#1083'.'
      FieldName = 'TIMEFROM'
      Origin = 'LICENSE.TIMEFROM'
    end
    object dstListTIMETO: TTimeField
      DisplayLabel = #1050#1110#1085#1077#1094#1100' '#1084#1086#1074#1083
      FieldName = 'TIMETO'
      Origin = 'LICENSE.TIMETO'
    end
    object dstListANNUL: TSmallintField
      Alignment = taCenter
      DisplayLabel = #1040#1085#1085#1091#1083#1080#1088#1086#1074#1072#1085#1072
      DisplayWidth = 5
      FieldName = 'ANNUL'
      Origin = 'LICENSE.ANNUL'
      OnGetText = dstListANNULGetText
      OnSetText = dstListANNULSetText
    end
  end
  object ibqOwner: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAMEORGANIZATION from OWNER')
    Left = 56
    Top = 124
    object ibqOwnerID: TIntegerField
      FieldName = 'ID'
      Origin = 'OWNER.ID'
      Required = True
    end
    object ibqOwnerNAMEORGANIZATION: TIBStringField
      FieldName = 'NAMEORGANIZATION'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
  end
  object dsOwner: TDataSource
    DataSet = ibqOwner
    Left = 152
    Top = 160
  end
end
