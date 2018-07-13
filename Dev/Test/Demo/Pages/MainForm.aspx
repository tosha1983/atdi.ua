
<%@ Page Title="Default pages" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="MainForm.aspx.cs" Inherits="OnlinePortal.MainForm" EnableEventValidation="false" SmartNavigation="true" MaintainScrollPositionOnPostback="true"  %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register Src="ContainerFinder.ascx" TagName="ContainerFinder" TagPrefix="TContainerFinder" %>
<%@ Register Src="ContainerViewer.ascx" TagName="ContainerViewer" TagPrefix="TContainerViewer" %>



<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

<style type="text/css">
        .cpHeader
        {
            cursor: pointer;
        }

        #mask
        {
            position: fixed;
            left: 0px;
            top: 0px;
            z-index: 4;
            opacity: 0.4;
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=40)"; /* first!*/
            filter: alpha(opacity=40); /* second!*/
            background-color: gray;
            display: none;
            width: 100%;
            height: 100%;
        }
    </style>
    <style type="text/css">
        .HandleCSS
        {
            width:20px;
            height:20px;
            background-color:Red;
            }

        #loader {
  position: absolute;
  left: 50%;
  top: 50%;
  z-index: 1;
  width: 150px;
  height: 150px;
  margin: -75px 0 0 -75px;
  border: 16px solid #f3f3f3;
  border-radius: 50%;
  border-top: 16px solid #3498db;
  width: 120px;
  height: 120px;
  -webkit-animation: spin 2s linear infinite;
  animation: spin 2s linear infinite;
}

@-webkit-keyframes spin {
  0% { -webkit-transform: rotate(0deg); }
  100% { -webkit-transform: rotate(360deg); }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Add animation to "page content" */
.animate-bottom {
  position: relative;
  -webkit-animation-name: animatebottom;
  -webkit-animation-duration: 1s;
  animation-name: animatebottom;
  animation-duration: 1s
}

@-webkit-keyframes animatebottom {
  from { bottom:-100px; opacity:0 } 
  to { bottom:0px; opacity:1 }
}

@keyframes animatebottom { 
  from{ bottom:-100px; opacity:0 } 
  to{ bottom:0; opacity:1 }
}

#myDiv {
  display: none;
  text-align: left;
}

    </style>
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript" type="text/javascript" language="javascript">


        function ShowPopup() {
            $('#mask').show();
            $('#<%=pnlpopup.ClientID %>').show();
        }
        function HidePopup() {
            $('#mask').hide();
            $('#<%=pnlpopup.ClientID %>').hide();
        }
        $(".btnClose").live('click', function () {
            HidePopup();
        });

        function showProgress() {
            $('#<%=PanelPreLoad.ClientID %>').show();
        };
   

           
    </script>
    <div id="mask">
    </div>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
  
       <asp:Panel ID="PanelPreLoad" runat="server"  BackColor="White" Height="34px"  
            Width="100px" Style="z-index:100;background-color:white; position: absolute; left: 50%; top: 50%; 
border: outset 2px gray;padding:1px;display:none" HorizontalAlign="Center" Visible="true">
            <asp:Image ImageUrl="~/Images/loading.gif" runat="server" ImageAlign="Bottom" />
            </asp:Panel>

       <div id="ScrollList" style="width:98%;  overflow:auto">
   
    <asp:Table id="MainTable1" runat="server" Width="100%" OnPreRender="MainTable1_PreRender">
        <asp:TableRow ID="Row1" runat="server" Width="100%">
            <asp:TableCell ID="TableRow3" runat="server" Width="100%">

                    
     
                    

    <div style ="height:411px; width:auto; height:auto; overflow:auto;" runat="server">


<asp:Panel ID="pnlpopup" runat="server"  BackColor="White" Height="80%" GroupingText="Вікно редагування"
            Width="800px" Style="z-index:111;background-color: White; position: absolute; left: 15%; top: 12%; 
border: outset 2px gray;padding:1px;display:none" ScrollBars="Vertical">
    <asp:Panel ID="pnlpopup_detail" runat="server"  BorderStyle="Dotted" BorderWidth="1" >
        <asp:Table ID="TableFinder" runat ="server" Width="80%">
            <asp:TableRow ID="TableRow1" runat ="server" >
                <asp:TableCell ID="TableCell1" runat="server" Width="50%">
                    <asp:Panel ID="PanelEditor1" runat="server" >

                    </asp:Panel>
                </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" Width="50%">
                <asp:Panel ID="PanelEditor2" runat="server">

                </asp:Panel>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    </asp:Panel>   
                        
    <asp:Panel ID="PanelAdditional" runat="server"  BorderStyle="Dotted" BorderWidth="1" GroupingText="Додаткові обов'язкові поля">
     <asp:Table ID="Table3" runat ="server" Width="80%">
            <asp:TableRow ID="TableRow2" runat ="server" >
                 <asp:TableCell ID="TableCell4" runat="server" Width="50%">
                    <asp:Panel ID="PanelAdditionalMandatory1" runat="server"  >

                    </asp:Panel>
                </asp:TableCell>
              
                </asp:TableRow>
                </asp:Table>
        </asp:Panel>

     <asp:Panel ID="PanelSave" runat="server" BorderWidth="1" >
         <asp:Button ID="btnUpdate" CommandName="Update" runat="server" Text="Оновлення" OnClick="btnUpdate_Click" />
                        <input type="button" class="btnClose" value="Скасувати" />
         </asp:Panel>

        </asp:Panel>
        </div>



      
        <asp:Panel ID="PanelMain" runat="server" Width="100%">

            <asp:Table ID="Table1" runat="server" Width="100%">
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell Width="20%"  VerticalAlign="Top">
           <asp:Panel ID="Panel2" runat="server" GroupingText="Меню" ScrollBars="Auto">
        

               <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
         <div style ="height:auto; width:auto; overflow:auto;">      
        <asp:TreeView ID="TreeViewMenu" runat="server"  ShowLines="True" ExpandDepth="1"  OnSelectedNodeChanged="TreeViewMenu_SelectedNodeChanged" ShowExpandCollapse="true" SelectedNodeStyle-Font-Bold="true"  EnableClientScript="true" onclick="TreeViewMenu_SelectedNodeChanged">

        </asp:TreeView>
         </div>
                   
    </ContentTemplate>  
     <Triggers>
            <asp:AsyncPostBackTrigger ControlID="TreeViewMenu" EventName="SelectedNodeChanged"  />
            </Triggers>
    </asp:UpdatePanel>  

     </asp:Panel>  
            </asp:TableCell>
              
                     <asp:TableCell Width="80%"  HorizontalAlign="Left" runat="server" >
                       <asp:Label  ID="ViewFind"  Text="Показати вікно пошуку. (Будь-ласка, натисніть тут.)" runat="server" BackColor="#ccccff" CssClass="cpHeader" ></asp:Label>
                        <asp:Table ID="Table2" runat="server" Width="100%">

                                     <asp:TableRow ID="TableRow4" HorizontalAlign="Left" runat="server">
                            <asp:TableCell ID="TableCell3"  HorizontalAlign="Justify" runat="server">
                                <asp:Panel runat="server" ID="Panel1"  GroupingText="" Visible="true" Height="95%" >
                                    <asp:Label Width="100%" runat="server" Font-Bold="true" ID="Comments_Request1" Text="Коментар:"></asp:Label>
                                    <asp:Label Width="100%" runat="server" ID="Comments_Request2" Text=""></asp:Label>
                                      
                                    </asp:Panel>
                                </asp:TableCell>
                             
                        </asp:TableRow>

                     

                        <asp:TableRow HorizontalAlign="Center" Width="100%">
                            <asp:TableCell HorizontalAlign="Center" Width="100%">
                                
                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server"   
                                CollapsedSize="0"
                                
                                Collapsed="True"
                                ExpandControlID="ViewFind"
                                CollapseControlID="ViewFind"
                                AutoCollapse="false"
                                AutoExpand="false"
                                TargetControlID="PanelFinder"
                                    />   
                                <asp:Panel runat="server" ID="PanelFinder" GroupingText="Finder" ValidationGroup="FindGroupEx">
                                    <TContainerFinder:ContainerFinder ID="ContainerFinder1" runat="server" ValidationGroup="FindGroupEx" />
                                    
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:Panel runat="server" ID="PanelCreateStation" GroupingText="Розділ створення нової публікації" HorizontalAlign="Left" Visible="true">
                                    <asp:Button ID="BtnCreateStation" runat="server" Text="Створити нову публікацію" Width="30%" OnClick="BtnCreateStation_Click" Enabled="true" OnClientClick="showProgress();" ValidationGroup="FindGroupEx"/>
                                    
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>                         

                       <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:Panel runat="server" ID="PanelDownloadDocs" GroupingText="Розділ завантаження документації" HorizontalAlign="Left" Visible="false">
                                    
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow> 

                            
                         <asp:TableRow HorizontalAlign="Left" runat="server">
                            <asp:TableCell  HorizontalAlign="Left" runat="server">
                                <asp:Panel runat="server" ID="PanelViewer" GroupingText="Вид" Visible="true" Height="95%" >
                                         <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" >
                   <ProgressTemplate>
                       <img alt="" src="../Images/loading.gif" runat="server"/> 
                       Loading...
                   </ProgressTemplate>

               </asp:UpdateProgress>

                                    <asp:Panel runat="server" ID="PanelMessagesAdditional" GroupingText=""  BorderWidth="0" Height="95%" HorizontalAlign="Center"  >
                                    <asp:Label runat="server" ID="LabelMessages" ForeColor="GrayText" Font-Size="Larger"></asp:Label>
                                        </asp:Panel>

                                       <TContainerViewer:ContainerViewer ID="ContainerViewer1" runat="server"  />
                                    </asp:Panel>
                                </asp:TableCell>
                             
                        </asp:TableRow>

                        <asp:TableRow HorizontalAlign="Center" VerticalAlign="Middle">
                            <asp:TableCell HorizontalAlign="Center" VerticalAlign="Middle" Height="700" BackColor="#d2d9e3" ID="ErrorMessagesCell" Visible="false">
                                    <asp:Label runat="server" ID="LblMessages" Visible="false"  ForeColor="GrayText" Font-Size="Larger"></asp:Label>
                            </asp:TableCell>
                        </asp:TableRow>
                        </asp:Table>
                                             
                              
               </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </asp:Panel>
          
            </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
</div>        
    
    

</asp:Content>