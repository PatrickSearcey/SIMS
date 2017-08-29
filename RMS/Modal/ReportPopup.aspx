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
                    <asp:BoundColumn DataField="new_va" HeaderText="Analysis" />
                </Columns>
                <HeaderStyle Font-Bold="true" CssClass="Header" />
            </asp:DataGrid>
        </asp:Panel>
        <asp:Panel ID="pnlDialog" runat="server" Visible="false">
            <p style="font-weight:bold;">
                Period timeframe: <asp:Literal ID="ltlPeriod2" runat="server" />
            </p>
            <asp:DataGrid ID="dgDialog" runat="server" AutoGenerateColumns="false" Width="95%">
                <Columns>
                    <asp:BoundColumn DataField="dialog_dt" HeaderText="Date and Time" ItemStyle-Width="180px" />
                    <asp:BoundColumn DataField="origin_va" HeaderText="From" ItemStyle-Width="80px" />
                    <asp:BoundColumn DataField="dialog_by" HeaderText="User ID" />
                    <asp:BoundColumn DataField="status_set_to_va" HeaderText="Status Set To" />
                    <asp:BoundColumn DataField="comments_va" HeaderText="Comments" />
                </Columns>
                <HeaderStyle Font-Bold="true" CssClass="Header" />
            </asp:DataGrid>
        </asp:Panel>
        <asp:Panel ID="pnlWYAnalysisNotes" runat="server" Visible="false">
            <asp:DataList ID="dlWYAnalysisNotes" runat="server" width="95%">
                <ItemTemplate>
                    <hr />
                    <p style="font-weight:bold;">Analysis Period: <%# Eval("timespan") %></p>
                    <p style="font-weight:bold;">Analysis:</p>
                    <div>
                        <%# Eval("analysis_notes_va") %>
                    </div>
                    <p style="font-style:italic;">Analysis for this period last updated <%# Eval("edited_dt") %> by <%# Eval("edited_by_uid") %></p>
                    <table border="0" width="650px">
                        <tr>
                            <td align="right"><span style="font-style:italic;">Analyzed By: </span></td>
                            <td><%# Eval("analyzed_by") %></td>
                            <td align="right"><span style="font-style:italic;">Approved By: </span></td>
                            <td><%# Eval("approved_by") %></td>
                        </tr>
                        <tr>
                            <td align="right"><span style="font-style:italic;">Date:</span></td>
                            <td><%# Eval("analyzed_dt") %></td>
                            <td align="right"><span style="font-style:italic;">Date:</span></td>
                            <td><%# Eval("approved_dt") %></td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:DataList>
            <asp:Literal ID="ltlNoPeriods" runat="server" Text="No Station Analyses exist for the current WY." Visible="false" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
