<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportPopup.aspx.cs" Inherits="RMS.Modal.ReportPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RMS Report</title>
    <style type="text/css">
        body {
            background:white;
            font-family: Arial, Helvetica, sans-serif;
            min-height:500px;
            width:100%;
            margin:0;
            padding:10px;
        }
        .Header {
            padding:5px;
            text-align:center;
            background-color:#f0eac8;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="rsm2" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Bootstrap" RenderMode="Lightweight" />
    <telerik:RadStyleSheetManager ID="rssm2" runat="server" CdnSettings-TelerikCdn="Enabled" />
    <div>
        <h2><asp:Literal ID="ltlReportTitle" runat="server" /></h2>
        <hr />
        <p style="font-weight:bold;">
            <asp:Literal ID="ltlSite" runat="server" /><br />
            <asp:Literal ID="ltlRecord" runat="server" />
        </p>
        <asp:Panel ID="pnlNoRecordID" runat="server" Visible="false">
            <p style="font-weight:bold;">The record ID passed is not valid.</p>
        </asp:Panel>
        <asp:Panel ID="pnlChangeLog" runat="server" Visible="false">
            <p style="font-weight:bold;">
                Period timeframe: <asp:Literal ID="ltlPeriod1" runat="server" />
            </p>
            <asp:DataGrid ID="dgChangeLog" runat="server" AutoGenerateColumns="false" Width="95%">
                <Columns>
                    <asp:BoundColumn DataField="edited_dt" HeaderText="Date and Time" ItemStyle-Width="180px" />
                    <asp:BoundColumn DataField="edited_by_uid" HeaderText="User ID" ItemStyle-Width="80px" />
                    <asp:BoundColumn DataField="new_va" HeaderText="Analysis Notes" />
                </Columns>
                <HeaderStyle Font-Bold="true" CssClass="Header" />
            </asp:DataGrid>
        </asp:Panel>
        <asp:Panel ID="pnlDialog" runat="server" Visible="false">
            <p style="font-weight:bold;">
                Period timeframe: <asp:Literal ID="ltlPeriod2" runat="server" />
            </p>
        </asp:Panel>
        <asp:Panel ID="pnlWYAnalysisNotes" runat="server" Visible="false">

        </asp:Panel>
    </div>
    </form>
</body>
</html>
