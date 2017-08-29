<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAudit.aspx.cs" Inherits="RMS.Modal.ViewAudit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RMS Audit Details</title>
    <style type="text/css">
        body {
            background:white;
            font-family: Arial, Helvetica, sans-serif;
            min-height:500px;
            width:100%;
            margin:0;
            padding:10px;
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
        <h2>Audit Details</h2>
        <hr />
        <p style="font-weight:bold;">
            <asp:Literal ID="ltlSite" runat="server" /><br />
            <asp:Literal ID="ltlRecord" runat="server" />
        </p>
        <hr />
        <asp:Panel ID="pnlError" runat="server">
            <p style="font-weight:bold;">There was an error. No audit periods found.</p>
        </asp:Panel>
        <asp:Panel ID="pnlAudits" runat="server">
            <asp:DataList ID="dlAudits" runat="server" OnItemDataBound="dlAudits_ItemDataBound" DataKeyField="rms_audit_id">
                <ItemTemplate>
                    <table id="tableForm1" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: #f0eac8;">
                        <tr>
                            <td colspan="3">
                                <h4>Audit Period <%# Eval("DateRange") %></h4>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label>Audit By: <%# Eval("audit_by") %></label>
                            </td>
                            <td colspan="2">
                                <label>Audited Date: <%# Eval("audit_dt") %></label>
                            </td>
                        </tr>
                        <tr>
                            <td width="200" valign="top">
                                <label>Audit Type:</label>
                            </td>
                            <td>
                                <%# Eval("AuditType") %>
                            </td>
                            <td rowspan="6" valign="top" width="200">
                                <div class="DocBox">
                                    <h5>Currently Uploaded Documents for this Audit</h5>
                                    <div style="padding: 10px 0 10px 20px;">
                                        <telerik:RadListView ID="rlvAuditDocs" runat="server" ItemPlaceholderID="DocsHolder" Skin="Bootstrap">
                                            <LayoutTemplate>
                                                <asp:Panel ID="DocsHolder" runat="server">
                                                </asp:Panel>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <a href='<%# String.Format("{0}Handler/DocHandler.ashx?task=get&ID={1}", System.Configuration.ConfigurationManager.AppSettings["SIMSURL"], Eval("rms_audit_document_id")) %>' target="_blank"><%# Eval("document_nm") %></a>
                                            </ItemTemplate>
                                            <ItemSeparatorTemplate>
                                                <br />
                                            </ItemSeparatorTemplate>
                                            <EmptyDataTemplate>
                                                <i>No audit documents have been uploaded.</i>
                                            </EmptyDataTemplate>
                                        </telerik:RadListView>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <label>Audit Results:</label>
                            </td>
                            <td>
                                <%# Eval("AuditResults") %>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <label>Audit Reason:</label>
                            </td>
                            <td>
                                <%# Eval("audit_reason") %>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <label>Data Audited:</label>
                            </td>
                            <td>
                                <%# Eval("audit_data") %>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <label>Audit Findings:</label>
                            </td>
                            <td>
                                <%# Eval("audit_findings") %>
                            </td>
                        </tr>
                    </table>
                    <hr />
                </ItemTemplate>
            </asp:DataList>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
