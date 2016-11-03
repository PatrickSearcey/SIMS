<%@ Page Title="" Language="C#" MasterPageFile="~/RMS.Master" AutoEventWireup="true" CodeBehind="RMSWSCHome.aspx.cs" Inherits="SIMS2017.RMSWSCHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/default.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadPageLayout runat="server" ID="RadPageLayout1">
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
                        <h3>H3 text, font size 24 px </h3>
                        Ut aliquam elit eget quam tincidunt, et aliquam libero congue. Phasellus aliquet sed quam vitae dictum. Aliquam erat volutpat. Morbi accumsan a mi quis pretium. 
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
</asp:Content>
