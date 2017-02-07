<%@ Page Title="" Language="C#" MasterPageFile="~/RMS.Master" AutoEventWireup="true" CodeBehind="RMSWSCHome.aspx.cs" Inherits="RMS.RMSWSCHome" %>
<%@ Register Src="~/Control/OfficeSelector.ascx" TagName="OfficeSelector" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/default.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="osHome">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="osHome" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <telerik:RadPageLayout runat="server" ID="rplTop">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn CssClass="jumbotron">
                        <h2>Welcome to the <asp:Literal ID="ltlWSCName" runat="server" /></h2>
                        <img src="images/RMSTitle.png" alt="SIMS" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn HiddenMd="true" HiddenSm="true" HiddenXs="true">
                        <uc:OfficeSelector id="osHome" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">

</asp:Content>
