<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="PeriodDetails.aspx.cs" Inherits="RMS.Report.PeriodDetails" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/audit.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <br />
    <div class="mainContent">
        <p>
            Select from the list of active records, and specify the timeframe for which you would like to retrieve period details.
        </p>
        <table>
            <tr></tr>
        </table>
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Station Analyses" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Change Logs" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="Dialogs" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <p style="font-weight:bold; padding: 0 5px 0 5px;"></p>
                
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv1">
                <p style="font-weight:bold;padding: 0 5px 0 5px;"></p>
                
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv2">
                <p style="font-weight:bold;padding: 0 5px 0 5px;"></p>
                
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
