<%@ MasterType VirtualPath="~/Site.master" %> 
<%@ Page Title="Authorize" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="OnlinePortal.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Вхід</h2>
    <p>
        Введіть  ім'я користувача та пароль.
    </p>
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" 
        RenderOuterTable="false" OnAuthenticate="LoginUser_Authenticate" >
        <LayoutTemplate>

            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" 
                 ValidationGroup="LoginUserValidationGroup"/>
            <div class="accountInfo">

                <fieldset class="login">
                    <legend>Інформація для авторизації</legend>
                    <p>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Width="120px">Ім'я користувача:</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                             CssClass="failureNotification" ErrorMessage="Поле «Ім'я користувача» обов'язкове." ToolTip="Поле «Ім'я користувача» обов'язкове." 
                             ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Пароль:</asp:Label>
                        <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                             CssClass="failureNotification" ErrorMessage="Поле «Пароль» обов'язкове." ToolTip="Поле «Пароль» обов'язкове." 
                             ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:CheckBox ID="RememberMe" runat="server"/>
                        <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Зберегти параметри авторизації</asp:Label>
                    </p>
                </fieldset>
                <p class="submitButton">
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Вхід" ValidationGroup="LoginUserValidationGroup" />
                </p>
            </div>
        </LayoutTemplate>
    </asp:Login>
</asp:Content>
