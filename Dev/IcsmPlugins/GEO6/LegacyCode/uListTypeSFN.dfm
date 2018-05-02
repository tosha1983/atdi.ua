inherited frmListTypeSFN: TfrmListTypeSFN
  Tag = 44
  Left = 259
  Top = 207
  Width = 864
  Height = 240
  Caption = #1058#1080#1087#1080' '#1089#1080#1085#1093#1088#1086#1085#1085#1080#1093' '#1084#1077#1088#1077#1078
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited tbrList: TToolBar
    Width = 856
  end
  inherited panList: TPanel
    Width = 856
    Height = 188
    Caption = #1058#1080#1087#1080' '#1089#1080#1085#1093#1088#1086#1085#1085#1080#1093' '#1084#1077#1088#1077#1078
    inherited dgrList: TDBGrid
      Width = 856
      Height = 188
      Columns = <
        item
          Expanded = False
          FieldName = 'TYPENAME'
          Width = 133
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'NETGEOMETRY'
          Width = 72
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'NUMTRANSMITTERS'
          Width = 69
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'POWER_CENTRANS'
          Width = 75
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'POWER_PERTRANS'
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'COVERAGEAREA'
          Width = 57
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'DIRECTIVITY_CENTRANS'
          Width = 62
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'DIRECTIVITY_PERTRANS'
          Width = 58
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'DISTANCE_ADJACENT_TRANS'
          Width = 62
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'EFFHEIGHTANT_CENTRANS'
          Width = 62
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'EFFHEIGHTANT_PERTRANS'
          Width = 94
          Visible = True
        end>
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from TYPESYNHRONET'
      'where ID = :ID')
    InsertSQL.Strings = (
      'insert into TYPESYNHRONET('
      #9'ID, '
      #9'TYPENAME, '
      #9'NETGEOMETRY, '
      #9'NUMTRANSMITTERS, '
      #9'POWER_CENTRANS, '
      #9'POWER_PERTRANS, '
      #9'COVERAGEAREA, '
      #9'DIRECTIVITY_CENTRANS, '
      #9'DIRECTIVITY_PERTRANS, '
      #9'DISTANCE_ADJACENT_TRANS, '
      #9'EFFHEIGHTANT_CENTRANS, '
      #9'EFFHEIGHTANT_PERTRANS)'
      'values ('#9
      #9':ID, '
      #9':TYPENAME, '
      #9':NETGEOMETRY, '
      #9':NUMTRANSMITTERS, '
      #9':POWER_CENTRANS, '
      #9':POWER_PERTRANS, '
      #9':COVERAGEAREA, '
      #9':DIRECTIVITY_CENTRANS, '
      #9':DIRECTIVITY_PERTRANS, '
      #9':DISTANCE_ADJACENT_TRANS, '
      #9':EFFHEIGHTANT_CENTRANS, '
      #9':EFFHEIGHTANT_PERTRANS )')
    RefreshSQL.Strings = (
      'select '
      #9'ID, '
      #9'TYPENAME, '
      #9'NETGEOMETRY, '
      #9'NUMTRANSMITTERS, '
      #9'POWER_CENTRANS, '
      #9'POWER_PERTRANS, '
      #9'COVERAGEAREA, '
      #9'DIRECTIVITY_CENTRANS, '
      #9'DIRECTIVITY_PERTRANS, '
      #9'DISTANCE_ADJACENT_TRANS, '
      #9'EFFHEIGHTANT_CENTRANS, '
      #9'EFFHEIGHTANT_PERTRANS'
      'from TYPESYNHRONET'
      'where ID = :ID')
    SelectSQL.Strings = (
      'select '
      #9'ID, '
      #9'TYPENAME, '
      #9'NETGEOMETRY, '
      #9'NUMTRANSMITTERS, '
      #9'POWER_CENTRANS, '
      #9'POWER_PERTRANS, '
      #9'COVERAGEAREA, '
      #9'DIRECTIVITY_CENTRANS, '
      #9'DIRECTIVITY_PERTRANS, '
      #9'DISTANCE_ADJACENT_TRANS, '#9#9#9'EFFHEIGHTANT_CENTRANS, '
      #9'EFFHEIGHTANT_PERTRANS'
      'from TYPESYNHRONET')
    ModifySQL.Strings = (
      'update TYPESYNHRONET set'
      #9'TYPENAME = :TYPENAME, '
      #9'NETGEOMETRY = :NETGEOMETRY, '
      #9'NUMTRANSMITTERS = :NUMTRANSMITTERS, '
      #9'POWER_CENTRANS = :POWER_CENTRANS, '
      #9'POWER_PERTRANS = :POWER_PERTRANS, '
      #9'COVERAGEAREA = :COVERAGEAREA, '
      #9'DIRECTIVITY_CENTRANS = :DIRECTIVITY_CENTRANS, '
      #9'DIRECTIVITY_PERTRANS = :DIRECTIVITY_PERTRANS, '
      #9'DISTANCE_ADJACENT_TRANS = :DISTANCE_ADJACENT_TRANS,'
      #9'EFFHEIGHTANT_CENTRANS = :EFFHEIGHTANT_CENTRANS, '
      #9'EFFHEIGHTANT_PERTRANS = :EFFHEIGHTANT_PERTRANS'
      'where ID = :ID')
    Top = 52
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'TYPESYNHRONET.ID'
      Required = True
      Visible = False
    end
    object dstListTYPENAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'TYPENAME'
      Origin = 'TYPESYNHRONET.TYPENAME'
      Size = 32
    end
    object dstListNETGEOMETRY: TIBStringField
      DisplayLabel = #1043#1077#1086#1084#1077#1090#1088#1110#1103
      FieldName = 'NETGEOMETRY'
      Origin = 'TYPESYNHRONET.NETGEOMETRY'
      Size = 16
    end
    object dstListNUMTRANSMITTERS: TSmallintField
      DisplayLabel = #1050#1110#1083#1100#1082#1110#1089#1090#1100' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
      FieldName = 'NUMTRANSMITTERS'
      Origin = 'TYPESYNHRONET.NUMTRANSMITTERS'
    end
    object dstListPOWER_CENTRANS: TFloatField
      DisplayLabel = #1055#1086#1090#1091#1078#1085#1110#1089#1090#1100' '#1094#1077#1085#1090#1088
      FieldName = 'POWER_CENTRANS'
      Origin = 'TYPESYNHRONET.POWER_CENTRANS'
    end
    object dstListPOWER_PERTRANS: TFloatField
      DisplayLabel = #1055#1086#1090#1091#1078#1085#1110#1089#1090#1100' '#1087#1077#1088#1077#1092#1077#1088
      FieldName = 'POWER_PERTRANS'
      Origin = 'TYPESYNHRONET.POWER_PERTRANS'
    end
    object dstListCOVERAGEAREA: TFloatField
      DisplayLabel = #1047#1086#1085#1072' '#1087#1086#1082#1088#1080#1090#1090#1103
      FieldName = 'COVERAGEAREA'
      Origin = 'TYPESYNHRONET.COVERAGEAREA'
    end
    object dstListDIRECTIVITY_CENTRANS: TFloatField
      DisplayLabel = #1053#1072#1087#1088#1072#1074#1083#1077#1085#1110#1089#1090#1100' '#1072#1085#1090' '#1094#1077#1085#1090#1088
      FieldName = 'DIRECTIVITY_CENTRANS'
      Origin = 'TYPESYNHRONET.DIRECTIVITY_CENTRANS'
    end
    object dstListDIRECTIVITY_PERTRANS: TFloatField
      DisplayLabel = #1053#1072#1087#1088#1072#1074#1083#1077#1085#1110#1089#1090#1100' '#1072#1085#1090' '#1087#1077#1088#1077#1092
      FieldName = 'DIRECTIVITY_PERTRANS'
      Origin = 'TYPESYNHRONET.DIRECTIVITY_PERTRANS'
    end
    object dstListDISTANCE_ADJACENT_TRANS: TFloatField
      DisplayLabel = #1044#1110#1089#1090#1072#1085#1094#1110#1103' '#1084#1110#1078' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072#1084#1080
      FieldName = 'DISTANCE_ADJACENT_TRANS'
      Origin = 'TYPESYNHRONET.DISTANCE_ADJACENT_TRANS'
    end
    object dstListEFFHEIGHTANT_CENTRANS: TIntegerField
      DisplayLabel = #1045#1092' '#1074#1080#1089#1086#1090#1072' '#1072#1085#1090' '#1094#1077#1085#1090#1088
      FieldName = 'EFFHEIGHTANT_CENTRANS'
      Origin = 'TYPESYNHRONET.EFFHEIGHTANT_CENTRANS'
    end
    object dstListEFFHEIGHTANT_PERTRANS: TIntegerField
      DisplayLabel = #1045#1092' '#1074#1080#1089#1086#1090#1072' '#1072#1085#1090' '#1087#1077#1088#1077#1092
      FieldName = 'EFFHEIGHTANT_PERTRANS'
      Origin = 'TYPESYNHRONET.EFFHEIGHTANT_PERTRANS'
    end
  end
end
