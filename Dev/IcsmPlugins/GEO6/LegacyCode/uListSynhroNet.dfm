inherited frmListSynhroNet: TfrmListSynhroNet
  Tag = 45
  Left = 789
  Top = 309
  Caption = #1057#1080#1085#1093#1088#1086#1085#1085#1110' '#1084#1077#1088#1077#1078#1110
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1057#1080#1085#1093#1088#1086#1085#1085#1110' '#1084#1077#1088#1077#1078#1110
    inherited dgrList: TDBGrid
      Columns = <
        item
          Expanded = False
          FieldName = 'SYNHRONETID'
          Width = 92
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'TYPENAME'
          Visible = True
        end>
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from SYNHROFREQNET '
      'where ID = :ID')
    InsertSQL.Strings = (
      'insert into  SYNHROFREQNET ('
      #9'ID, '
      #9'SYNHRONETID, '
      #9'TYPESYNHRONET_ID)'
      'values ('
      #9':ID, '
      #9':SYNHRONETID, '
      #9':TYPESYNHRONET_ID)')
    RefreshSQL.Strings = (
      'select '
      #9'SFN.ID, '
      #9'SFN.SYNHRONETID, '
      #9'SFN.TYPESYNHRONET_ID,'
      #9'TN.TYPENAME '
      'from SYNHROFREQNET SFN'
      
        'left outer join  TYPESYNHRONET TN on (TN.ID = SFN.TYPESYNHRONET_' +
        'ID)'
      'where SFN.ID = :ID')
    SelectSQL.Strings = (
      'select '
      #9'SFN.ID, '
      #9'SFN.SYNHRONETID, '
      #9'SFN.TYPESYNHRONET_ID,'
      #9'TN.TYPENAME '
      'from SYNHROFREQNET SFN'
      
        'left outer join  TYPESYNHRONET TN on (TN.ID = SFN.TYPESYNHRONET_' +
        'ID)')
    ModifySQL.Strings = (
      'update SYNHROFREQNET set'
      #9'SYNHRONETID = :SYNHRONETID, '
      #9'TYPESYNHRONET_ID = :TYPESYNHRONET_ID'
      'where ID = :ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'SYNHROFREQNET.ID'
      Required = True
      Visible = False
    end
    object dstListSYNHRONETID: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'SYNHRONETID'
      Origin = 'SYNHROFREQNET.SYNHRONETID'
      Size = 30
    end
    object dstListTYPESYNHRONET_ID: TIntegerField
      FieldName = 'TYPESYNHRONET_ID'
      Origin = 'SYNHROFREQNET.TYPESYNHRONET_ID'
      Required = True
      Visible = False
    end
    object dstListTYPENAME: TIBStringField
      DisplayLabel = #1058#1080#1087
      FieldKind = fkLookup
      FieldName = 'TYPENAME'
      LookupDataSet = ibqTypeSFN
      LookupKeyFields = 'ID'
      LookupResultField = 'TYPENAME'
      KeyFields = 'TYPESYNHRONET_ID'
      Size = 32
      Lookup = True
    end
  end
  object ibqTypeSFN: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, TYPENAME from TYPESYNHRONET')
    Left = 68
    Top = 188
    object ibqTypeSFNID: TIntegerField
      FieldName = 'ID'
      Origin = 'TYPESYNHRONET.ID'
      Required = True
    end
    object ibqTypeSFNTYPENAME: TIBStringField
      FieldName = 'TYPENAME'
      Origin = 'TYPESYNHRONET.TYPENAME'
      Size = 32
    end
  end
end
