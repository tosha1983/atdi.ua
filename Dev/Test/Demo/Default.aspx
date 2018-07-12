<%@ Page Title="Default pages" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="OnlinePortal._Default" SmartNavigation="true" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
        &nbsp;</p>
    <asp:Panel ID="PanelMenu" runat="server">



    
            <asp:Table ID="Table1" runat="server" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="245px" >
           <asp:Panel ID="Panel2" runat="server" GroupingText="Meniu">
              


<asp:TreeView ID="TreeViewMenu" runat="server" Width="240px" OnSelectedNodeChanged="TreeViewMenu_SelectedNodeChanged" ShowLines="True" >

        </asp:TreeView>

     </asp:Panel>  
            </asp:TableCell>
                <asp:TableCell Width="100%"  HorizontalAlign="Center">
                    <asp:Panel ID="Panel3" runat="server">
                        <asp:Label ID="Label1" runat="server" Text="Label">
                           <h1> Sveiki atvykę į Ryšių tarnybos Lietuvoje - dažnių spektro valdymo portale!  </h1>
                        </asp:Label>
                    </asp:Panel>
               </asp:TableCell>

                     <asp:TableCell Width="100%"  HorizontalAlign="Center">
                    <asp:Panel ID="Panel4" runat="server">
                        
                    </asp:Panel>
               </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </asp:Panel>

    <asp:Panel ID="PanelWelcome" runat="server">
            <h1 runat="server" align="center"> Dažnių spektro valdymo portalas. Prašome leisti!</h1>
        </asp:Panel>

</asp:Content>
