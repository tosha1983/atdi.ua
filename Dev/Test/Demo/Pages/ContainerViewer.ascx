<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContainerViewer.ascx.cs" Inherits="OnlinePortal.ContainerViewer"  %>
<head>
    <style type="text/css">
        /**/
        #mask {
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
       .tp {
            width: 10px;
            height: 10px;
            background-color: red;
        }
    </style>
    
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.2/themes/smoothness/jquery-ui.css" />
    <script src="http://code.jquery.com/jquery-1.9.1.js"></script>
    <script src="http://code.jquery.com/ui/1.10.2/jquery-ui.js"></script>
    <link rel="stylesheet" href="/resources/demos/style.css" />
    <script src="Scripts/JavaScriptCustom.js" type="text/javascript"></script>    
    <script src="Scripts/MaskedEditFix.js" type="text/javascript"></script>



  

    <script type="text/javascript" language="javascript">
        
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
</head>
<body runat="server">
    

    
        <asp:Panel ID="PanelPreLoad" runat="server"  BackColor="White" Height="34px"  
            Width="100px" Style="z-index:100;background-color:white; position: absolute; left: 50%; top: 50%; 
border: outset 2px gray;padding:1px;display:none" HorizontalAlign="Center" Visible="true">
            <asp:Image ImageUrl="~/Images/loading.gif" runat="server" ImageAlign="Bottom" />
            </asp:Panel>

    
        <asp:Panel ID="pnlpopup" runat="server" BackColor="White" Height="80%" GroupingText="Редагування елементів вікна"
            Width="1000px" Style="z-index: 111; background-color: White; position: absolute; left: 15%; top: 12%; border: outset 2px gray; padding: 1px; display: none"
            ScrollBars="Vertical">
            <asp:Panel ID="pnlpopup_detail" runat="server" BorderStyle="Dotted" BorderWidth="1">
                <asp:Table ID="TableFinder" runat="server" Width="100%">
                    <asp:TableRow ID="TableRow1" runat="server" Width="100%">
                        <asp:TableCell ID="TableCell1" runat="server" Width="100%">
                            <asp:Panel ID="PanelEditor1" runat="server" Width="100%">
                            </asp:Panel>
                        </asp:TableCell>
                       
                    </asp:TableRow>
                </asp:Table>

            </asp:Panel>


            <asp:Panel ID="PanelWebQuestionInput" runat="server" BorderStyle="Dotted" BorderWidth="1" GroupingText="" Visible="true" Width="100%">
                <asp:Table ID="Table2" runat="server" Width="100%">
                    <asp:TableRow ID="TableRow3" runat="server">
                        <asp:TableCell ID="TableCell3" runat="server" Width="100%">
                            <asp:Panel ID="PanelWebQuestion" runat="server" GroupingText="Ваш коментар" HorizontalAlign="Center" Visible="false" Width="100%">
                                <asp:TextBox runat="server" ID="WebQuestion" Width="100%" Height="200" TextMode="MultiLine"  Enabled="true"> </asp:TextBox>
                                <asp:Button ID="SaveWebQuestion" runat="server" CommandName="UpdateQuestion" Text="Надіслати коментар" OnClick="SaveWebQuestion_Click"/>
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>

             <asp:Panel ID="PanelWebQuestionOutput" runat="server" BorderStyle="Dotted" BorderWidth="1" GroupingText="" Visible="false" Width="100%">
                <asp:Table ID="Table3" runat="server" Width="100%">
                    <asp:TableRow ID="TableRow4" runat="server">
                        <asp:TableCell ID="TableCell2" runat="server" Width="100%">
                            <asp:Panel ID="PanelAnswer" runat="server" GroupingText="Інші деталі" Width="100%" HorizontalAlign="Center" Visible="false" >
                                <asp:Label ID="Label1" runat="server">Klausimo data</asp:Label><br>
                                <asp:TextBox ID="DateQuestionOut" runat="server" Width="100%"></asp:TextBox>
                                <asp:Label ID="Label2" runat="server">Komentarą pateikęs asmuo</asp:Label><br>
                                <asp:TextBox ID="AuthorQuestionOut" runat="server" Width="100%"></asp:TextBox>
                                <asp:Label ID="Label4" runat="server">Atsakymo data</asp:Label><br>
                                <asp:TextBox ID="DateAnswerOut" runat="server" Width="100%"></asp:TextBox>
                                <asp:Label ID="Label5" runat="server">Atsakymą pateikęs tarnautojas</asp:Label><br>
                                <asp:TextBox ID="AuthorAnswerOut" runat="server" Width="100%"></asp:TextBox>
                             </asp:Panel>
                            <asp:Panel ID="PanelWebAnswer" runat="server" GroupingText="Atsakymas" HorizontalAlign="Center" Visible="false" Width="100%">
                                <asp:TextBox runat="server" ID="WebAnswer" Width="100%" Height="340" TextMode="MultiLine"  Enabled="true"> </asp:TextBox>
                                
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>

            <asp:Panel ID="PanelConstraint" runat="server" BorderWidth="0">
                <asp:Label ID="LabelConstraints"  ForeColor="#ff661c"  runat="server"/>
            </asp:Panel>
            <asp:Panel ID="PanelSave" runat="server" BorderWidth="1">
                <asp:Button ID="btnUpdate" CommandName="Update" runat="server" Text="Так" OnClick="btnUpdate_Click" ValidationGroup="FindGroupEx" />
                <input type="button" class="btnClose" value="Скасувати" />
            </asp:Panel>
        </asp:Panel>
        
        
        <div style="overflow-x:auto;width: 250px; max-width:100%;min-width:100%">
        <asp:Panel ID="ViewUpdate" runat="server" ScrollBars="None" Height="450">
        <asp:Label ID="LabelConstraintViewAll"  ForeColor="#ff661c"  runat="server"/>
        <asp:GridView ID="GridView" runat="server"  HeaderStyle-Wrap="true"   BackColor="#E0E4E4" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnRowCommand="dgProviders_RowCommand" OnRowDataBound="GridViewMain_RowDataBound" AutoGenerateColumns="false" OnRowUpdating="GridViewMain_RowUpdating" AllowSorting="true" OnSorting="GridViewMain_Sorting" DataKeyNames="ID"  AllowPaging="True" OnPageIndexChanging="GridView_PageIndexChanging" PageSize="30" ShowFooter="True" AllowCustomPaging="False" Font-Size="Small" >
            <Columns>
                <asp:TemplateField ItemStyle-Width="1px">
                    <ItemTemplate>
                        <asp:Button ID="ButtonLink" runat="server" ButtonType="Link" CommandName="Approval" Text="Редагувати публікацію" CommandArgument='<%# Container.DataItemIndex %>' OnClientClick="showProgress();"  ValidationGroup="FindGroupEx" />
                       </ItemTemplate>
                </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="1px">
                        <ItemTemplate>
                            <asp:Button ID="ButtonDelete" runat="server" ButtonType="Link" CommandName="DeleteApproval" Text="Видалити" CommandArgument='<%# Container.DataItemIndex %>' OnClientClick= "javascript: ConfirmDelete(this.name);return false;"  ValidationGroup="FindGroupEx" Visible="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
            </Columns>



            <FooterStyle BackColor="White" ForeColor="#000066" />
            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
            <PagerSettings FirstPageText="Перший" LastPageText="Останній" PageButtonCount="7" Mode="NumericFirstLast" />
            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
            <RowStyle ForeColor="#000066" />
            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#007DBB" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#00547E" />
        </asp:GridView>
            </asp:Panel>
            </div>
            
                    
            

        


    

                
</body>



